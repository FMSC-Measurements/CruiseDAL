using System;
using System.Linq;
using Xunit;
using FMSC.ORM.TestSupport.TestModels;
using FMSC.ORM.XUnit;
using Xunit.Abstractions;

namespace FMSC.ORM.Core.EntityModel
{
    public class EntityFormatterTest : TestClassBase
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
            var ed = new EntityDescription(typeof(DOMultiPropType));
            var formatter = new EntityFormatter(ed);
            var data = new DOMultiPropType();

            var formatString = String.Join(",",
                (from string field in TestSupport.TestSQLConstants.MULTI_PROP_TABLE_FIELDS
                 select "[" + field + formatVariant + "]"));

            _output.WriteLine(formatString);

            var formatedString = formatter.Format(formatString, data, null);
            Assert.NotNull(formatString);
            Assert.NotEmpty(formatString);

            Assert.NotEqual(formatString, formatedString);

            
            _output.WriteLine(formatedString);

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
            var ed = new EntityDescription(typeof(DOMultiPropType));
            var formatter = new EntityFormatter(ed);
            var data = new DOMultiPropType();

            foreach (string fieldName in TestSupport.TestSQLConstants.MULTI_PROP_TABLE_FIELDS)
            {
                var formatString = "[" + fieldName + formatVariant + "]";

                _output.WriteLine(formatString);

                var formatedString = formatter.Format(formatString, data, null);
                Assert.NotNull(formatString);
                Assert.NotEmpty(formatString);

                Assert.NotEqual(formatString, formatedString);


                _output.WriteLine(formatedString);
            }

        }
    }
}
