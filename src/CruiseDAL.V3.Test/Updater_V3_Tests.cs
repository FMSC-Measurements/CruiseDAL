using CruiseDAL.TestCommon;
using CruiseDAL.Update;
using CruiseDAL.V3.Models;
using FluentAssertions;
using FMSC.ORM.Core;
using FMSC.ORM.SQLite;
using System.IO;
using System.Linq;
using Xunit;
using Xunit.Abstractions;

namespace CruiseDAL.V3.Test
{
    public class Updater_V3_Tests : TestBase
    {
        public Updater_V3_Tests(ITestOutputHelper output) : base(output)
        {
        }

        //[Fact]
        //public void Update_From_3_0_0()
        //{
        //    var V3_0_0_0filePath = InitializeTestFile("MultiTest.3.0.0.crz3");

        //    using (var ds = new CruiseDatastore(V3_0_0_0filePath))
        //    {
        //        var tallyLedgerDDL = ds.ExecuteScalar<string>("SELECT sql FROM sqlite_master WHERE name = 'TallyLedger';");
        //        tallyLedgerDDL.Should().NotContain("FOREIGN KEY (PlotNumber, CuttingUnitCode) REFERENCES Plot_V3 (PlotNumber, CuttingUnitCode) ON DELETE CASCADE ON UPDATE CASCADE");
        //        var treeCount = ds.ExecuteScalar<int>("SELECT count(*) FROM Tree_V3;");

        //        ds.DatabaseVersion.Should().Be("3.0.0");

        //        var updater = new Updater_V3();

        //        updater.Update(ds);

        //        var tallyLedgerDDLafter = ds.ExecuteScalar<string>("SELECT sql FROM sqlite_master WHERE name = 'TallyLedger';");
        //        tallyLedgerDDLafter.Should().Contain("FOREIGN KEY (PlotNumber, CuttingUnitCode, CruiseID) REFERENCES Plot (PlotNumber, CuttingUnitCode, CruiseID) ON DELETE CASCADE ON UPDATE CASCADE");
        //        tallyLedgerDDLafter.Should().NotBe(tallyLedgerDDL);

        //        var treeCountAfter = ds.ExecuteScalar<int>("SELECT count(*) FROM Tree;");
        //        treeCountAfter.Should().Be(treeCount);

        //        ds.DatabaseVersion.Should().Be(CruiseDatastoreBuilder_V3.DATABASE_VERSION.ToString());
        //        ds.CheckTableExists("TallyLedger_Tree_Totals");
        //    }
        //}

        [InlineData("3.3.0.crz3")]
        [InlineData("3.3.4.crz3")]
        [InlineData("3.5.4.crz3")]
        [Theory]
        public void Update(string fileName)
        {
            var filePath = InitializeTestFile(fileName);

            using (var ds = new CruiseDatastore(filePath))
            {
                var orgDbVersion = ds.DatabaseVersion;
                orgDbVersion.Should().Be(Path.GetFileNameWithoutExtension(fileName));

                var unitCount = ds.GetRowCount("CuttingUnit", "");


                var updater = new Updater_V3();

                updater.Update(ds);

                var unitCountAfter = ds.GetRowCount("CuttingUnit", "");
                unitCountAfter.Should().Be(unitCount);

                var verAfter = ds.DatabaseVersion;
                verAfter.Should().Be(CruiseDatastoreBuilder_V3.DATABASE_VERSION.ToString());

                var tableDefs = CruiseDatastoreBuilder_V3.TABLE_DEFINITIONS;
                foreach (var t in tableDefs)
                {
                    var ti = ds.GetTableInfo(t.TableName);
                    ti.Should().NotBeNullOrEmpty(t.TableName);
                }

                var sale = ds.From<Sale>().Query().Single();
                sale.Should().NotBeNull();

                var cruise = ds.From<Cruise>().Query().Single();
                cruise.SaleNumber.Should().Be(sale.SaleNumber);

                var logs = ds.From<Log>().Query().ToArray();
                if (logs.Any())
                {
                    logs.Should().OnlyContain(x => string.IsNullOrEmpty(x.CruiseID) == false);
                }

                // do integrity check
                var ic_results = ds.QueryScalar<string>("PRAGMA integrity_check;");
                ic_results.Should().HaveCount(1);
                ic_results.Single().Should().Be("ok");
            }
        }

        [InlineData("3.3.0.crz3")]
        [InlineData("3.3.4.crz3")]
        [InlineData("3.5.4.crz3")]
        [Theory]
        public void UpdateNG(string fileName)
        {
            var filePath = InitializeTestFile(fileName);

            using (var ds = new CruiseDatastore(filePath))
            {
                var orgDbVersion = ds.DatabaseVersion;
                orgDbVersion.Should().Be(Path.GetFileNameWithoutExtension(fileName));

                var unitCount = ds.GetRowCount("CuttingUnit", "");


                var updater = new Updater_V3();

                using var connection = ds.OpenConnection();
                updater.HasUpdate(connection).Should().BeTrue();
                updater.Update(connection, ds.ExceptionProcessor);

                var unitCountAfter = ds.GetRowCount("CuttingUnit", "");
                unitCountAfter.Should().Be(unitCount);

                var verAfter = ds.DatabaseVersion;
                verAfter.Should().Be(CruiseDatastoreBuilder_V3.DATABASE_VERSION.ToString());

                var tableDefs = CruiseDatastoreBuilder_V3.TABLE_DEFINITIONS;
                foreach (var t in tableDefs)
                {
                    var ti = ds.GetTableInfo(t.TableName);
                    ti.Should().NotBeNullOrEmpty(t.TableName);
                }

                var sale = ds.From<Sale>().Query().Single();
                sale.Should().NotBeNull();

                var cruise = ds.From<Cruise>().Query().Single();
                cruise.SaleNumber.Should().Be(sale.SaleNumber);

                var logs = ds.From<Log>().Query().ToArray();
                if (logs.Any())
                {
                    logs.Should().OnlyContain(x => string.IsNullOrEmpty(x.CruiseID) == false);
                }

                // do integrity check
                var ic_results = ds.QueryScalar<string>("PRAGMA integrity_check;");
                ic_results.Should().HaveCount(1);
                ic_results.Single().Should().Be("ok");
            }
        }

        [Fact]
        public void UpdateFrom340_To_342()
        {
            var filePath = InitializeTestFile("UpdateFrom340_To_342.crz3");

            using var ds = new CruiseDatastore(filePath);

            var updater = new Updater_V3();
            updater.Update(ds);

            ds.Invoking(x => x.Execute("DELETE FROM Plot_Stratum;")).Should().NotThrow<FMSC.ORM.SQLException>();
        }

        [Fact]
        public void UpdateFrom354_To_355()
        {
            var filePath = InitializeTestFile("3.5.4.crz3");

            using var ds = new CruiseDatastore(filePath);

            ds.Execute("UPDATE Sale SET District = 'something';");

            var updateTo355 = new UpdateTo_3_5_5();

            using var conn = ds.OpenConnection();
            updateTo355.Invoking( x => x.Update(conn)).Should().NotThrow();

            var saleAfter = ds.From<Sale>().Query().Single();
            saleAfter.District.Should().Be("so");
        }
    }
}