using System;
using System.Linq;
using Xunit;

using FMSC.ORM.TestSupport.TestModels;
using System.Data.Common;
using Xunit.Abstractions;
using FMSC.ORM.Core;
using FluentAssertions;
using FMSC.ORM.SQLite;

namespace FMSC.ORM.EntityModel.Support
{
    public class EntityCommandBuilderTest : TestClassBase
    {
        private SqliteDialect dialect;


        public EntityCommandBuilderTest(ITestOutputHelper output)
            : base(output)
        {
            dialect = new SQLite.SqliteDialect();
        }

        [Fact]
        public void MakeSelectCommandTest()
        {
            

            var ed = new EntityDescription(typeof(POCOMultiTypeObject));
            var commandBuilder = ed.CommandBuilder;

            var selectBuilder = commandBuilder.MakeSelectCommand(null);
            var commandText = selectBuilder.ToSQL();

            _output.WriteLine(commandText);

            commandText.Should().NotBeNullOrWhiteSpace();

            VerifyCommandSyntex(dialect, commandText);
        }

        [Fact]
        public void BuildInsertTest_withOutKeyData()
        {
            var data = new POCOMultiTypeObject();

            var ed = new EntityDescription(typeof(POCOMultiTypeObject));
            var commandBuilder = ed.CommandBuilder;

            
            using (var command = dialect.CreateCommand())
            {
                commandBuilder.BuildInsertCommand(command, data, null, Core.SQL.OnConflictOption.Default);
                var commandText = command.CommandText;

                _output.WriteLine(commandText);

                commandText.Should().NotBeNullOrWhiteSpace();
                commandText.Should().NotContain("ID", "Inset with no keyData should not assign ID column");

                VerifyCommandSyntex(dialect, commandText);
            }
        }

        [Fact]
        public void BuildInsertTest_withKeyData()
        {
            var data = new POCOMultiTypeObject();

            var ed = new EntityDescription(typeof(POCOMultiTypeObject));
            var commandBuilder = ed.CommandBuilder;

            using (var command = dialect.CreateCommand())
            {
                commandBuilder.BuildInsertCommand(command, data, 1, Core.SQL.OnConflictOption.Default);
                var commandText = command.CommandText;
                _output.WriteLine(commandText);

                commandText.Should().NotBeNullOrWhiteSpace();
                commandText.Should().Contain("ID", "Insert with keyData should assign ID column");

                VerifyCommandSyntex(dialect, commandText);
            }
        }

        [Fact]
        public void BuildUpdateTest()
        {
            var ed = new EntityDescription(typeof(POCOMultiTypeObject));
            var commandBuilder = ed.CommandBuilder;

            var data = new POCOMultiTypeObject();

            
            using (var command = dialect.CreateCommand())
            {

                commandBuilder.BuildUpdateCommand(command, data, 1, Core.SQL.OnConflictOption.Default);
                var commandText = command.CommandText;

                commandText.Should().NotBeNullOrWhiteSpace();
                commandText.Should().Contain("ID", "");

                command.Parameters.OfType<DbParameter>().Where(x => x.ParameterName == "@id")
                    .Should().HaveCount(1);

                command.Parameters.OfType<DbParameter>().Select(x => x.ParameterName).Should().OnlyHaveUniqueItems();

                VerifyCommandSyntex(dialect, commandText);

                _output.WriteLine(command.CommandText);
            }

            //Assert.Throws(typeof(InvalidOperationException), () => commandBuilder.BuildUpdateCommand(provider, data, null, Core.SQL.OnConflictOption.Default));
        }

        [Fact]
        public void BuildDeleteTest()
        {
            var ed = new EntityDescription(typeof(POCOMultiTypeObject));

            var data = new POCOMultiTypeObject();

            var commandBuilder = ed.CommandBuilder;
            using (var command = dialect.CreateCommand())
            {
                commandBuilder.BuildSQLDeleteCommand(command, data);
                var commandText = command.CommandText;

                commandText.Should().NotBeNullOrWhiteSpace();

                command.Parameters.Should().HaveCount(1);
                command.Parameters.OfType<DbParameter>().Where(x => x.ParameterName == "@keyValue")
                    .Should().HaveCount(1);

                VerifyCommandSyntex(dialect, commandText);
                _output.WriteLine(command.CommandText);
            }
        }

        public void VerifyCommandSyntex(ISqlDialect dialect, String commandText)
        {
            var explainCommand = "EXPLAIN " + commandText;

            using (var connection = dialect.CreateConnection())
            {
                connection.ConnectionString = "Data Source =:memory:; Version = 3; New = True;";
                connection.Open();

                using (var command = connection.CreateCommand())
                {
                    command.CommandText = explainCommand;

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
}