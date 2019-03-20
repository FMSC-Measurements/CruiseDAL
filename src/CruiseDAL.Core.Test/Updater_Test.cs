using FluentAssertions;
using FMSC.ORM.SQLite;
using System;
using System.IO;
using System.Linq;
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
        public void Migrate_From_V2_Test()
        {
            var testFileName = Guid.NewGuid().ToString() + ".cruise";
            var filePath = Path.Combine(TestTempPath, testFileName);

            try
            {
                // create database file from scratch
                using (var setup = new SQLiteDatastore(filePath))
                {
                    setup.Execute(CruiseDAL.Tests.SQL.CRUISECREATE_2015_01_05);
                }

                using (var datastore = new DAL(filePath))
                {
                    CruiseDAL.Updater.CheckNeedsMajorUpdate(datastore).Should().BeTrue();

                    CruiseDAL.Updater.Migrate_From_V2(datastore);

                    var semVerActual = new Version(datastore.DatabaseVersion);
                    var semVerExpected = new Version("3.0");

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
        public void Migrate_From_V2_Test_With_Existing_File()
        {
            var filePath = Path.Combine(TestFilesDirectory, "7Wolf.cruise");

            try
            {
                using (var datastore = new DAL(filePath))
                {
                    //CruiseDAL.Updater.CheckNeedsMajorUpdate(datastore).Should().BeTrue();

                    CruiseDAL.Updater.Migrate_From_V2(datastore);

                    var semVerActual = new Version(datastore.DatabaseVersion);
                    var semVerExpected = new Version("3.0");

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