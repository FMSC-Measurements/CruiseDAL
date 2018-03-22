using FluentAssertions;
using SqlBuilder;
using System.Collections.Generic;
using System.Data;
using Xunit;
using Xunit.Abstractions;

namespace SqlBuilders.Test
{
    public class CreateTableTest : TestBase
    {
        public CreateTableTest(ITestOutputHelper output) : base(output)
        {
        }

        [Theory]
        [InlineData("CREATE TABLE tbl (col1 TEXT)", false, false, false, "tbl", "col1")]
        [InlineData("CREATE TEMP TABLE tbl (col1 TEXT)", true, false, false, "tbl", "col1")]
        [InlineData("CREATE TABLE IF NOT EXISTS tbl (col1 TEXT)", false, true, false, "tbl", "col1")]
        [InlineData("CREATE TABLE tbl (col1 TEXT) WITHOUT ROWID", false, false, true, "tbl", "col1")]
        [InlineData("CREATE TABLE tbl (col1 TEXT, col2 TEXT)", false, false, false, "tbl", "col1", "col2")]
        public void Create_Table(string expected, bool temp, bool ifNotExists, bool withoutRowID, string tableName, params string[] colNames)
        {
            var collumnList = new List<ColumnInfo>();

            foreach (var colName in colNames)
            {
                collumnList.Add(new ColumnInfo(colName));
            }

            var builder = new CreateTable(Dialect)
            {
                TableName = tableName,
                Columns = collumnList,
                Temp = temp,
                IfNotExists = ifNotExists,
                WithoutRowid = withoutRowID
            };

            var sql = builder.ToString();

            VerifyCommandSyntex(sql);
            sql.ShouldBeEquivalentTo(expected);
        }

        [Theory]
        [InlineData("CREATE TABLE tbl (col1 TEXT)", DbType.AnsiString)]
        [InlineData("CREATE TABLE tbl (col1 TEXT)", DbType.String)]
        [InlineData("CREATE TABLE tbl (col1 INTEGER)", DbType.Int32)]
        [InlineData("CREATE TABLE tbl (col1 INTEGER)", DbType.Int64)]
        [InlineData("CREATE TABLE tbl (col1 REAL)", DbType.Double)]
        public void Create_Table_ColTypes_DbType(string expected, DbType type)
        {
            var colList = new List<ColumnInfo>();

            colList.Add(new ColumnInfo("col1", type));

            var builder = new CreateTable(Dialect)
            {
                TableName = "tbl",
                Columns = colList
            };

            var sql = builder.ToString();

            VerifyCommandSyntex(sql);
            sql.ShouldBeEquivalentTo(expected);

        }

        [Theory]
        [InlineData("CREATE TABLE tbl (col1 TEXT)", "TEXT")]
        [InlineData("CREATE TABLE tbl (col1 INTEGER)", "INTEGER")]
        public void Create_Table_ColTypes_StrType(string expected, string type)
        {
            var colList = new List<ColumnInfo>();

            colList.Add(new ColumnInfo("col1", type));

            var builder = new CreateTable(Dialect)
            {
                TableName = "tbl",
                Columns = colList
            };

            var sql = builder.ToString();

            VerifyCommandSyntex(sql);
            sql.ShouldBeEquivalentTo(expected);

        }



        [Theory]
        [InlineData("CREATE TABLE tbl (col1 TEXT)", "tbl")]
        [InlineData("CREATE TABLE tbl (col1 TEXT, CHECK (1))", "tbl", "CHECK (1)")]
        public void Create_Table_With_TableConstr(string expected, string tableName, params string[] tableConstrs)
        {
            var collumnList = new List<ColumnInfo>();
            collumnList.Add(new ColumnInfo("col1"));

            var tableConstrList = new List<string>();
            tableConstrList.AddRange(tableConstrs);

            var builder = new CreateTable(Dialect)
            {
                TableName = tableName,
                Columns = collumnList,
                TableConstraints = tableConstrList
            };

            var sql = builder.ToString();

            VerifyCommandSyntex(sql);
            sql.ShouldBeEquivalentTo(expected);
        }

        [Fact]
        public void Create_Table_With_Select()
        {
            string expected = "CREATE TABLE tbl AS SELECT * FROM tbl2";

            var collumnList = new List<ColumnInfo>();
            collumnList.Add(new ColumnInfo("col1"));

            var builder = new CreateTable(Dialect)
            {
                TableName = "tbl",
                Columns = collumnList,
                SelectStatment = new SqlSelectBuilder() { Source = new TableOrSubQuery("tbl2") }
            };

            var sql = builder.ToString();

            VerifyCommandSyntex(sql);
            sql.ShouldBeEquivalentTo(expected);
        }
    }
}