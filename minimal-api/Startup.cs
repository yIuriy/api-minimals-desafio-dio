using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using minimal_api.Domain.DTOs;
using minimal_api.Domain.Entitys;
using minimal_api.Domain.Enuns;
using minimal_api.Domain.ModelViews;
using minimal_api.Domain.Services;
using minimal_api.Infrastructure.Db;
using minimal_api.Infrastructure.Interfaces;

namespace minimal_api
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
            key = Configuration["Jwt"]?.ToString() ?? "";
        }

        private string key;
        public IConfiguration Configuration { get; set; } = default!;

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddAuthentication(option =>
            {
                option.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                option.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(option =>
            {
                option.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    ValidateLifetime = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key)),
                    ValidateIssuer = false,
                    ValidateAudience = false
                };
            });

            services.AddAuthorization();

            services.AddDbContext<DbContextMySql>(
                options =>
                {
                    options.UseMySql(Configuration.GetConnectionString("MySql"),
                    ServerVersion.AutoDetect(Configuration.GetConnectionString("MySql")));
                }
            );

            services.AddScoped<IAdministratorService, AdministratorService>();
            services.AddScoped<IVehicleService, VehicleService>();

            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen(options =>
            {
                options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.Http,
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "Insert the Jwt token here"
                });

                options.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
                });
            });

        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseSwagger();
            app.UseSwaggerUI();

            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                #region Home
                endpoints.MapGet("/", () => Results.Json(new Home())).AllowAnonymous().WithTags("Home");
                #endregion

                #region Administrator
                string generateTokenJwt(Administrator administrator)
                {
                    if (string.IsNullOrEmpty(key)) return string.Empty;

                    var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
                    var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

                    var claims = new List<Claim>()
                    {
                        new Claim("Email", administrator.Email),
                        new Claim("Profile", administrator.Profile),
                        new Claim(ClaimTypes.Role, administrator.Profile)
                    };

                    var token = new JwtSecurityToken(
                        claims: claims,
                        expires: DateTime.Now.AddDays(1),
                        signingCredentials: credentials
                    );

                    return new JwtSecurityTokenHandler().WriteToken(token);
                }

                endpoints.MapPost("/admin/login", ([FromBody] LoginDTO loginDTO, IAdministratorService administratorService) =>
                {
                    var adm = administratorService.Login(loginDTO);
                    if (adm != null)
                    {
                        string token = generateTokenJwt(adm);
                        return Results.Ok(new LoggedAdministrator
                        {
                            Email = adm.Email,
                            Profile = adm.Profile,
                            Token = token
                        });
                    }
                    else
                        return Results.Unauthorized();
                }
                ).AllowAnonymous().WithTags("Admin");

                endpoints.MapGet("/admin", ([FromQuery] int? page, IAdministratorService administratorService) =>
                {
                    var admins = new List<AdministratorModelView>();
                    var adminsFormatted = administratorService.getAll(page);
                    foreach (var adm in adminsFormatted)
                    {
                        admins.Add(new AdministratorModelView
                        {
                            ID = adm.ID,
                            Email = adm.Email,
                            Profile = adm.Profile
                        });
                    }
                    return Results.Ok(admins);
                }
                ).RequireAuthorization().RequireAuthorization(new AuthorizeAttribute { Roles = "Adm" }).WithTags("Admin");


                endpoints.MapGet("/admin/{id}", ([FromRoute] int id, IAdministratorService administratorService) =>
                {
                    var admin = administratorService.findById(id);

                    if (admin == null) return Results.NotFound();

                    return Results.Ok(admin);
                }).RequireAuthorization().RequireAuthorization(new AuthorizeAttribute { Roles = "Adm" }).WithTags("Admin");

                endpoints.MapPost("/admin", ([FromBody] AdministratorDTO administratorDTO, IAdministratorService administratorService) =>
                {
                    var errorMessages = new ValidationErrors
                    {
                        Messages = new List<string>()
                    };

                    if (string.IsNullOrEmpty(administratorDTO.Email))
                        errorMessages.Messages.Add("The Email cannot be empty.");

                    if (string.IsNullOrEmpty(administratorDTO.Password))
                        errorMessages.Messages.Add("The Password cannot be empty.");

                    if (administratorDTO.Profile == null)
                        errorMessages.Messages.Add("The Profile cannot be empty.");

                    if (errorMessages.Messages.Count() > 0)
                        return Results.BadRequest(errorMessages);

                    var admin = new Administrator
                    {
                        Email = administratorDTO.Email,
                        Password = administratorDTO.Password,
                        Profile = administratorDTO.Profile.ToString() ?? Profile.Editor.ToString()
                    };
                    administratorService.save(admin);
                    return Results.Created($"/admin/{admin.ID}", new AdministratorModelView
                    {
                        ID = admin.ID,
                        Email = admin.Email,
                        Profile = admin.Profile
                    });
                }

                ).RequireAuthorization().RequireAuthorization(new AuthorizeAttribute { Roles = "Adm" }).WithTags("Admin");
                #endregion

                #region Vehicle

                ValidationErrors validateDTO(VehicleDTO vehicleDTO)
                {
                    var errorMessages = new ValidationErrors
                    {
                        Messages = new List<string>()
                    };

                    if (string.IsNullOrEmpty(vehicleDTO.Name))
                        errorMessages.Messages.Add("The name cannot be null.");

                    if (string.IsNullOrEmpty(vehicleDTO.Model))
                        errorMessages.Messages.Add("The model cannot be null.");

                    if (vehicleDTO.Year < 1950)
                        errorMessages.Messages.Add("The vehicle is too old, it's accepted vehicles past 1950.");

                    return errorMessages;
                }

                endpoints.MapPost("/vehicles", ([FromBody] VehicleDTO vehicleDTO, IVehicleService vehicleService) =>
                {
                    var errorMessages = validateDTO(vehicleDTO);

                    if (errorMessages.Messages.Count() > 0)
                        return Results.BadRequest(errorMessages);


                    var vehicle = new Vehicle
                    {
                        Name = vehicleDTO.Name,
                        Model = vehicleDTO.Model,
                        Year = vehicleDTO.Year
                    };
                    vehicleService.save(vehicle);
                    return Results.Created($"/vehicle/{vehicle.ID}", vehicle);
                }
                ).RequireAuthorization()
                .RequireAuthorization(new AuthorizeAttribute { Roles = "Adm, Editor" })
                .WithTags("Vehicle");

                endpoints.MapGet("/vehicles", ([FromQuery] int? page, IVehicleService vehicleService) =>
                {
                    var vehicle = vehicleService.getAll(page);

                    return Results.Ok(vehicle);
                }
                ).RequireAuthorization()
                .RequireAuthorization(new AuthorizeAttribute { Roles = "Adm, Editor" })
                .WithTags("Vehicle");

                endpoints.MapGet("/vehicles/{id}", ([FromRoute] int id, IVehicleService vehicleService) =>
                {
                    var vehicle = vehicleService.findById(id);

                    if (vehicle == null) return Results.NotFound();

                    return Results.Ok(vehicle);
                }).RequireAuthorization()
                .RequireAuthorization(new AuthorizeAttribute { Roles = "Adm, Editor" })
                .WithTags("Vehicle");

                endpoints.MapPut("/vehicles/{id}", ([FromRoute] int id, VehicleDTO vehicleDTO, IVehicleService vehicleService) =>
                {
                    var errorMessages = validateDTO(vehicleDTO);

                    if (errorMessages.Messages.Count() > 0)
                        return Results.BadRequest(errorMessages);

                    var vehicle = vehicleService.findById(id);

                    if (vehicle == null) return Results.NotFound();

                    vehicle.Name = vehicleDTO.Name;
                    vehicle.Model = vehicleDTO.Model;
                    vehicle.Year = vehicleDTO.Year;

                    vehicleService.update(vehicle);

                    return Results.Ok(vehicle);

                }).RequireAuthorization()
                .RequireAuthorization(new AuthorizeAttribute { Roles = "Adm" })
                .WithTags("Vehicle");

                endpoints.MapDelete("/vehicles/{id}", ([FromRoute] int id, IVehicleService vehicleService) =>
                {
                    var vehicle = vehicleService.findById(id);

                    if (vehicle == null) return Results.NotFound();

                    vehicleService.delete(vehicle);

                    return Results.NoContent();
                }).RequireAuthorization()
                .RequireAuthorization(new AuthorizeAttribute { Roles = "Adm" })
                .WithTags("Vehicle");
                #endregion
            });
        }
    }
}