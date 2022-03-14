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

namespace CruiseDAL.V3.Test.Schema.Views
{
    public class Tree_TreeDefaultValue_Test : TestBase
    {
        public Tree_TreeDefaultValue_Test(ITestOutputHelper output) : base(output)
        {
        }

        [Fact]
        public void SpeciesAndProductMatch()
        {
            var init = new DatabaseInitializer()
            {
                SampleGroups = new[]
                {
                    new SampleGroup {SampleGroupCode = "sg1", StratumCode = "st1", SamplingFrequency = 101, PrimaryProduct = "01" },
                    new SampleGroup {SampleGroupCode = "sg2", StratumCode = "st1", SamplingFrequency = 101, PrimaryProduct = "20" },
                },
                TreeDefaults = new[]
                {
                    new TreeDefaultValue {SpeciesCode = "sp1", PrimaryProduct = "01"},
                    new TreeDefaultValue {SpeciesCode = "sp1", PrimaryProduct = "20"},
                },
                Subpops = new[]
                {
                    new SubPopulation {SampleGroupCode = "sg1", StratumCode = "st1", SpeciesCode = "sp1", LiveDead = "L" },
                    new SubPopulation {SampleGroupCode = "sg1", StratumCode = "st1", SpeciesCode = "sp2", LiveDead = "L" },
                }
            };

            

            using var db = init.CreateDatabase();

            var sg = init.SampleGroups[0];
            var tree = new Tree
            {
                CruiseID = init.CruiseID,
                TreeID = Guid.NewGuid().ToString(),
                CuttingUnitCode = "u1",
                TreeNumber = 1,
                StratumCode = sg.StratumCode,
                SampleGroupCode = sg.SampleGroupCode,
                SpeciesCode = "sp1",
                LiveDead = "L",
            };
            db.Insert(tree);

            var tree_tdvs = db.From<Tree_TreeDefaultValue>().Query().ToArray();
            var thing = tree_tdvs.Single();
            thing.Should().NotBeNull();

            var tdv = db.From<TreeDefaultValue>().Where("TreeDefaultValue_CN = @p1")
                .Query(thing.TreeDefaultValue_CN).FirstOrDefault();

            tdv.Should().NotBeNull();
            tdv.PrimaryProduct.Should().Be(sg.PrimaryProduct);
        }

        [Fact]
        public void SpeciesMatch()
        {
            var init = new DatabaseInitializer()
            {
                SampleGroups = new[]
                {
                    new SampleGroup {SampleGroupCode = "sg1", StratumCode = "st1", SamplingFrequency = 101, PrimaryProduct = "01" },
                    new SampleGroup {SampleGroupCode = "sg2", StratumCode = "st1", SamplingFrequency = 101, PrimaryProduct = "20" },
                },
                TreeDefaults = new[]
                {
                    new TreeDefaultValue {SpeciesCode = "sp1", PrimaryProduct = null, TreeGrade = "1"},
                    new TreeDefaultValue {SpeciesCode = "sp1", PrimaryProduct = "20", TreeGrade = "2"},
                    new TreeDefaultValue {SpeciesCode = null, PrimaryProduct = null, TreeGrade = "3"},
                },
                Subpops = new[]
                {
                    new SubPopulation {SampleGroupCode = "sg1", StratumCode = "st1", SpeciesCode = "sp1", LiveDead = "L" },
                    new SubPopulation {SampleGroupCode = "sg1", StratumCode = "st1", SpeciesCode = "sp2", LiveDead = "L" },
                }
            };



            using var db = init.CreateDatabase();

            var sg = init.SampleGroups[0];
            var tree = new Tree
            {
                CruiseID = init.CruiseID,
                TreeID = Guid.NewGuid().ToString(),
                CuttingUnitCode = "u1",
                TreeNumber = 1,
                StratumCode = sg.StratumCode,
                SampleGroupCode = sg.SampleGroupCode,
                SpeciesCode = "sp1",
                LiveDead = "L",
            };
            db.Insert(tree);

            var tree_tdvs = db.From<Tree_TreeDefaultValue>().Query().ToArray();
            var thing = tree_tdvs.Single();
            thing.Should().NotBeNull();

            var tdv = db.From<TreeDefaultValue>().Where("TreeDefaultValue_CN = @p1")
                .Query(thing.TreeDefaultValue_CN).FirstOrDefault();

            tdv.Should().NotBeNull();
            tdv.TreeGrade.Should().Be("1");
            //tdv.PrimaryProduct.Should().Be(sg.PrimaryProduct);
        }
    }
}
