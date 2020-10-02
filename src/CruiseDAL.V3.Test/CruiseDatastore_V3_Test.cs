using FluentAssertions;
using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace CruiseDAL.V3.Test
{
    public class CruiseDatastore_V3_Test : TestBase
    {
        public CruiseDatastore_V3_Test(ITestOutputHelper output) : base(output)
        {
        }

        [Fact]
        public void Constructor_inMemory_create_test()
        {
            using (var datastore = new CruiseDatastore_V3())
            {
                ValidateDAL(datastore);
            }
        }

        [Fact]
        public void Ctor_with_null_path()
        {
            Action action = () =>
            {
                var db = new CruiseDatastore_V3(null);
            };
            action.Should().Throw<ArgumentException>();

        }

        [Fact]
        public void Ctor_with_empty_path()
        {
            Action action = () =>
            {
                var db = new CruiseDatastore_V3("");
            };
            action.Should().Throw<ArgumentException>();
        }

        [Fact]
        public void Constructor_file_create_test()
        {
            var filePath = Path.Combine(base.TestTempPath, "testCreate.crz3");

            try
            {
                var datastore = new CruiseDatastore_V3(filePath, true);

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

        void ValidateDAL(CruiseDatastore_V3 db)
        {
            var f_keys = db.ExecuteScalar<string>("PRAGMA foreign_keys;");
            f_keys.Should().BeOneOf("on","1","yes","true");

            var version = db.DatabaseVersion;
            version.Should().NotBeNull();
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

            var filePath = Path.Combine(base.TestTempPath, "testReadGlobal.crz3");

            using (var datastore = new CruiseDatastore_V3(filePath, true))
            {
                datastore.WriteGlobalValue(block, key, value);

                datastore.ReadGlobalValue(block, key).Should().Be(value);
            }
        }

        [Fact]
        public void TypeReflect_Test()
        {
            var cruisedalAssm = System.Reflection.Assembly.GetAssembly(typeof(CruiseDatastore_V3));
            var entityTypes = cruisedalAssm.ExportedTypes
                .Where(t => t.GetCustomAttributes(typeof(FMSC.ORM.EntityModel.Attributes.TableAttribute), true).Any())
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
            var fileToCopyPath = Path.Combine(TestTempPath, "TestCopy.crz3");
            var copiedFilePath = Path.Combine(TestTempPath, "TestCopy2.crz3");
            try
            {
                using (var dal = new CruiseDatastore_V3(fileToCopyPath, true))
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
    }
}