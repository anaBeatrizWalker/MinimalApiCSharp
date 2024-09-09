using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MinimalApiCSharp.Domain.Entities;
using MinimalApiCSharp.Domain.Interfaces;
using MinimalApiCSharp.Infra.DB;

namespace MinimalApiCSharp.Domain.Services
{
    public class VehicleService : IVehicleService
    {
        private readonly DataBaseContext _context;
        public VehicleService(DataBaseContext db)
        {
            _context = db;
        }
        public void Add(Vehicle vehicle)
        {
           _context.Vehicles.Add(vehicle);
           _context.SaveChanges();
        }

        public void Delete(Vehicle vehicle)
        {
           _context.Vehicles.Remove(vehicle);
           _context.SaveChanges();
        }

        public List<Vehicle> GetAll(int page = 1, string? name = null, string? brand = null)
        {
            var query =_context.Vehicles.AsQueryable();

            if(!string.IsNullOrEmpty(name)){
                query = query.Where(v => EF.Functions.Like(v.Name.ToLower(), $"%{name.ToLower()}%"));
            }

            int itemsPerPage = 3;

            query = query.Skip((page -1) * itemsPerPage).Take(itemsPerPage);

            return query.ToList();
        }

        public Vehicle? GetById(int id)
        {
            return _context.Vehicles.Where(v => v.Id == id).FirstOrDefault();
        }

        public void Update(Vehicle vehicle)
        {
            _context.Vehicles.Update(vehicle);
            _context.SaveChanges();
        }
    }
}