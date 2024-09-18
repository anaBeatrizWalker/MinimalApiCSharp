using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MinimalApiCSharp.Domain.Entities;

namespace Test.Domain.Entities
{
    public class AdministratorTest
    {
         [TestMethod]
        public void GetSetPropsTest()
        {
            //Arrange (variávies)
            var adm = new Administrator();

            //Act (ações)
            adm.Id = 1;
            adm.Email = "admtest@admtest.com";
            adm.Password = "123456";
            adm.Profile = "Adm";

            //Assert (validações)
            Assert.AreEqual(1, adm.Id);
            Assert.AreEqual("admtest@admtest.com", adm.Email);
            Assert.AreEqual("123456", adm.Password);
            Assert.AreEqual("Adm", adm.Profile);

        }
    }
}