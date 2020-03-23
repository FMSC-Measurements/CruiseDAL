using CruiseDAL.V2.Test;
using FluentAssertions;
using System;
using System.IO;
using System.Linq;
using Xunit;
using Xunit.Abstractions;

namespace CruiseDAL.Tests
{
    public class DALTest : TestBase
    {
        public DALTest(ITestOutputHelper output) : base(output)
        {
        }

        [Fact]
        public void Constructor_inMemory_create_test()
        {
            using (var datastore = new DAL())
            {
                ValidateDAL(datastore);
            }
        }

        [Fact]
        public void Ctor_with_null_path()
        {
            Action action = () =>
            {
                var db = new DAL(null);
            };
            action.Should().Throw<ArgumentException>();
        }

        [Fact]
        public void Ctor_with_empty_path()
        {
            Action action = () =>
            {
                var db = new DAL("");
            };
            action.Should().Throw<ArgumentException>();
        }

        [Fact]
        public void Update()
        {
            var fileName = "MultiTest.2014.10.31.cruise";
            var testFile = GetTestFile(fileName);

            using (var database = new DAL(testFile))
            {
                var strata = database.From<DataObjects.StratumDO>()
                    .Query().ToArray();

            }
        }

        [Fact]
        public void Constructor_file_create_test()
        {
            var filePath = Path.Combine(base.TestTempPath, "testCreate.cruise");

            try
            {
                var datastore = new DAL(filePath, true);

                ValidateDAL(datastore);
            }
            finally
            {
                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                }
            }
        }

        private void ValidateDAL(DAL db)
        {
            var version = db.DatabaseVersion;
            version.Should().NotBeNullOrEmpty();
            Version.TryParse(version, out var versionParsed).Should().BeTrue(version);

            var createdVersion = db.CreatedVersion;
            createdVersion.Should().NotBeNullOrEmpty();
            // validate that created version in in valid format
            Version.TryParse(version, out var createdVersionParsed).Should().BeTrue(createdVersion);

            db.ExecuteScalar<int>("SELECT count(*) FROM MessageLog WHERE Program IS NULL or Program = '';")
                .Should().Be(0);

            db.ExecuteScalar<string>("SELECT Message FROM MessageLog ORDER BY RowID DESC LIMIT 1;")
                .Should().Be("File Opened");

            //var timeStamp = latestMessage.Time;
            //timeStamp.Should().NotBeNullOrWhiteSpace();
            ////assert that file opened message was within the last 30 seconds
            //DateTime.Parse(timeStamp).Subtract(DateTime.Now).TotalSeconds.Should().BeLessThan(30);
        }

        [Fact]
        public void ReadGlobalValue()
        {
            var block = "block";
            var key = "key";
            var value = "test";

            var filePath = Path.Combine(base.TestTempPath, "testReadGlobal.cruise");

            using (var datastore = new DAL(filePath, true))
            {
                datastore.WriteGlobalValue(block, key, value);

                datastore.ReadGlobalValue(block, key).Should().Be(value);
            }
        }

        [Fact]
        public void TypeReflect_Test()
        {
            var cruisedalAssm = System.Reflection.Assembly.GetAssembly(typeof(DAL));
            var entityTypes = cruisedalAssm.ExportedTypes
                .Where(t => t.GetCustomAttributes(typeof(FMSC.ORM.EntityModel.Attributes.EntitySourceAttribute), true).Any())
                .ToArray();

            Output.WriteLine(String.Join(", ", entityTypes.Select(x => x.Name).ToArray()));

            foreach (var type in entityTypes)
            {
                var typeInfo = FMSC.ORM.EntityModel.Support.GlobalEntityDescriptionLookup.Instance.LookUpEntityByType(type);
                typeInfo.Should().NotBeNull();
            }
        }

        [Fact]
        public void TestCopyTo()
        {
            var fileToCopyPath = Path.Combine(TestTempPath, "TestCopy.cruise");
            var copiedFilePath = Path.Combine(TestTempPath, "TestCopy2.cruise");
            try
            {
                using (var dal = new DAL(fileToCopyPath, true))
                {
                    dal.CopyTo(copiedFilePath);

                    File.Exists(copiedFilePath).Should().BeTrue();
                }
            }
            finally
            {
                if (File.Exists(fileToCopyPath)) { File.Delete(fileToCopyPath); }
                if (File.Exists(copiedFilePath)) { File.Delete(copiedFilePath); }
            }
        }

        [Theory]
        [InlineData("", CruiseFileType.Unknown)]
        [InlineData("something", CruiseFileType.Unknown)]
        [InlineData("something.cruise", CruiseFileType.Cruise)]
        [InlineData("something.CRUISE", CruiseFileType.Cruise)]
        [InlineData("something.bananas.cruise", CruiseFileType.Cruise)]
        [InlineData("something.m.cruise", CruiseFileType.Master, CruiseFileType.Cruise)]
        [InlineData("something.M.cruise", CruiseFileType.Master, CruiseFileType.Cruise)]
        [InlineData("something.m.CRUISE", CruiseFileType.Master, CruiseFileType.Cruise)]
        [InlineData("something.bananas.m.CRUISE", CruiseFileType.Master, CruiseFileType.Cruise)]
        [InlineData("something.1.cruise", CruiseFileType.Component, CruiseFileType.Cruise)]
        [InlineData("something.bananas.1.cruise", CruiseFileType.Component, CruiseFileType.Cruise)]
        [InlineData("something.123456789.cruise", CruiseFileType.Component, CruiseFileType.Cruise)]
        [InlineData("something.back-cruise", CruiseFileType.Backup, CruiseFileType.Cruise)]
        [InlineData("something.m.back-cruise", CruiseFileType.Backup, CruiseFileType.Cruise)]
        [InlineData("something.M.back-cruise", CruiseFileType.Backup, CruiseFileType.Cruise)]
        [InlineData("something.1.back-cruise", CruiseFileType.Backup, CruiseFileType.Cruise)]
        [InlineData("something.cut", CruiseFileType.Template)]
        [InlineData("something.design", CruiseFileType.Design)]
        public void ExtrapolateCruiseFileType_Test(string fileName, CruiseFileType expectedType, params CruiseFileType[] flags)
        {
            var result = CruiseDAL.DAL.ExtrapolateCruiseFileType(fileName);
            result.Should().Be(expectedType);

            foreach (var flag in flags)
            {
                result.Should().HaveFlag(flag);
            }
        }
    }
}