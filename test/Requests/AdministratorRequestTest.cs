using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using minimal_api.Domain.DTOs;
using minimal_api.Domain.ModelViews;
using test.Helpers;

namespace test.Requests
{
    [TestClass]
    public class AdministratorRequestTest
    {

        [ClassInitialize]
        public static void ClassInit(TestContext testContext)
        {
            Setup.ClassInit(testContext);
        }

        [ClassCleanup]
        public static void ClassCleanup()
        {
            Setup.ClassCleanup();
        }

        [TestMethod]
        public async Task TestGetSetProps()
        {
            var loginDTO = new LoginDTO
            {
                Email = "adm@test.com",
                Password = "123456"
            };

            var content = new StringContent(JsonSerializer.Serialize(loginDTO), Encoding.UTF8, "Application/json");

            var response = await Setup.client.PostAsync("/admin/login", content);

            Assert.AreEqual(200, (int)response.StatusCode);

            var result = await response.Content.ReadAsStringAsync();

            var loggedAdm = JsonSerializer.Deserialize<LoggedAdministrator>(result, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            Assert.IsNotNull(loggedAdm?.Profile ?? "");
            Assert.IsNotNull(loggedAdm?.Email ?? "");
            Assert.IsNotNull(loggedAdm?.Token ?? "");
        }
    }
}