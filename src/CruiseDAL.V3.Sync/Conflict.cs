using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CruiseDAL.V3.Sync
{
    public class Conflict
    {
        public enum ConflictResolutionType { NotSet, ChoseSource, ChoseDest, ModifySource, ModifyDest }

        public string Table { get; set; }

        public string SourctRecID { get; set; }

        public string DestRecID { get; set; }

        public object SourceRec { get; set; }

        public object DestRec { get; set; }

        public bool MergeChildren { get; set; }

        public ConflictResolutionType ConflictResolution { get; set; } = Conflict.ConflictResolutionType.NotSet;
    }
}
