using CruiseDAL.V3.Models;
using CruiseDAL.V3.Sync.Test.Support;
using TechTalk.SpecFlow.Infrastructure;

namespace CruiseDAL.V3.Sync.Test.Spec.StepDefinitions
{
    [Binding]
    public class SyncSampleGroupsStepDefinitions : SyncStepDefinitionBase
    {
        public SyncSampleGroupsStepDefinitions(ISpecFlowOutputHelper output, ScenarioContext senarioContext, FeatureContext featureContext)
            : base(output, senarioContext, featureContext)
        {
        }

        [Given(@"in '([^']*)' file the following sample groups exist:")]
        public void GivenInFileTheFollowingSampleGroupsExist(string dbNamesArg, Table table)
        {
            var cruiseID = CruiseID;

            var databasesNames = dbNamesArg.Split(",", StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
            var databases = databasesNames.Select(x => DatabaseLookup[x]).ToArray();
            databases.Should().NotBeEmpty();

            foreach (var row in table.Rows)
            {
                string sampleGroupID;
                if (row.TryGetValue(nameof(SampleGroup.SampleGroupID), out var sampleGroupIDAlias))
                {
                    sampleGroupID = GetOrGenerateRecordID(sampleGroupIDAlias);
                }
                else
                {
                    sampleGroupID = Guid.NewGuid().ToString();
                }

                var sgCode = row["SampleGroupCode"];
                var stCode = row["StratumCode"];
                var samplingFreq = row.GetValueOrDefault<int?>(nameof(SampleGroup.SamplingFrequency));
                var kz = row.GetValueOrDefault<int?>(nameof(SampleGroup.KZ));
                var iFreq = row.GetValueOrDefault<int?>(nameof(SampleGroup.InsuranceFrequency));
                var bigBAF = row.GetValueOrDefault<double?>(nameof(SampleGroup.BigBAF));
                var smallFPS = row.GetValueOrDefault<double?>(nameof(SampleGroup.SmallFPS));
                var tallyBySubPop = row.GetValueOrDefault<bool?>(nameof(SampleGroup.TallyBySubPop));

                var sg = new SampleGroup
                {
                    CruiseID = cruiseID,
                    SampleGroupID = sampleGroupID,
                    SampleGroupCode = sgCode,
                    StratumCode = stCode,
                    SamplingFrequency = samplingFreq,
                    KZ = kz,
                    BigBAF = bigBAF,
                    SmallFPS = smallFPS,
                    InsuranceFrequency = iFreq,
                    TallyBySubPop = tallyBySubPop,
                };

                foreach (var db in databases)
                { db.Insert(sg); }
            }
        }

        [Given(@"in '([^']*)' the following species exist:")]
        public void GivenInTheFollowingSpeciesExist(string dbNamesArg, Table table)
        {
            var cruiseID = CruiseID;

            var databasesNames = dbNamesArg.Split(",", StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
            var databases = databasesNames.Select(x => DatabaseLookup[x]).ToArray();
            databases.Should().NotBeEmpty();

            foreach (var row in table.Rows)
            {
                var spCode = row["SpeciesCode"];
                var sp = new Species
                {
                    CruiseID = cruiseID,
                    SpeciesCode = spCode,
                };

                foreach (var db in databases)
                { db.Insert(sp); }
            }
        }

        [Then(@"Sample Group Conflicts Has (.*) Conflict\(s\)")]
        public void ThenSampleGroupConflictsHasConflictS(int conflictCount)
        {
            var sgConflicts = ConflictResults.SampleGroup;
            sgConflicts.Should().HaveCount(conflictCount);
        }

        [Then(@"'([^']*)' contains sample groups:")]
        public void ThenContainsSampleGroups(string dbAlias, Table table)
        {
            var db = GetDatabase(dbAlias);

            var sgs = db.From<SampleGroup>().Query().ToArray();
            foreach (var row in table.Rows)
            {
                var sgIDAlias = row[nameof(SampleGroup.SampleGroupID)];
                var sgID = GetRedordID(sgIDAlias);

                row.TryGetValue(nameof(SampleGroup.StratumCode), out var stratumCode);
                row.TryGetValue(nameof(SampleGroup.SampleGroupCode), out var sgCode);

                sgs.Should().Contain(
                    x => x.SampleGroupID == sgID
                        && (sgCode == null || x.SampleGroupCode == sgCode)
                        && (stratumCode == null || x.StratumCode == stratumCode),
                    because: sgIDAlias);
            }

            sgs.Should().HaveCount(table.RowCount);
        }

        [Then(@"Sample Group Conflicts Has:")]
        public void ThenSampleGroupConflictsHas(Table table)
        {
            var conflictResults = ConflictResults;
            var sgConflicts = conflictResults.SampleGroup;

            foreach (var row in table.Rows)
            {
                var srcRecID = GetRedordID(row[nameof(Conflict.SourceRecID)]);
                var destRecID = GetRedordID(row[nameof(Conflict.DestRecID)]);

                var sgConflict = sgConflicts.Single(x => x.SourceRecID == srcRecID && x.DestRecID == destRecID);
                //treeConflict.Should().NotBeNull();

                if (row.TryGetValue("DownstreamConflictCount", out var downstreamConfCountStr))
                {
                    var downstreamConfCount = int.Parse(downstreamConfCountStr);
                    if (downstreamConfCount > 0)
                    {
                        sgConflict.DownstreamConflicts.Should().NotBeNull();
                        sgConflict.DownstreamConflicts.Count().Should().Be(downstreamConfCount);
                    }
                }
            }
        }

        [When(@"I resolve all Sample Group conflicts with '([^']*)'")]
        public void WhenIResolveAllSampleGroupConflictsWith(string resolutionOptionStr)
        {
            var resolution = Enum.Parse<ConflictResolutionType>(resolutionOptionStr);

            var sgConflicts = ConflictResults.SampleGroup;
            foreach (var conf in sgConflicts)
            {
                conf.ConflictResolution = resolution;
            }
        }

        [When(@"I resolve Sample Group Conflicts with ModifyDest using:")]
        public void WhenIResolveSampleGroupConflictsWithModifyDestUsing(Table table)
        {
            var sgConflicts = ConflictResults.SampleGroup;

            foreach (var row in table.Rows)
            {
                var destRecAlias = row[nameof(Conflict.DestRecID)];
                var destRecID = GetRedordID(destRecAlias);

                var conflict = sgConflicts.Single(x => x.DestRecID == destRecID);
                conflict.ConflictResolution = ConflictResolutionType.ModifyDest;

                var newSampleGroupCode = row[nameof(SampleGroup.SampleGroupCode)];
                ((SampleGroup)conflict.DestRec).SampleGroupCode = newSampleGroupCode;
            }
        }

        [When(@"I resolve Sample Group Conflicts with ModifySource using:")]
        public void WhenIResolveSampleGroupConflictsWithModifySourceUsing(Table table)
        {
            var sgConflicts = ConflictResults.SampleGroup;

            foreach (var row in table.Rows)
            {
                var sourceRecAlias = row[nameof(Conflict.SourceRecID)];
                var sourceRecID = GetRedordID(sourceRecAlias);

                var conflict = sgConflicts.Single(x => x.SourceRecID == sourceRecID);
                conflict.ConflictResolution = ConflictResolutionType.ModifySource;

                var newSampleGroupCode = row[nameof(SampleGroup.SampleGroupCode)];
                ((SampleGroup)conflict.SourceRec).SampleGroupCode = newSampleGroupCode;
            }
        }

        //[When(@"I resolve Sample Group conflicts with '([^']*)'")]
        //public void WhenIResolveSampleGroupConflictsWith(string resolutionOptionStr)
        //{
        //    var resolution = Enum.Parse<ConflictResolutionType>(resolutionOptionStr);
        //    //var dsResolution = Enum.Parse<ConflictResolutionType>(dsResolutionOptionStr);

        //    var sgConflicts = ConflictResults.SampleGroup;
        //    foreach (var conf in sgConflicts)
        //    {
        //        conf.ConflictResolution = resolution;

        //        //foreach (var dsconf in conf.DownstreamConflicts)
        //        //{
        //        //    dsconf.ConflictResolution = dsResolution;
        //        //}
        //    }
        //}
    }
}