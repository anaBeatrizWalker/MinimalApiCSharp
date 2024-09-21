using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using MinimalApiCSharp.Domain.Entities;
using MinimalApiCSharp.Domain.Services;
using MinimalApiCSharp.Infra.DB;

namespace Test.Domain.Services
{
    public class AdministratorServiceTest
    {
        private DataBaseContext CreateTestContext()
        {
            var assemblyPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            var path = Path.GetFullPath(Path.Combine(assemblyPath ?? "", "..", "..", ".."));

            var builder = new ConfigurationBuilder().SetBasePath(path ?? Directory.GetCurrentDirectory()).AddJsonFile("appsettings.json", optional: false, reloadOnChange: true).AddEnvironmentVariables();

            var configuration = builder.Build();

            return new DataBaseContext(configuration);
        }

        [TestMethod]
        public void SaveAdministratorTest()
        {
            //Arrange (variávies)
            var context = CreateTestContext();
            context.Database.ExecuteSqlRaw("TRUNCATE TABLE Administrators");

            var adm = new Administrator();

            adm.Email = "admtest@admtest.com";
            adm.Password = "123456";
            adm.Profile = "Adm";

            var administratorService = new AdministratorService(context);

            //Act (ações)
            administratorService.Add(adm);
            
            //Assert (validações)
            Assert.AreEqual(1, administratorService.GetAll(1).Count());

        }

        [TestMethod]
        public void SaveAdministratorAndGetByIdTest()
        {
            //Arrange (variávies)
            var context = CreateTestContext();
            context.Database.ExecuteSqlRaw("TRUNCATE TABLE Administrators");

            var adm = new Administrator();

            adm.Email = "admtest@admtest.com";
            adm.Password = "123456";
            adm.Profile = "Adm";

            var administratorService = new AdministratorService(context);

            //Act (ações)
            administratorService.Add(adm);
            var newAdm = administratorService.GetById(adm.Id);
            
            //Assert (validações)
            Assert.AreEqual(1, newAdm.Id);
        }
    }
}