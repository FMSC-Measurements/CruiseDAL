using FluentAssertions;
using FMSC.ORM.Core;
using System;
using System.Linq;
using System.Text;
using Xunit;
using Xunit.Abstractions;

namespace FMSC.ORM.Test.Core
{
    public class ValueMapper_Test : TestBase
    {
        public enum MyEnum { Zero, One, Two, Three };

        public ValueMapper_Test(ITestOutputHelper output) : base(output)
        {
        }

        [Fact]
        public void ProcessValue_string_to_Guid()
        {
            var targetType = typeof(Guid);
            var expected = Guid.NewGuid();

            var value = expected.ToString();

            ValueMapper.ProcessValue(targetType, value).Should().Be(expected);
        }

        [Fact]
        public void ProcessValue_byteArray_to_Guid()
        {
            var targetType = typeof(Guid);
            var expected = Guid.NewGuid();

            var value = expected.ToByteArray();

            ValueMapper.ProcessValue(targetType, value).Should().Be(expected);
        }

        [Fact]
        public void ProcessValue_byteArray_to_string()
        {
            var targetType = typeof(string);
            var expected = "something";

            
            var value = Encoding.Default.GetBytes(expected);

            ValueMapper.ProcessValue(targetType, value).Should().Be(expected);
        }

        [Fact]
        public void ProcessValue_string_to_DateTime()
        {
            var targetType = typeof(DateTime);
            var expected = DateTime.Now;

            var value = expected.ToString();

            var result = ValueMapper.ProcessValue(targetType, value);
            result.Should().BeOfType<DateTime>();

            ((DateTime)result).Should().BeCloseTo(expected, 1000);
        }

        [Theory]
        [InlineData(MyEnum.One, "one")]
        [InlineData(MyEnum.One, "ONE")]
        [InlineData(MyEnum.One, "One")]
        [InlineData(MyEnum.One, "1")]
        [InlineData(MyEnum.One, 1)]
        [InlineData(MyEnum.Zero, 0)]
        //[InlineData(MyEnum.One, "1.0")]
        public void ProcessValue_value_to_Enum(MyEnum expected, object value)
        {
            var targetType = typeof(MyEnum);

            var result = ValueMapper.ProcessValue(targetType, value);
            result.Should().BeOfType<MyEnum>();

            ((MyEnum)result).Should().Be(expected);
        }

        [Theory]
        [InlineData(typeof(string), 1, "1")]
        [InlineData(typeof(string), 1.1d, "1.1")]
        [InlineData(typeof(int?), 1)]
        [InlineData(typeof(int?), 1L)]
        [InlineData(typeof(int?), null)]
        [InlineData(typeof(Guid?), null)]
        [InlineData(typeof(DateTime?), null)]
        [InlineData(typeof(MyEnum?), null)]
        [InlineData(typeof(MyEnum?), MyEnum.One)]
        [InlineData(typeof(MyEnum?), "one", MyEnum.One)]
        public void ProcessValue_value_to_Type(Type targetType, object value, object expected = null)
        {
            var result = ValueMapper.ProcessValue(targetType, value);

            var prop = typeof(MultiPropType).GetProperties()
                .Where(x => x.PropertyType == targetType)
                .Single();

            var instance = new MultiPropType();
            prop.SetMethod.Invoke(instance, new[] { result });
            prop.GetMethod.Invoke(instance, null).Should().Be(result);
            //result.Should().BeAssignableTo(targetType);

            if(expected == null) { expected = value; }
            result.Should().Be(expected);
        }

        [Theory]
        [InlineData(typeof(int?))]
        [InlineData(typeof(Guid?))]
        [InlineData(typeof(DateTime?))]
        [InlineData(typeof(MyEnum?))]
        public void ProcessValue_DBNull_to_NullabeType(Type targetType)
        {
            var result = ValueMapper.ProcessValue(targetType, DBNull.Value);

            var prop = typeof(MultiPropType).GetProperties()
                .Where(x => x.PropertyType == targetType)
                .Single();

            var instance = new MultiPropType();
            prop.SetMethod.Invoke(instance, new[] { result });
            //result.Should().BeAssignableTo(targetType);

            result.Should().Be(null);
        }

        [Fact]
        public void ProcessValue_DBNull_to_string()
        {
            var result = ValueMapper.ProcessValue(typeof(string), DBNull.Value);

            result.Should().Be(null);
        }

        private class MultiPropType
        {
            public int? Integer { get; set; }
            public DateTime? DT { get; set; }
            public Guid? Guid { get; set; }

            public MyEnum? ME { get; set; }

            public string Str { get; set; }
        }
    }
}