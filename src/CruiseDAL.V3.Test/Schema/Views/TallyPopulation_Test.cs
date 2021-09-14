using CruiseDAL.TestCommon;
using CruiseDAL.V3.Models;
using FluentAssertions;
using System;
using System.Linq;
using Xunit;
using Xunit.Abstractions;

namespace CruiseDAL.V3.Test.Schema.Views
{
    public class TallyPopulation_Test : TestBase
    {
        public TallyPopulation_Test(ITestOutputHelper output) : base(output)
        {
        }

        [Theory]
        [InlineData("3P")]
        [InlineData("F3P")]
        [InlineData("P3P")]
        [InlineData("S3P")]
        public void ReadTallyPop_3P_Test(string method)
        {
            var sts = new[]
            {
                new Stratum
                {
                    StratumID = Guid.NewGuid().ToString(),
                    StratumCode = "st101",
                    Method = method,
                },
            };

            var unitSt = new[]
            {
                new CuttingUnit_Stratum
                {
                    CuttingUnitCode = "u1",
                    StratumCode = sts[0].StratumCode,
                },
            };

            var sgs = new[]
            {
                new SampleGroup
                {
                    SampleGroupID = Guid.NewGuid().ToString(),
                    StratumCode = sts[0].StratumCode,
                    SampleGroupCode = "sg101",
                },
            };

            var subpops = new[]
            {
                new SubPopulation
                {
                    SubPopulationID = Guid.NewGuid().ToString(),
                    StratumCode = sts[0].StratumCode,
                    SampleGroupCode = sgs[0].SampleGroupCode,
                    SpeciesCode = "sp1",
                    LiveDead = "L",
                },

                new SubPopulation
                {
                    SubPopulationID = Guid.NewGuid().ToString(),
                    StratumCode = sts[0].StratumCode,
                    SampleGroupCode = sgs[0].SampleGroupCode,
                    SpeciesCode = "sp2",
                    LiveDead = "L",
                },
            };

            var init = new DatabaseInitializer()
            {
                Strata = sts,
                UnitStrata = unitSt,
                SampleGroups = sgs,
                Subpops = subpops,
                TreeDefaults = null,
            };

            using var db = init.CreateDatabase();

            var subPops = db.From<TallyPopulation>().Where("CruiseID = @p1").Query(init.CruiseID).ToArray();
            subPops.Should().HaveCount(2);
            subPops.Should().OnlyContain(x => x.SpeciesCode != null);
        }


        [Theory]
        [InlineData("STR")]
        [InlineData("FIX")]
        [InlineData("FCM")]
        [InlineData("PNT")]
        [InlineData("PCM")]
        public void ReadTallyPop_Not3P_Test(string method)
        {
            var sts = new[]
            {
                new Stratum
                {
                    StratumID = Guid.NewGuid().ToString(),
                    StratumCode = "st101",
                    Method = method,
                },
            };

            var unitSt = new[]
            {
                new CuttingUnit_Stratum
                {
                    CuttingUnitCode = "u1",
                    StratumCode = sts[0].StratumCode,
                },
            };

            var sgs = new[]
            {
                new SampleGroup
                {
                    SampleGroupID = Guid.NewGuid().ToString(),
                    StratumCode = sts[0].StratumCode,
                    SampleGroupCode = "sg101",
                },
            };

            var subpops = new[]
            {
                new SubPopulation
                {
                    SubPopulationID = Guid.NewGuid().ToString(),
                    StratumCode = sts[0].StratumCode,
                    SampleGroupCode = sgs[0].SampleGroupCode,
                    SpeciesCode = "sp1",
                    LiveDead = "L",
                },

                new SubPopulation
                {
                    SubPopulationID = Guid.NewGuid().ToString(),
                    StratumCode = sts[0].StratumCode,
                    SampleGroupCode = sgs[0].SampleGroupCode,
                    SpeciesCode = "sp2",
                    LiveDead = "L",
                },
            };

            var init = new DatabaseInitializer()
            {
                Strata = sts,
                UnitStrata = unitSt,
                SampleGroups = sgs,
                Subpops = subpops,
                TreeDefaults = null,
            };

            using var db = init.CreateDatabase();

            //var sgAgain = db.From<SampleGroup>().Where("CruiseID = @p1 ").Query(init.CruiseID).ToArray();
            //sgAgain.Should().OnlyContain(x => x.TallyBySubPop == false);


            var subPops = db.From<TallyPopulation>().Where("CruiseID = @p1 ").Query(init.CruiseID).ToArray();
            subPops.Should().HaveCount(1);
            subPops.Should().OnlyContain(x => x.SpeciesCode == null);
        }
    }
}