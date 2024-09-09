using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MinimalApiCSharp.Domain.Entities;

namespace MinimalApiCSharp.Infra.DB
{
    public class DataBaseContext : DbContext
    {
        private readonly IConfiguration _configAppSettings;
        public DataBaseContext(IConfiguration configAppSettings)
        {
            _configAppSettings = configAppSettings;
        }

        public DbSet<Administrator> Administrators {get; set;} = default!;
        public DbSet<Vehicle> Vehicles {get; set;} = default!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Administrator>().HasData(
                new Administrator {
                    Id = 1,
                    Email = "administrador@adm.com",
                    Password = "123456",
                    Profile = "Adm"
                }
            );
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if(!optionsBuilder.IsConfigured){
                var connectionString = _configAppSettings.GetConnectionString("mysql")?.ToString();

                if(!string.IsNullOrEmpty(connectionString))
                {
                    optionsBuilder.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));
                }
            }

        }
    }
}