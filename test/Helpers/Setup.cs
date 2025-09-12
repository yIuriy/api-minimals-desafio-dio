using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using minimal_api;
using minimal_api.Infrastructure.Db;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using minimal_api.Domain.Entitys;
using minimal_api.Infrastructure.Interfaces;
using test.Mocks;

namespace test.Helpers
{
    public class Setup
    {
        public const string PORT = "5001";
        public static TestContext testContext = default!;
        public static WebApplicationFactory<Program> http = default!;
        public static HttpClient client = default!;

        public static void ClassInit(TestContext testContext)
        {
            Setup.testContext = testContext;
            http = new WebApplicationFactory<Program>();

            http = http.WithWebHostBuilder(builder =>
            {
                builder.UseSetting("https_port", Setup.PORT).UseEnvironment("Testing");

                builder.ConfigureServices(services =>
                {
                    services.AddScoped<IAdministratorService, AdministratorServiceMock>();
                });
            });

            client = http.CreateClient();
        }
        public static void ClassCleanup()
        {
            http.Dispose();
        }
    }
}