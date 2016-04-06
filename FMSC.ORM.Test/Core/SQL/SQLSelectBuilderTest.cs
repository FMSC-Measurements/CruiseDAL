using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace FMSC.ORM.Core.SQL
{
    public class SQLSelectBuilderTest : TestClassBase
    {
        public SQLSelectBuilderTest(ITestOutputHelper output)
            : base(output)
        {

        }

        [Fact]
        public void NestedSourcesTest()
        {
            var provider = new SQLite.SQLiteProviderFactory();

            SQLSelectBuilder builder = new SQLSelectBuilder();
            builder.Source = new TableOrSubQuery("something", "t1");
            builder.ResultColumns.Add("col1");

            VerifyCommandSyntex(provider, builder.ToSQL());

            var builder2 = new SQLSelectBuilder();
            builder2.Source = new TableOrSubQuery(builder, "t2");
            builder2.ResultColumns.Add("col2");

            VerifyCommandSyntex(provider, builder2.ToSQL());

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

            var provider = new SQLite.SQLiteProviderFactory();
            VerifyCommandSyntex(provider, builder.ToSQL());
        }

        public void VerifyCommandSyntex(DbProviderFactoryAdapter provider, string commandText)
        {
            var command = provider.CreateCommand();
            _output.WriteLine("testing:\r\n" + commandText);
            command.CommandText = "EXPLAIN " + commandText;


            using (DbConnection conn = provider.CreateConnection())
            {
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
