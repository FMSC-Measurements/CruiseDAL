using FluentAssertions;
using FMSC.ORM.SQLite;
using System;
using System.IO;
using System.Linq;
using Xunit;
using Xunit.Abstractions;

namespace CruiseDAL.V3.Tests
{
    public class Migrator_Test : TestBase
    {
        public Migrator_Test(ITestOutputHelper output) : base(output)
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
        public void MigrateFromV2ToV3_Test()
        {
            var testFileName = Guid.NewGuid().ToString() + ".cruise";
            var filePath = Path.Combine(TestTempPath, testFileName);

            using (var v2db = new DAL(filePath, true))
            {
                var newFilePath = Migrator.GetConvertedPath(filePath);
                Migrator.MigrateFromV2ToV3(filePath, newFilePath);
                using (var newCruise = new CruiseDatastore_V3(newFilePath))
                {
                    var semVerActual = new Version(newCruise.DatabaseVersion);
                    var semVerExpected = new Version("3.0");

                    semVerActual.Major.Should().Be(semVerExpected.Major);
                    semVerActual.Minor.Should().Be(semVerExpected.Minor);
                }
            }
        }

        [Theory]
        [InlineData("7Wolf.cruise")]
        [InlineData("MultiTest.2014.10.31.cruise")]
        public void MigrateFromV2ToV3_Test_With_Existing_File(string fileName)
        {
            var filePath = Path.Combine(TestFilesDirectory, fileName);
            // copy file to test temp dir
            var tempPath = Path.Combine(TestTempPath, fileName);
            File.Copy(filePath, tempPath);

            var newCruisePath = Migrator.MigrateFromV2ToV3(tempPath);
            using (var newCruise = new CruiseDatastore_V3(newCruisePath))
            {
                var semVerActual = new Version(newCruise.DatabaseVersion);
                var semVerExpected = new Version("3.0");

                semVerActual.Major.Should().Be(semVerExpected.Major);
                semVerActual.Minor.Should().Be(semVerExpected.Minor);
            }
        }

        [Theory]
        [InlineData("7Wolf.cruise")]
        [InlineData("MultiTest.2014.10.31.cruise")]
        public void Migrate(string fileName)
        {
            var filePath = Path.Combine(TestFilesDirectory, fileName);
            // copy file to test temp dir
            var tempPath = Path.Combine(TestTempPath, fileName);
            File.Copy(filePath, tempPath);

            var newFilePath = Migrator.MigrateFromV2ToV3(filePath, true);

            using (var tempdb = new CruiseDatastore_V3())
            using (var database = new CruiseDatastore_V3(newFilePath))
            {
                var from = "fromdb";
                tempdb.AttachDB(database, from);

                try
                {
                    var conn = tempdb.OpenConnection();
                    Migrator.Migrate(conn, from);

                    var dumpPath = newFilePath + ".dump.crz3";
                    RegesterFileForCleanUp(dumpPath);
                    tempdb.BackupDatabase(dumpPath);
                    tempdb.DetachDB(from);
                    File.Exists(dumpPath).Should().BeTrue();

                    using (var newdb = new CruiseDatastore_V3(dumpPath))
                    {
                        var tables = newdb.GetTableNames();

                        newdb.AttachDB(database, "olddb");

                        foreach(var t in tables)
                        {

                            var stuff = newdb.QueryGeneric($"SELECT * FROM main.{t} EXCEPT SELECT * FROM olddb.{t};")
                                .ToArray();

                            if(t == "MessageLog")
                            {
                                stuff.Should().NotBeEmpty();
                                continue;
                            }

                            stuff.Should().BeEmpty();
                        }
                    }
                }
                finally
                {
                    tempdb.ReleaseConnection();
                }
            }
        }
    }
}