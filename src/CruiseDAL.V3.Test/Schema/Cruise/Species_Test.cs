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
    public class Species_Test : TestBase
    {
        public Species_Test(ITestOutputHelper output) : base(output)
        {
        }

        [Fact]
        public void UpdateContractSpecies()
        {
            var init = new DatabaseInitializer();
            using var db = init.CreateDatabase();


            var spCode = init.Species.First();

            var spRec = db.From<Species>().Where("SpeciesCode = @p1").Query(spCode).Single();
            var spProdRec = db.From<Species_Product>().Where("SpeciesCode = @p1").Query(spCode).Single();


            spRec.ContractSpecies = "somethingElse";
            db.Update(spRec);

            var spRecAgain = db.From<Species>().Where("SpeciesCode = @p1").Query(spCode).Single();
            var spProdRecAgain = db.From<Species_Product>().Where("SpeciesCode = @p1").Query(spCode).Single();

            spRecAgain.ContractSpecies.Should().Be(spRec.ContractSpecies);
            spProdRecAgain.ContractSpecies.Should().Be(spRec.ContractSpecies);
        }


        // due to confusing behavior of using a INSERT...ON CLONLICT command with a trigger that 
        // does a INSERT OR REPLACE. The INSERT OR REPLACE in the trigger changes its behavior to 
        // that of the parent standard INSERT. See https://sqlite.org/forum/info/8c8de6ff91cb602b3d636d391b9bfb58ccb275e2623df00305d5c3ee648a9f1a

        [Fact]
        public void UpdateContractSpecies_LikeNatCruiseDoes()
        {
            var init = new DatabaseInitializer();
            using var db = init.CreateDatabase();


            var spCode = init.Species.First();

            var spRec = db.From<Species>().Where("SpeciesCode = @p1").Query(spCode).Single();
            var spProdRec = db.From<Species_Product>().Where("SpeciesCode = @p1").Query(spCode).Single();


            spRec.ContractSpecies = "somethingElse";
            //db.Execute2("UPDATE Species SET " +
            //    "ContractSpecies = @ContractSpecies, " +
            //    "FIACode = @FIACode " +
            //    "WHERE CruiseID = @CruiseID AND SpeciesCode = @SpeciesCode;",
            //    new
            //    {
            //        init.CruiseID,
            //        spRec.SpeciesCode,
            //        spRec.ContractSpecies,
            //        spRec.FIACode,
            //    });

            db.Execute2(
@"INSERT INTO Species (
    CruiseID,
    SpeciesCode,
    ContractSpecies,
    FIACode
) VALUES (
    @CruiseID,
    @SpeciesCode,
    @ContractSpecies,
    @FIACode
)
ON CONFLICT (CruiseID, SpeciesCode) DO
UPDATE SET
    ContractSpecies = @ContractSpecies,
    FIACode = @FIACode
WHERE CruiseID = @CruiseID AND SpeciesCode = @SpeciesCode;",
new
{
    init.CruiseID,
    spRec.SpeciesCode,
    spRec.ContractSpecies,
    spRec.FIACode,
});



            var spRecAgain = db.From<Species>().Where("SpeciesCode = @p1").Query(spCode).Single();
            var spProdRecAgain = db.From<Species_Product>().Where("SpeciesCode = @p1").Query(spCode).Single();

            spRecAgain.ContractSpecies.Should().Be(spRec.ContractSpecies);
            spProdRecAgain.ContractSpecies.Should().Be(spRec.ContractSpecies);
        }
    }
}
