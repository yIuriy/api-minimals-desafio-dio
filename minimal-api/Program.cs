using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using minimal_api.Domain.DTOs;
using minimal_api.Domain.Entitys;
using minimal_api.Domain.Enuns;
using minimal_api.Domain.ModelViews;
using minimal_api.Domain.Services;
using minimal_api.Infrastructure.Db;
using minimal_api.Infrastructure.Interfaces;


#region Builder
var builder = WebApplication.CreateBuilder(args);

var key = builder.Configuration.GetSection("Jwt").ToString();
if (string.IsNullOrEmpty(key)) key = "123456";

builder.Services.AddAuthentication(option =>
{
    option.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    option.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(option =>
{
    option.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateLifetime = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key))
    };
});

builder.Services.AddAuthorization(

);

builder.Services.AddDbContext<DbContextMySql>(
    options =>
    {
        options.UseMySql(builder.Configuration.GetConnectionString("MySql"),
        ServerVersion.AutoDetect(builder.Configuration.GetConnectionString("MySql")));
    }
);

builder.Services.AddScoped<IAdministratorService, AdministratorService>();
builder.Services.AddScoped<IVehicleService, VehicleService>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

#endregion

#region Home
app.MapGet("/", () => Results.Json(new Home())).WithTags("Home");
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
        new Claim("Profile", administrator.Profile)
    };

    var token = new JwtSecurityToken(
        claims: claims,
        expires: DateTime.Now.AddDays(1),
        signingCredentials: credentials
    );

    return new JwtSecurityTokenHandler().WriteToken(token);
}



app.MapPost("/admin/login", ([FromBody] LoginDTO loginDTO, IAdministratorService administratorService) =>
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
).WithTags("Admin");

app.MapGet("/admin", ([FromQuery] int? page, IAdministratorService administratorService) =>
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
).RequireAuthorization().WithTags("Admin");


app.MapGet("/admin/{id}", ([FromRoute] int id, IAdministratorService administratorService) =>
{
    var admin = administratorService.findById(id);

    if (admin == null) return Results.NotFound();

    return Results.Ok(admin);
}).RequireAuthorization().WithTags("Admin");

app.MapPost("/admin", ([FromBody] AdministratorDTO administratorDTO, IAdministratorService administratorService) =>
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

).RequireAuthorization().WithTags("Admin");
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

app.MapPost("/vehicles", ([FromBody] VehicleDTO vehicleDTO, IVehicleService vehicleService) =>
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
).RequireAuthorization().WithTags("Vehicle");

app.MapGet("/vehicles", ([FromQuery] int? page, IVehicleService vehicleService) =>
{
    var vehicle = vehicleService.getAll(page);

    return Results.Ok(vehicle);
}
).RequireAuthorization().WithTags("Vehicle");

app.MapGet("/vehicles/{id}", ([FromRoute] int id, IVehicleService vehicleService) =>
{
    var vehicle = vehicleService.findById(id);

    if (vehicle == null) return Results.NotFound();

    return Results.Ok(vehicle);
}).RequireAuthorization().WithTags("Vehicle");

app.MapPut("/vehicles/{id}", ([FromRoute] int id, VehicleDTO vehicleDTO, IVehicleService vehicleService) =>
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

}).RequireAuthorization().WithTags("Vehicle");

app.MapDelete("/vehicles/{id}", ([FromRoute] int id, IVehicleService vehicleService) =>
{
    var vehicle = vehicleService.findById(id);

    if (vehicle == null) return Results.NotFound();

    vehicleService.delete(vehicle);

    return Results.NoContent();
}).RequireAuthorization().WithTags("Vehicle");
#endregion

#region App
app.UseSwagger();
app.UseSwaggerUI();

app.UseAuthentication();
app.UseAuthorization();


app.Run();
#endregion