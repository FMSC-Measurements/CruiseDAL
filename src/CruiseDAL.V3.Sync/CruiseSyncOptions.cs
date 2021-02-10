using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CruiseDAL.V3.Sync
{
    [Flags]
    public enum SyncFlags { Lock = 0, Insert = 1, Update = 3 };

    public class CruiseSyncOptions
    {
        public SyncFlags Design { get; set; }

        public SyncFlags TreeFlags { get; set; }

        public SyncFlags TreeDataFlags { get; set; }

        public SyncFlags FieldData { get; set; }

        public SyncFlags SamplerState { get; set; }

        public SyncFlags Validation { get; set; }

        public SyncFlags Processing { get; set; }

        public bool StemAllOrNone { get; set; }

        public bool PlotTreeAllOrNone { get; set; }

        public bool NonPlotTreeAllOrNone { get; set; }

    }
}
