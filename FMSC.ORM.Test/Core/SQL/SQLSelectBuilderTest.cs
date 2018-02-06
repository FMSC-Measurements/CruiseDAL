using System;
using System.Data.Common;
using Xunit;
using Xunit.Abstractions;

namespace FMSC.ORM.Core.SQL
{
    public class SQLSelectBuilderTest : TestClassBase
    {
        private ISqlDialect dialect;

        public SQLSelectBuilderTest(ITestOutputHelper output)
            : base(output)
        {
            dialect = new SQLite.SqliteDialect();
        }

        [Fact]
        public void NestedSourcesTest()
        {
            SQLSelectBuilder builder = new SQLSelectBuilder();
            builder.Source = new TableOrSubQuery("something", "t1");
            builder.ResultColumns.Add("col1");

            VerifyCommandSyntex(builder.ToSQL());

            var builder2 = new SQLSelectBuilder();
            builder2.Source = new TableOrSubQuery(builder, "t2");
            builder2.ResultColumns.Add("col2");

            VerifyCommandSyntex(builder2.ToSQL());
        }

        [Fact]
        public void FluentInterfaceTest()
        {
            base.StartTimer();
            SQLSelectBuilder builder = new SQLSelectBuilder();
            builder.Source = new TableOrSubQuery("something", "t1");
            builder.ResultColumns.Add("col1");

            builder.Join("something2", "Using (FKey)", null)
                .Join("something3", "Using (FKey2)", null)
                .Where("x > 1")
                .GroupBy("col1", "col2")
                .Limit(1, 0);

            var sql = builder.ToSQL();

            Assert.Contains("Join", sql, StringComparison.InvariantCultureIgnoreCase);
            Assert.Contains("Where", sql, StringComparison.InvariantCultureIgnoreCase);
            Assert.Contains("Group By", sql, StringComparison.InvariantCultureIgnoreCase);
            Assert.Contains("Limit", sql, StringComparison.InvariantCultureIgnoreCase);
            base.EndTimer();

            VerifyCommandSyntex(builder.ToSQL());
        }

        public void VerifyCommandSyntex(string commandText)
        {
            using (var conn = dialect.CreateConnection())
            {
                var command = conn.CreateCommand();
                _output.WriteLine("testing:\r\n" + commandText);
                command.CommandText = "EXPLAIN " + commandText;

                conn.ConnectionString = "Data Source =:memory:; Version = 3; New = True;";
                conn.Open();

                command.Connection = conn;

                try
                {
                    command.ExecuteNonQuery();
                }
                catch (DbException ex)
                {
                    Assert.DoesNotContain("syntax ", ex.Message, StringComparison.InvariantCultureIgnoreCase);
                }
            }
        }
    }
}