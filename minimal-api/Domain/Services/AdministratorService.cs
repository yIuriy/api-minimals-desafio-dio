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
        public Administrator? Login(LoginDTO loginDTO)
        {
            var admin = _context.Administrators.Where(
                a => a.Email == loginDTO.Email && a.Password == loginDTO.Password).FirstOrDefault();
            return admin;
        }
    }
}