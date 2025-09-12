using minimal_api.Domain.Entitys;

namespace test.Domain.Entitys;

[TestClass]
public sealed class AdministratorTest
{
    [TestMethod]
    public void TestGetSetProps()
    {
        var adm = new Administrator();

        adm.ID = 1;
        adm.Email = "test@test.com";
        adm.Password = "123456";
        adm.Profile = "Adm";

        Assert.AreEqual(1, adm.ID);
        Assert.AreEqual("test@test.com", adm.Email);
        Assert.AreEqual("123456", adm.Password);
        Assert.AreEqual("Adm", adm.Profile);
    }
}
