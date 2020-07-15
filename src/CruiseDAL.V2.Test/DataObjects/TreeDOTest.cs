using CruiseDAL.DataObjects;
using CruiseDAL;
using System.ComponentModel;
using System.Collections.Generic;
using Xunit;
using System;
using FluentAssertions;
using FMSC.ORM;
using System.Linq;

namespace CruiseDAL.V2.DataObjects
{
    public class TreeDOTest
    {
        [Fact]
        public void Tree_GUID_no_unique_constraint()
        {
            using (var ds = new DAL())
            {
                ds.Insert(new CuttingUnitDO() { Code = "u1", CuttingUnit_CN = 1 });
                ds.Insert(new StratumDO() { Code = "st1", Stratum_CN = 1, Method = "something" });
                ds.Insert(new SampleGroupDO() { Code = "sg1", SampleGroup_CN = 1, Stratum_CN = 1, CutLeave = "Something", UOM = "bla", PrimaryProduct = "wha" });

                var tree_guid_1 = Guid.NewGuid();

                ds.Insert(new TreeDO()
                {
                    CuttingUnit_CN = 1,
                    Stratum_CN = 1,
                    SampleGroup_CN = 1,
                    TreeNumber = 1,
                    Tree_GUID = tree_guid_1
                });

                ds.Invoking(x => x.Insert(new TreeDO()
                {
                    CuttingUnit_CN = 1,
                    Stratum_CN = 1,
                    SampleGroup_CN = 1,
                    TreeNumber = 1,
                    Tree_GUID = tree_guid_1
                })).Should().NotThrow();
            }
        }

        [Fact]
        public void Query()
        {
            using(var ds = new DAL())
            {
                ds.Insert(new CuttingUnitDO() { Code = "u1", CuttingUnit_CN = 1 });
                ds.Insert(new StratumDO() { Code = "st1", Stratum_CN = 1, Method = "something" });
                ds.Insert(new SampleGroupDO() { Code = "sg1", SampleGroup_CN = 1, Stratum_CN = 1, CutLeave = "Something", UOM = "bla", PrimaryProduct = "wha" });

                var tree_guid_1 = Guid.NewGuid();

                ds.Insert(new TreeDO()
                {
                    CuttingUnit_CN = 1,
                    Stratum_CN = 1,
                    SampleGroup_CN = 1,
                    TreeNumber = 1,
                    Tree_GUID = tree_guid_1
                });

                //var types = ds.QueryGeneric("SELECT typeof(Tree_GUID) FROM Tree WHERE typeof(Tree_guid) = 'blob';");
                //types.Should().NotBeEmpty();

                var trees = ds.From<TreeDO>().Query().ToArray();

                trees.Should().HaveCount(1);

                var tree = trees.Single();
                tree.Tree_GUID.Should().Be(tree_guid_1);
            }
        }

        [Fact]
        public void Save()
        {
            using (var ds = new DAL())
            {
                ds.Insert(new CuttingUnitDO() { Code = "u1", CuttingUnit_CN = 1 });
                ds.Insert(new StratumDO() { Code = "st1", Stratum_CN = 1, Method = "something" });
                ds.Insert(new SampleGroupDO() { Code = "sg1", SampleGroup_CN = 1, Stratum_CN = 1, CutLeave = "Something", UOM = "bla", PrimaryProduct = "wha" });

                var tree_guid_1 = Guid.NewGuid();

                var tree = new TreeDO(ds)
                {
                    CuttingUnit_CN = 1,
                    Stratum_CN = 1,
                    SampleGroup_CN = 1,
                    TreeNumber = 1,
                    Tree_GUID = tree_guid_1
                };
                tree.Save();

                var trees = ds.From<TreeDO>().Query().ToArray();
                trees.Should().HaveCount(1);
                var treeAgain = trees.Single();
                treeAgain.Should().BeEquivalentTo(tree, config => config
                    .Excluding(x => x.CreatedBy)
                    .Excluding(x => x.CreatedDate)
                    .Excluding(x => x.ModifiedBy)
                    .Excluding(x => x.ModifiedDate)
                    .Excluding(x => x.Self));

                tree.DBH = 101;
                tree.Save();

                trees = ds.From<TreeDO>().Query().ToArray();
                trees.Should().HaveCount(1);
                treeAgain = trees.Single();
                treeAgain.Should().BeEquivalentTo(tree, config => config
                    .Excluding(x => x.CreatedBy)
                    .Excluding(x => x.CreatedDate)
                    .Excluding(x => x.ModifiedBy)
                    .Excluding(x => x.ModifiedDate)
                    .Excluding(x => x.Self));
            }
        }

        [Fact]
        public void Save_with_Datastore()
        {
            using (var ds = new DAL())
            {
                ds.Insert(new CuttingUnitDO() { Code = "u1", CuttingUnit_CN = 1 });
                ds.Insert(new StratumDO() { Code = "st1", Stratum_CN = 1, Method = "something" });
                ds.Insert(new SampleGroupDO() { Code = "sg1", SampleGroup_CN = 1, Stratum_CN = 1, CutLeave = "Something", UOM = "bla", PrimaryProduct = "wha" });

                var tree_guid_1 = Guid.NewGuid();

                var tree = new TreeDO(ds)
                {
                    CuttingUnit_CN = 1,
                    Stratum_CN = 1,
                    SampleGroup_CN = 1,
                    TreeNumber = 1,
                    Tree_GUID = tree_guid_1
                };
                ds.Save(tree);

                var trees = ds.From<TreeDO>().Query().ToArray();
                trees.Should().HaveCount(1);
                var treeAgain = trees.Single();
                treeAgain.Should().BeEquivalentTo(tree, config => config
                    .Excluding(x => x.CreatedBy)
                    .Excluding(x => x.CreatedDate)
                    .Excluding(x => x.ModifiedBy)
                    .Excluding(x => x.ModifiedDate)
                    .Excluding(x => x.Self));

                tree.DBH = 101;
                ds.Save(tree);

                trees = ds.From<TreeDO>().Query().ToArray();
                trees.Should().HaveCount(1);
                treeAgain = trees.Single();
                treeAgain.Should().BeEquivalentTo(tree, config => config
                    .Excluding(x => x.CreatedBy)
                    .Excluding(x => x.CreatedDate)
                    .Excluding(x => x.ModifiedBy)
                    .Excluding(x => x.ModifiedDate)
                    .Excluding(x => x.Self));
            }
        }
    }
}
