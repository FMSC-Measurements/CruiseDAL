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
                    CruiseDAL.Updater.UpdateMajorVersion(datastore);

                    var semVerActual = new Version(datastore.DatabaseVersion);
                    var semVerExpected = new Version(DAL.CURENT_DBVERSION);

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