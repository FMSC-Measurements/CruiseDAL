using System;
using System.Diagnostics;

namespace FMSC.ORM.Logging
{
    public class DefaultLogger : ILogger
    {
        public void Log(string message, string cat, LogLevel lev)
        {
            Debug.WriteLine($"[{cat}]:[{lev}]-{message}");
        }

        public void LogException(Exception e, object data = null)
        {
            Debug.WriteLine($"[Error]:::::::::::::{e.ToString()}");
        }
    }
}