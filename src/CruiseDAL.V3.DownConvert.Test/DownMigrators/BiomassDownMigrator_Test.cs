using AutoBogus;
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

namespace CruiseDAL.V3.DownConvert.Test.DownMigrators
{
    public class BiomassDownMigrator_Test : TestBase
    {
        public BiomassDownMigrator_Test(ITestOutputHelper output) : base(output)
        {
        }

        [Fact]
        public void DownMigrate()
        {
            var init = new DatabaseInitializer();
            var fromDbPath = GetTempFilePathWithExt(".crz3");
            RegesterFileForCleanUp(fromDbPath);

            using var db = init.CreateDatabaseFile(fromDbPath);

            var bioMassEq = AutoFaker.Generate<BiomassEquation>();
            bioMassEq.CruiseID = init.CruiseID;

            db.Insert(bioMassEq);

            var toDbPath = GetTempFilePathWithExt(".cruise");
            RegesterFileForCleanUp(toDbPath);
            var toDb = new DAL(toDbPath, true);

            var downMigrator = new DownMigrator();
            downMigrator.MigrateFromV3ToV2(init.CruiseID, db, toDb);

            var biomassEqsAgain = toDb.From<CruiseDAL.V2.Models.BiomassEquation>()
                .Query().ToArray();
            biomassEqsAgain.Should().HaveCount(1);

            var biomassEqAgain = biomassEqsAgain.Single();
            biomassEqAgain.Should().BeEquivalentTo(bioMassEq, config: x => 
            x.Excluding( y => y.CruiseID)
            .Excluding( y => y.CreatedBy)
            .Excluding( y => y.Created_TS)
            .Excluding( y => y.ModifiedBy)
            .Excluding( y => y.Modified_TS));



            
        }
    }
}
