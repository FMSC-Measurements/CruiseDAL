using FMSC.ORM.TestSupport.TestModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace FMSC.ORM.EntityModel.Support
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
        public void CreateInstanceOfEntityTest_WithDataObject()
        {
            var ed = new EntityDescription(typeof(DOMultiPropType));

            var instance = ed.Inflator.CreateInstanceOfEntity();
            Assert.True(instance is DOMultiPropType);
            Assert.NotNull(instance);
        }

        [Fact]
        public void ReadDataTest()
        {
            var poco = new POCOMultiTypeObject()
            {
                ID = 1,
                StringField = "1",
                IntField = 1
            };

            var reader = new TestSupport.ObjectDataReader<POCOMultiTypeObject>(new POCOMultiTypeObject[] { poco });
            Assert.True(reader.Read());

            var ed = new EntityDescription(typeof(POCOMultiTypeObject));

            var data = new POCOMultiTypeObject();
            ed.Inflator.CheckOrdinals(reader);
            ed.Inflator.ReadData(reader, data);

            Assert.Equal(data.StringField, "1");
            Assert.Equal(data.ID, 1);
        }

        [Fact]
        public void ReadPrimaryKeyTest()
        {
            var poco = new POCOMultiTypeObject()
            {
                ID = 1
            };

            var reader = new TestSupport.ObjectDataReader<POCOMultiTypeObject>(new POCOMultiTypeObject[] { poco });
            Assert.True(reader.Read());

            var ed = new EntityDescription(typeof(POCOMultiTypeObject));

            ed.Inflator.CheckOrdinals(reader);
            var id = ed.Inflator.ReadPrimaryKey(reader);

            Assert.Equal(id, 1);
        }
    }
}