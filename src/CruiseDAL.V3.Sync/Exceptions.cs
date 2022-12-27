using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CruiseDAL.V3.Sync
{
    public class SyncException : Exception
    {
        public SyncException()
        {
        }

        public SyncException(string message) : base(message)
        {
        }

        public SyncException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }


    public class SampleGroupSettingMismatchException : SyncException
    {
        public SampleGroupSettingMismatchException(string message) : base(message)
        {
        }

        public string CruiseID { get; set; }
        public string StratumCode { get; set; }
        public string SampleGroupCode { get; set; }

    }

    public class StratumSettingMismatchException : SyncException
    {
        public StratumSettingMismatchException(string message) : base(message)
        {
        }

        public string CruiseID { get; set; }
        public string StratumCode { get; set; }
    }
}
