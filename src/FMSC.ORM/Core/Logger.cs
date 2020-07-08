using System;
using System.Data;
using System.Diagnostics;

namespace FMSC.ORM.Core
{
    public class Logger
    {
        public const string DB_CONTROL_VERBOSE = "DB_Control_Verbose";
        public const string DB_CONTROL = "DB_Control";
        public const string DS_EVENT = "DS_Event";
        public const string DS_DATA = "DS_DATA";

        [Conditional("Debug")]
        public void LogCommand(IDbCommand command)
        {
            Debug.WriteLine("Executing Command:" + command.CommandText);
        }

        public void LogConnectionEvent(System.Data.StateChangeEventArgs e)
        {
            Debug.WriteLine("Connection state changed From " + e.OriginalState.ToString() + " to " + e.CurrentState.ToString(), DS_EVENT);
        }

        public void LogConnection(string message)
        {
            Debug.WriteLine(message, DS_EVENT);
        }

        public void LogException(Exception ex)
        {
            Debug.WriteLine(ex.ToString());
        }

        public void WriteLine(string message, string catagory)
        {
            Debug.WriteLine(message, catagory);
        }
    }
}