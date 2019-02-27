using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using Xunit;

namespace CruiseDAL.Tests.Schema.V3
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
                var type = typeof(CruiseDAL.Schema.V3.DDL);
                var fields = type.GetFields(System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public);

                return fields.Where(f => f.Name.StartsWith("CREATE_VIEW", StringComparison.InvariantCultureIgnoreCase) || f.Name.StartsWith("CREATE_TABLE", StringComparison.InvariantCultureIgnoreCase))
                    .Select(f => new object[] { f.Name }).GetEnumerator();
            }

            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        }

        [Theory]
        [ClassData(typeof(Test_DDL_Syntax_Data))]
        //[InlineData(nameof(DDL.CREATE_TABLE_CUTTINGUNIT_STRATUM))]
        //[InlineData(nameof(DDL.CREATE_TABLE_SAMPLEGROUP_SPECIES))]
        //[InlineData(nameof(DDL.CREATE_TABLE_SampleGroup_V3))]
        //[InlineData(nameof(DDL.CREATE_TABLE_SPECIES))]
        //[InlineData(nameof(DDL.CREATE_TABLE_TALLYPOPULATION))]
        //[InlineData(nameof(DDL.CREATE_TABLE_TREEMEASURMENT))]
        //[InlineData(nameof(DDL.CREATE_TALBE_PLOT_STRATUM))]
        public void Test_DDL_Syntax(string commandName)
        {
            var type = typeof(CruiseDAL.Schema.V3.DDL);
            var fieldAccessor = type.GetField(commandName, System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public);
            var command = (string)fieldAccessor.GetValue(null);

            TestSyntax(command);
        }
    }
}
