using CruiseDAL.TestCommon;
using CruiseDAL.V3.Models;
using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace CruiseDAL.V3.Test.Schema.Cruise
{
    public class TallyLedger_Test
    {
        [Fact]
        public void ModifiedByTS_Test()
        {
            var init = new DatabaseInitializer();
            using var db = init.CreateDatabase();

            var tallyLedger = new TallyLedger()
            {
                CruiseID = init.CruiseID,
                TallyLedgerID = Guid.NewGuid().ToString(),
                CuttingUnitCode = "u1",
                StratumCode = "st1",
                SampleGroupCode = "sg1",
                SpeciesCode = "sp1",
                LiveDead = "L",
                //Modified_TS = DateTime.Today,
            };

            db.Insert(tallyLedger);

            var tallyLedgerAgain = db.From<TallyLedger>().Query().Single();
            tallyLedgerAgain.Modified_TS.Should().BeNull();

            db.Execute("UPDATE TallyLedger SET TreeCount = TreeCount + 1 WHERE TallyLedgerID = @p1", tallyLedger.TallyLedgerID);



            var tallyLedgerAgianAgain = db.From<TallyLedger>().Query().Single();
            tallyLedgerAgianAgain.Modified_TS.Should().NotBeNull();

            

            Thread.Sleep(1000);

            db.Execute("UPDATE TallyLedger SET TreeCount = TreeCount + 1 WHERE TallyLedgerID = @p1", tallyLedger.TallyLedgerID);

            var tallyLedgerAgianAgainAgain = db.From<TallyLedger>().Query().Single();
            tallyLedgerAgianAgainAgain.TreeCount.Should().Be(2);
            tallyLedgerAgianAgainAgain.Modified_TS.Should().NotBe(tallyLedgerAgianAgain.Modified_TS.Value);

        }

        [Fact]
        public void CurrentTimeStamp()
        {
            var init = new DatabaseInitializer();
            using var db = init.CreateDatabase();

            var currTS = db.ExecuteScalar("SELECT CURRENT_TIMESTAMP");

            Thread.Sleep(1000);

            var currTSAgain = db.ExecuteScalar("SELECT CURRENT_TIMESTAMP");
            currTSAgain.Should().NotBe(currTS);
        }
    }

    
}
