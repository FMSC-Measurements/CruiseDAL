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
            // clean up the test temp path
            foreach (var file in Directory.GetFiles(TestTempPath))
            {
                try
                {
                    File.Delete(file);
                }
                catch { }
            }
        }

        [Fact]
        public void Migrate_From_V2_Test()
        {
            var testFileName = Guid.NewGuid().ToString() + ".cruise";
            var filePath = Path.Combine(TestTempPath, testFileName);

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

        [Theory]
        [InlineData("7Wolf.cruise")]
        [InlineData("MultiTest.2014.10.31.cruise")]
        public void Migrate_From_V2_Test_With_Existing_File(string fileName)
        {
            var filePath = Path.Combine(TestFilesDirectory, fileName);
            // copy file to test temp dir
            var tempPath = Path.Combine(TestTempPath, fileName);
            File.Copy(filePath, tempPath);

            using (var datastore = new DAL(tempPath))
            {
                //CruiseDAL.Updater.CheckNeedsMajorUpdate(datastore).Should().BeTrue();

                CruiseDAL.Updater.Migrate_From_V2(datastore);

                var semVerActual = new Version(datastore.DatabaseVersion);
                var semVerExpected = new Version("3.0");

                semVerActual.Major.Should().Be(semVerExpected.Major);
                semVerActual.Minor.Should().Be(semVerExpected.Minor);
            }
        }
    }
}