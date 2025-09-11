using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using minimal_api.Domain.DTOs;
using minimal_api.Domain.Entitys;

namespace minimal_api.Infrastructure.Interfaces
{
    public interface IAdministratorService
    {
        Administrator? Login(LoginDTO loginDTO);

        Administrator save(Administrator administrator);

        List<Administrator> getAll(int? page = 1);

        Administrator? findById(int id);

    }
}