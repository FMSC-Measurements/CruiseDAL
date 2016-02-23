using FMSC.ORM.TestSupport.TestModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace FMSC.ORM.Core.EntityModel
{
    public class EntityInflatorTest : TestClassBase
    {
        public EntityInflatorTest(ITestOutputHelper output)
            : base(output)
        { }

        [Fact]
        public void CreateInstanceOfEntityTest_WithPOCO()
        {
            var ed = new EntityDescription(typeof(POCOMultiTypeObject));

            var instance = ed.Inflator.CreateInstanceOfEntity();
            Assert.True(instance is POCOMultiTypeObject);
            Assert.NotNull(instance);
        }

        [Fact]
        public void CreateInstanceOfEntityTest_WithOataObject()
        {
            var ed = new EntityDescription(typeof(DOMultiPropType));

            var instance = ed.Inflator.CreateInstanceOfEntity();
            Assert.True(instance is DOMultiPropType);
            Assert.NotNull(instance);
        }

        [Fact]
        public void ReadDataTest()
        {
            throw new NotImplementedException();
        }


        [Fact]
        public void ReadPrimaryKeyTest()
        {
            throw new NotImplementedException();
        }





    }
}
