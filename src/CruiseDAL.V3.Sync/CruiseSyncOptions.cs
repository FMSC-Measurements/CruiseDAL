using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CruiseDAL.V3.Sync
{
    [Flags]
    public enum SyncFlags { Lock = 0, Insert = 1, Update = 2, InsertUpdate = 3, ForceInsert = 5, ForceUpdate = 10, ForceInsertUpdate = 15 };

    public class CruiseSyncOptions
    {
        public SyncFlags Sale { get; set; } = SyncFlags.InsertUpdate;

        public SyncFlags Design { get; set; } = SyncFlags.InsertUpdate;

        public SyncFlags TreeFlags { get; set; } = SyncFlags.InsertUpdate;

        public SyncFlags TreeDataFlags { get; set; } = SyncFlags.InsertUpdate;

        public SyncFlags FieldData { get; set; } = SyncFlags.InsertUpdate;

        public SyncFlags SamplerState { get; set; } = SyncFlags.InsertUpdate;

        public SyncFlags Validation { get; set; } = SyncFlags.InsertUpdate;

        public SyncFlags Processing { get; set; } = SyncFlags.InsertUpdate;

        public SyncFlags TreeDefaultValue { get; set; } = SyncFlags.InsertUpdate;

        public SyncFlags Template { get; set; } = SyncFlags.InsertUpdate;

        public bool LogAllOrNone { get; set; } = false;

        public bool StemAllOrNone { get; set; } = false;

        public bool PlotTreeAllOrNone { get; set; } = false;

        public bool NonPlotTreeAllOrNone { get; set; } = false;
    }
}
