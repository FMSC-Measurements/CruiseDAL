using FMSC.ORM.Logging;
using System;
using TechTalk.SpecFlow.Infrastructure;

namespace CruiseDAL.V3.Sync.Test.Spec
{
    public class TestLogger : FMSC.ORM.Logging.ILogger
    {
        public ISpecFlowOutputHelper Output { get; set; }

        public TestLogger(ISpecFlowOutputHelper output)
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