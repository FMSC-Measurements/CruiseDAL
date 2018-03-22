using SqlBuilder;
using SqlBuilders.Test.Util;
using System.Collections.Generic;
using Xunit;
using Xunit.Abstractions;

namespace SqlBuilders.Test
{
    public class SqlInsertCommandTest : TestBase
    {
        public SqlInsertCommandTest(ITestOutputHelper output) : base(output)
        {
        }

        [Fact]
        public void Insert_With_Values()
        {
            var values = new { intCol = 1, textCol = "text", boolCol = true, floatCol = 0.1 };
            IDictionary<string, object> valueDict = values.AsDictionary();

            var builder = new SqlInsertCommand()
            {
                TableName = "something",
                Values = valueDict
            };

            VerifyCommandSyntex(builder.ToString() + ";");
        }

        [Fact]
        public void Insert_With_ValueExpressions()
        {
            var cols = new string[] { "intCol", "textCol", "boolCol", "floatCol" };
            var valueExprs = new string[] { "1", "'text'", "0", "0.0" };

            var builder = new SqlInsertCommand()
            {
                TableName = "something",
                ColumnNames = cols,
                ValueExpressions = valueExprs
            };

            VerifyCommandSyntex(builder.ToString() + ";");
        }

        [Fact]
        public void Insert_With_Defaults()
        {
            var cols = new string[] { "intCol", "textCol", "boolCol", "floatCol" };
            var valueExprs = new string[] { "1", "'text'", "0", "0.0" };

            var builder = new SqlInsertCommand()
            {
                TableName = "something"
            };

            VerifyCommandSyntex(builder.ToString() + ";");
        }
    }
}