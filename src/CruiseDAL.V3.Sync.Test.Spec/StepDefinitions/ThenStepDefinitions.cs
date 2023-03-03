using CruiseDAL.V3.Sync.Test.Spec.StepDefinitions;
using TechTalk.SpecFlow.Assist;
using TechTalk.SpecFlow.Infrastructure;

namespace CruiseDAL.V3.Sync.Test.StepDefinitions
{
    [Binding]
    public class ThenStepDefinitions : SyncStepDefinitionBase
    {
        public ThenStepDefinitions(ISpecFlowOutputHelper output, ScenarioContext senarioContext, FeatureContext featureContext) : base(output, senarioContext, featureContext)
        {
        }

        #region Then Conflict Checks

        [Then(@"Has No Conflicts")]
        public void ThenHasNoConflicts()
        {
            var conflictResults = ConflictResults;
            conflictResults.HasConflicts.Should().BeFalse();
        }

        [Then(@"Has No Stratum Conflicts")]
        public void ThenHasNoStratumConflicts()
        {
            var conflictResults = ConflictResults;
            var strataConflicts = conflictResults.Stratum;
            strataConflicts.Should().BeEmpty();
        }

        [Then(@"Strata Conflicts Has:")]
        public void ThenStrataConflictsHas(Table table)
        {
            var conflictResults = ConflictResults;
            var strataConflicts = conflictResults.Stratum;

            foreach (var row in table.Rows)
            {
                var srcRecID = GetRedordID(row[nameof(Conflict.SourceRecID)]);
                var destRecID = GetRedordID(row[nameof(Conflict.DestRecID)]);

                var stratumConflict = strataConflicts.Single(x => x.SourceRecID == srcRecID && x.DestRecID == destRecID);
                //treeConflict.Should().NotBeNull();

                if (row.TryGetValue("DownstreamConflictCount", out var downstreamConfCountStr))
                {
                    var downstreamConfCount = int.Parse(downstreamConfCountStr);
                    if (downstreamConfCount > 0)
                    {
                        stratumConflict.DownstreamConflicts.Should().NotBeNull();
                        stratumConflict.DownstreamConflicts.Count().Should().Be(downstreamConfCount);
                    }
                }
            }
        }

        #endregion Then Conflict Checks

        #region Design Errors

        [Then(@"Design Errors Has:")]
        public void ThenDesignErrorsHas(Table table)
        {
            var errorMessages = DesignErrors.ToHashSet();

            foreach(var row in table.Rows)
            {
                var error = row.GetString("error");
                errorMessages.Should().Contain(error);
            }
        }

        #endregion Design Errors
    }
}