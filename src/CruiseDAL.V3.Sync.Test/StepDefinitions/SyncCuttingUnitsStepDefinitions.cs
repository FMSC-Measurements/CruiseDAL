using CruiseDAL.V3.Models;
using CruiseDAL.V3.Sync.Test.Support;
using TechTalk.SpecFlow.Infrastructure;
using CruiseDAL.TestCommon.Util;

namespace CruiseDAL.V3.Sync.Test.Spec.StepDefinitions
{
    [Binding]
    public class SyncCuttingUnitsStepDefinitions : SyncStepDefinitionBase
    {
        public SyncCuttingUnitsStepDefinitions(ISpecFlowOutputHelper output, ScenarioContext senarioContext, FeatureContext featureContext)
            : base(output, senarioContext, featureContext)
        {
        }

        [Given(@"in '([^']*)' the following units exist:")]
        public void GivenInTheFollowingUnitsExist(string dbNamesArg, Table table)
        {
            var cruiseID = CruiseID;

            var databasesNames = dbNamesArg.Split(",", StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
            var databases = databasesNames.Select(x => DatabaseLookup[x]).ToArray();

            foreach (var row in table.Rows)
            {
                var unitCode = row["CuttingUnitCode"];
                var createdByDevice = GetDevice(row.GetValueOrDefault<string>("CreatedBy"));

                string unitID;
                if (row.TryGetValue(nameof(CuttingUnit.CuttingUnitID), out var unitIDAlias))
                {
                    unitID = GetOrGenerateRecordID(unitIDAlias);
                }
                else
                { unitID = Guid.NewGuid().ToString(); }

                var cuttingUnit = new CuttingUnit
                {
                    CruiseID = cruiseID,
                    CuttingUnitID = unitID,
                    CuttingUnitCode = unitCode,
                    CreatedBy = createdByDevice?.DeviceID,
                };
                //SrcDatabase.Insert(cuttingUnit);
                //DestDatabase.Insert(cuttingUnit);
                foreach (var db in databases)
                { db.Insert(cuttingUnit); }

                // create cutting unit strata
                if (row.TryGetValue("Strata", out var strataCodesCsv))
                {
                    var strataCodes = strataCodesCsv.Split(",", StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

                    foreach (var stCode in strataCodes)
                    {
                        var cust = new CuttingUnit_Stratum
                        {
                            CuttingUnitCode = unitCode,
                            StratumCode = stCode,
                            CreatedBy = createdByDevice?.DeviceID,
                        };

                        foreach (var db in databases)
                        {
                            db.Insert(cust);
                        }
                    }
                }
            }
        }

        [Then(@"Cutting Unit Conflicts Has:")]
        public void ThenCuttingUnitConflictsHas(Table table)
        {
            var conflictResults = ConflictResults;
            var unitConflicts = conflictResults.CuttingUnit;

            foreach (var row in table.Rows)
            {
                var srcRecID = GetRedordID(row[nameof(Conflict.SourceRecID)]);
                var destRecID = GetRedordID(row[nameof(Conflict.DestRecID)]);

                var srcDevice = GetDevice(row.GetValueOrDefault<string>("SrcDevice"));
                var destDevice = GetDevice(row.GetValueOrDefault<string>("DestDevice"));

                var unitConflict = unitConflicts.Single(x => x.SourceRecID == srcRecID && x.DestRecID == destRecID);
                //treeConflict.Should().NotBeNull();

                if (srcDevice != null) { unitConflict.SrcDeviceName.Should().Be(srcDevice.Name); }
                if (destDevice != null) { unitConflict.DestDeviceName.Should().Be(destDevice.Name); }

                if (row.TryGetValue("DownstreamConflictCount", out var downstreamConfCountStr))
                {
                    var downstreamConfCount = int.Parse(downstreamConfCountStr);
                    if (downstreamConfCount > 0)
                    {
                        unitConflict.DownstreamConflicts.Should().NotBeNull();
                        unitConflict.DownstreamConflicts.Count().Should().Be(downstreamConfCount);

                        
                    }
                }
            }
        }

        [Then(@"'([^']*)' contains cutting units:")]
        public void ThenContainsCuttingUnits(string dbAlias, Table table)
        {
            var db = GetDatabase(dbAlias);
            

            var cuttingUnits = db.From<CuttingUnit>().Query().ToArray();
            foreach (var row in table.Rows)
            {
                var cuttingUnitIDAlias = row[nameof(CuttingUnit.CuttingUnitID)];
                var cuttingUnitID = GetRedordID(cuttingUnitIDAlias);
                var createdByDevice = GetDevice(row.GetValueOrDefault<string>("CreatedBy"));

                row.TryGetValue(nameof(CuttingUnit.CuttingUnitCode), out var cuttingUnitCode);
                cuttingUnits.Should().Contain(
                    x => x.CuttingUnitID == cuttingUnitID
                        && (cuttingUnitCode == null || x.CuttingUnitCode == cuttingUnitCode)
                        && (createdByDevice == null || x.CreatedBy == createdByDevice.DeviceID)
                    , because: cuttingUnitIDAlias);
            }
            cuttingUnits.Should().HaveCount(table.RowCount);
        }

        [When(@"I resolve all Cutting Unit conflicts with '([^']*)'")]
        public void WhenIResolveAllCuttingUnitConflictsWith(string resolutionOptionStr)
        {
            var resolution = Enum.Parse<ConflictResolutionType>(resolutionOptionStr);

            var unitConflicts = ConflictResults.CuttingUnit;
            foreach (var conf in unitConflicts)
            {
                conf.ConflictResolution = resolution;
            }
        }

        [When(@"I resolve CuttingUnit Conflicts with ModifySource using:")]
        public void WhenIResolveCuttingUnitConflictsWithModifySourceUsing(Table table)
        {
            var unitConflicts = ConflictResults.CuttingUnit;

            foreach (var row in table.Rows)
            {
                var sourceRecAlias = row[nameof(Conflict.SourceRecID)];
                var sourceRedID = GetRedordID(sourceRecAlias);

                var conflict = unitConflicts.Single(x => x.SourceRecID == sourceRedID);
                conflict.ConflictResolution = ConflictResolutionType.ModifySource;

                var newCuttingUnitCode = row[nameof(CuttingUnit.CuttingUnitCode)];
                ((CuttingUnit)conflict.SourceRec).CuttingUnitCode = newCuttingUnitCode;
            }
        }

        [When(@"I resolve CuttingUnit Conflicts with ModifyDest using:")]
        public void WhenIResolveCuttingUnitConflictsWithModifyDestUsing(Table table)
        {
            var unitConflicts = ConflictResults.CuttingUnit;

            foreach (var row in table.Rows)
            {
                var destRecAlias = row[nameof(Conflict.DestRecID)];
                var destRecID = GetRedordID(destRecAlias);

                var conflict = unitConflicts.Single(x => x.DestRecID == destRecID);
                conflict.ConflictResolution = ConflictResolutionType.ModifyDest;

                var newCuttingUnitCode = row[nameof(CuttingUnit.CuttingUnitCode)];
                ((CuttingUnit)conflict.DestRec).CuttingUnitCode = newCuttingUnitCode;
            }
        }

        [When(@"I resolve unit conflicts with '([^']*)' and downstream conflicts with '([^']*)'")]
        public void WhenIResolveUnitConflictsWithAndDownstreamConflictsWith(string resolutionOptionStr, string dsResolutionOptionStr)
        {
            var resolution = Enum.Parse<ConflictResolutionType>(resolutionOptionStr);
            var dsResolution = Enum.Parse<ConflictResolutionType>(dsResolutionOptionStr);

            var unitConflicts = ConflictResults.CuttingUnit;
            foreach (var conf in unitConflicts)
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