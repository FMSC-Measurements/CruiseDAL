using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CruiseDAL.V3.Sync
{
    public class ConflictResolutionOptions
    {
        public ConflictResolutionOptions()
        {
            CuttingUnit = new Dictionary<string, Conflict>();
            Stratum = new Dictionary<string, Conflict>();
            SampleGroup = new Dictionary<string, Conflict>();
            Plot = new Dictionary<string, Conflict>();
            Tree = new Dictionary<string, Conflict>();
            Log = new Dictionary<string, Conflict>();
        }

        public ConflictResolutionOptions(IEnumerable<Conflict> excludeUnitIDs,
            IEnumerable<Conflict> excludeStratumIDs,
            IEnumerable<Conflict> excludeSampleGroupIDs,
            IEnumerable<Conflict> excludePlotIDs,
            IEnumerable<Conflict> excludeTreeIDs,
            IEnumerable<Conflict> excludeLogIDs)
        {
            CuttingUnit = excludeUnitIDs.ToDictionary(x => x.SourctRecID, x => x);
            Stratum = excludeStratumIDs.ToDictionary(x => x.SourctRecID, x => x);
            SampleGroup = excludeSampleGroupIDs.ToDictionary(x => x.SourctRecID, x => x);
            Plot = excludePlotIDs.ToDictionary(x => x.SourctRecID, x => x);
            Tree = excludeTreeIDs.ToDictionary(x => x.SourctRecID, x => x);
            Log = excludeLogIDs.ToDictionary(x => x.SourctRecID, x => x);
        }

        public Dictionary<string, Conflict> CuttingUnit { get; set; }

        public Dictionary<string, Conflict> Stratum { get; set; }

        public Dictionary<string, Conflict> SampleGroup { get; set; }

        public Dictionary<string, Conflict> Plot { get; set; }

        public Dictionary<string, Conflict> Tree { get; set; }

        public Dictionary<string, Conflict> Log { get; set; }
    }
}
