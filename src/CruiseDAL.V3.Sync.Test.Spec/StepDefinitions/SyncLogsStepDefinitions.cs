using CruiseDAL.V3.Models;
using System;
using TechTalk.SpecFlow;
using TechTalk.SpecFlow.Infrastructure;

namespace CruiseDAL.V3.Sync.Test.Spec.StepDefinitions
{
    [Binding]
    public class SyncLogsStepDefinitions : SyncStepDefinitionBase
    {

        public SyncLogsStepDefinitions(ISpecFlowOutputHelper output, ScenarioContext senarioContext, FeatureContext featureContext) :
            base(output, senarioContext, featureContext)
        {
        }

        [Then(@"Log Conflict List has (.*) conflict\(s\)")]
        public void ThenLogConflictListHasConflictS(int conflictCount)
        {
            var logConflicts = ConflictResults.Log;
            logConflicts.Should().HaveCount(conflictCount);
        }

        [Then(@"Log Conflict List has conflicts:")]
        public void ThenLogConflictListHasConflicts(Table table)
        {
            var logConflicts = ConflictResults.Log;

            foreach (var row in table.Rows)
            {
                var srcRecID = GetRedordID(row[0]);
                var destRecID = GetRedordID(row[1]);

                logConflicts.Should().Contain(x => x.SourctRecID == srcRecID && x.DestRecID == destRecID);
            }
        }

        [When(@"I resolve all log conflicts with '([^']*)'")]
        public void WhenIResolveAllLogConflictsWith(string resOptionStr)
        {

            var resOption = Enum.Parse<ConflictResolutionType>(resOptionStr, true);

            var logConflicts = ConflictResults.Log;
            foreach(var conflict in logConflicts)
            {
                conflict.ConflictResolution = resOption;
            }
        }

        [Then(@"'([^']*)' contains logIDs:")]
        public void ThenContainsLogIDs(string dbAlias, Table table)
        {
            var db = GetDatabase(dbAlias);

            var logs = db.From<Log>().Query().ToArray();

            foreach (var row in table.Rows)
            {
                var alias = row[0];
                var logID = GetRedordID(alias);
                logs.Should().Contain(x => x.LogID == logID, alias);
            }
            logs.Count().Should().Be(table.RowCount);
        }


        [When(@"I resolve log conflicts with ModifyDest using:")]
        public void WhenIResolveLogConflictsWithModifyDestUsing(Table table)
        {
            var logConflicts = ConflictResults.Log;

            foreach(var row in table.Rows)
            {
                var destRecIDAlias = row[nameof(Conflict.DestRecID)];
                var destRecID = GetRedordID(destRecIDAlias);

                var conflict = logConflicts.Where(x => x.DestRecID == destRecID).Single();
                conflict.ConflictResolution = ConflictResolutionType.ModifyDest;

                var newLogNumber = row[nameof(Log.LogNumber)];
                ((Log)conflict.DestRec).LogNumber = newLogNumber;
            }
        }


    }
}
