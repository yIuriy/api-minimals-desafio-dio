using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using minimal_api.Domain.Entitys;


[TestClass]
public sealed class VehicleTest
{
    [TestMethod]
    public void TestGetSetProps()
    {
        var vehicle = new Vehicle();

        vehicle.ID = 1;
        vehicle.Name = "Car";
        vehicle.Model = "Super";
        vehicle.Year = 2000;

        Assert.AreEqual(1, vehicle.ID);
        Assert.AreEqual("Car", vehicle.Name);
        Assert.AreEqual("Super", vehicle.Model);
        Assert.AreEqual(2000, vehicle.Year);

    }
}
