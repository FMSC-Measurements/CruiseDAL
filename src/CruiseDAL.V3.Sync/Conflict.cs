using CruiseDAL.V3.Sync.Util;
using System;
using System.Collections.Generic;

namespace CruiseDAL.V3.Sync
{
    public enum ConflictResolutionType
    { NotSet, ChoseSource, ChoseDest, ChoseLatest, ModifySource, ModifyDest, ChoseSourceMergeData, ChoseDestMergeData }

    public class Conflict : INPC_Base
    {
        public event EventHandler ResolutionChanged;

        private ConflictResolutionType _conflictResolution = ConflictResolutionType.NotSet;

        public string Table { get; set; }

        public string Identity { get; set; }

        public string SourceRecID { get; set; }

        public string DestRecID { get; set; }

        public object SourceRec { get; set; }

        public object DestRec { get; set; }

        public DateTime SourceMod { get; set; }

        public DateTime DestMod { get; set; }

        public IEnumerable<Conflict> DownstreamConflicts { get; set; }

        public ConflictResolutionType ConflictResolution 
        {
            get => _conflictResolution;
            set
            {
                SetProperty(ref _conflictResolution, value);
                ResolutionChanged?.Invoke(this, EventArgs.Empty);
            }
        } 
    }
}