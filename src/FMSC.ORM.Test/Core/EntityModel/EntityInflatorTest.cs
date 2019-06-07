using FluentAssertions;
using FMSC.ORM.Core;
using FMSC.ORM.SQLite;
using FMSC.ORM.TestSupport.TestModels;
using System;
using System.Data;
using System.Data.Common;
using Xunit;
using Xunit.Abstractions;

namespace FMSC.ORM.EntityModel.Support
{
    public class EntityInflatorTest : TestBase
    {
        public EntityInflatorTest(ITestOutputHelper output)
            : base(output)
        { }

        //[Fact]
        //public void CreateInstanceOfEntityTest_WithPOCO()
        //{
        //    var ed = new EntityDescription(typeof(POCOMultiTypeObject));

        //    var instance = ed.Inflator.CreateInstanceOfEntity();
        //    Assert.True(instance is POCOMultiTypeObject);
        //    Assert.NotNull(instance);
        //}

        //[Fact]
        //public void CreateInstanceOfEntityTest_WithDataObject()
        //{
        //    var ed = new EntityDescription(typeof(DOMultiPropType));

        //    var instance = ed.Inflator.CreateInstanceOfEntity();
        //    Assert.True(instance is DOMultiPropType);
        //    Assert.NotNull(instance);
        //}

        [Fact]
        public void ReadDataTest()
        {
            var rand = new Random();

            var poco = new POCOMultiTypeObject()
            {
                ID = rand.Next(),
                StringField = "1",
                IntField = rand.Next(),
                NIntField = rand.Next(),
                BoolField = (rand.Next() & 1) == 1,
                NBoolField = (rand.Next() & 1) == 1,
                FloatField = (float)rand.NextDouble(),
                NFloatField = (float)rand.NextDouble(),
                DoubleField = rand.NextDouble(),
                NDoubleField = rand.NextDouble(),
                GuidField = Guid.NewGuid(),
                DateTimeField = DateTime.Now,
            };

            var reader = new TestSupport.ObjectDataReader<POCOMultiTypeObject>(new POCOMultiTypeObject[] { poco });
            Assert.True(reader.Read());

            var inflator = new EntityInflator(new EntityDescription(typeof(POCOMultiTypeObject)));

            var data = new POCOMultiTypeObject();
            inflator.CheckOrdinals(reader);
            inflator.ReadData(reader, data);

            data.Should().BeEquivalentTo(poco);
        }

        [Fact]
        public void ReadDataTest_with_defaultValues()
        {
            

            var poco = new POCOMultiTypeObject()
            {

            };

            var reader = new TestSupport.ObjectDataReader<POCOMultiTypeObject>(new POCOMultiTypeObject[] { poco });
            reader.Read().Should().BeTrue();

            var inflator = new EntityInflator(new EntityDescription(typeof(POCOMultiTypeObject)));

            var data = new POCOMultiTypeObject();
            inflator.CheckOrdinals(reader);
            inflator.ReadData(reader, data);

            data.StringField.Should().BeNull();
            data.ID.Should().Be(0);
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

            var inflator = new EntityInflator(new EntityDescription(typeof(POCOMultiTypeObject)));

            inflator.CheckOrdinals(reader);
            inflator.ReadPrimaryKey(reader).Should().Be(1);
        }

        [Fact]
        public void GetGUIDTest()
        {
            using (var ds = new SQLiteDatastore())
            {
                using (var connection = ds.OpenConnection())
                {
                    var value = Guid.NewGuid();
                    using (var reader = connection.ExecuteReader($"SELECT '{value}' as thing;", (object[])null, (DbTransaction)null))
                    {
                        reader.Read();
                        var result = EntityInflator.GetGuid(reader, 0);
                        result.Should().Be(value);
                    }

                    using (var reader = connection.ExecuteReader($"SELECT hex(randomblob(16)) as thing;", (object[])null, (DbTransaction)null))
                    {
                        reader.Read();
                        var result = EntityInflator.GetGuid(reader, 0);
                        result.Should().NotBe(Guid.Empty);
                    }

                    using (var reader = connection.ExecuteReader($"SELECT null as thing;", (object[])null, (DbTransaction)null))
                    {
                        reader.Read();
                        var result = EntityInflator.GetGuid(reader, 0);
                        result.Should().Be(Guid.Empty);
                    }
                }
            }
        }

        private enum something { a = 0, b };

        private enum somethingElse { b = 1 };

        [Theory]
        [InlineData("0", something.a, typeof(something))]
        [InlineData("'b'", something.b, typeof(something))]
        [InlineData("'c'", something.a, typeof(something))]//exception will be thrown but caught
        [InlineData("'1'", something.b, typeof(something))]
        [InlineData("null", something.a, typeof(something))]
        [InlineData("null", (somethingElse)0, typeof(somethingElse))]
        [InlineData("0", (somethingElse)0, typeof(somethingElse))]
        public void GetEnumTest(string sqlStr, object expectedValue, Type typeEnum)
        {
            using (var ds = new SQLiteDatastore())
            {
                using (var connection = ds.OpenConnection())
                {
                    using (var reader = connection.ExecuteReader($"SELECT {sqlStr} as thing;", (object[])null, (DbTransaction)null))
                    {
                        reader.Read();
                        var result = EntityInflator.GetEnum(reader, 0, typeEnum);
                        result.Should().Be(expectedValue);
                    }
                }
            }
        }
    }
}