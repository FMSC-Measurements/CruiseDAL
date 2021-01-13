using FluentAssertions;
using FMSC.ORM.TestSupport.TestModels;
using Backpack.SqlBuilder;
using System.Data.Common;
using System.Linq;
using Xunit;
using Xunit.Abstractions;
using FMSC.ORM.Test;
using FMSC.ORM.Sql;

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
            var commandBuilder = new CommandBuilder();

            var selectBuilder = commandBuilder.BuildSelect(ed.Source, ed.Fields);
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
            var commandBuilder = new CommandBuilder();

            using (var command = DbProvider.CreateCommand())
            {
                commandBuilder.BuildInsert(command, data, ed.SourceName, ed.Fields, OnConflictOption.Default);

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
            var commandBuilder = new CommandBuilder();

            using (var command = DbProvider.CreateCommand())
            {
                commandBuilder.BuildInsert(command, data, ed.SourceName, ed.Fields, OnConflictOption.Default, keyValue: 1 );
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
            var commandBuilder = new CommandBuilder();

            var data = new POCOMultiTypeObject()
            {
                ID = 1,
            };

            using (var command = DbProvider.CreateCommand())
            {
                commandBuilder.BuildUpdate(command, data, ed.SourceName, ed.Fields, option: OnConflictOption.Default);
                var commandText = command.CommandText;

                ValidateCommand(command);
                commandText.Should().Contain("ID", "");

                command.Parameters.OfType<DbParameter>().Where(x => x.ParameterName == "@ID")
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
            var data = new POCOMultiTypeObject()
            {
                ID = 1,
            };

            var ed = new EntityDescription(typeof(POCOMultiTypeObject));
            var commandBuilder = new CommandBuilder();
            using (var command = DbProvider.CreateCommand())
            {
                commandBuilder.BuildDelete(command, data, ed.SourceName, ed.Fields);
                var commandText = command.CommandText;

                ValidateCommand(command);

                command.Parameters.Should().HaveCount(1);

                VerifyCommandSyntex(commandText);
                Output.WriteLine(command.CommandText);
            }
        }
    }
}