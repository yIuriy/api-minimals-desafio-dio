using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using minimal_api.Domain.DTOs;
using minimal_api.Domain.Entitys;

namespace minimal_api.Infrastructure.Interfaces
{
    public interface IVehicleService
    {
        List<Vehicle>? getAll(int? page = 1, string? name = null, string? model = null);

        Vehicle? findById(int id);

        void save(Vehicle vehicle);

        void update(Vehicle vehicle);

        void delete(Vehicle vehicle);
    }
}