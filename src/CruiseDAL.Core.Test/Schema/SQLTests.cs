using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using Xunit;

namespace CruiseDAL.Tests.Schema
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
                var ddlTypes = Assembly.GetAssembly(typeof(CruiseDAL.DAL)).GetTypes().Where(x => x.Name == "DDL");

                return ddlTypes.SelectMany(type => type.GetFields(System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public).Select(f => new { type, f })
                        .Where(x => x.f.Name.StartsWith("CREATE_VIEW", StringComparison.InvariantCultureIgnoreCase) 
                            || x.f.Name.StartsWith("CREATE_TABLE", StringComparison.InvariantCultureIgnoreCase)))
                    .Select(x => new object[] { x.type.FullName,  x.f.Name })
                .GetEnumerator();
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
        public void Test_DDL_Syntax(string ddlClassName, string commandName)
        {
            var type = Assembly.GetAssembly(typeof(CruiseDAL.DAL)).GetType(ddlClassName, true, true);

            var fieldAccessor = type.GetField(commandName, System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public);
            var command = (string)fieldAccessor.GetValue(null);

            TestSyntax(command);
        }
    }
}
