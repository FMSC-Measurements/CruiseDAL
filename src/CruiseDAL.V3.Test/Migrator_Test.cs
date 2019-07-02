using FluentAssertions;
using FMSC.ORM.SQLite;
using System;
using System.IO;
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
    }
}