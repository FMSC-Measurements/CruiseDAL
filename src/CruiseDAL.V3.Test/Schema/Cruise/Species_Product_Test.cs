using CruiseDAL.TestCommon;
using CruiseDAL.V3.Models;
using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace CruiseDAL.V3.Test.Schema.Cruise
{
    public class Species_Product_Test : TestBase
    {
        public Species_Product_Test(ITestOutputHelper output) : base(output)
        {
        }

        [Fact]
        public void UpdateContractSpecies_With_Existing_Species_Product()
        {
            var init = new DatabaseInitializer();
            using var db = init.CreateDatabase();


            var spCode = init.Species.First();

            var spProdRec = new Species_Product
            {
                CruiseID = init.CruiseID,
                SpeciesCode = spCode,
                ContractSpecies = "something",
            };
            db.Insert(spProdRec);
            spProdRec = db.From<Species_Product>().Where("SpeciesCode = @p1").Query(spCode).Single();

            spProdRec.ContractSpecies = "somethingElse";
            db.Update(spProdRec);

            var spRecAgain = db.From<Species>().Where("SpeciesCode = @p1").Query(spCode).Single();
            var spProdRecAgain = db.From<Species_Product>().Where("SpeciesCode = @p1").Query(spCode).Single();

            spRecAgain.ContractSpecies.Should().Be(spProdRec.ContractSpecies);
            spProdRecAgain.ContractSpecies.Should().Be(spProdRec.ContractSpecies);
        }
    }
}
