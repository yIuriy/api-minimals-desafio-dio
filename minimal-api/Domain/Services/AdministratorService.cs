using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using minimal_api.Domain.DTOs;
using minimal_api.Domain.Entitys;
using minimal_api.Infrastructure.Db;
using minimal_api.Infrastructure.Interfaces;

namespace minimal_api.Domain.Services
{
    public class AdministratorService : IAdministratorService
    {
        private readonly DbContextMySql _context;
        public AdministratorService(DbContextMySql context)
        {
            _context = context;
        }

        public Administrator? findById(int id)
        {
            return _context.Administrators.Where(
                a => a.ID == id
            ).FirstOrDefault();
        }

        public List<Administrator> getAll(int? page = 1)
        {
            var query = _context.Administrators.AsQueryable();

            int itensPerPage = 10;

            if (page != null)
            {
                query = query.Skip(((int)page - 1) * itensPerPage).Take(itensPerPage);
            }

            return query.ToList();
        }

        public Administrator? Login(LoginDTO loginDTO)
        {
            var admin = _context.Administrators.Where(
                a => a.Email == loginDTO.Email && a.Password == loginDTO.Password).FirstOrDefault();
            return admin;
        }

        public Administrator save(Administrator administrator)
        {
            _context.Administrators.Add(administrator);
            _context.SaveChanges();
            return administrator;
        }
    }
}