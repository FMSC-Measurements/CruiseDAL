using CruiseDAL.V2.Test;
using FMSC.ORM.TestSupport.TestModels;
using System;
using System.Linq;
using Xunit;
using Xunit.Abstractions;

namespace FMSC.ORM.EntityModel.Support
{
    public class EntityFormatterTest : TestBase
    {
        public EntityFormatterTest(ITestOutputHelper output)
            : base(output)
        { }

        [Theory]
        [InlineData("")]
        [InlineData(":null")]
        [InlineData(":8675307")]
        [InlineData(":null:5U")]
        [InlineData(":null:5L")]
        [InlineData(":null:5C")]
        [InlineData("::5U")]
        public void FormatTest_WithAllFieldsAtOnce(string formatVariant)
        {
            var formatter = new EntityFormatter(typeof(POCOMultiTypeObject));
            var data = new POCOMultiTypeObject();

            var formatString = String.Join(",",
                (from string field in TestSupport.TestSQLConstants.MULTI_PROP_TABLE_FIELDS
                 .Except(TestSupport.TestSQLConstants.NON_REFLECTED_MULTI_PROP_TABLE_FIELDS)
                 select "[" + field + formatVariant + "]"));

            Output.WriteLine(formatString);

            var formatedString = formatter.Format(formatString, data, null);
            Assert.NotNull(formatString);
            Assert.NotEmpty(formatString);

            Assert.NotEqual(formatString, formatedString);

            Output.WriteLine(formatedString);
        }

        [Theory]
        [InlineData("")]
        [InlineData(":null")]
        [InlineData(":8675307")]
        [InlineData(":null:5U")]
        [InlineData(":null:5L")]
        [InlineData(":null:5C")]
        [InlineData("::5U")]
        public void FormatTest_WithSingleFieldAtaTime(string formatVariant)
        {
            var formatter = new EntityFormatter(typeof(POCOMultiTypeObject));
            var data = new POCOMultiTypeObject();

            foreach (string fieldName in TestSupport.TestSQLConstants.MULTI_PROP_TABLE_FIELDS
                .Except(TestSupport.TestSQLConstants.NON_REFLECTED_MULTI_PROP_TABLE_FIELDS))
            {
                var formatString = "[" + fieldName + formatVariant + "]";

                Output.WriteLine(formatString);

                var formatedString = formatter.Format(formatString, data, null);
                Assert.NotNull(formatString);
                Assert.NotEmpty(formatString);

                Assert.NotEqual(formatString, formatedString);

                Output.WriteLine(formatedString);
            }
        }
    }
}