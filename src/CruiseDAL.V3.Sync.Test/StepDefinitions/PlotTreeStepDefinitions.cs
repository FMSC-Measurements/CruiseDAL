using CruiseDAL.V3.Sync.Test.Spec.StepDefinitions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TechTalk.SpecFlow.Infrastructure;

namespace CruiseDAL.V3.Sync.Test.StepDefinitions
{
    [Binding]
    public class PlotTreeStepDefinitions : SyncStepDefinitionBase
    {
        public PlotTreeStepDefinitions(ISpecFlowOutputHelper output, ScenarioContext senarioContext, FeatureContext featureContext) 
            : base(output, senarioContext, featureContext)
        {
        }

        [Then(@"PlotTreeConflicts has (.*) conflict\(s\)")]
        public void ThenPlotTreeConflictsHasConflictS(int p0)
        {
            var plotTreeConflicts = ConflictResults.PlotTree;
            plotTreeConflicts.Should().HaveCount(p0);
        }

        [Then(@"PlotTreeConflicts has:")]
        public void ThenPlotTreeConflictsHas(Table table)
        {
            var plotTreeConflicts = ConflictResults.PlotTree;

            foreach(var row in table.Rows)
            {
                var srcRecID = GetRedordID(row[nameof(Conflict.SourceRecID)]);
                var destRecID = GetRedordID(row[nameof(Conflict.DestRecID)]);

                var conflict = plotTreeConflicts.SingleOrDefault(x => x.SourceRecID == srcRecID && x.DestRecID == destRecID);
                conflict.Should().NotBeNull();
            }
        }


    }
}
