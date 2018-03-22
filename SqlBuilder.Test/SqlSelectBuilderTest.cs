using FluentAssertions;
using SqlBuilder;
using System;
using System.Linq;
using Xunit;
using Xunit.Abstractions;

namespace SqlBuilders.Test
{
    public class SqlSelectBuilderTest : TestBase
    {
        public SqlSelectBuilderTest(ITestOutputHelper output)
            : base(output)
        {
        }

        [Theory]
        [InlineData("")]
        [InlineData("GROUP BY expr1", "expr1")]
        [InlineData("GROUP BY expr1, expr2", "expr1", "expr2")]
        public void GroupByClause(string result, params string[] exprs)
        {
            var clause = new GroupByClause(exprs);

            var sql = clause.ToString();
            Output.WriteLine(sql);

            sql.ShouldBeEquivalentTo(result);
        }

        [Theory]
        [InlineData("", "")]
        [InlineData("WHERE expr", "expr")]
        public void WhereClause(string result, params string[] exprs)
        {
            var clause = new WhereClause(exprs.First());

            var sql = clause.ToString();
            Output.WriteLine(sql);

            sql.ShouldBeEquivalentTo(result);
        }

        [Fact]
        public void WhereClause_add()
        {
            var clause1 = new WhereClause("expr1");

            var clause2 = new WhereClause("expr2");

            var reslut = clause1 + clause2;

            Output.WriteLine(clause1.ToString());
            Output.WriteLine(clause2.ToString());
            Output.WriteLine(reslut.ToString());

            clause1.ToString().ShouldBeEquivalentTo("WHERE expr1");
            clause2.ToString().ShouldBeEquivalentTo("WHERE expr2");
            reslut.ToString().ShouldBeEquivalentTo("WHERE expr1 AND expr2");
        }

        [Theory]
        [InlineData("LIMIT 0", 0, 0)]
        [InlineData("LIMIT -1", -1, 0)]//negative is allowed, but is the same a no limit
        [InlineData("LIMIT 1", 1, 0)]
        [InlineData("LIMIT 1 OFFSET 1", 1, 1)]
        public void LimitClause(string expected, int size, int offset)
        {
            var clause = new LimitClause(size, offset);

            var sql = clause.ToString();
            Output.WriteLine(sql);
            sql.ShouldBeEquivalentTo(expected);
        }

        [Theory]
        [InlineData("SELECT * FROM tbl")]
        [InlineData("SELECT col1 FROM tbl", "col1")]
        [InlineData("SELECT col1, col2 FROM tbl", "col1", "col2")]
        public void Select_from_table(string expected, params string[] cols)
        {
            var builder = new SqlSelectBuilder()
            {
                Source = new TableOrSubQuery("tbl"),
            };

            if (cols != null)
            {
                foreach (var col in cols)
                {
                    builder.ResultColumns.Add(col);
                }
            }

            var sql = builder.ToString();
            Output.WriteLine(sql);
            sql.ShouldBeEquivalentTo(expected);

            VerifyCommandSyntex(sql + ";");
        }

        [Theory]
        [InlineData("SELECT * FROM (SELECT * FROM tbl)", null)]
        [InlineData("SELECT * FROM (SELECT * FROM tbl) AS something", "something")]
        public void Select_from_subquery(string expected, string alias)
        {
            var subQuery = new SqlSelectBuilder()
            {
                Source = new TableOrSubQuery("tbl")
            };

            var builder = new SqlSelectBuilder()
            {
                Source = new TableOrSubQuery(subQuery, alias),
            };

            var sql = builder.ToString();
            Output.WriteLine(sql);
            sql.ShouldBeEquivalentTo(expected);

            VerifyCommandSyntex(sql + ";");
        }

        [Theory]
        [InlineData("SELECT * FROM tbl", null)]
        [InlineData("SELECT * FROM tbl JOIN tbl2 USING (ID)", null, "tbl2")]
        [InlineData("SELECT * FROM tbl JOIN tbl2 USING (ID) JOIN tbl3 USING (ID)", null, "tbl2", "tbl3")]
        [InlineData("SELECT * FROM tbl JOIN tbl2 AS T2 USING (ID)", "T2", "tbl2")]
        public void Select_with_joins(string expected, string alias, params string[] joinTables)
        {
            var constr = "USING (ID)";

            var builder = new SqlSelectBuilder()
            {
                Source = new TableOrSubQuery("tbl")
            };

            foreach (var joinTable in joinTables)
            {
                builder.JoinClauses.Add(new JoinClause(joinTable, constr, alias));
            }

            var sql = builder.ToString();
            Output.WriteLine(sql);
            sql.ShouldBeEquivalentTo(expected);

            VerifyCommandSyntex(sql + ";");
        }

        [Fact]
        public void FluentInterfaceTest()
        {
            SqlSelectBuilder builder = new SqlSelectBuilder();
            builder.Source = new TableOrSubQuery("something", "t1");
            builder.ResultColumns.Add("col1");

            builder.Join("something2", "Using (FKey)")
                .Join("something3", "Using (FKey2)")
                .Where("x > 1")
                .GroupBy("col1", "col2")
                .Limit(1, 0);

            var sql = builder.ToString();

            sql.Should().ContainEquivalentOf("JOIN");
            sql.Should().ContainEquivalentOf("WHERE");
            sql.Should().ContainEquivalentOf("GROUP BY");
            sql.Should().ContainEquivalentOf("LIMIT");

            VerifyCommandSyntex(sql + ";");
        }
    }
}