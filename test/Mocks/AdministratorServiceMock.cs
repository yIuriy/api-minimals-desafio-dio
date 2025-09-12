using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using minimal_api.Domain.DTOs;
using minimal_api.Domain.Entitys;
using minimal_api.Infrastructure.Interfaces;

namespace test.Mocks
{
    public class AdministratorServiceMock : IAdministratorService
    {

        private static List<Administrator> administrators = new List<Administrator>()
        {
            new Administrator{
                ID = 1,
                Email = "adm@test.com",
                Password = "123456",
                Profile = "Adm"
            },
            new Administrator{
                ID = 2,
                Email = "editor@test.com",
                Password = "123",
                Profile = "Editor"
            }
        }
        ;

        public Administrator? findById(int id)
        {
            return administrators.Find(a => a.ID == id);
        }

        public List<Administrator> getAll(int? page = 1)
        {
            return administrators;
        }

        public Administrator? Login(LoginDTO loginDTO)
        {
            return administrators.Find(a =>
                a.Email == loginDTO.Email && a.Password == loginDTO.Password
             );
        }

        public Administrator save(Administrator administrator)
        {
            administrator.ID = administrators.Count() + 1;
            administrators.Add(administrator);
            return administrator;
        }
    }
}