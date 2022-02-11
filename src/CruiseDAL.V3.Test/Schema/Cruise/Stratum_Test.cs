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
    public class Stratum_Test : TestBase
    {
        public Stratum_Test(ITestOutputHelper output) : base(output)
        {
        }


        [Fact]
        public void UpdateStratumCodeCascade()
        {
            var init = new DatabaseInitializer();
            using var db = init.CreateDatabase();

            var tree = new Tree
            {
                CruiseID = init.CruiseID,
                TreeID = Guid.NewGuid().ToString(),
                TreeNumber = 1,
                CuttingUnitCode = "u1",
                StratumCode = "st1",
                SampleGroupCode = "sg1",
            };
            db.Insert(tree);

            var tl = new TallyLedger
            {
                CruiseID = init.CruiseID,
                TallyLedgerID = Guid.NewGuid().ToString(),
                CuttingUnitCode = "u1",
                StratumCode = "st1",
                SampleGroupCode = "sg1",
                TreeID = tree.TreeID,
            };
            db.Insert(tl);

            var unitStratum = db.From<CuttingUnit_Stratum>().Where("StratumCode = 'st1'").Query().First();

            var stratum = db.From<Stratum>().Where("StratumCode = 'st1'").Query().Single();
            stratum.StratumCode = "st101";
            db.Update(stratum);

            var unitStratumAgain = db.From<CuttingUnit_Stratum>().Where("CuttingUnit_Stratum_CN = @p1").Query(unitStratum.CuttingUnit_Stratum_CN).Single();
            unitStratumAgain.StratumCode.Should().Be(stratum.StratumCode);

            var tlAgain = db.From<TallyLedger>().Query().First();
            tlAgain.StratumCode.Should().Be(stratum.StratumCode);

            var treeAgain = db.From<Tree>().Query().First();
            treeAgain.StratumCode.Should().Be(stratum.StratumCode);
        }
    }
}
