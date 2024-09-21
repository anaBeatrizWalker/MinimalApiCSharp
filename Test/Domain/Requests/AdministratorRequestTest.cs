using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using MinimalApiCSharp.Domain.DTOs;
using MinimalApiCSharp.Domain.Entities;
using MinimalApiCSharp.Domain.ModelViews;
using Test.Helpers;

namespace Test.Domain.Requests
{
    public class AdministratorRequestTest
    {
        [ClassInitialize]
        public static void ClassInit(TestContext testContext)
        {
            Setup.ClassInit(testContext);
        }

        [ClassCleanup]
        public static void ClassCleanup()
        {
            Setup.ClassCleanup();
        }


        [TestMethod]
        public async Task GetSetPropsTest()
        {
            //Arrange (variávies)
            var loginDTO = new LoginDTO{
                Email = "adm@test.com",
                Password= "123456"
            };

            var content = new StringContent(JsonSerializer.Serialize(loginDTO), Encoding.UTF8, "Application/json");

            //Act (ações)
            var response = await Setup.client.PostAsync("/adm/login", content);

            //Assert (validações)
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);

            var result = await response.Content.ReadAsStringAsync();
            var loggedAdm = JsonSerializer.Deserialize<LoggedAdm>(result, new JsonSerializerOptions{PropertyNameCaseInsensitive = true});

            Assert.IsNotNull(loggedAdm?.Email ?? "");
            Assert.IsNotNull(loggedAdm?.Profile ?? "");
            Assert.IsNotNull(loggedAdm?.Token ?? "");

           Console.WriteLine("Adm logged:");
        }
    }
}