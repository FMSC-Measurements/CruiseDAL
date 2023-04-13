using CruiseDAL.TestCommon;
using CruiseDAL.Update;
using CruiseDAL.V3.Models;
using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace CruiseDAL.V3.Test.Update
{
    public class UpdateTo_363_Test : TestBase
    {
        public UpdateTo_363_Test(ITestOutputHelper output) : base(output)
        {
        }

        [Fact]
        public void UpdateFrom362()
        {
            var filePath = InitializeTestFile("361_AllTables.crz3");

            using var db = new CruiseDatastore(filePath);
            var cruise = db.From<Cruise>().Query().Single();

            db.Insert(new Species_Product
            {
                CruiseID = cruise.CruiseID,
                SpeciesCode = "sp1",
                ContractSpecies = "",
            });

            db.Insert(new Species_Product
            {
                CruiseID = cruise.CruiseID,
                SpeciesCode = "sp2",
                ContractSpecies = "123",
            });

            var updateTo363 = new UpdateTo_3_6_3();

            using var conn = db.OpenConnection();
            updateTo363.Update(conn);

            db.From<Species_Product>()
                .Where("SpeciesCode = 'sp1'")
                .Query().FirstOrDefault().Should().BeNull();

            var spProd = db.From<Species_Product>()
                .Where("SpeciesCode = 'sp2'")
                .Query().FirstOrDefault();
            spProd.Should().NotBeNull();

        }
    }
}
