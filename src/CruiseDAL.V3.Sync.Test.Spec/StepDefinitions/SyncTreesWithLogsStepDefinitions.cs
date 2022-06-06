using System;
using TechTalk.SpecFlow;
using TechTalk.SpecFlow.Infrastructure;

namespace CruiseDAL.V3.Sync.Test.Spec.StepDefinitions
{
    [Binding]
    public class SyncTreesWithLogsStepDefinitions : SyncStepDefinitionBase
    {
        public SyncTreesWithLogsStepDefinitions(ISpecFlowOutputHelper output, ScenarioContext senarioContext, FeatureContext featureContext) 
            : base(output, senarioContext, featureContext)
        {
        }

        [When(@"I resolve tree conflicts with ChoseSourceMergeData using:")]
        public void WhenIResolveTreeConflictsWithChoseSourceMergeDataUsing(Table table)
        {
            var treeConflicts = ConflictResults.Tree;

            foreach(var row in table.Rows)
            {
                var sourceRecIDAlias = row[nameof(Conflict.SourceRecID)];
                var sourceRecID = GetRedordID(sourceRecIDAlias);

                var conflict = treeConflicts.Single(x => x.SourceRecID == sourceRecID);
                conflict.ConflictResolution = ConflictResolutionType.ChoseSourceMergeData;

                var dscResolutionStr = row["DownstreamConflictResolution"];
                var dscResolution = Enum.Parse<ConflictResolutionType>(dscResolutionStr);

                foreach(var dsc in conflict.DownstreamConflicts)
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
                    .Should().Throw<NotImplementedException>();
            }
            finally
            {
                srcDb.ReleaseConnection();
                destDb.ReleaseConnection();
            }
        }

    }
}
