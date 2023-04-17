using CruiseDAL.V3.Sync.Syncers;
using CruiseDAL.V3.Sync.Test.Spec.StepDefinitions;
using Gherkin.CucumberMessages.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TechTalk.SpecFlow.Infrastructure;

namespace CruiseDAL.V3.Sync.Test.StepDefinitions
{
    [Binding]
    public class WhenStepDefinitions : SyncStepDefinitionBase
    {
        public WhenStepDefinitions(ISpecFlowOutputHelper output, ScenarioContext senarioContext, FeatureContext featureContext) : base(output, senarioContext, featureContext)
        {
        }


        [When(@"I check '([^']*)' for Design Mismatch errors against '([^']*)'")]
        public void WhenICheckForDesignMismatchErrorsAgainst(string source, string dest)
        {
            var cruiseID = CruiseID;

            var srcDb = GetDatabase(source);
            var destDb = GetDatabase(dest);

            var destConn = destDb.OpenConnection();
            var sourceConn = srcDb.OpenConnection();
            try
            {
                var errorsList = new List<string>();
                if (StratumSyncer.CheckHasDesignMismatchErrors(cruiseID, sourceConn, destConn, out var stratumErrors))
                {
                    errorsList.AddRange(stratumErrors);
                }

                if (SampleGroupSyncer.CheckHasDesignMismatchErrors(cruiseID, sourceConn, destConn, out var sgErrror))
                {
                    errorsList.AddRange(sgErrror);
                }

                DesignErrors = errorsList;
            }
            finally
            {
                destDb.ReleaseConnection();
                srcDb.ReleaseConnection();
            }
        }

    }
}
