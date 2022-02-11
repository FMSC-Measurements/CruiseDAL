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
    public class Tree_Test : TestBase
    {
        public Tree_Test(ITestOutputHelper output) : base(output)
        {
        }

        [Fact]
        public void Update_Tree_SpeciesLiveDead_Cascades_To_TallyLedger_TallyBySp()
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
                SpeciesCode = "sp1",
                LiveDead = "L",
            };
            db.Insert(tree);

            var tl = new TallyLedger
            {
                CruiseID = init.CruiseID,
                TallyLedgerID = Guid.NewGuid().ToString(),
                CuttingUnitCode = "u1",
                StratumCode = "st1",
                SampleGroupCode = "sg1",
                SpeciesCode = "sp1",
                LiveDead = "L",
                TreeID = tree.TreeID,
            };
            db.Insert(tl);


            tree.SpeciesCode = "sp2";
            tree.LiveDead = "D";
            db.Update(tree);

            var tlAgain = db.From<TallyLedger>().Query().First();
            tlAgain.SpeciesCode.Should().Be(tree.SpeciesCode);
            tlAgain.LiveDead.Should().Be(tree.LiveDead);
        }

        [Fact]
        public void Update_Tree_Species_Cascades_To_TallyLedger_TallyBySg()
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
                //SpeciesCode = "sp1",
                //LiveDead = "L",
            };
            db.Insert(tree);

            var tl = new TallyLedger
            {
                CruiseID = init.CruiseID,
                TallyLedgerID = Guid.NewGuid().ToString(),
                CuttingUnitCode = "u1",
                StratumCode = "st1",
                SampleGroupCode = "sg1",
                //SpeciesCode = "sp1",
                //LiveDead = "L",
                TreeID = tree.TreeID,
            };
            db.Insert(tl);


            tree.SpeciesCode = "sp2";
            tree.LiveDead = "D";
            db.Update(tree);

            var tlAgain = db.From<TallyLedger>().Query().First();
            tlAgain.SpeciesCode.Should().Be(tree.SpeciesCode);
            tlAgain.LiveDead.Should().Be(tree.LiveDead);
        }

        [Fact]
        public void Update_Tree_StSg_Cascades_To_TallyLedger()
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


            //tree.CuttingUnitCode = "u2";
            tree.StratumCode = "st2";
            tree.SampleGroupCode = "sg2";
            db.Update(tree);

            var tlAgain = db.From<TallyLedger>().Query().First();
            //tlAgain.CuttingUnitCode.Should().Be(tree.CuttingUnitCode);
            tlAgain.StratumCode.Should().Be(tree.StratumCode);
            tlAgain.SampleGroupCode.Should().Be(tree.SampleGroupCode);
        }
    }
}
