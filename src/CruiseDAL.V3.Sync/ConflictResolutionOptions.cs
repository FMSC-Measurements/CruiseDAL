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
            PlotTree = new Dictionary<string, Conflict>();
            Log = new Dictionary<string, Conflict>();
        }

        public ConflictResolutionOptions(IEnumerable<Conflict> cuttingUnit,
            IEnumerable<Conflict> strata,
            IEnumerable<Conflict> sampleGroup,
            IEnumerable<Conflict> plot,
            IEnumerable<Conflict> tree,
            IEnumerable<Conflict> plotTree,
            IEnumerable<Conflict> log)
        {
            CuttingUnit = cuttingUnit.ToDictionary(x => x.SourctRecID, x => x);
            Stratum = strata.ToDictionary(x => x.SourctRecID, x => x);
            SampleGroup = sampleGroup.ToDictionary(x => x.SourctRecID, x => x);
            Plot = plot.ToDictionary(x => x.SourctRecID, x => x);
            Tree = tree.ToDictionary(x => x.SourctRecID, x => x);
            PlotTree = plotTree.ToDictionary(x => x.SourctRecID, x => x);
            Log = log.ToDictionary(x => x.SourctRecID, x => x);
        }

        public Dictionary<string, Conflict> CuttingUnit { get; set; }

        public Dictionary<string, Conflict> Stratum { get; set; }

        public Dictionary<string, Conflict> SampleGroup { get; set; }

        public Dictionary<string, Conflict> Plot { get; set; }

        public Dictionary<string, Conflict> Tree { get; set; }

        public Dictionary<string, Conflict> PlotTree { get; set; }

        public Dictionary<string, Conflict> Log { get; set; }
    }
}
