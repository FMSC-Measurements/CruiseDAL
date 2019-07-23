using FluentAssertions;
using FMSC.ORM.TestSupport.TestModels;
using Backpack.SqlBuilder;
using System.Data.Common;
using System.Linq;
using Xunit;
using Xunit.Abstractions;

namespace FMSC.ORM.EntityModel.Support
{
    public class EntityCommandBuilderTest : TestBase
    {
        public EntityCommandBuilderTest(ITestOutputHelper output)
            : base(output)
        { }

        private void ValidateCommand(DbCommand command)
        {
            for (int i = 0; i < command.Parameters.Count; i++)
            {
                var param = command.Parameters[i];
                param.Value.Should().NotBeNull();
                param.ParameterName.Should().NotBeNull();
            }

            command.CommandText.Should().NotBeNullOrWhiteSpace();
        }

        [Fact]
        public void MakeSelectCommandTest()
        {
            var ed = new EntityDescription(typeof(POCOMultiTypeObject));
            var commandBuilder = ed.CommandBuilder;

            var selectBuilder = commandBuilder.MakeSelectCommand(null);
            var commandText = selectBuilder.ToString();

            Output.WriteLine(commandText);

            commandText.Should().NotBeNullOrWhiteSpace();

            VerifyCommandSyntex(commandText);
        }

        [Fact]
        public void BuildInsert_Without_KeyData()
        {
            var data = new POCOMultiTypeObject();

            var ed = new EntityDescription(typeof(POCOMultiTypeObject));
            var commandBuilder = ed.CommandBuilder;

            using (var command = DbProvider.CreateCommand())
            {
                commandBuilder.BuildInsertCommand(command, data, null, OnConflictOption.Default);

                ValidateCommand(command);

                var commandText = command.CommandText;

                Output.WriteLine(commandText);
                commandText.Should().NotContain("@ID", "Insert with no keyData should not assign ID column");

                VerifyCommandSyntex(commandText);
            }
        }

        [Fact]
        public void BuildInsertTest_withKeyData()
        {
            var data = new POCOMultiTypeObject();

            var ed = new EntityDescription(typeof(POCOMultiTypeObject));
            var commandBuilder = ed.CommandBuilder;

            using (var command = DbProvider.CreateCommand())
            {
                commandBuilder.BuildInsertCommand(command, data, 1, OnConflictOption.Default);
                var commandText = command.CommandText;
                Output.WriteLine(commandText);

                ValidateCommand(command);
                commandText.Should().Contain("ID", "Insert with keyData should assign ID column");

                VerifyCommandSyntex(commandText);
            }
        }

        [Fact]
        public void BuildUpdateTest()
        {
            var ed = new EntityDescription(typeof(POCOMultiTypeObject));
            var commandBuilder = ed.CommandBuilder;

            var data = new POCOMultiTypeObject();

            using (var command = DbProvider.CreateCommand())
            {
                commandBuilder.BuildUpdateCommand(command, data, 1, OnConflictOption.Default);
                var commandText = command.CommandText;

                ValidateCommand(command);
                commandText.Should().Contain("ID", "");

                command.Parameters.OfType<DbParameter>().Where(x => x.ParameterName == "@id")
                    .Should().HaveCount(1);

                command.Parameters.OfType<DbParameter>().Select(x => x.ParameterName).Should().OnlyHaveUniqueItems();

                VerifyCommandSyntex(commandText);

                Output.WriteLine(command.CommandText);
            }

            //Assert.Throws(typeof(InvalidOperationException), () => commandBuilder.BuildUpdateCommand(provider, data, null, Core.SQL.OnConflictOption.Default));
        }

        [Fact]
        public void BuildDeleteTest()
        {
            var ed = new EntityDescription(typeof(POCOMultiTypeObject));

            var data = new POCOMultiTypeObject()
            {
                ID = 1,
            };

            var commandBuilder = ed.CommandBuilder;
            using (var command = DbProvider.CreateCommand())
            {
                commandBuilder.BuildSQLDeleteCommand(command, data);
                var commandText = command.CommandText;

                ValidateCommand(command);

                command.Parameters.Should().HaveCount(1);

                VerifyCommandSyntex(commandText);
                Output.WriteLine(command.CommandText);
            }
        }
    }
}