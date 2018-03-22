using FluentAssertions;
using System;
using System.IO;
using System.Linq;
using System.Threading;
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
                foreach (var table in Schema.Schema.TABLE_NAMES)
                {
                    datastore.CheckTableExists(table).Should().BeTrue();
                }
            }
        }

        [Fact]
        public void Constructor_file_create_test()
        {
            var filePath = Path.Combine(base.TestTempPath, "testCreate.cruise");

            try
            {
                var datastore = new DAL(filePath, true);
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
        public void TestAllowMultDALOnSameThread()
        {
            var dal1 = new DAL(".\\TestResources\\Test.cruise");
            DAL dal2 = null;
            Action action = () => dal2 = new DAL(".\\TestResources\\Test.cruise");
            action.ShouldNotThrow<Exception>();
        }

        private void DoWorkBlockMultiThreadDALFileAcess()
        {
            DAL newDAL = new DAL(".\\TestResources\\Test.cruise");
            Output.WriteLine("waiting");
            Thread.Sleep(2000);
        }

        [Fact]
        public void BlockMultiThreadDALFileAcess()
        {
            Thread thread = new Thread(DoWorkBlockMultiThreadDALFileAcess);
            thread.Start();

            DAL dal = null;
            Action action = () => dal = new DAL(".\\TestResources\\Test.cruise");
            action.ShouldThrow<DatabaseShareException>();
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