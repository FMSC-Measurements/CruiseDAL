using CruiseDAL.V3.Models;
using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace CruiseDAL.V3.Sync
{
    public partial class CruiseSyncer_Test
    {
        [Fact]
        public void Sync_Stratum_Add()
        {
            var fromPath = base.GetTempFilePath(".crz3", "Stratum_Add_fromFile");
            var toPath = base.GetTempFilePath(".crz3", "Stratum_Add_toFile");

            var syncOptions = new CruiseSyncOptions();

            var cruiseID = Guid.NewGuid().ToString();
            var saleID = Guid.NewGuid().ToString();

            using var fromDb = CreateDatabaseFile(fromPath, cruiseID, saleID);

            fromDb.CopyTo(toPath, true);
            using var toDb = new CruiseDatastore_V3(toPath);

            var stratumID = Guid.NewGuid().ToString();
            fromDb.Insert(new Stratum()
            {
                CruiseID = cruiseID,
                StratumID = stratumID,
                StratumCode = "10",
                Method = "100",
            });
            var newStratum = fromDb.From<Stratum>().Where("StratumID = @p1").Query(stratumID).Single();

            var syncer = new CruiseSyncer();
            syncer.Sync(cruiseID, fromDb, toDb, syncOptions);

            var stratumAgain = toDb.From<Stratum>().Where("StratumID =  @p1")
                .Query(stratumID).FirstOrDefault();
            stratumAgain.Should().BeEquivalentTo(newStratum, x => x.Excluding(y => y.Modified_TS));
        }

        [Fact]
        public void Sync_Stratum_Update()
        {
            var fromPath = base.GetTempFilePath(".crz3", "Stratum_Updated_fromFile");
            var toPath = base.GetTempFilePath(".crz3", "Stratum_Update_toFile");

            var syncOptions = new CruiseSyncOptions();

            var cruiseID = Guid.NewGuid().ToString();
            var saleID = Guid.NewGuid().ToString();

            // initialize source database
            using var fromDb = CreateDatabaseFile(fromPath, cruiseID, saleID);
            var stratumID = Guid.NewGuid().ToString();
            var stratum = new Stratum()
            {
                CruiseID = cruiseID,
                StratumID = stratumID,
                StratumCode = "10",
                Method = "100",
            };
            fromDb.Insert(stratum);

            // initialize dest database 
            // as exact copy of source database
            fromDb.CopyTo(toPath, true);
            using var toDb = new CruiseDatastore_V3(toPath);

            // modify stratum value and save to source database
            stratum.Hotkey = Rand.String();
            fromDb.Update(stratum);

            // run sync
            var syncer = new CruiseSyncer();
            syncer.Sync(cruiseID, fromDb, toDb, syncOptions);

            var stratumAgain = toDb.From<Stratum>().Where("StratumID =  @p1")
                .Query(stratumID).FirstOrDefault();
            stratumAgain.Should().BeEquivalentTo(stratum, x => x.Excluding(y => y.Modified_TS));
        }

        [Fact]
        public void Sync_Stratum_Update_StratumCode()
        {
            var fromPath = base.GetTempFilePath(".crz3", "Stratum_Updated_stCode_fromFile");
            var toPath = base.GetTempFilePath(".crz3", "Stratum_Update_stCode_toFile");

            var syncOptions = new CruiseSyncOptions();

            var cruiseID = Guid.NewGuid().ToString();
            var saleID = Guid.NewGuid().ToString();

            // initialize source database
            using var fromDb = CreateDatabaseFile(fromPath, cruiseID, saleID);
            var stratumID = Guid.NewGuid().ToString();
            var stratum = new Stratum()
            {
                CruiseID = cruiseID,
                StratumID = stratumID,
                StratumCode = "10",
                Method = "100",
            };
            fromDb.Insert(stratum);

            // initialize dest database 
            // as exact copy of source database
            fromDb.CopyTo(toPath, true);
            using var toDb = new CruiseDatastore_V3(toPath);

            // modify stratum code and save to source database
            stratum.StratumCode = "11";
            fromDb.Update(stratum);

            // run sync
            var syncer = new CruiseSyncer();
            syncer.Sync(cruiseID, fromDb, toDb, syncOptions);

            var stratumAgain = toDb.From<Stratum>().Where("StratumID =  @p1")
                .Query(stratumID).FirstOrDefault();
            stratumAgain.StratumCode.Should().BeEquivalentTo(stratum.StratumCode);
        }
    }
}
