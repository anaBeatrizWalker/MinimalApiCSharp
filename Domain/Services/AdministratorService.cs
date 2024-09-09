using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;
using MinimalApiCSharp.Domain.DTOs;
using MinimalApiCSharp.Domain.Entities;
using MinimalApiCSharp.Infra.DB;
using MinimalApiCSharp.Infra.Interfaces;

namespace MinimalApiCSharp.Domain.Services
{
    public class AdministratorService : IAdministratorService
    {
        private readonly DataBaseContext _context;
        public AdministratorService(DataBaseContext db)
        {
            _context = db;
        }
        public Administrator? Login(LoginDTO loginDTO)
        {
          
            return _context.Administrators.Where(a => a.Email == loginDTO.Email && a.Password == loginDTO.Password).FirstOrDefault();
        }
    }
}