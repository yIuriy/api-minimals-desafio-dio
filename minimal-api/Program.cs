using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using minimal_api.Domain.DTOs;
using minimal_api.Domain.Entitys;
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
#endregion

#region Vehicle
app.MapPost("/vehicles", ([FromBody] VehicleDTO vehicleDTO, IVehicleService vehicleService) =>
{
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
}

).WithTags("Vehicle");
#endregion

#region App
app.UseSwagger();
app.UseSwaggerUI();
app.Run();
#endregion