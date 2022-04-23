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
            CuttingUnit = Enumerable.Empty<Conflict>();
            Stratum = Enumerable.Empty<Conflict>();
            SampleGroup = Enumerable.Empty<Conflict>();
            Plot = Enumerable.Empty<Conflict>();
            Tree = Enumerable.Empty<Conflict>();
            PlotTree = Enumerable.Empty<Conflict>();
            Log = Enumerable.Empty<Conflict>();
        }

        public ConflictResolutionOptions(IEnumerable<Conflict> cuttingUnit,
            IEnumerable<Conflict> strata,
            IEnumerable<Conflict> sampleGroup,
            IEnumerable<Conflict> plot,
            IEnumerable<Conflict> tree,
            IEnumerable<Conflict> plotTree,
            IEnumerable<Conflict> log)
        {
            CuttingUnit = cuttingUnit.ToArray();
            Stratum = strata.ToArray();
            SampleGroup = sampleGroup.ToArray();
            Plot = plot.ToArray();
            Tree = tree.ToArray();
            PlotTree = plotTree.ToArray();
            Log = log.ToArray();
        }

        public IEnumerable<Conflict> CuttingUnit { get; set; }

        public IEnumerable<Conflict> Stratum { get; set; }

        public IEnumerable<Conflict> SampleGroup { get; set; }

        public IEnumerable<Conflict> Plot { get; set; }

        public IEnumerable<Conflict> Tree { get; set; }

        public IEnumerable<Conflict> PlotTree { get; set; }

        public IEnumerable<Conflict> Log { get; set; }


        public bool AllHasResolutions()
        {
            var units = !CuttingUnit.Any() || CuttingUnit.All(x => x.ConflictResolution != ConflictResolutionType.NotSet);
            var strata = !Stratum.Any() || Stratum.All(x => x.ConflictResolution != ConflictResolutionType.NotSet);
            var sgs = !SampleGroup.Any() || SampleGroup.All(x => x.ConflictResolution != ConflictResolutionType.NotSet);
            var plots = !Plot.Any() || Plot.All(x => x.ConflictResolution != ConflictResolutionType.NotSet);
            var trees = !Tree.Any() || Tree.All(x => x.ConflictResolution != ConflictResolutionType.NotSet);
            var plotTrees = !PlotTree.Any() || PlotTree.All(x => x.ConflictResolution != ConflictResolutionType.NotSet);
            var logs = !Log.Any() || Log.All(x => x.ConflictResolution != ConflictResolutionType.NotSet);

            return units && strata && sgs && plots && trees && plotTrees && logs;
        }

        public bool HasAny()
        {
            var units = CuttingUnit.Any();
            var strata = Stratum.Any();
            var sgs = SampleGroup.Any();
            var plots = Plot.Any();
            var trees = Tree.Any();
            var plotTrees = PlotTree.Any();
            var logs = Log.Any();

            return units || strata || sgs || plots || trees || plotTrees || logs;
        }
    }
}
