using System;
using System.Linq;
using Xunit;

using FMSC.ORM.TestSupport.TestModels;
using System.Data.Common;
using Xunit.Abstractions;
using FMSC.ORM.Core;

namespace FMSC.ORM.EntityModel.Support
{
    public class EntityCommandBuilderTest : TestClassBase
    {
        public EntityCommandBuilderTest(ITestOutputHelper output)
            : base(output)
        { }

        [Fact]
        public void BuildSelectTest()
        {
            var ed = new EntityDescription(typeof(POCOMultiTypeObject));
            var provider = new SQLite.SQLiteProviderFactory();

            var commandBuilder = ed.CommandBuilder;
            using (var command = commandBuilder.BuildSelectCommand(provider, null))
            {
                _output.WriteLine(command.CommandText);
                Assert.NotNull(command);
                AssertEx.NotNullOrWhitespace(command.CommandText);
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
            using (var command = commandBuilder.BuildInsertCommand(provider, data, null, Core.SQL.OnConflictOption.Default))
            {
                _output.WriteLine(command.CommandText);

                Assert.NotNull(command);
                AssertEx.NotNullOrWhitespace(command.CommandText);

                Assert.DoesNotContain("ID", command.CommandText);

                VerifyCommandSyntex(provider, command);
            }

            using (var command = commandBuilder.BuildInsertCommand(provider, data, 1, Core.SQL.OnConflictOption.Default))
            {
                _output.WriteLine(command.CommandText);

                Assert.NotNull(command);
                AssertEx.NotNullOrWhitespace(command.CommandText);

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
            using (var command = commandBuilder.BuildUpdateCommand(provider, data, 1, Core.SQL.OnConflictOption.Default))
            {
                _output.WriteLine(command.CommandText);

                Assert.NotNull(command);
                AssertEx.NotNullOrWhitespace(command.CommandText);

                Assert.Contains("ID", command.CommandText);

                VerifyCommandSyntex(provider, command);
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
            using (var command = commandBuilder.BuildSQLDeleteCommand(provider, data))
            {
                _output.WriteLine(command.CommandText);

                Assert.NotNull(command);
                AssertEx.NotNullOrWhitespace(command.CommandText);

                VerifyCommandSyntex(provider, command);
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
                catch(DbException ex)
                {
                    Assert.DoesNotContain("syntax ", ex.Message, StringComparison.InvariantCultureIgnoreCase);
                }

            }
        }
    }
}
