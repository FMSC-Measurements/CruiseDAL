using System.Collections.Generic;

namespace CruiseDAL.V3.Sync
{
    internal class SyncExcludeOptions
    {
        public SyncExcludeOptions()
        {
            ExcludeUnitIDs = new HashSet<string>();
            ExcludeStrataIDs = new HashSet<string>();
            ExcludeSampleGroupIDs = new HashSet<string>();
            ExcludePlotIDs = new HashSet<string>();
            ExcludeTreeIDs = new HashSet<string>();
            ExcludeLogIDs = new HashSet<string>();
        }

        public SyncExcludeOptions(IEnumerable<string> excludeUnitIDs,
            IEnumerable<string> excludeStratumIDs,
            IEnumerable<string> excludeSampleGroupIDs,
            IEnumerable<string> excludePlotIDs,
            IEnumerable<string> excludeTreeIDs,
            IEnumerable<string> excludeLogIDs)
        {
            ExcludeUnitIDs = new HashSet<string>(excludeUnitIDs);
            ExcludeStrataIDs = new HashSet<string>(excludeStratumIDs);
            ExcludeSampleGroupIDs = new HashSet<string>(excludeSampleGroupIDs);
            ExcludePlotIDs = new HashSet<string>(excludePlotIDs);
            ExcludeTreeIDs = new HashSet<string>(excludeTreeIDs);
            ExcludeLogIDs = new HashSet<string>(excludeLogIDs);
        }

        public HashSet<string> ExcludeUnitIDs { get; set; }

        public HashSet<string> ExcludeStrataIDs { get; set; }

        public HashSet<string> ExcludeSampleGroupIDs { get; set; }

        public HashSet<string> ExcludePlotIDs { get; set; }

        public HashSet<string> ExcludeTreeIDs { get; set; }

        public HashSet<string> ExcludeLogIDs { get; set; }
    }
}