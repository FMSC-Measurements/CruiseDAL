using FluentAssertions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Xunit;
using Xunit.Abstractions;

namespace CruiseDAL.Tests.Schema
{
    public class DDL_Tests : TestBase
    {
        public DDL_Tests(ITestOutputHelper output) : base(output)
        {
        }

        private void TestSyntax(string commandText)
        {
            using (var database = new FMSC.ORM.SQLite.SQLiteDatastore())
            {
                database.Invoking(x => x.Execute("EXPLAIN " + commandText)).Should().NotThrow();
            }
        }

        private class Test_DDL_CREATE_TABLE_Syntax_data : IEnumerable<object[]>
        {
            public IEnumerator<object[]> GetEnumerator()
            {
                var ddlType = typeof(CruiseDAL.Schema.DDL);

                return ddlType.GetFields(System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public)
                        .Where(x => x.Name.StartsWith("CREATE_TABLE", StringComparison.InvariantCultureIgnoreCase))
                    .Select(x => new object[] { x.Name })
                .GetEnumerator();
            }

            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        }

        private class Test_DDL_CREATE_VIEW_Syntax_data : IEnumerable<object[]>
        {
            public IEnumerator<object[]> GetEnumerator()
            {
                var ddlType = typeof(CruiseDAL.Schema.DDL);

                return ddlType.GetFields(System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public)
                        .Where(x => x.Name.StartsWith("CREATE_VIEW", StringComparison.InvariantCultureIgnoreCase))
                    .Select(x => new object[] { x.Name })
                .GetEnumerator();
            }

            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        }

        [Theory]
        [ClassData(typeof(Test_DDL_CREATE_TABLE_Syntax_data))]
        public void Test_DDL_CREATE_TABLE_Syntax(string commandName)
        {
            var type = typeof(CruiseDAL.Schema.DDL);

            var fieldAccessor = type.GetField(commandName, System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public);
            var command = (string)fieldAccessor.GetValue(null);

            TestSyntax(command);
        }

        [Theory]
        [ClassData(typeof(Test_DDL_CREATE_VIEW_Syntax_data))]
        public void Test_DDL_CREATE_VIEW_Syntax(string commandName)
        {
            var type = typeof(CruiseDAL.Schema.DDL);

            var fieldAccessor = type.GetField(commandName, System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public);
            var command = (string)fieldAccessor.GetValue(null);

            TestSyntax(command);
        }

        [Fact]
        public void CREATE_COMMANDS_Contains_All_Public_Static_String_commands()
        {
            var commandsLookup = CruiseDAL.Schema.DDL.CREATE_COMMANDS.ToDictionary(x => x);

            var type = typeof(CruiseDAL.Schema.DDL);

            var fields = type.GetFields(System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public)
                .Where(x => x.FieldType == typeof(string));

            

            foreach(var field in  fields)
            {
                var command = (string)field.GetValue(null);
                commandsLookup.ContainsKey(command).Should().BeTrue(field.Name);
            }

            commandsLookup.Count().Should().Be(fields.Count());
        }
    }
}