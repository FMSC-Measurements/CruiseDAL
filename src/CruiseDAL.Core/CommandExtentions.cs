using FMSC.ORM.Core;
using System;
using System.Data.Common;

namespace CruiseDAL
{
    public static class CommandExtentions
    {
        public static void LogMessage(this DbConnection connection, string message, string level)
        {
            LogMessage(connection, message, level, null);
        }

        public static void LogMessage(this DbConnection connection, string message, string level, DbTransaction transaction)
        {
            string appStr = GetCallingProgram();

            LogMessage(connection, appStr, message, level, transaction);
        }

        public static void LogMessage(this DbConnection connection, string program, string message, string level, DbTransaction transaction)
        {
            Logger.Log.L(message);

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

        private static string GetCallingProgram()
        {
#if !WindowsCE
            try
            {
                return System.Reflection.Assembly.GetEntryAssembly().FullName;
            }
            catch
            {
                //TODO add error report message so we know when we encounter this exception and what platforms
                return AppDomain.CurrentDomain.FriendlyName;
            }
#else
            return AppDomain.CurrentDomain.FriendlyName;

#endif
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