﻿using FMSC.ORM.Logging;
using System;
using Xunit.Abstractions;

namespace CruiseDAL.TestCommon
{
    public class TestLogger : FMSC.ORM.Logging.ILogger
    {
        public ITestOutputHelper Output { get; set; }

        public TestLogger(ITestOutputHelper output)
        {
            Output = output ?? throw new ArgumentNullException(nameof(output));
        }

        public void Log(string message, string cat, LogLevel lev)
        {
            Output.WriteLine($"CruiseDAL Log::{cat}::{lev}::{message}");
        }

        public void LogException(Exception e, object data = null)
        {
            Output.WriteLine($"CruiseDAL Log::[Error]:::::::::::::{e.ToString()}");
        }
    }
}