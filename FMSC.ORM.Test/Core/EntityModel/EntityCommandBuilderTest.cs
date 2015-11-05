using System;
using System.Linq;
using Xunit;

using FMSC.ORM.TestSupport.TestModels;
using FMSC.ORM.XUnit;
using System.Data.Common;
using Xunit.Abstractions;

namespace FMSC.ORM.Core.EntityModel
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
            using (var command = commandBuilder.BuildInsertCommand(provider, data, SQL.OnConflictOption.Default))
            {
                _output.WriteLine(command.CommandText);

                Assert.NotNull(command);
                AssertEx.NotNullOrWhitespace(command.CommandText);

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
            using (var command = commandBuilder.BuildUpdateCommand(provider, data, SQL.OnConflictOption.Default))
            {
                _output.WriteLine(command.CommandText);

                Assert.NotNull(command);
                AssertEx.NotNullOrWhitespace(command.CommandText);

                VerifyCommandSyntex(provider, command);
            }
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
