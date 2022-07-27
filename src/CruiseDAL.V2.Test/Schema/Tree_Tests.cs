using CruiseDAL.TestCommon;
using CruiseDAL.TestCommon.V2;
using CruiseDAL.V2.Models;
using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace CruiseDAL.V2.Schema
{
    public class Tree_Tests : TestBase
    {
        public Tree_Tests(ITestOutputHelper output) : base(output)
        {
        }

        [Fact]
        public void OnDeleteTree_Test()
        {
            var init = new DatabaseInitializer_V2();
            using var db = init.CreateDatabase();

            var tree = new Tree()
            {
                CuttingUnit_CN = 1,
                Stratum_CN = 1,
                TreeNumber = 1,
            };
            db.Insert(tree);

            db.From<Tree>().Where("Tree_CN = @p1").Count(tree.Tree_CN).Should().Be(1);

            db.Delete(tree);
            db.From<Tree>().Where("Tree_CN = @p1").Count(tree.Tree_CN).Should().Be(0);
            var tombs = db.From<Util_Tombstone>().Where("TableName = 'Tree'").Query().Single();
            tombs.Data.Should().NotBeNullOrWhiteSpace();
            Output.WriteLine(tombs.Data);
        }
    }
}
