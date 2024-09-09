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