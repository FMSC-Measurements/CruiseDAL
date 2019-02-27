using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using Xunit;
using CruiseDAL.Schema.V2Backports;
using System.Collections;

namespace CruiseDAL.Tests.Schema.V2Backports
{
    public class SQLTests
    {
        void TestSyntax(string commandText)
        {
            using (var database = new FMSC.ORM.SQLite.SQLiteDatastore())
            {
                database.Invoking(x => x.Execute("EXPLAIN " + commandText)).Should().NotThrow();
            }
        }

        class Test_DDL_Syntax_Data : IEnumerable<object[]>
        {
            public IEnumerator<object[]> GetEnumerator()
            {
                var type = typeof(CruiseDAL.Schema.V2Backports.DDL);
                var fields = type.GetFields(System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public);

                return fields.Where(f => f.Name.StartsWith("CREATE_VIEW", StringComparison.InvariantCultureIgnoreCase) || f.Name.StartsWith("CREATE_TABLE", StringComparison.InvariantCultureIgnoreCase))
                    .Select(f => new object[] { f.Name }).GetEnumerator();
            }

            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        }

        [Theory]
        [ClassData(typeof(Test_DDL_Syntax_Data))]
        //[InlineData(nameof(DDL.CREATE_VIEW_COUNT_TREE))]
        //[InlineData(nameof(DDL.CREATE_VIEW_CUTTINGUNITSTRATUM))]
        //[InlineData(nameof(DDL.CREATE_VIEW_PLOT))]
        //[InlineData(nameof(DDL.CREATE_VIEW_SampleGroup))]
        //[InlineData(nameof(DDL.CREATE_VIEW_SAMPLEGROUPTREEDEFAULTVALUE))]
        //[InlineData(nameof(DDL.CREATE_VIEW_TREE))]
        //[InlineData(nameof(DDL.CREATE_VIEW_TREEESTIMATE))]
        public void Test_DDL_Syntax(string commandName)
        {
            var type = typeof(CruiseDAL.Schema.V2Backports.DDL);
            var fieldAccessor = type.GetField(commandName, System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public);
            var command = (string)fieldAccessor.GetValue(null);

            TestSyntax(command);
        }
    }
}
