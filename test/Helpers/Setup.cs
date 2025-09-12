using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using minimal_api;
using minimal_api.Infrastructure.Db;
using Microsoft.AspNetCore.Mvc.Testing;

namespace test.Helpers
{
    public class Setup
    {
        public const string PORT = "5001";
        public static TestContext testContext = default!;
        public static WebApplicationFactory<Program> http = default!;
        public static HttpClient client = default!;

        

        




    }
}