using FluentAssertions;
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
                    setup.Execute(CruiseDAL.Test.SQL.CRUISECREATE_05_30_2013);
                }

                using (var datastore = new DAL(filePath))
                {
                    var semVerActual = new Version(datastore.DatabaseVersion);
                    var semVerExpected = new Version("2.5.0");

                    semVerActual.Major.Should().Be(semVerExpected.Major);
                    semVerActual.Minor.Should().Be(semVerExpected.Minor);
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
        public void Update_FROM_05_30_2013_to_v3()
        {
            var filePath = Path.Combine(TestTempPath, "TestUpdate.cruise");

            try
            {
                using (var setup = new SQLiteDatastore(filePath))
                {
                    setup.Execute(CruiseDAL.Test.SQL.CRUISECREATE_05_30_2013);
                }

                using (var datastore = new DAL(filePath))
                {
                    CruiseDAL.Updater.CheckNeedsMajorUpdate(datastore).Should().BeTrue();
                    CruiseDAL.Updater.UpdateMajorVersion(datastore);

                    var semVerActual = new Version(datastore.DatabaseVersion);
                    var semVerExpected = new Version(DAL.CURENT_DBVERSION);

                    semVerActual.Major.Should().Be(semVerExpected.Major);
                    semVerActual.Minor.Should().Be(semVerExpected.Minor);
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
    }
}