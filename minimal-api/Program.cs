using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using minimal_api.Domain.DTOs;
using minimal_api.Domain.ModelViews;
using minimal_api.Domain.Services;
using minimal_api.Infrastructure.Db;
using minimal_api.Infrastructure.Interfaces;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<DbContextMySql>(
    options =>
    {
        options.UseMySql(builder.Configuration.GetConnectionString("mysql"),
        ServerVersion.AutoDetect(builder.Configuration.GetConnectionString("mysql")));
    }
);

builder.Services.AddScoped<IAdministratorService, AdministratorService>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.MapGet("/", () => Results.Json(new Home()));


app.MapPost("/login", ([FromBody] LoginDTO loginDTO, IAdministratorService administratorService) =>
{
    if (administratorService.Login(loginDTO) != null)
    {
        return Results.Ok("Login feito com sucesso.");
    }
    else
        return Results.Unauthorized();
}
);

app.UseSwagger();
app.UseSwaggerUI();
app.Run();
