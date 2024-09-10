
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MinimalApiCSharp.Domain.DTOs;
using MinimalApiCSharp.Domain.Entities;
using MinimalApiCSharp.Domain.Enums;
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

app.MapPost("/administrator", ([FromBody] AdministratorDTO administratorDTO, IAdministratorService administratorService) => {
  
  var validation = new ValidationErrors{
    Messages = new List<string>()
  };

  if(string.IsNullOrEmpty(administratorDTO.Email))
     validation.Messages.Add("E-mail cannot be empty");

  if(string.IsNullOrEmpty(administratorDTO.Password))
     validation.Messages.Add("Password cannot be empty");

  if(administratorDTO.Profile == null)
     validation.Messages.Add("Profile cannot be empty");

  if(validation.Messages.Count > 0)
    return Results.BadRequest(validation);
 
  var adm = new Administrator {
    Email = administratorDTO.Email,
    Password = administratorDTO.Password,
    Profile = administratorDTO.Profile.ToString() ?? EProfile.Editor.ToString(),
  };
   
  administratorService.Add(adm);
  
  return Results.Created($"/administrator/{adm.Id}", new AdministratorModelView{
      Id = adm.Id,
      Email = adm.Email,
      Profile = adm.Profile,
    });
  
}).WithTags("Administrators");

app.MapGet("/administrators", ([FromQuery] int? page, IAdministratorService administratorService) => {

  var adms = new List<AdministratorModelView>();

  var requestedData = administratorService.GetAll(page);

  foreach (var adm in requestedData)
  {
    adms.Add(new AdministratorModelView{
      Id = adm.Id,
      Email = adm.Email,
      Profile = adm.Profile,
    });
  }
   
  return Results.Ok(adms);

}).WithTags("Administrators");

app.MapGet("/administrator/{id}", ([FromRoute] int id,  IAdministratorService administratorService) => {
  
  var administrator = administratorService.GetById(id);

  if(administrator == null) return Results.NotFound();
   
   return Results.Ok(new AdministratorModelView{
      Id = administrator.Id,
      Email = administrator.Email,
      Profile = administrator.Profile,
    });

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