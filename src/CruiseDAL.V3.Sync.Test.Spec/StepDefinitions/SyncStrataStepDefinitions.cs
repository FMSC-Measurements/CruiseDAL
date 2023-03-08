using CruiseDAL.Schema;
using CruiseDAL.V3.Models;
using System;
using TechTalk.SpecFlow;
using TechTalk.SpecFlow.Infrastructure;

namespace CruiseDAL.V3.Sync.Test.Spec.StepDefinitions
{
    [Binding]
    public class SyncStrataStepDefinitions : SyncStepDefinitionBase
    {
        public SyncStrataStepDefinitions(ISpecFlowOutputHelper output, ScenarioContext senarioContext, FeatureContext featureContext) : base(output, senarioContext, featureContext)
        {
        }

        [Given(@"in '([^']*)' the following strata exist:")]
        public void GivenInTheFollowingStrataExist(string dbNamesArg, Table table)
        {
            var cruiseID = CruiseID;

            var databasesNames = dbNamesArg.Split(",", StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
            var databases = databasesNames.Select(x => DatabaseLookup[x]).ToArray();
            databases.Should().NotBeEmpty();

            foreach (var row in table.Rows)
            {
                var stratumCode = row["StratumCode"];
                row.TryGetValue("Method", out var method);
                row.TryGetValue("Units", out var units);

                string stratumID;
                if (row.TryGetValue(nameof(Stratum.StratumID), out var stratumIDAlias))
                {
                    stratumID = GetOrGenerateRecordID(stratumIDAlias);
                }
                else
                {
                    stratumID = Guid.NewGuid().ToString();
                }

                var stratum = new Stratum
                {
                    CruiseID = cruiseID,
                    StratumID = stratumID,
                    StratumCode = stratumCode,
                    Method = method ?? CruiseMethods.STR,
                };

                var unitCodes = units?.Split(",", options: StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
                foreach (var db in databases)
                {
                    db.Insert(stratum);

                    var myUnitCodes = unitCodes;
                    if (myUnitCodes == null || myUnitCodes.Length == 0)
                    {
                        myUnitCodes = db.QueryScalar<string>("SELECT CuttingUnitCode FROM CuttingUnit;").ToArray();
                    }
                    foreach (var unitCode in myUnitCodes)
                    {
                        var cust = new CuttingUnit_Stratum
                        {
                            CruiseID = cruiseID,
                            CuttingUnitCode = unitCode,
                            StratumCode = stratumCode,
                        };
                        db.Insert(cust);
                    }
                }
            }
        }

        //[Then(@"Strata Conflicts Has:")]
        //public void ThenStrataConflictsHas(Table table)
        //{
        //    var conflictResults = ConflictResults;
        //    var strataConflicts = conflictResults.Stratum;

        //    foreach (var row in table.Rows)
        //    {
        //        var srcRecID = GetRedordID(row[nameof(Conflict.SourceRecID)]);
        //        var destRecID = GetRedordID(row[nameof(Conflict.DestRecID)]);

        //        var stratumConflict = strataConflicts.Single(x => x.SourceRecID == srcRecID && x.DestRecID == destRecID);
        //        //treeConflict.Should().NotBeNull();

        //        if (row.TryGetValue("DownstreamConflictCount", out var downstreamConfCountStr))
        //        {
        //            var downstreamConfCount = int.Parse(downstreamConfCountStr);
        //            if (downstreamConfCount > 0)
        //            {
        //                stratumConflict.DownstreamConflicts.Should().NotBeNull();
        //                stratumConflict.DownstreamConflicts.Count().Should().Be(downstreamConfCount);
        //            }
        //        }
        //    }
        //}

        [When(@"I resolve all Strata conflicts with '([^']*)'")]
        public void WhenIResolveAllStrataConflictsWith(string resolutionOptionStr)
        {
            var resolution = Enum.Parse<ConflictResolutionType>(resolutionOptionStr);

            var stratumConflicts = ConflictResults.Stratum;
            foreach (var conf in stratumConflicts)
            {
                conf.ConflictResolution = resolution;
            }
        }

        [Then(@"'([^']*)' contains strata:")]
        public void ThenContainsStrata(string dbAlias, Table table)
        {
            var db = GetDatabase(dbAlias);

            var strata = db.From<Stratum>().Query().ToArray();
            foreach (var row in table.Rows)
            {
                var stratumIDAlias = row[nameof(Stratum.StratumID)];
                var stratumID = GetRedordID(stratumIDAlias);

                row.TryGetValue(nameof(Stratum.StratumCode), out var stratumCode);

                strata.Should().Contain(
                    x => x.StratumID == stratumID
                        && (stratumCode == null || x.StratumCode == stratumCode),
                    because: stratumIDAlias);
            }

            strata.Should().HaveCount(table.RowCount);
        }

        [When(@"I resolve Stratum Conflicts with ModifyDest using:")]
        public void WhenIResolveStratumConflictsWithModifyDestUsing(Table table)
        {
            var strataConflicts = ConflictResults.Stratum;

            foreach (var row in table.Rows)
            {
                var destRecAlias = row[nameof(Conflict.DestRecID)];
                var destRecID = GetRedordID(destRecAlias);

                var conflict = strataConflicts.Single(x => x.DestRecID == destRecID);
                conflict.ConflictResolution = ConflictResolutionType.ModifyDest;

                var newStratumCode = row[nameof(Stratum.StratumCode)];
                ((Stratum)conflict.DestRec).StratumCode = newStratumCode;
            }
        }

        [When(@"I resolve Stratum Conflicts with ModifySource using:")]
        public void WhenIResolveStratumConflictsWithModifySourceUsing(Table table)
        {
            var stratumConflicts = ConflictResults.Stratum;

            foreach (var row in table.Rows)
            {
                var sourceRecAlias = row[nameof(Conflict.SourceRecID)];
                var sourceRedID = GetRedordID(sourceRecAlias);

                var conflict = stratumConflicts.Single(x => x.SourceRecID == sourceRedID);
                conflict.ConflictResolution = ConflictResolutionType.ModifySource;

                var newStratumCode = row[nameof(Stratum.StratumCode)];
                ((Stratum)conflict.SourceRec).StratumCode = newStratumCode;
            }
        }

        [When(@"I resolve stratum conflicts with '([^']*)' and downstream conflicts with '([^']*)'")]
        public void WhenIResolveStratumConflictsWithAndDownstreamConflictsWith(string resolutionOptionStr, string dsResolutionOptionStr)
        {
            var resolution = Enum.Parse<ConflictResolutionType>(resolutionOptionStr);
            var dsResolution = Enum.Parse<ConflictResolutionType>(dsResolutionOptionStr);

            var stratumConflicts = ConflictResults.Stratum;
            foreach (var conf in stratumConflicts)
            {
                conf.ConflictResolution = resolution;

                foreach (var dsconf in conf.DownstreamConflicts)
                {
                    dsconf.ConflictResolution = dsResolution;
                }
            }
        }
    }
}