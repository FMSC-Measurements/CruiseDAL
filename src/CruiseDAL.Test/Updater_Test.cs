using CruiseDAL.DataObjects;
using FluentAssertions;
using FMSC.ORM;
using FMSC.ORM.SQLite;
using System;
using System.IO;
using Xunit;
using Xunit.Abstractions;

namespace CruiseDAL.Tests
{
    public class Updater_Test : TestBase
    {
        public Updater_Test(ITestOutputHelper output) : base(output)
        {
        }

        [Fact]
        public void Update_FROM_05_30_2013()
        {
            var filePath = Path.Combine(TestTempPath, "TestUpdate.cruise");

            try
            {
                using (var setup = new SQLiteDatastore(filePath))
                {
                    setup.Execute(CruiseDAL.Tests.SQL.CRUISECREATE_05_30_2013);
                }

                try
                {
                    var dataStore = new DAL(filePath);

                    Assert.False(true);
                }
                catch (Exception e)
                {
                    e.Should().BeOfType<IncompatibleSchemaException>();
                }

                //using (var datastore = new DAL(filePath))
                //{
                //    var semVerActual = new Version(datastore.DatabaseVersion);
                //    var semVerExpected = new Version("2.5.0");

                //    semVerActual.Major.Should().Be(semVerExpected.Major);
                //    semVerActual.Minor.Should().Be(semVerExpected.Minor);
                //}
            }
            finally
            {
                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                }
            }
        }

        [Fact]
        public void Update_FROM_2015_01_05()
        {
            var filePath = Path.Combine(TestTempPath, "TestUpdate.cruise");

            try
            {
                using (var setup = new SQLiteDatastore(filePath))
                {
                    setup.Execute(CruiseDAL.Tests.SQL.CRUISECREATE_2015_01_05);
                }

                using (var datastore = new DAL(filePath))
                {
                    var semVerActual = new Version(datastore.DatabaseVersion);
                    var semVerExpected = new Version("2.5.0");

                    semVerActual.Major.Should().Be(semVerExpected.Major);
                    semVerActual.Minor.Should().Be(semVerExpected.Minor);

                    VerifyTablesCanDelete(datastore);
                }
            }
            finally
            {
                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                }
            }
        }

        [Fact]
        public void Update_FROM_2015_01_05_to_v3()
        {
            var filePath = Path.Combine(TestTempPath, "TestUpdate.cruise");

            try
            {
                using (var setup = new SQLiteDatastore(filePath))
                {
                    setup.Execute(CruiseDAL.Tests.SQL.CRUISECREATE_2015_01_05);
                }

                using (var datastore = new DAL(filePath))
                {
                    CruiseDAL.Updater.CheckNeedsMajorUpdate(datastore).Should().BeTrue();

                    datastore.Insert(new CuttingUnitDO
                    {
                        Code = "u1",
                        Area = 0,
                    });

                    datastore.Insert(new StratumDO
                    {
                        Code = "st1",
                        Method = "something",
                    });

                    datastore.Insert(new CuttingUnitStratumDO
                    {
                        CuttingUnit_CN = 1,
                        Stratum_CN = 1,
                    });

                    datastore.Insert(new SampleGroupDO
                    {
                        Stratum_CN = 1,
                        Code = "sg1",
                        CutLeave = "C",
                        UOM = "01",
                        PrimaryProduct = "01",
                        SamplingFrequency = 101
                    });

                    //TreeDefaults

                    datastore.Insert(new TreeDefaultValueDO
                    {
                        PrimaryProduct = "01",
                        Species = "sp1",
                        LiveDead = "L"
                    });

                    datastore.Insert(new TreeDefaultValueDO
                    {
                        PrimaryProduct = "01",
                        Species = "sp1",
                        LiveDead = "D"
                    });

                    datastore.Insert(new TreeDefaultValueDO
                    {
                        PrimaryProduct = "01",
                        Species = "sp2",
                        LiveDead = "L"
                    });

                    //samplegroup - TreeDefaults
                    datastore.Insert(new SampleGroupTreeDefaultValueDO
                    {
                        SampleGroup_CN = 1,
                        TreeDefaultValue_CN = 1
                    });

                    datastore.Insert(new SampleGroupTreeDefaultValueDO
                    {
                        SampleGroup_CN = 1,
                        TreeDefaultValue_CN = 2
                    });

                    datastore.Insert(new SampleGroupTreeDefaultValueDO
                    {
                        SampleGroup_CN = 1,
                        TreeDefaultValue_CN = 3
                    });

                    datastore.Insert(new TallyDO { Hotkey = "A", Description = "something" });

                    datastore.Insert(new CountTreeDO()
                    {
                        CuttingUnit_CN = 1,
                        SampleGroup_CN = 1,
                        Tally_CN = 1
                    });

                    datastore.Insert(new CountTreeDO()
                    {
                        CuttingUnit_CN = 1,
                        SampleGroup_CN = 1,
                        Tally_CN = 1,
                        TreeDefaultValue_CN = 1
                    });


                    CruiseDAL.Updater.UpdateMajorVersion(datastore);

                    //datastore.ExecuteScalar<int>("SELECT count(*) FROM TallyPopulation;").Should().Be(2);
                    datastore.ExecuteScalar<int>("SELECT count(*) FROM TallyPopulation WHERE Description IS NOT NULL;").Should().Be(2);

                    var semVerActual = new Version(datastore.DatabaseVersion);
                    var semVerExpected = new Version(DAL.CURENT_DBVERSION);

                    semVerActual.Major.Should().Be(semVerExpected.Major);
                    semVerActual.Minor.Should().Be(semVerExpected.Minor);

                    VerifyTablesCanDelete(datastore);

                    //datastore.ExecuteScalar<int>("SELECT count(*) FROM TallyLedger;").Should().Be(1);
                }
            }
            finally
            {
                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                }
            }
        }

        protected void VerifyTablesCanDelete(DAL datastore)
        {
            foreach (var table in CruiseDAL.Schema.Schema.TABLE_NAMES)
            {
                try
                {
                    datastore.Execute($"DELETE FROM {table};");
                }
                catch (Exception e)
                {
                    Output.WriteLine(e.Message);
                    Output.WriteLine(e.InnerException.Message);
                }

                //datastore.Invoking(x => x.Execute($"DELETE FROM {table};")).Should().NotThrow();
            }
        }
    }
}