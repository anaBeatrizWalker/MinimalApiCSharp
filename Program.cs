
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MinimalApiCSharp.Domain.DTOs;
using MinimalApiCSharp.Domain.Entities;
using MinimalApiCSharp.Domain.Interfaces;
using MinimalApiCSharp.Domain.ModelViews;
using MinimalApiCSharp.Domain.Services;
using MinimalApiCSharp.Infra.DB;
using MinimalApiCSharp.Infra.Interfaces;

#region Builder
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddScoped<IAdministratorService, AdministratorService>();
builder.Services.AddScoped<IVehicleService, VehicleService>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<DataBaseContext>(options => {
        options.UseMySql(builder.Configuration.GetConnectionString("mysql"), 
        ServerVersion.AutoDetect(builder.Configuration.GetConnectionString("mysql")));
});

var app = builder.Build();
#endregion

#region Home
app.MapGet("/", () => Results.Json(new Home())).WithTags("Home");
#endregion

#region Adm
app.MapPost("/adm/login", ([FromBody] LoginDTO loginDTO, IAdministratorService administratorService) => {
    if(administratorService.Login(loginDTO) != null){
        return Results.Ok("Login feito com sucesso!");
    }else {
        return Results.Unauthorized();
    }
}).WithTags("Administrators");
#endregion

#region Vehicles
ValidationErrors validationDTO(VehicleDTO vehicleDTO)
{
    var validation = new ValidationErrors{
      Messages = new List<string>()
    };

    if(string.IsNullOrEmpty(vehicleDTO.Name))
        validation.Messages.Add("Provide a Name");

    if(string.IsNullOrEmpty(vehicleDTO.Brand))
        validation.Messages.Add("Provide a Brand");

    if(vehicleDTO.Year < 1900)
        validation.Messages.Add("Provide a Year >= than 1900");

    return validation;
}

app.MapPost("/vehicle", ([FromBody] VehicleDTO vehicleDTO, IVehicleService vehicleService) => {
   
  var validation = validationDTO(vehicleDTO);

  if(validation.Messages.Count > 0)
    return Results.BadRequest(validation);
   
  var vehicle = new Vehicle {
    Name = vehicleDTO.Name,
    Brand = vehicleDTO.Brand,
    Year = vehicleDTO.Year,
   };
   
   vehicleService.Add(vehicle);
   
   return Results.Created($"/vehicle/{vehicle.Id}", vehicle);
}).WithTags("Vehicles");

app.MapGet("/vehicles", ([FromQuery] int? page, IVehicleService vehicleService) => {
  var vehicles = vehicleService.GetAll(page);
   
   return Results.Ok(vehicles);
}).WithTags("Vehicles");

app.MapGet("/vehicle/{id}", ([FromRoute] int id, IVehicleService vehicleService) => {
  var vehicle = vehicleService.GetById(id);

  if(vehicle == null) return Results.NotFound();
   
   return Results.Ok(vehicle);
}).WithTags("Vehicles");

app.MapPut("/vehicle/{id}", ([FromRoute] int id, VehicleDTO vehicleDTO, IVehicleService vehicleService) => {
  
  var vehicle = vehicleService.GetById(id);

  if(vehicle == null) return Results.NotFound();

  var validation = validationDTO(vehicleDTO);

  if(validation.Messages.Count > 0)
    return Results.BadRequest(validation);

  vehicle.Name = vehicleDTO.Name;
  vehicle.Brand = vehicleDTO.Brand;
  vehicle.Year = vehicleDTO.Year;

  vehicleService.Update(vehicle);
   
   return Results.Ok(vehicle);
}).WithTags("Vehicles");

app.MapDelete("/vehicle/{id}", ([FromRoute] int id, IVehicleService vehicleService) => {
  var vehicle = vehicleService.GetById(id);

  if(vehicle == null) return Results.NotFound();

  vehicleService.Delete(vehicle);
   
   return Results.NoContent();
}).WithTags("Vehicles");
#endregion

#region App
app.UseSwagger();
app.UseSwaggerUI();

app.Run();
#endregion