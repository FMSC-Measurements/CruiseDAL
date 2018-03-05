using System;
using System.Collections.Generic;
using System.Threading;
using System.IO;
using Xunit;
using Xunit.Abstractions;
using CruiseDAL;
using CruiseDAL.DataObjects;
using CruiseDAL.Schema;
using FluentAssertions;
using System.Linq;

namespace FMSCORM.Tests
{
    public class DALTest
    {
        private readonly ITestOutputHelper _output;


        public DALTest(ITestOutputHelper output)
        {
            _output = output;
        }

        [Fact]
        public void TestAllowMultDALOnSameThread()
        {
            var dal1 = new DAL(".\\TestResources\\Test.cruise");
            DAL dal2 = null;
            Action action = () => dal2 = new DAL(".\\TestResources\\Test.cruise");
            action.ShouldNotThrow<Exception>();
        }

        void DoWorkBlockMultiThreadDALFileAcess()
        {
            DAL newDAL = new DAL(".\\TestResources\\Test.cruise");
            _output.WriteLine("waiting");
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
        public void TypeReflect_Test()
        {
            var cruisedalAssm = System.Reflection.Assembly.GetAssembly(typeof(DAL));
            var entityTypes = cruisedalAssm.ExportedTypes
                .Where(t => t.GetCustomAttributes(typeof(FMSC.ORM.EntityModel.Attributes.EntitySourceAttribute), true).Any())
                .ToArray();

            foreach (var type in entityTypes)
            {
                var typeInfo = DAL.LookUpEntityByType(type);
                typeInfo.Should().NotBeNull();
            }
        }

        [Fact(Skip =" ")]
        public void DatabaseUpdateTest()
        {
            throw new NotImplementedException();
        }

        [Fact(Skip =" ")]
        public void TestCopyTo()
        {
            throw new NotImplementedException();
        }


    }
}