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

        [Fact]
        public void FormatTest()
        {
            var ed = new EntityDescription(typeof(DOMultiPropType));
            var formatter = new EntityFormatter(ed);
            var data = new DOMultiPropType();

            var formatString = String.Join(",",
                (from string field in TestSupport.TestSQLConstants.MULTI_PROP_TABLE_FIELDS
                 select "[" + field + ":null" + "]"));

            _output.WriteLine(formatString);

            var formatedString = formatter.Format(formatString, data, null);
            Assert.NotNull(formatString);
            Assert.NotEmpty(formatString);

            _output.WriteLine(formatedString);
        }
    }
}
