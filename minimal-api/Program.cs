using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using minimal_api.Domain.DTOs;
using minimal_api.Domain.Entitys;
using minimal_api.Domain.Enuns;
using minimal_api.Domain.ModelViews;
using minimal_api.Domain.Services;
using minimal_api.Infrastructure.Db;
using minimal_api.Infrastructure.Interfaces;


#region Builder
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<DbContextMySql>(
    options =>
    {
        options.UseMySql(builder.Configuration.GetConnectionString("mysql"),
        ServerVersion.AutoDetect(builder.Configuration.GetConnectionString("mysql")));
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
app.MapPost("/admin/login", ([FromBody] LoginDTO loginDTO, IAdministratorService administratorService) =>
{
    if (administratorService.Login(loginDTO) != null)
    {
        return Results.Ok("Login feito com sucesso.");
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
).WithTags("Admin");


app.MapGet("/admin/{id}", ([FromRoute] int id, IAdministratorService administratorService) =>
{
    var admin = administratorService.findById(id);

    if (admin == null) return Results.NotFound();

    return Results.Ok(admin);
}).WithTags("Admin");

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

).WithTags("Admin");



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
).WithTags("Vehicle");

app.MapGet("/vehicles", ([FromQuery] int? page, IVehicleService vehicleService) =>
{
    var vehicle = vehicleService.getAll(page);

    return Results.Ok(vehicle);
}
).WithTags("Vehicle");

app.MapGet("/vehicles/{id}", ([FromRoute] int id, IVehicleService vehicleService) =>
{
    var vehicle = vehicleService.findById(id);

    if (vehicle == null) return Results.NotFound();

    return Results.Ok(vehicle);
}).WithTags("Vehicle");

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

}).WithTags("Vehicle");

app.MapDelete("/vehicles/{id}", ([FromRoute] int id, IVehicleService vehicleService) =>
{
    var vehicle = vehicleService.findById(id);

    if (vehicle == null) return Results.NotFound();

    vehicleService.delete(vehicle);

    return Results.NoContent();
}).WithTags("Vehicle");
#endregion

#region App
app.UseSwagger();
app.UseSwaggerUI();
app.Run();
#endregion