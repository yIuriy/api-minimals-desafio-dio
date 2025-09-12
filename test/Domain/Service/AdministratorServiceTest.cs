using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using minimal_api.Domain.Entitys;
using minimal_api.Domain.Services;
using minimal_api.Infrastructure.Db;

namespace test.Domain.Service
{



    [TestClass]
    public sealed class AdministratorServiceTest
    {
        private DbContextMySql CreateTestContext()
        {
            var assemblyPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            var path = Path.GetFullPath(Path.Combine(assemblyPath ?? "", "..", "..", ".."));
            var builder = new ConfigurationBuilder()
            .SetBasePath(path
            ).AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .AddEnvironmentVariables();

            var configuration = builder.Build();

            return new DbContextMySql(configuration);
        }
        [TestMethod]
        public void TestingSaveAdministrator()
        {
            var context = CreateTestContext();
            context.Database.ExecuteSqlRaw("TRUNCATE TABLE administrators");

            var adm = new Administrator();
            adm.Email = "test@test.com";
            adm.Password = "123456";
            adm.Profile = "Adm";

            var admService = new AdministratorService(context);

            admService.save(adm);

            Assert.AreEqual(1, admService.getAll().Count());
        }
        [TestMethod]
        public void TestingSaveAdministratorAndFindById()
        {
            var context = CreateTestContext();
            context.Database.ExecuteSqlRaw("TRUNCATE TABLE administrators");

            var adm = new Administrator();
            adm.Email = "test@test.com";
            adm.Password = "123456";
            adm.Profile = "Adm";

            var admService = new AdministratorService(context);

            // admService.save(adm);
            var admFind = admService.findById(1);

            Assert.AreEqual(1, admFind.ID);
        }
    }
}