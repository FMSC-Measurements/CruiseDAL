using AutoBogus;
using CruiseDAL.TestCommon;
using CruiseDAL.V3.Models;
using FluentAssertions;
using System;
using System.Linq;
using Xunit;
using Xunit.Abstractions;

namespace CruiseDAL.V3.Test.Schema.Views
{
    public class TreeMeasurment_DefaultResolved_Test : TestBase
    {
        public TreeMeasurment_DefaultResolved_Test(ITestOutputHelper output) : base(output)
        {
        }

        [Fact]
        public void Read_With_TDV()
        {
            var init = new DatabaseInitializer
            {
                TreeDefaults = null,
            };

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

            var tm = new TreeMeasurment
            {
                TreeID = tree.TreeID,
            };

            db.Insert(tm);

            var tdv = new AutoFaker<TreeDefaultValue>()
                .Ignore(x => x.TreeDefaultValue_CN)
                .Ignore(x => x.CreatedBy)
                .Ignore(x => x.Created_TS)
                .Ignore(x => x.ModifiedBy)
                .Ignore(x => x.Modified_TS)
                .RuleFor(x => x.CruiseID, y => init.CruiseID)
                .Generate();

            tdv.CruiseID = init.CruiseID;
            tdv.SpeciesCode = "sp1";
            tdv.PrimaryProduct = "01";

            db.Insert(tdv);

            var tm_defaults = db.From<TreeMeasurment_DefaultResolved>().Query().Single();

            tm_defaults.Should().NotBeNull();

            tm_defaults.FormClass.Should().Be(tdv.FormClass.ToString());
            tm_defaults.Grade.Should().Be(tdv.TreeGrade);
            tm_defaults.HiddenPrimary.Should().Be(tdv.HiddenPrimary.ToString());
            tm_defaults.RecoverablePrimary.Should().Be(tdv.Recoverable.ToString());
        }

        [Fact]
        public void Read_With_FieldSetup()
        {
            var init = new DatabaseInitializer
            {
                TreeDefaults = null,
            };

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

            var tm = new TreeMeasurment
            {
                TreeID = tree.TreeID,
            };

            db.Insert(tm);

            var tfs = new TreeFieldSetup
            {
                CruiseID = init.CruiseID,
                Field = "DBH",
                StratumCode = "st1",
                SampleGroupCode = "sg1",
                DefaultValueReal = 101.3,
            };
            db.Insert(tfs);

            var tm_defaults = db.From<TreeMeasurment_DefaultResolved>().Query().Single();

            tm_defaults.Should().NotBeNull();

            tm_defaults.DBH.Should().Be("101.3");
        }

        [Fact]
        public void Read_With_FieldSetup_and_TreeDefault()
        {
            var init = new DatabaseInitializer
            {
                TreeDefaults = null,
            };

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

            var tm = new TreeMeasurment
            {
                TreeID = tree.TreeID,
            };

            db.Insert(tm);

            var tfs = new TreeFieldSetup
            {
                CruiseID = init.CruiseID,
                Field = "Grade",
                StratumCode = "st1",
                SampleGroupCode = "sg1",
                DefaultValueText = "09",
            };
            db.Insert(tfs);

            var tdv = new AutoFaker<TreeDefaultValue>()
    .Ignore(x => x.TreeDefaultValue_CN)
    .Ignore(x => x.CreatedBy)
    .Ignore(x => x.Created_TS)
    .Ignore(x => x.ModifiedBy)
    .Ignore(x => x.Modified_TS)
    .RuleFor(x => x.CruiseID, y => init.CruiseID)
    .Generate();

            tdv.CruiseID = init.CruiseID;
            tdv.SpeciesCode = "sp1";
            tdv.PrimaryProduct = "01";

            db.Insert(tdv);

            var tm_defaults = db.From<TreeMeasurment_DefaultResolved>().Query().Single();

            tm_defaults.Should().NotBeNull();

            tm_defaults.FormClass.Should().Be(tdv.FormClass.ToString());
            tm_defaults.Grade.Should().Be(tfs.DefaultValueText);
            tm_defaults.HiddenPrimary.Should().Be(tdv.HiddenPrimary.ToString());
            tm_defaults.RecoverablePrimary.Should().Be(tdv.Recoverable.ToString());
        }
    }
}