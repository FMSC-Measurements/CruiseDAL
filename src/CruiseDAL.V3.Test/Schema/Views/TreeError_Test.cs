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
    public class TreeError_Test : TestBase
    {
        public TreeError_Test(ITestOutputHelper output) : base(output)
        {
        }

        [Fact]
        public void TreeError_MeasureTree()
        {
            var init = new DatabaseInitializer();

            using var db = init.CreateDatabase();

            var tree = new Tree
            {
                // just requited fields
                CruiseID = init.CruiseID,
                TreeID = Guid.NewGuid().ToString(),
                CuttingUnitCode = "u1",
                StratumCode = "st1",
                SampleGroupCode = "sg1",

                CountOrMeasure = "M",
            };
            db.Insert(tree);

            var treeErrors = db.From<TreeError>().Query().ToArray();
            treeErrors.Should().NotBeEmpty();
        }
    }
}
