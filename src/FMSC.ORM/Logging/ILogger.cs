using System;

namespace FMSC.ORM.Logging
{
    public interface ILogger
    {
        void Log(string message, string cat, LogLevel lev);

        void LogException(Exception e, object data = null);
    }
}