﻿using CruiseDAL.DataObjects;
using System;
using System.Collections.Generic;
using System.Text;

namespace CruiseDAL
{
    public static class DALExtentions
    {
        public static void LogMessage(this DAL dal, string message, string level)
        {
            string appStr = GetCallingProgram();
            dal.LogMessage(appStr, message, level);
        }

        public static void LogMessage(this DAL dal, string program, string message, string level)
        {
            Logger.Log.L(message);

            if (dal.Exists)
            {
                var msg = new MessageLogDO(dal);
                msg.Program = program;
                msg.Message = message;
                msg.Level = level;
                msg.Date = DateTime.Now.ToString("yyyy/MM/dd");
                msg.Time = DateTime.Now.ToString("HH:mm");
                msg.Save();
            }
        }

        private static string GetCallingProgram()
        {
#if !NetCF
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

        
    }
}