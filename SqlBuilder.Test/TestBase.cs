using FluentAssertions;
using SqlBuilder.Dialects;
using System.Data.Common;
using System.IO;
using Xunit.Abstractions;

namespace SqlBuilders.Test
{
    public abstract class TestBase
    {
        protected readonly ITestOutputHelper Output;
        private string _testTempPath;
        protected DbProviderFactory DbProvider { get; set; }
        protected ISqlDialect Dialect { get; private set; }

        public TestBase(ITestOutputHelper output)
        {
            Output = output;
            Dialect = new SqliteDialect();

#if SYSTEM_DATA_SQLITE
            DbProvider = System.Data.SQLite.SQLiteFactory.Instance;
#endif
        }

        public string TestFilePath
        {
            get
            {
                return _testTempPath ?? (_testTempPath = Path.Combine(Path.GetTempPath(), "TestTemp", this.GetType().FullName));
            }
        }

        protected void VerifyCommandSyntex(string commandText)
        {
            using (var conn = DbProvider.CreateConnection())
            {
                var command = conn.CreateCommand();
                Output.WriteLine("testing:\r\n" + commandText);
                command.CommandText = "EXPLAIN " + commandText;

#if MICROSOFT_DATA_SQLITE
                var connectionString = "Data Source =:memory:;";
#elif SYSTEM_DATA_SQLITE
                var connectionString = "Data Source =:memory:; Version = 3; New = True;";
#endif
                conn.ConnectionString = connectionString;
                conn.Open();

                command.Connection = conn;

                try
                {
                    command.ExecuteNonQuery();//calling execute should always throw but we check that it isn't a syntax exception
                }
                catch (DbException ex)
                {
                    ex.Message.Should().NotContainEquivalentOf("syntax");
                    //Assert.DoesNotContain("syntax ", ex.Message, StringComparison.InvariantCultureIgnoreCase);
                }
            }
        }
    }
}