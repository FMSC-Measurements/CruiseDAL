using CruiseDAL.Schema;
using CruiseDAL.V3.Models;
using TechTalk.SpecFlow.Infrastructure;

namespace CruiseDAL.V3.Sync.Test.Spec.StepDefinitions
{
    [Binding]
    public class SyncTreesStepDefinitions : SyncStepDefinitionBase
    {
        public SyncTreesStepDefinitions(ISpecFlowOutputHelper output, ScenarioContext senarioContext, FeatureContext featureContext) :
            base(output, senarioContext, featureContext)
        {
        }

        [Given(@"the following cruise files exist:")]
        public void GivenTheFollowingCruiseFilesExist(Table table)
        {
            var databaseLookup = DatabaseLookup;
            foreach (var row in table.Rows)
            {
                var alias = row["FileAlias"];
                if (databaseLookup.ContainsKey(alias)) { throw new InvalidOperationException("Database Already Exists: " + alias); }

                var filePath = base.GetTempFilePath(alias + ".crz3");

                var database = new CruiseDatastore_V3(filePath, true);
                Output.WriteLine("Created Database: " + alias);
                Output.AddAttachment(filePath);
                databaseLookup.Add(alias, database);

                database.Insert(DefaultSale);
                database.Insert(DefaultCruise);
            }
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

                var unitID = Guid.NewGuid().ToString();
                var cuttingUnit = new CuttingUnit
                {
                    CruiseID = cruiseID,
                    CuttingUnitID = unitID,
                    CuttingUnitCode = unitCode,
                };
                //SrcDatabase.Insert(cuttingUnit);
                //DestDatabase.Insert(cuttingUnit);
                foreach (var db in databases)
                { db.Insert(cuttingUnit); }
            }
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

                var stratumID = Guid.NewGuid().ToString();
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

        [Given(@"in '([^']*)' file the following sample groups exist:")]
        public void GivenInFileTheFollowingSampleGroupsExist(string dbNamesArg, Table table)
        {
            var cruiseID = CruiseID;

            var databasesNames = dbNamesArg.Split(",", StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
            var databases = databasesNames.Select(x => DatabaseLookup[x]).ToArray();
            databases.Should().NotBeEmpty();

            foreach (var row in table.Rows)
            {
                var sampleGroupID = Guid.NewGuid().ToString();
                var sgCode = row["SampleGroupCode"];
                var stCode = row["StratumCode"];
                var sg = new SampleGroup
                {
                    CruiseID = cruiseID,
                    SampleGroupID = sampleGroupID,
                    SampleGroupCode = sgCode,
                    StratumCode = stCode,
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

        [Given(@"in '([^']*)' the following trees exist:")]
        public void GivenInTheFollowingTreesExist(string dbNamesArg, Table table)
        {
            var databasesNames = dbNamesArg.Split(",", StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
            var databases = databasesNames.Select(x => DatabaseLookup[x]).ToArray();
            databases.Should().NotBeEmpty();

            var cruiseID = CruiseID;

            foreach (var row in table.Rows)
            {
                row.TryGetValue("TreeID", out var treeIDTag);
                var treeID = (treeIDTag != null) ? GetOrGenerateRecordID(treeIDTag)
                    : Guid.NewGuid().ToString();
                var treeNumber = Int32.Parse(row["TreeNumber"]);
                var unitCode = row["CuttingUnitCode"];
                var stCode = row["StratumCode"];
                var sgCode = row["SampleGroupCode"];
                var spCode = row["SpeciesCode"];

                row.TryGetValue(nameof(Tree.PlotNumber), out var plotNumberStr);
                var plotNumber = (plotNumberStr != null) ? (int?)int.Parse(plotNumberStr) : null;

                var tree = new Tree
                {
                    CruiseID = cruiseID,
                    TreeID = treeID,
                    TreeNumber = treeNumber,
                    CuttingUnitCode = unitCode,
                    StratumCode = stCode,
                    SampleGroupCode = sgCode,
                    SpeciesCode = spCode,
                    PlotNumber = plotNumber,
                };

                //var tallyLedgerID = Guid.NewGuid().ToString();
                var tallyLedgerID = treeID; //reuse treeID for tally ledger ID
                var tallyLedger = new TallyLedger
                {
                    CruiseID = cruiseID,
                    TreeID = treeID,
                    TallyLedgerID = tallyLedgerID,
                    CuttingUnitCode = unitCode,
                    StratumCode = stCode,
                    SampleGroupCode = sgCode,
                    SpeciesCode = spCode,
                    PlotNumber = plotNumber,
                };

                var tm = new TreeMeasurment
                {
                    TreeID = treeID,
                };

                foreach (var db in databases)
                {
                    db.Insert(tree);
                    db.Insert(tallyLedger);
                    db.Insert(tm);
                }
            }
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

        [When(@"I conflict check '([^']*)' file against '([^']*)'")]
        public void WhenIConflictCheckFileAgainst(string source, string dest)
        {
            var conflictChecker = new ConflictChecker();

            var srcDb = GetDatabase(source);
            var destDb = GetDatabase(dest);
            var srcConn = srcDb.OpenConnection();
            var destConn = destDb.OpenConnection();
            try
            {
                ConflictResults = conflictChecker.CheckConflicts(srcConn, destConn, CruiseID);
            }
            finally
            {
                srcDb.ReleaseConnection();
                destDb.ReleaseConnection();
            }
        }

        [Then(@"TreeConflicts has no downstream conflicts")]
        public void ThenTreeConflictsHasNoDownstreamConflicts()
        {
            var conflictResults = ConflictResults;
            conflictResults.Tree.Should().NotContain(x => x.DownstreamConflicts != null && x.DownstreamConflicts.Any());
        }

        [Then(@"TreeConflicts has (.*) conflict\(s\)")]
        public void ThenTreeConflictsHasConflictS(int conflictCount)
        {
            var conflictResults = ConflictResults;
            conflictResults.Tree.Should().HaveCount(conflictCount);
        }

        [Then(@"TreeConflicts records has:")]
        public void ThenTreeConflictsRecordsHas(Table table)
        {
            var conflictResults = ConflictResults;
            var treeConflicts = conflictResults.Tree;

            foreach (var row in table.Rows)
            {
                var srcRecID = GetRedordID(row[nameof(Conflict.SourceRecID)]);
                var destRecID = GetRedordID(row[nameof(Conflict.DestRecID)]);

                var treeConflict = treeConflicts.Single(x => x.SourceRecID == srcRecID && x.DestRecID == destRecID);
                //treeConflict.Should().NotBeNull();

                if(row.TryGetValue("DownstreamConflictCount", out var downstreamConfCountStr))
                {
                    var downstreamConfCount = int.Parse(downstreamConfCountStr);
                    if (downstreamConfCount > 0)
                    {
                        treeConflict.DownstreamConflicts.Should().NotBeNull();
                        treeConflict.DownstreamConflicts.Count().Should().Be(downstreamConfCount);
                    }
                }

            }
        }

        [When(@"I resolve all tree conflicts with '([^']*)'")]
        public void WhenIResolveAllTreeConflictsWith(string resolutionOptionStr)
        {
            var resolution = Enum.Parse<ConflictResolutionType>(resolutionOptionStr);

            var conflictResults = ConflictResults;
            foreach (var conf in conflictResults.Tree)
            {
                conf.ConflictResolution = resolution;
            }
        }

        [When(@"I run conflict resolution of '([^']*)' file against '([^']*)'")]
        public void WhenIRunConflictResolutionOfFileAgainst(string source, string dest)
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
                conflictResolver.ResolveConflicts(srcConn, destConn, conflictResults);
            }
            finally
            {
                srcDb.ReleaseConnection();
                destDb.ReleaseConnection();
            }
        }


        [When(@"sync '([^']*)' into '([^']*)'")]
        public void WhenSyncInto(string source, string dest)
        {
            var syncOptions = new CruiseSyncOptions();
            var syncer = new CruiseSyncer();

            var srcDb = GetDatabase(source);
            var destDb = GetDatabase(dest);

            syncer.Sync(CruiseID, srcDb, destDb, syncOptions);
        }

        [Then(@"'([^']*)' contains trees:")]
        public void ThenContainsTrees(string dbAlias, Table table)
        {
            var db = GetDatabase(dbAlias);

            var trees = db.From<Tree>().Query().ToArray();
            
            foreach (var row in table.Rows)
            {
                var treeIDAlias = row[nameof(Tree.TreeID)];
                var treeID = GetRedordID(treeIDAlias);
                
                row.TryGetValue(nameof(Tree.TreeNumber), out var treeNumberStr);
                var tree = trees.SingleOrDefault(x =>
                    x.TreeID == treeID
                        && (treeNumberStr == null || x.TreeNumber == int.Parse(treeNumberStr)));

                tree.Should().NotBeNull(because: treeIDAlias);

                db.From<TreeMeasurment>().Where("TreeID = @p1").Count(treeID).Should().Be(1);
                db.From<TallyLedger>().Where("TreeID = @p1").Count(treeID).Should().Be(1);
            }
            trees.Should().HaveCount(table.RowCount);
        }

        [When(@"I resolve tree conflicts with ModifyDest using:")]
        public void WhenIResolveTreeConflictsWithModifyDestUsing(Table table)
        {
            var treeConflicts = ConflictResults.Tree;

            foreach(var row in table.Rows)
            {
                var destRecIDAlias = row[nameof(Conflict.DestRecID)];
                var destRecID = GetRedordID(destRecIDAlias);

                var conflict = treeConflicts.Single(x => x.DestRecID == destRecID);
                conflict.ConflictResolution = ConflictResolutionType.ModifyDest;

                var newTreeNumber = int.Parse(row[nameof(Tree.TreeNumber)]);
                ((Tree)conflict.DestRec).TreeNumber = newTreeNumber;
            }
        }

        [When(@"I resolve tree conflicts with ModifySource using:")]
        public void WhenIResolveTreeConflictsWithModifySourceUsing(Table table)
        {
            var treeConflicts = ConflictResults.Tree;

            foreach (var row in table.Rows)
            {
                var srcRecIDAlias = row[nameof(Conflict.SourceRecID)];
                var srcRecID = GetRedordID(srcRecIDAlias);

                var conflict = treeConflicts.Single(x => x.SourceRecID == srcRecID);
                conflict.ConflictResolution = ConflictResolutionType.ModifySource;

                var newTreeNumber = int.Parse(row[nameof(Tree.TreeNumber)]);
                ((Tree)conflict.SourceRec).TreeNumber = newTreeNumber;
            }
        }


    }
}