using System;
using System.Linq;
using Xunit;

using FMSC.ORM.TestSupport.TestModels;
using System.Data.Common;
using Xunit.Abstractions;
using FMSC.ORM.Core;
using FluentAssertions;

namespace FMSC.ORM.EntityModel.Support
{
    public class EntityCommandBuilderTest : TestClassBase
    {
        public EntityCommandBuilderTest(ITestOutputHelper output)
            : base(output)
        { }

        [Fact]
        public void MakeSelectCommandTest()
        {
            var ed = new EntityDescription(typeof(POCOMultiTypeObject));
            var provider = new SQLite.SQLiteProviderFactory();

            var commandBuilder = ed.CommandBuilder;
            var selectBuilder = commandBuilder.MakeSelectCommand(null);
            using (var command = provider.CreateCommand(selectBuilder.ToSQL()))
            {
                _output.WriteLine(command.CommandText);

                command.Should().NotBeNull();
                command.CommandText.Should().NotBeNullOrWhiteSpace();

                VerifyCommandSyntex(provider, command);
            }
        }

        [Fact]
        public void BuildInsertTest()
        {
            var ed = new EntityDescription(typeof(POCOMultiTypeObject));
            var provider = new SQLite.SQLiteProviderFactory();

            var data = new POCOMultiTypeObject();

            var commandBuilder = ed.CommandBuilder;
            using (var command = provider.CreateCommand())
            {
                commandBuilder.BuildInsertCommand(command, data, null, Core.SQL.OnConflictOption.Default);

                _output.WriteLine(command.CommandText);

                command.Should().NotBeNull();
                command.CommandText.Should().NotBeNullOrWhiteSpace();

                Assert.DoesNotContain("ID", command.CommandText);

                VerifyCommandSyntex(provider, command);
            }

            using (var command = provider.CreateCommand())
            {
                commandBuilder.BuildInsertCommand(command, data, 1, Core.SQL.OnConflictOption.Default);

                _output.WriteLine(command.CommandText);

                command.Should().NotBeNull();
                command.CommandText.Should().NotBeNullOrWhiteSpace();

                Assert.Contains("ID", command.CommandText);

                VerifyCommandSyntex(provider, command);
            }
        }

        [Fact]
        public void BuildUpdateTest()
        {
            var ed = new EntityDescription(typeof(POCOMultiTypeObject));
            var provider = new SQLite.SQLiteProviderFactory();

            var data = new POCOMultiTypeObject();

            var commandBuilder = ed.CommandBuilder;
            using (var command = provider.CreateCommand())
            {

                commandBuilder.BuildUpdateCommand(command, data, 1, Core.SQL.OnConflictOption.Default);


                command.Should().NotBeNull();
                command.CommandText.Should().NotBeNullOrWhiteSpace();

                Assert.Contains("ID", command.CommandText);

                command.Parameters.OfType<DbParameter>().Where(x => x.ParameterName == "@id")
                    .Should().HaveCount(1);

                command.Parameters.OfType<DbParameter>().Select(x => x.ParameterName).Should().OnlyHaveUniqueItems();

                VerifyCommandSyntex(provider, command);

                _output.WriteLine(command.CommandText);
            }

            //Assert.Throws(typeof(InvalidOperationException), () => commandBuilder.BuildUpdateCommand(provider, data, null, Core.SQL.OnConflictOption.Default));
        }

        [Fact]
        public void BuildDeleteTest()
        {
            var ed = new EntityDescription(typeof(POCOMultiTypeObject));
            var provider = new SQLite.SQLiteProviderFactory();

            var data = new POCOMultiTypeObject();

            var commandBuilder = ed.CommandBuilder;
            using (var command = provider.CreateCommand())
            {
                commandBuilder.BuildSQLDeleteCommand(command, data);

                command.Should().NotBeNull();
                command.CommandText.Should().NotBeNullOrWhiteSpace();

                command.Parameters.Should().HaveCount(1);
                command.Parameters.OfType<DbParameter>().Where(x => x.ParameterName == "@keyValue")
                    .Should().HaveCount(1);

                VerifyCommandSyntex(provider, command);
                _output.WriteLine(command.CommandText);
            }
        }

        public void VerifyCommandSyntex(DbProviderFactoryAdapter provider, DbCommand command)
        {
            command.CommandText = "EXPLAIN " + command.CommandText;

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