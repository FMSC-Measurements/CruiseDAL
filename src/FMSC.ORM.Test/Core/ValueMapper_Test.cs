﻿using FluentAssertions;
using System;
using System.Linq;
using System.Text;
using Xunit;
using Xunit.Abstractions;

namespace FMSC.ORM.Core
{
    public class ValueMapper_Test : TestBase
    {
        public enum MyEnum { Zero, One, Two, Three };
        public enum MyEnum2 { A = 1, B, C};

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

        [Theory]
        [InlineData(typeof(int?))]
        [InlineData(typeof(float?))]
        [InlineData(typeof(double?))]
        [InlineData(typeof(bool?))]
        [InlineData(typeof(char?))]
        //[InlineData(typeof(Guid?))]// see ProcessValue_jiberrishString_to_NullableGuid
        [InlineData(typeof(DateTime?))]
        [InlineData(typeof(MyEnum?))]
        public void ProcessValue_jiberrishString_to_NullableValue(Type targetType)
        {
            var value = "jibberish";

            ValueMapper.ProcessValue(targetType, value).Should().BeNull();
        }

        [Fact]
        // ReadData depends on this behavior
        public void ProcessValue_jiberrishString_to_NullableGuid()
        {
            var value = "jibberish";

            Action a = () =>
            {
                ValueMapper.ProcessValue(typeof(Guid?), value).Should().BeNull();
            };
            a.Should().Throw<FormatException>();
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

            var value = Encoding.UTF8.GetBytes(expected);

            ValueMapper.ProcessValue(targetType, value).Should().Be(expected);
        }

        [Theory]
        [InlineData("6/22/2015 4:20 PM")]
        public void ProcessValue_string_to_DateTime(string value)
        {
            var targetType = typeof(DateTime);
            var expected = DateTime.Parse(value);

            var result = ValueMapper.ProcessValue(targetType, value);
            result.Should().BeOfType<DateTime>();

            ((DateTime)result).Should().BeCloseTo(expected, 1000);
        }

        [Fact]
        public void ProcessValue_empty_string_to_DateTime()
        {
            var targetType = typeof(DateTime);
            var expected = default(DateTime);

            var value = "";

            var result = ValueMapper.ProcessValue(targetType, value);
            result.Should().BeOfType<DateTime>();

            ((DateTime)result).Should().BeCloseTo(expected, 1000);
        }

        [Theory]
        [InlineData(typeof(MyEnum), MyEnum.One, "one")]
        [InlineData(typeof(MyEnum), MyEnum.One, "ONE")]
        [InlineData(typeof(MyEnum), MyEnum.One, "One")]
        [InlineData(typeof(MyEnum), MyEnum.One, "1")]
        [InlineData(typeof(MyEnum), MyEnum.One, 1)]
        [InlineData(typeof(MyEnum), MyEnum.Zero, 0)]
        [InlineData(typeof(MyEnum), MyEnum.Zero, "invalid")]
        [InlineData(typeof(MyEnum), MyEnum.Zero, "")]

        [InlineData(typeof(MyEnum2), (MyEnum2)0, "")]
        [InlineData(typeof(MyEnum2), (MyEnum2)0, "invalid")]
        //[InlineData(MyEnum.One, "1.0")]
        public void ProcessValue_value_to_Enum(Type targetType, object expected, object value)
        {
            var result = ValueMapper.ProcessValue(targetType, value);
            result.Should().BeOfType(targetType);
            result.Should().Be(expected);
        }

        [Theory]
        [InlineData(typeof(string), 1, "1")] // integer to string
        [InlineData(typeof(string), 1.1d, "1.1")] // float to string
        [InlineData(typeof(int), 1, 1)] // int to int
        [InlineData(typeof(int), "1", 1)] // string to int
        [InlineData(typeof(int?), 1, 1)] // int to nullable int
        [InlineData(typeof(int?), 1L, 1)] // long to int
        [InlineData(typeof(int?), "1", 1)] // string to int
        [InlineData(typeof(int?), null, null)]
        [InlineData(typeof(float), 1.1f, 1.1f)]
        [InlineData(typeof(float), 1.1d, 1.1f)]
        [InlineData(typeof(Guid?), null, null)]
        [InlineData(typeof(DateTime?), null, null)]
        [InlineData(typeof(MyEnum?), null, null)]
        [InlineData(typeof(MyEnum?), MyEnum.One, MyEnum.One)]
        [InlineData(typeof(MyEnum?), "one", MyEnum.One)]
        [InlineData(typeof(MyEnum), MyEnum.One, MyEnum.One)]
        [InlineData(typeof(MyEnum), "one", MyEnum.One)]
        public void ProcessValue_value_to_Type(Type targetType, object value, object expected)
        {
            var result = ValueMapper.ProcessValue(targetType, value);

            result.Should().Be(expected);

            if (result != null)
            {
                result.GetType().Should().Be(Nullable.GetUnderlyingType(targetType) ?? targetType);
            }

            ValidateIsAssignable(targetType, result);
        }

        [Theory]
        [InlineData(typeof(int))]
        [InlineData(typeof(float))]
        [InlineData(typeof(double))]
        [InlineData(typeof(bool))]
        [InlineData(typeof(char))]
        [InlineData(typeof(DateTime))]
        //[InlineData(typeof(Guid), Skip = "initializing an guid with an empty string throws an exception, not sure if I want to override that behavior")]
        [InlineData(typeof(MyEnum))]
        public void ProcessValue_empty_string_to_valueType(Type targetType)
        {
            var value = "";
            var expected = Activator.CreateInstance(targetType);
            var result = ValueMapper.ProcessValue(targetType, value);

            ValidateIsAssignable(targetType, result);

            if (expected == null) { expected = value; }
            result.Should().Be(expected);
            Output.WriteLine(result?.ToString() ?? "<null>");
        }

        [Theory]
        [InlineData(typeof(int?))]
        [InlineData(typeof(float?))]
        [InlineData(typeof(double?))]
        [InlineData(typeof(bool?))]
        [InlineData(typeof(char?))]
        [InlineData(typeof(DateTime?))]
        //[InlineData(typeof(Guid?), Skip = "initializing an guid with an empty string throws an exception, not sure if I want to override that behavior")]
        [InlineData(typeof(MyEnum?))]
        public void ProcessValue_empty_string_to_NullableType(Type targetType)
        {
            var value = "";
            var result = ValueMapper.ProcessValue(targetType, value);

            Output.WriteLine(result?.ToString() ?? "<null>");
            result.Should().BeNull();
            ValidateIsAssignable(targetType, result);
        }

        [Theory]
        [InlineData(typeof(int))]
        [InlineData(typeof(float))]
        [InlineData(typeof(double))]
        [InlineData(typeof(bool))]
        [InlineData(typeof(char))]
        [InlineData(typeof(DateTime))]
        [InlineData(typeof(Guid))]
        [InlineData(typeof(MyEnum))]
        public void ProcessValue_null_to_valueType(Type targetType)
        {
            var value = (object)null;
            var expected = Activator.CreateInstance(targetType);
            var result = ValueMapper.ProcessValue(targetType, value);

            ValidateIsAssignable(targetType, result);

            if (expected == null) { expected = value; }
            result.Should().Be(expected);
            Output.WriteLine(result?.ToString() ?? "<null>");
        }

        [Theory]
        [InlineData(typeof(int))]
        [InlineData(typeof(float))]
        [InlineData(typeof(double))]
        [InlineData(typeof(bool))]
        [InlineData(typeof(char))]
        [InlineData(typeof(DateTime))]
        [InlineData(typeof(Guid))]
        [InlineData(typeof(MyEnum))]
        public void ProcessValue_DBnull_to_valueType(Type targetType)
        {
            var value = DBNull.Value;
            var expected = Activator.CreateInstance(targetType);
            var result = ValueMapper.ProcessValue(targetType, value);

            ValidateIsAssignable(targetType, result);

            if (expected == null) { expected = value; }
            result.Should().Be(expected);
            Output.WriteLine(result?.ToString() ?? "<null>");
        }

        [Theory]
        [InlineData(typeof(int?))]
        [InlineData(typeof(float?))]
        [InlineData(typeof(double?))]
        [InlineData(typeof(bool?))]
        [InlineData(typeof(char?))]
        [InlineData(typeof(Guid?))]
        [InlineData(typeof(DateTime?))]
        [InlineData(typeof(MyEnum?))]
        public void ProcessValue_DBNull_to_NullabeType(Type targetType)
        {
            var result = ValueMapper.ProcessValue(targetType, DBNull.Value);

            result.Should().Be(null);
            ValidateIsAssignable(targetType, result);
        }

        [Fact]
        public void ProcessValue_DBNull_to_string()
        {
            var result = ValueMapper.ProcessValue(typeof(string), DBNull.Value);

            result.Should().Be(null);
        }

        [Fact]
        public void ProcessValue_DBNull_to_DateTime()
        {
            var result = ValueMapper.ProcessValue(typeof(DateTime), DBNull.Value);

            result.Should().Be(default(DateTime));
        }

        [Fact]
        public void ProcessValue_Null_to_DateTime()
        {
            var result = ValueMapper.ProcessValue(typeof(DateTime), null);

            result.Should().Be(default(DateTime));
            var date = default(DateTime);
        }

        private static void ValidateIsAssignable(Type targetType, object value)
        {
            var prop = typeof(MultiPropType).GetProperties()
                            .Where(x => x.PropertyType == targetType)
                            .Single();

            var instance = new MultiPropType();
            prop.SetMethod.Invoke(instance, new[] { value });
            prop.GetMethod.Invoke(instance, null).Should().Be(value);
            //result.Should().BeAssignableTo(targetType);
        }

        // use this class with ValidateIsAssignable to validate that
        // values can be assigned to properties matching a target type
        // since reflection is used to assign values to propertys in the orm
        // I just want to be extra sure that values returned from the ValueMapper
        // can be set to properties using reflection
        private class MultiPropType
        {
            public int Integer { get; set; }
            public float Float { get; set; }
            public double Double { get; set; }
            public bool Bool { get; set; }
            public char Char { get; set; }
            public DateTime DateTime { get; set; }
            public Guid Guid { get; set; }

            public MyEnum MyEnum { get; set; }

            public int? NInteger { get; set; }
            public float? NFloat { get; set; }
            public bool? NBool { get; set; }
            public double? NDouble { get; set; }
            public char? NChar { get; set; }
            public DateTime? NDateTime { get; set; }
            public Guid? NGuid { get; set; }

            public MyEnum? NMyEnume { get; set; }

            public string String { get; set; }
        }
    }
}