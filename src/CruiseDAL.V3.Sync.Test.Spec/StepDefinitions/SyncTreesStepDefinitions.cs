using CruiseDAL.V3.Models;
using TechTalk.SpecFlow.Infrastructure;

namespace CruiseDAL.V3.Sync.Test.Spec.StepDefinitions
{
    [Binding]
    public class SyncTreesStepDefinitions : SyncStepDefinitionBase
    {
        public SyncTreesStepDefinitions(ISpecFlowOutputHelper output, ScenarioContext senarioContext, FeatureContext featureContext) :
            base(output, senarioContext, featureContext)
        {
        }

        [Given(@"in '([^']*)' the following trees exist:")]
        public void GivenInTheFollowingTreesExist(string dbNamesArg, Table table)
        {
            var databasesNames = dbNamesArg.Split(",", StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
            var databases = databasesNames.Select(x => DatabaseLookup[x]).ToArray();
            databases.Should().NotBeEmpty();

            var cruiseID = CruiseID;

            foreach (var row in table.Rows)
            {
                row.TryGetValue("TreeID", out var treeIDTag);
                var treeID = (treeIDTag != null) ? GetOrGenerateRecordID(treeIDTag)
                    : Guid.NewGuid().ToString();
                var treeNumber = Int32.Parse(row["TreeNumber"]);
                var unitCode = row["CuttingUnitCode"];
                var stCode = row["StratumCode"];
                var sgCode = row["SampleGroupCode"];
                var spCode = row["SpeciesCode"];

                row.TryGetValue(nameof(Tree.PlotNumber), out var plotNumberStr);
                var plotNumber = (plotNumberStr != null) ? (int?)int.Parse(plotNumberStr) : null;

                var tree = new Tree
                {
                    CruiseID = cruiseID,
                    TreeID = treeID,
                    TreeNumber = treeNumber,
                    CuttingUnitCode = unitCode,
                    StratumCode = stCode,
                    SampleGroupCode = sgCode,
                    SpeciesCode = spCode,
                    PlotNumber = plotNumber,
                };

                //var tallyLedgerID = Guid.NewGuid().ToString();
                var tallyLedgerID = treeID; //reuse treeID for tally ledger ID
                var tallyLedger = new TallyLedger
                {
                    CruiseID = cruiseID,
                    TreeID = treeID,
                    TallyLedgerID = tallyLedgerID,
                    CuttingUnitCode = unitCode,
                    StratumCode = stCode,
                    SampleGroupCode = sgCode,
                    SpeciesCode = spCode,
                    PlotNumber = plotNumber,
                };

                var tm = new TreeMeasurment
                {
                    TreeID = treeID,
                };

                foreach (var db in databases)
                {
                    db.Insert(tree);
                    db.Insert(tallyLedger);
                    db.Insert(tm);
                }
            }
        }

        [Then(@"TreeConflicts has no downstream conflicts")]
        public void ThenTreeConflictsHasNoDownstreamConflicts()
        {
            var conflictResults = ConflictResults;
            conflictResults.Tree.Should().NotContain(x => x.DownstreamConflicts != null && x.DownstreamConflicts.Any());
        }

        [Then(@"TreeConflicts has (.*) conflict\(s\)")]
        public void ThenTreeConflictsHasConflictS(int conflictCount)
        {
            var conflictResults = ConflictResults;
            conflictResults.Tree.Should().HaveCount(conflictCount);
        }

        [Then(@"TreeConflicts records has:")]
        public void ThenTreeConflictsRecordsHas(Table table)
        {
            var conflictResults = ConflictResults;
            var treeConflicts = conflictResults.Tree;

            foreach (var row in table.Rows)
            {
                var srcRecID = GetRedordID(row[nameof(Conflict.SourceRecID)]);
                var destRecID = GetRedordID(row[nameof(Conflict.DestRecID)]);

                var treeConflict = treeConflicts.Single(x => x.SourceRecID == srcRecID && x.DestRecID == destRecID);
                //treeConflict.Should().NotBeNull();

                if (row.TryGetValue("DownstreamConflictCount", out var downstreamConfCountStr))
                {
                    var downstreamConfCount = int.Parse(downstreamConfCountStr);
                    if (downstreamConfCount > 0)
                    {
                        treeConflict.DownstreamConflicts.Should().NotBeNull();
                        treeConflict.DownstreamConflicts.Count().Should().Be(downstreamConfCount);
                    }
                }
            }
        }

        [Then(@"'([^']*)' contains trees:")]
        public void ThenContainsTrees(string dbAlias, Table table)
        {
            var db = GetDatabase(dbAlias);

            var trees = db.From<Tree>().Query().ToArray();

            foreach (var row in table.Rows)
            {
                var treeIDAlias = row[nameof(Tree.TreeID)];
                var treeID = GetRedordID(treeIDAlias);

                row.TryGetValue(nameof(Tree.TreeNumber), out var treeNumberStr);
                row.TryGetValue(nameof(Tree.CuttingUnitCode), out var cuttingUnitCode);
                var tree = trees.SingleOrDefault(x =>
                    x.TreeID == treeID
                        && (treeNumberStr == null || x.TreeNumber == int.Parse(treeNumberStr))
                        && (cuttingUnitCode == null || x.CuttingUnitCode == cuttingUnitCode));

                tree.Should().NotBeNull(because: treeIDAlias);

                db.From<TreeMeasurment>().Where("TreeID = @p1").Count(treeID).Should().Be(1);
                db.From<TallyLedger>().Where("TreeID = @p1").Count(treeID).Should().Be(1);
            }
            trees.Should().HaveCount(table.RowCount);
        }

        [When(@"I resolve tree conflicts with ModifyDest using:")]
        public void WhenIResolveTreeConflictsWithModifyDestUsing(Table table)
        {
            var treeConflicts = ConflictResults.Tree;

            foreach (var row in table.Rows)
            {
                var destRecIDAlias = row[nameof(Conflict.DestRecID)];
                var destRecID = GetRedordID(destRecIDAlias);

                var conflict = treeConflicts.Single(x => x.DestRecID == destRecID);
                conflict.ConflictResolution = ConflictResolutionType.ModifyDest;

                var newTreeNumber = int.Parse(row[nameof(Tree.TreeNumber)]);
                ((Tree)conflict.DestRec).TreeNumber = newTreeNumber;
            }
        }

        [When(@"I resolve tree conflicts with ModifySource using:")]
        public void WhenIResolveTreeConflictsWithModifySourceUsing(Table table)
        {
            var treeConflicts = ConflictResults.Tree;

            foreach (var row in table.Rows)
            {
                var srcRecIDAlias = row[nameof(Conflict.SourceRecID)];
                var srcRecID = GetRedordID(srcRecIDAlias);

                var conflict = treeConflicts.Single(x => x.SourceRecID == srcRecID);
                conflict.ConflictResolution = ConflictResolutionType.ModifySource;

                var newTreeNumber = int.Parse(row[nameof(Tree.TreeNumber)]);
                ((Tree)conflict.SourceRec).TreeNumber = newTreeNumber;
            }
        }

        [When(@"I resolve tree conflicts with ChoseSourceMergeData using:")]
        public void WhenIResolveTreeConflictsWithChoseSourceMergeDataUsing(Table table)
        {
            var treeConflicts = ConflictResults.Tree;

            foreach (var row in table.Rows)
            {
                var sourceRecIDAlias = row[nameof(Conflict.SourceRecID)];
                var sourceRecID = GetRedordID(sourceRecIDAlias);

                var conflict = treeConflicts.Single(x => x.SourceRecID == sourceRecID);
                conflict.ConflictResolution = ConflictResolutionType.ChoseSourceMergeData;

                var dscResolutionStr = row["DownstreamConflictResolution"];
                var dscResolution = Enum.Parse<ConflictResolutionType>(dscResolutionStr);

                foreach (var dsc in conflict.DownstreamConflicts)
                {
                    dsc.ConflictResolution = dscResolution;
                }
            }
        }

        [Then(@"running conflict resolution of '([^']*)' file against '([^']*)' not supported")]
        public void ThenRunningConflictResolutionOfFileAgainstNotSupported(string source, string dest)
        {
            var srcDb = GetDatabase(source);
            var destDb = GetDatabase(dest);
            var srcConn = srcDb.OpenConnection();
            var destConn = destDb.OpenConnection();

            var conflictResults = ConflictResults;
            conflictResults.AllHasResolutions().Should().BeTrue();
            try
            {
                var conflictResolver = new ConflictResolver();
                conflictResolver.Invoking(x => x.ResolveConflicts(srcConn, destConn, conflictResults))
                    .Should().Throw<NotSupportedException>();
            }
            finally
            {
                srcDb.ReleaseConnection();
                destDb.ReleaseConnection();
            }
        }
    }
}