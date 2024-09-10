using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MinimalApiCSharp.Domain.DTOs;
using MinimalApiCSharp.Domain.Entities;

namespace MinimalApiCSharp.Infra.Interfaces
{
    public interface IAdministratorService
    {
        Administrator? Login(LoginDTO loginDTO);

        Administrator Add(Administrator administrator);

        List<Administrator> GetAll(int? page);

        Administrator? GetById(int id);
        
    }
}