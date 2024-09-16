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

        public Administrator Add(Administrator administrator)
        {
            _context.Administrators.Add(administrator);
            _context.SaveChanges();

            return administrator;
        }

        public List<Administrator> GetAll(int? page)
        {
            var query =_context.Administrators.AsQueryable();

            int itemsPerPage = 3;

            if(page != null){
                query = query.Skip(((int)page -1) * itemsPerPage).Take(itemsPerPage);
            }

            return query.ToList();
        }

         public Administrator? GetById(int id)
        {
            return _context.Administrators.Where(v => v.Id == id).FirstOrDefault();
        }

        public Administrator? Login(LoginDTO loginDTO)
        {
          
            return _context.Administrators.Where(a => a.Email == loginDTO.Email && a.Password == loginDTO.Password).FirstOrDefault();
        }
    }
}