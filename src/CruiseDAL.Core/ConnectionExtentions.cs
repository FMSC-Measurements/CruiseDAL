using FMSC.ORM.Core;
using FMSC.ORM.Logging;
using System;
using System.Data.Common;

namespace CruiseDAL
{
    public static class ConnectionExtentions
    {
        private static ILogger Logger = LoggerProvider.Get();

        public static void LogMessage(this DbConnection connection,  string message, string level = "I", string program = null, DbTransaction transaction = null)
        {
            if (connection is null) { throw new ArgumentNullException(nameof(connection)); }
            if (message is null) { throw new ArgumentNullException(nameof(message)); }

            if(program == null)
            {
                program = CruiseDatastore.GetCallingProgram();
            }

            Logger.Log(message, "LogMessage", LogLevel.Info);

            connection.ExecuteNonQuery("INSERT INTO MessageLog (Program, Message, Level, Date, Time) " +
                    "VALUES " +
                    "(@p1, @p2, @p3, @p4, @p5)",
                    new object[] {
                        program,
                        message,
                        level,
                        DateTime.Now.ToString("yyyy/MM/dd"),
                        DateTime.Now.ToString("HH:mm") }
                    , transaction
                    );
        }

        public static string ReadDatabaseVersion(this DbConnection connection)
        {
            return connection.ReadGlobalValue("Database", "Version", (DbTransaction)null);
        }

        public static string ReadGlobalValue(this DbConnection connection, String block, String key, DbTransaction transaction)
        {
            return connection.ExecuteScalar("SELECT Value FROM GLOBALS WHERE " +
            "ifnull(Block, '') = ifnull(@p1, '') AND ifnull(Key, '') = ifnull(@p2, '');",
            new object[] { block, key },
            transaction) as string;
        }

        public static void WriteGlobalValue(this DbConnection connection, String block, String key, String value, DbTransaction transaction)
        {
            connection.ExecuteNonQuery("INSERT OR REPLACE INTO Globals (Block, Key, Value) " +
                "Values (@p1, @p2, @p3);",
                new object[] { block, key, value },
                transaction);
        }
    }
}