using FluentAssertions;
using System;
using System.Data.Common;

namespace CruiseDAL.TestCommon
{
    public static class SqlTestHelper
    {
        public static DbProviderFactory DbProvider { get; set; }

        static SqlTestHelper()
        {
#if SYSTEM_DATA_SQLITE
            DbProvider = System.Data.SQLite.SQLiteFactory.Instance;
#elif MICROSOFT_DATA_SQLITE
            DbProvider = Microsoft.Data.Sqlite.SqliteFactory.Instance;
#endif
        }

        public static void VerifyCommandSyntex(string commandText)
        {
#if SYSTEM_DATA_SQLITE || MICROSOFT_DATA_SQLITE
            using (var conn = DbProvider.CreateConnection())
            {
                var command = conn.CreateCommand();
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
#else
            throw new NotImplementedException();
#endif
        }
    }
}