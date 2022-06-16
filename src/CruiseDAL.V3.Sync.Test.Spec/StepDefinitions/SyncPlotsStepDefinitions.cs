using CruiseDAL.V3.Models;
using TechTalk.SpecFlow.Infrastructure;

namespace CruiseDAL.V3.Sync.Test.Spec.StepDefinitions
{
    [Binding]
    public class SyncPlotsStepDefinitions : SyncStepDefinitionBase
    {
        public SyncPlotsStepDefinitions(ISpecFlowOutputHelper output, ScenarioContext senarioContext, FeatureContext featureContext)
            : base(output, senarioContext, featureContext)
        {
        }

        [Given(@"in '([^']*)' the following plots exist:")]
        public void GivenInTheFollowingPlotsExist(string dbNamesArg, Table table)
        {
            var cruiseID = CruiseID;

            var databasesNames = dbNamesArg.Split(",", StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
            var databases = databasesNames.Select(x => DatabaseLookup[x]).ToArray();
            databases.Should().NotBeEmpty();

            foreach (var row in table.Rows)
            {
                var plotID = GetOrGenerateRecordID(row[nameof(Plot.PlotID)]);
                var plotNumber = int.Parse(row[nameof(Plot.PlotNumber)]);
                var cuttingUnitCode = row[nameof(Plot.CuttingUnitCode)];

                var plot = new Plot
                {
                    CruiseID = cruiseID,
                    PlotID = plotID,
                    PlotNumber = plotNumber,
                    CuttingUnitCode = cuttingUnitCode,
                };

                foreach (var db in databases)
                {
                    db.Insert(plot);
                }

                if (row.TryGetValue("Strata", out var strataCsv))
                {
                    var strata = strataCsv.Split(",", StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
                    foreach (var st in strata)
                    {
                        var plotStratum = new Plot_Stratum
                        {
                            CruiseID = cruiseID,
                            PlotNumber = plotNumber,
                            CuttingUnitCode = cuttingUnitCode,
                            StratumCode = st,
                        };

                        foreach (var db in databases)
                        {
                            db.Insert(plotStratum);
                        }
                    }
                }
            }
        }

        [Then(@"PlotConflicts has:")]
        public void ThenPlotConflictsHas(Table table)
        {
            var plotConflicts = ConflictResults.Plot;

            foreach (var row in table.Rows)
            {
                var srcRecID = GetRedordID(row[nameof(Conflict.SourceRecID)]);
                var destRecID = GetRedordID(row[nameof(Conflict.DestRecID)]);

                var plotConflict = plotConflicts.SingleOrDefault(x => x.SourceRecID == srcRecID && x.DestRecID == destRecID);
                plotConflict.Should().NotBeNull();

                if (row.TryGetValue("DownstreamConflictCount", out var downstreamConfCountStr))
                {
                    var downstreamConfCount = Convert.ToInt32(downstreamConfCountStr);
                    if (downstreamConfCount > 0)
                    {
                        plotConflict.DownstreamConflicts.Count().Should().Be(downstreamConfCount);
                    }
                }
            }
        }

        [When(@"I resolve all plot conflicts with '([^']*)'")]
        public void WhenIResolveAllPlotConflictsWith(string resolutionOptionStr)
        {
            var resolution = Enum.Parse<ConflictResolutionType>(resolutionOptionStr);

            var plotConflicts = ConflictResults.Plot;
            foreach (var conf in plotConflicts)
            {
                conf.ConflictResolution = resolution;
            }
        }

        [Then(@"'([^']*)' contains plots:")]
        public void ThenContainsPlots(string dbAlias, Table table)
        {
            var db = GetDatabase(dbAlias);

            var plots = db.From<Plot>().Query().ToArray();
            foreach (var row in table.Rows)
            {
                var plotIDAlias = row[nameof(Plot.PlotID)];
                var plotID = GetRedordID(plotIDAlias);

                row.TryGetValue(nameof(Plot.PlotNumber), out var plotNumberStr);
                row.TryGetValue(nameof(Plot.CuttingUnitCode), out var cuttingUnitCode);

                plots.Should().Contain(
                    x => x.PlotID == plotID
                        && (plotNumberStr == null || x.PlotNumber == int.Parse(plotNumberStr))
                        && (cuttingUnitCode == null || x.CuttingUnitCode == cuttingUnitCode)
                    , because: plotIDAlias);
            }
            plots.Should().HaveCount(table.RowCount);
        }

        [When(@"I resolve plot conflicts with ModifyDest using:")]
        public void WhenIResolvePlotConflictsWithModifyDestUsing(Table table)
        {
            var plotConflicts = ConflictResults.Plot;

            foreach (var row in table.Rows)
            {
                var destRecAlias = row[nameof(Conflict.DestRecID)];
                var destRedID = GetRedordID(destRecAlias);

                var conflict = plotConflicts.Single(x => x.DestRecID == destRedID);
                conflict.ConflictResolution = ConflictResolutionType.ModifyDest;

                var newPlotNumber = int.Parse(row[nameof(Plot.PlotNumber)]);
                ((Plot)conflict.DestRec).PlotNumber = newPlotNumber;
            }
        }

        [When(@"I resolve plot conflicts with ModifySource using:")]
        public void WhenIResolvePlotConflictsWithModifySourceUsing(Table table)
        {
            var plotConflicts = ConflictResults.Plot;

            foreach (var row in table.Rows)
            {
                var sourceRecAlias = row[nameof(Conflict.SourceRecID)];
                var sourceRedID = GetRedordID(sourceRecAlias);

                var conflict = plotConflicts.Single(x => x.SourceRecID == sourceRedID);
                conflict.ConflictResolution = ConflictResolutionType.ModifySource;

                var newPlotNumber = int.Parse(row[nameof(Plot.PlotNumber)]);
                ((Plot)conflict.SourceRec).PlotNumber = newPlotNumber;
            }
        }

        [Then(@"Plot Conflicts has (.*) conflict\(s\)")]
        public void ThenPlotConflictsHasConflictS(int conflictCount)
        {
            var plotConflicts = ConflictResults.Plot;
            plotConflicts.Should().HaveCount(conflictCount);
        }

        //[Then(@"PlotConflicts records has:")]
        //public void ThenPlotConflictsRecordsHas(Table table)
        //{
        //    var plotConflicts = ConflictResults.Plot;

        //    foreach(var row in table.Rows)
        //    {
        //        var srcRecID = GetRedordID(row[nameof(Conflict.SourceRecID)]);
        //        var destRecID = GetRedordID(row[nameof(Conflict.DestRecID)]);

        //        var conflict = plotConflicts.Single(x => x.SourceRecID == srcRecID && x.DestRecID == destRecID);

        //        if (row.TryGetValue("DownstreamConflictCount", out var downstreamConfCountStr))
        //        {
        //            var downstreamConfCount = Convert.ToInt32(downstreamConfCountStr);
        //            if (downstreamConfCount > 0)
        //            {
        //                conflict.DownstreamConflicts.Count().Should().Be(downstreamConfCount);
        //            }
        //        }
        //    }
        //}

        [When(@"I resolve all plot conflicts with '([^']*)' and downstream conflicts with '([^']*)'")]
        public void WhenIResolveAllPlotConflictsWithAndDownstreamConflictsWith(string resolutionOptionStr, string dsResolutionOptionStr)
        {
            var resolution = Enum.Parse<ConflictResolutionType>(resolutionOptionStr);
            var dsResolution = Enum.Parse<ConflictResolutionType>(dsResolutionOptionStr);

            var plotConflicts = ConflictResults.Plot;
            foreach (var conf in plotConflicts)
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