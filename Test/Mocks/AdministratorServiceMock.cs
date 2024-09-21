using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MinimalApiCSharp.Domain.DTOs;
using MinimalApiCSharp.Domain.Entities;
using MinimalApiCSharp.Infra.Interfaces;

namespace Test.Mocks
{
    public class AdministratorServiceMock : IAdministratorService
    {
        private static List<Administrator> administrators = new List<Administrator>(){
            new Administrator {
                Id = 1,
                Email = "adm@test.com",
                Password = "123456",
                Profile = "Adm"
            },
            new Administrator {
                Id = 2,
                Email = "editor@test.com",
                Password = "123456",
                Profile = "Editor"
            }
        };

        public Administrator Add(Administrator administrator)
        {
            administrator.Id = administrators.Count() + 1;
            administrators.Add(administrator);
            return administrator;
        }

        public List<Administrator> GetAll(int? page)
        {
           return administrators;
        }

        public Administrator? GetById(int id)
        {
            return administrators.Find(item => item.Id == id);
        }

        public Administrator? Login(LoginDTO loginDTO)
        {
            return administrators.Find(item => item.Email == loginDTO.Email && item.Password == loginDTO.Password);
        }
    }
}