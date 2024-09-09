
using Microsoft.EntityFrameworkCore;
using MinimalApiCSharp.Domain.DTOs;
using MinimalApiCSharp.Infra.DB;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<DataBaseContext>(options => {
        options.UseMySql(builder.Configuration.GetConnectionString("mysql"), 
        ServerVersion.AutoDetect(builder.Configuration.GetConnectionString("mysql")));
});

var app = builder.Build();

app.MapGet("/", () => "Hello World!");

app.MapPost("/login", (LoginDTO loginDTO) => {
    if(loginDTO.Email == "adm@teste.com" && loginDTO.Password == "123456"){
        return Results.Ok("Login feito com sucesso!");
    }else {
        return Results.Unauthorized();
    }
});

app.Run();

