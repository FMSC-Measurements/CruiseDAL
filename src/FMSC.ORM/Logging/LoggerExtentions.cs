using System;
using System.Data;

namespace FMSC.ORM.Logging
{
    public static class LoggerExtentions
    {
        public static void Log(this ILogger @this, string message, LogCategory cat, LogLevel lev)
        {
            @this.Log(message, cat.ToString(), lev);
        }

        public static void LogCommand(this ILogger @this, IDbCommand command)
        {
            @this.Log("Executing Command:" + command.CommandText, LogCategory.Command, LogLevel.Info);
        }

        public static void LogConnectionEvent(this ILogger @this, System.Data.StateChangeEventArgs e)
        {
            @this.Log("Connection state changed From " + e.OriginalState.ToString() + " to " + e.CurrentState.ToString(), LogCategory.Connection, LogLevel.Info);
        }

        public static void LogException(this ILogger @this, Exception e, object data = null)
        {
            @this.LogException(e, data);
        }
    }
}