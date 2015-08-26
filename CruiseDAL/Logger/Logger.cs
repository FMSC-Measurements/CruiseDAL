using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Diagnostics;

namespace Logger
{
    //public enum AppName {None = 0, CP, FS, CD, CSM, CC}

    public static class Log
    {

        private static bool _verbose; 
        public static bool VerboseEnabled
        {
            get { return _verbose;}
            set { _verbose = value; }
        }


        #region methods 


        /// <summary>
        /// Log
        /// </summary>
        /// <param name="message"></param>
        /// <param name="app"></param>
        public static void L(string message)
        {
            System.Diagnostics.Debug.Write(message, "Info");
        }

        /// <summary>
        /// Verbose
        /// </summary>
        /// <param name="message"></param>
        /// <param name="app"></param>
        public static void V(string message)
        {
            if (_verbose)
            {
                System.Diagnostics.Debug.Write(message, "Verbose");
            }
            
        }

        /// <summary>
        /// Error
        /// </summary>
        /// <param name="message"></param>
        /// <param name="app"></param>
        public static void E(string message)
        {
            System.Diagnostics.Debug.Write(message, "Error");
        }

        public static void E(string message, Exception e)
        {

            System.Diagnostics.Debug.Write(message + " | " + e.ToString(), "Error");
            
        }

        public static void E(Exception e)
        {
            System.Diagnostics.Debug.Write(e.ToString(), "Error");
            
        }

        /// <summary>
        /// This will only be called when Visual Studio is in "Debug"
        /// </summary>
        /// <param name="message"></param>
        [Conditional("DEBUG")]//this is how to create a method that only gets called in "Debug"
        public static void D(string message)
        {
            System.Diagnostics.Debug.Write(message, "Debug");
        }

        #endregion




    }
}
