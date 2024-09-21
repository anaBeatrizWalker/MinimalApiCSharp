using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using MinimalApiCSharp.Domain.DTOs;
using MinimalApiCSharp.Domain.Entities;
using MinimalApiCSharp.Domain.Enums;
using MinimalApiCSharp.Domain.Interfaces;
using MinimalApiCSharp.Domain.ModelViews;
using MinimalApiCSharp.Domain.Services;
using MinimalApiCSharp.Infra.DB;
using MinimalApiCSharp.Infra.Interfaces;

namespace Api
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
            key = Configuration?.GetSection("Jwt").ToString() ?? "";
        }

        public IConfiguration Configuration {get; set;} = default!;
        private string key = "";

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddAuthentication(option => {
                option.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                option.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                }).AddJwtBearer(option => {
                option.TokenValidationParameters = new TokenValidationParameters{
                    ValidateLifetime = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key)),
                    ValidateIssuer = false,
                    ValidateAudience = false
                };
                });

                services.AddAuthorization();

                services.AddScoped<IAdministratorService, AdministratorService>();
                services.AddScoped<IVehicleService, VehicleService>();

                services.AddEndpointsApiExplorer();
                services.AddSwaggerGen(options => {
                options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme{
                    Name = "Authorization",
                    Type = SecuritySchemeType.Http,
                    Scheme = "bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "Enter the JWT Token"
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

                services.AddDbContext<DataBaseContext>(options => {
                        options.UseMySql(Configuration.GetConnectionString("Mysql"), 
                        ServerVersion.AutoDetect(Configuration.GetConnectionString("Mysql")));
                });
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseSwagger();
            app.UseSwaggerUI();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoint => {
                
                #region Home
                endpoint.MapGet("/", () => Results.Json(new Home())).AllowAnonymous().WithTags("Home");
                #endregion

                #region Adm
                string GenerateJwtToken(Administrator administrator){
                if(string.IsNullOrEmpty(key)) return string.Empty;

                var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
                var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

                var claims = new List<Claim>(){
                    new Claim("email", administrator.Email),
                    new Claim("profile", administrator.Profile),
                    new Claim(ClaimTypes.Role, administrator.Profile),
                };
                var token = new JwtSecurityToken(
                    claims: claims,
                    expires: DateTime.Now.AddDays(1),
                    signingCredentials: credentials
                );

                return new JwtSecurityTokenHandler().WriteToken(token);
                }

                endpoint.MapPost("/adm/login", ([FromBody] LoginDTO loginDTO, IAdministratorService administratorService) => {
                    var adm = administratorService.Login(loginDTO);
                    if(adm != null){
                        string token = GenerateJwtToken(adm);
                        return Results.Ok(new LoggedAdm{
                        Email = adm.Email,
                        Profile = adm.Profile,
                        Token = token
                        });
                    }else {
                        return Results.Unauthorized();
                    }
                }).AllowAnonymous().WithTags("Administrators");

                endpoint.MapPost("/administrator", ([FromBody] AdministratorDTO administratorDTO, IAdministratorService administratorService) => {
                
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
                
                })
                .RequireAuthorization()
                .RequireAuthorization(new AuthorizeAttribute {Roles = "Adm"})
                .WithTags("Administrators");

                endpoint.MapGet("/administrators", ([FromQuery] int? page, IAdministratorService administratorService) => {

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

                })
                .RequireAuthorization()
                .RequireAuthorization(new AuthorizeAttribute {Roles = "Adm"})
                .WithTags("Administrators");

                endpoint.MapGet("/administrator/{id}", ([FromRoute] int id,  IAdministratorService administratorService) => {
                
                var administrator = administratorService.GetById(id);

                if(administrator == null) return Results.NotFound();
                
                return Results.Ok(new AdministratorModelView{
                    Id = administrator.Id,
                    Email = administrator.Email,
                    Profile = administrator.Profile,
                    });

                })
                .RequireAuthorization()
                .RequireAuthorization(new AuthorizeAttribute {Roles = "Adm"})
                .WithTags("Administrators");
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

                endpoint.MapPost("/vehicle", ([FromBody] VehicleDTO vehicleDTO, IVehicleService vehicleService) => {
                
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
                })
                .RequireAuthorization()
                .RequireAuthorization(new AuthorizeAttribute {Roles = "Adm, Editor"})
                .WithTags("Vehicles");

                endpoint.MapGet("/vehicles", ([FromQuery] int? page, IVehicleService vehicleService) => {
                var vehicles = vehicleService.GetAll(page);
                
                return Results.Ok(vehicles);
                }).RequireAuthorization().WithTags("Vehicles");

                endpoint.MapGet("/vehicle/{id}", ([FromRoute] int id, IVehicleService vehicleService) => {
                var vehicle = vehicleService.GetById(id);

                if(vehicle == null) return Results.NotFound();
                
                return Results.Ok(vehicle);
                })
                .RequireAuthorization()
                .RequireAuthorization(new AuthorizeAttribute {Roles = "Adm, Editor"})
                .WithTags("Vehicles");

                endpoint.MapPut("/vehicle/{id}", ([FromRoute] int id, VehicleDTO vehicleDTO, IVehicleService vehicleService) => {
                
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
                })
                .RequireAuthorization()
                .RequireAuthorization(new AuthorizeAttribute {Roles = "Adm"})
                .WithTags("Vehicles");

                endpoint.MapDelete("/vehicle/{id}", ([FromRoute] int id, IVehicleService vehicleService) => {
                var vehicle = vehicleService.GetById(id);

                if(vehicle == null) return Results.NotFound();

                vehicleService.Delete(vehicle);
                
                return Results.NoContent();
                })
                .RequireAuthorization()
                .RequireAuthorization(new AuthorizeAttribute {Roles = "Adm"})
                .WithTags("Vehicles");
                #endregion
            });
        }
    }
}