using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using minimal_api.Domain.DTOs;
using minimal_api.Domain.Entitys;
using minimal_api.Infrastructure.Db;
using minimal_api.Infrastructure.Interfaces;

namespace minimal_api.Domain.Services
{
    public class VehicleService : IVehicleService
    {
        private readonly DbContextMySql _context;
        public VehicleService(DbContextMySql context)
        {
            _context = context;
        }

        public void delete(Vehicle vehicle)
        {
            _context.Vehicles.Remove(vehicle);
            _context.SaveChanges();
        }

        public Vehicle? findById(int id)
        {
            return _context.Vehicles.Where(
                vehicle => vehicle.ID == id
            ).FirstOrDefault();
        }


        public List<Vehicle>? getAll(int? page = 1, string? name = null, string? model = null)
        {
            var query = _context.Vehicles.AsQueryable();
            if (!string.IsNullOrEmpty(name))
            {
                query = query.Where(
                    vehicle => EF.Functions.Like(vehicle.Name.ToLower(), $"%{name.ToLower()}%")
                );
            }

            int itensPerPage = 10;

            if (page != null)
            {
                query = query.Skip(((int)page - 1) * itensPerPage).Take(itensPerPage);
            }

            return query.ToList();
        }

        public void save(Vehicle vehicle)
        {
            _context.Vehicles.Add(vehicle);
            _context.SaveChanges();
        }

        public void update(Vehicle vehicle)
        {
            _context.Vehicles.Update(vehicle);
            _context.SaveChanges();
        }
    }
}