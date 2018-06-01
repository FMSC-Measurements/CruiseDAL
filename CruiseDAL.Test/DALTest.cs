using CruiseDAL.DataObjects;
using FluentAssertions;
using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
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

        void ValidateDAL(DAL db)
        {
            db.DatabaseVersion.Should().Be(CruiseDAL.DAL.CURENT_DBVERSION);

            var messageLogs = db.From<MessageLogDO>().Query().ToArray();
            var latestMessage = messageLogs.Last();

            latestMessage.Program.Should().NotBeNullOrWhiteSpace();
            latestMessage.Message.Should().Be("File Opened");

            var timeStamp = latestMessage.Time;
            timeStamp.Should().NotBeNullOrWhiteSpace();
            //assert that file opened message was within the last 30 seconds
            DateTime.Parse(timeStamp).Subtract(DateTime.Now).TotalSeconds.Should().BeLessThan(30);


            foreach (var table in Schema.Schema.TABLE_NAMES)
            {
                db.CheckTableExists(table).Should().BeTrue();
            }

        }

        [Fact]
        public void TestAllowMultDALOnSameThread()
        {
            var filePath = Path.Combine(base.TestTempPath, "testCreate.cruise");

            try
            {
                using (var dal1 = new DAL(filePath, true))
                {

                    Action action = () =>
                    {
                        using (var dal2 = new DAL(filePath))
                        {
                            ValidateDAL(dal2);
                        }
                    };
                    action.ShouldNotThrow<Exception>();
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

        private void DoWorkBlockMultiThreadDALFileAcess(object obj)
        {
            var path = (string)obj;

            using (DAL newDAL = new DAL(path, true))
            {
                Output.WriteLine("waiting");
                Thread.Sleep(30000);
            }
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

                datastore.ReadGlobalValue(block, key).ShouldBeEquivalentTo(value);
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
                var typeInfo = DAL.LookUpEntityByType(type);
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
    }
}