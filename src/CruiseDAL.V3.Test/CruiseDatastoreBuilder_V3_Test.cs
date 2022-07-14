using CruiseDAL.Schema;
using CruiseDAL.TestCommon;
using FluentAssertions;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Xunit;
using Xunit.Abstractions;
using FMSC.ORM.Core;

namespace CruiseDAL.V3.Test
{
    public class CruiseDALDatastoreBuilder_Test : TestBase
    {
        public CruiseDALDatastoreBuilder_Test(ITestOutputHelper output) : base(output)
        {
        }

        [Fact]
        public void CreateTables_Test()
        {
            using (var database = new FMSC.ORM.SQLite.SQLiteDatastore())
            {
                var dbBuilder = new CruiseDatastoreBuilder_V3();

                var conn = database.OpenConnection();
                dbBuilder.BuildDatabase(conn, null);

                //database.Invoking(x => x.CreateDatastore(dbBuilder)).Should().NotThrow();

                foreach (var table in CruiseDatastoreBuilder_V3.TABLE_DEFINITIONS)
                {
                    Output.WriteLine(table.TableName);
                    database.CheckTableExists(table.TableName).Should().BeTrue(table.TableName);
                }

                //foreach(var view in CruiseDatastoreBuilder_V3.VIEW_DEFINITIONS)
                //{
                //    database.CheckTableExists(view.ViewName).Should().BeTrue();
                //}
            }
        }

        [Fact]
        public void whatIsHappening()
        {
            var tempPath = base.GetTempFilePathWithExt(".db");

            using(var db = new CruiseDatastore(tempPath, true, null, null))
            {
                var conn = db.OpenConnection();

                var tdvdef = new TreeDefaultValueTableDefinition();
                conn.ExecuteNonQuery(tdvdef.CreateTable);

            }
        }

        [Fact]
        public void WriteDDLToOutput()
        {
            foreach (var table in CruiseDatastoreBuilder_V3.TABLE_DEFINITIONS)
            {

                Output.WriteLine("-- " + table.TableName);

                var createCommand = table.CreateTable;
                Output.WriteLine(createCommand);

                var createIndexes = table.CreateIndexes;
                if (createIndexes != null)
                {
                    Output.WriteLine(createIndexes);
                }

                var createTombstone = table.CreateTombstoneTable;
                if (createTombstone != null)
                {
                    Output.WriteLine(createTombstone);
                }

                var initialize = table.InitializeTable;
                if (initialize != null)
                {
                    Output.WriteLine(initialize);
                }

                var triggers = table.CreateTriggers;
                if (triggers != null)
                {
                    foreach (var trigger in triggers)
                    {
                        Output.WriteLine(trigger);
                    }
                }
            }
        }

        [Fact(Skip= "since we now have multiple versions of a table definition this test nolonger passes")]
        public void ContainsAllTableDefinitions()
        {
            var tableTypes = Assembly.GetAssembly(typeof(CruiseDatastoreBuilder_V3)).GetTypes()
                .Where(x => typeof(ITableDefinition).IsAssignableFrom(x) && x != typeof(ITableDefinition))
                .ToArray();

            var stuff = CruiseDatastoreBuilder_V3.TABLE_DEFINITIONS.Select(x => x.GetType()).ToArray();
            stuff.Should().Contain(tableTypes);
        }

        [Fact]
        public void ContainsAllViewDefinitions()
        {
            var tableTypes = Assembly.GetAssembly(typeof(CruiseDatastoreBuilder_V3)).GetTypes()
                .Where(x => typeof(IViewDefinition).IsAssignableFrom(x) && x != typeof(IViewDefinition))
                .ToArray();

            CruiseDatastoreBuilder_V3.VIEW_DEFINITIONS.Select(x => x.GetType()).Should().Contain(tableTypes);
        }

        [Theory]
        [ClassData(typeof(TableDefinition_dataprovider))]
        public void ExcerciseCreateTable(ITableDefinition tableDef)
        {
            var createTable = tableDef.CreateTable;
            TestSyntax(createTable);
        }

        [Theory]
        [ClassData(typeof(ViewDefinition_dataprovider))]
        public void ExcerciseCreateView(IViewDefinition viewDef)
        {
            var createTable = viewDef.CreateView;
            TestSyntax(createTable);
        }

        private void TestSyntax(string commandText)
        {
            // test if it can be parsed
            var parsed = TSQL.TSQLTokenizer.ParseTokens(commandText).ToArray(); ;

            using (var database = new FMSC.ORM.SQLite.SQLiteDatastore())
            {
                database.Invoking(x => x.Execute("EXPLAIN " + commandText)).Should().NotThrow();
            }
        }

        private class TableDefinition_dataprovider : IEnumerable<object[]>
        {
            public IEnumerator<object[]> GetEnumerator()
            {
                return CruiseDatastoreBuilder_V3.TABLE_DEFINITIONS
                    .Select(x => new object[] { x })
                    .GetEnumerator();
            }

            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        }

        private class ViewDefinition_dataprovider : IEnumerable<object[]>
        {
            public IEnumerator<object[]> GetEnumerator()
            {
                return CruiseDatastoreBuilder_V3.VIEW_DEFINITIONS
                    .Select(x => new object[] { x })
                    .GetEnumerator();
            }

            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        }
    }
}