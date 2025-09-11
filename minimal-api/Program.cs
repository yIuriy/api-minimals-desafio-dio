using minimal_api.Domain.DTOs;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.MapGet("/", () => "Hello World!");


app.MapPost("/login", (LoginDTO loginDTO) =>
{
    if (loginDTO.Email == "adm@test.com" && loginDTO.Password == "123456")
    {
        return Results.Ok("Login feito com sucesso.");
    }
    else
        return Results.Unauthorized();
}
);

app.Run();
