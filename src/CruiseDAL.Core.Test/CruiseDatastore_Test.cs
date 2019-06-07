using FluentAssertions;
using System;
using System.IO;
using Xunit;
using Xunit.Abstractions;

namespace CruiseDAL.Tests
{
    public class CruiseDatastore_Test : TestBase
    {
        public CruiseDatastore_Test(ITestOutputHelper output) : base(output)
        {
        }

        //[Fact]
        //public void Constructor_inMemory_create_test()
        //{
        //    using (var datastore = new CruiseDatastore())
        //    {
        //        ValidateDAL(datastore);
        //    }
        //}

        [Fact]
        public void Ctor_with_null_path()
        {
            Action action = () =>
            {
                var db = new CruiseDAL.CruiseDatastore(null);
            };
            action.Should().Throw<ArgumentException>();
        }

        [Fact]
        public void Ctor_with_empty_path()
        {
            Action action = () =>
            {
                var db = new CruiseDAL.CruiseDatastore("");
            };
            action.Should().Throw<ArgumentException>();
        }

        [Fact]
        public void Constructor_existing_file_test()
        {
            var filePath = Path.Combine(TestFilesDirectory, "7wolf.cruise");

            using (var datastore = new CruiseDAL.CruiseDatastore(filePath))
            {
                ValidateDAL(datastore);
            }
        }

        private void ValidateDAL(CruiseDAL.CruiseDatastore db)
        {
            var version = db.DatabaseVersion;
            version.Should().NotBeNull();
            Version.TryParse(version, out var versionParsed).Should().BeTrue(version);

            //db.ExecuteScalar<int>("SELECT count(*) FROM MessageLog WHERE Program IS NULL or Program = '';")
            //    .Should().Be(0);

            db.ExecuteScalar<string>("SELECT Message FROM MessageLog ORDER BY RowID DESC LIMIT 1;")
                .Should().Be("File Opened");

            //var timeStamp = latestMessage.Time;
            //timeStamp.Should().NotBeNullOrWhiteSpace();
            ////assert that file opened message was within the last 30 seconds
            //DateTime.Parse(timeStamp).Subtract(DateTime.Now).TotalSeconds.Should().BeLessThan(30);
        }

        //[Fact]
        //public void ReadGlobalValue()
        //{
        //    var block = "block";
        //    var key = "key";
        //    var value = "test";

        //    var filePath = Path.Combine(base.TestTempPath, "testReadGlobal.cruise");

        //    using (var datastore = new DAL(filePath, true))
        //    {
        //        datastore.WriteGlobalValue(block, key, value);

        //        datastore.ReadGlobalValue(block, key).Should().Be(value);
        //    }
        //}

        //[Fact]
        //public void TestCopyTo()
        //{
        //    var fileToCopyPath = Path.Combine(TestTempPath, "TestCopy.cruise");
        //    var copiedFilePath = Path.Combine(TestTempPath, "TestCopy2.cruise");
        //    try
        //    {
        //        using (var dal = new CruiseDAL.CruiseDatastore(fileToCopyPath))
        //        {
        //            dal.CopyTo(copiedFilePath);

        //            File.Exists(copiedFilePath).Should().BeTrue();
        //        }
        //    }
        //    finally
        //    {
        //        if (File.Exists(fileToCopyPath)) { File.Delete(fileToCopyPath); }
        //        if (File.Exists(copiedFilePath)) { File.Delete(copiedFilePath); }
        //    }
        //}
    }
}