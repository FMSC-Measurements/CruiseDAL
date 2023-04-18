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

        [Given(@"in '([^']*)' the following logs exist:")]
        public void GivenInTheFollowingLogsExist(string dbNamesArg, Table table)
        {
            var databasesNames = dbNamesArg.Split(",", StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
            var databases = databasesNames.Select(x => DatabaseLookup[x]).ToArray();
            databases.Should().NotBeEmpty();

            foreach (var row in table.Rows)
            {
                var treeID = GetRedordID(row["TreeID"]);
                var logNumber = row[nameof(Log.LogNumber)];

                string logID;
                if (row.TryGetValue(nameof(Log.LogID), out var logIDAlias) && !string.IsNullOrEmpty(logIDAlias))
                {
                    logID = GetOrGenerateRecordID(logIDAlias);
                }
                else
                {
                    logID = Guid.NewGuid().ToString();
                }

                var log = new Log
                {
                    CruiseID = CruiseID,
                    TreeID = treeID,
                    LogNumber = logNumber,
                    LogID = logID,
                };

                foreach (var db in databases)
                {
                    db.Insert(log);
                }
            }
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
                var srcRecID = GetRedordID(row[nameof(Conflict.SourceRecID)]);
                var destRecID = GetRedordID(row[nameof(Conflict.DestRecID)]);

                logConflicts.Should().Contain(x => x.SourceRecID == srcRecID && x.DestRecID == destRecID);
            }
        }

        [When(@"I resolve all log conflicts with '([^']*)'")]
        public void WhenIResolveAllLogConflictsWith(string resOptionStr)
        {
            var resOption = Enum.Parse<ConflictResolutionType>(resOptionStr, true);

            var logConflicts = ConflictResults.Log;
            foreach (var conflict in logConflicts)
            {
                conflict.ConflictResolution = resOption;
            }
        }

        [Then(@"'([^']*)' contains logs:")]
        public void ThenContainsLogs(string dbAlias, Table table)
        {
            var db = GetDatabase(dbAlias);

            var logs = db.From<Log>().Query().ToArray();

            foreach (var row in table.Rows)
            {
                var alias = row[nameof(Log.LogID)];
                var logID = GetRedordID(alias);
                row.TryGetValue(nameof(Log.LogNumber), out var logNumber);

                logs.Should().Contain(
                    x => x.LogID == logID
                        && (string.IsNullOrEmpty(logNumber) || x.LogNumber == logNumber),
                    because: alias);
            }
            logs.Count().Should().Be(table.RowCount);
        }

        [When(@"I resolve log conflicts with ModifyDest using:")]
        public void WhenIResolveLogConflictsWithModifyDestUsing(Table table)
        {
            var logConflicts = ConflictResults.Log;

            foreach (var row in table.Rows)
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