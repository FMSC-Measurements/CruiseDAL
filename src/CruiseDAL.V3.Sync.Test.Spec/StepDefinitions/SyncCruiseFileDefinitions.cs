using TechTalk.SpecFlow.Infrastructure;

namespace CruiseDAL.V3.Sync.Test.Spec.StepDefinitions
{
    [Binding]
    public class SyncCruiseFileDefinitions : SyncStepDefinitionBase
    {
        public SyncCruiseFileDefinitions(ISpecFlowOutputHelper output, ScenarioContext senarioContext, FeatureContext featureContext)
            : base(output, senarioContext, featureContext)
        {
        }

        [Given(@"the following cruise files exist:")]
        public void GivenTheFollowingCruiseFilesExist(Table table)
        {
            var databaseLookup = DatabaseLookup;
            var deviceLookup = DeviceLookup;
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

                var deviceAlias = row["DeviceAlias"];
                // we might want to setup multiple files with the same device
                // so allow look up of existing devices alias
                if (deviceLookup.ContainsKey(deviceAlias))
                {
                    database.Insert(deviceLookup[deviceAlias]);
                }
                else
                {
                    var device = new CruiseDAL.V3.Models.Device
                    {
                        CruiseID = CruiseID,
                        DeviceID = Guid.NewGuid().ToString(),
                        Name = deviceAlias,
                    };

                    database.Insert(device);
                    deviceLookup.Add(deviceAlias, device);
                }
            }
        }

        protected void CopyTemplateData(CruiseDatastore_V3 cruise, string templatePath, string cruiseID)
        {
            using var templateDb = new CruiseDatastore_V3(templatePath);



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
            var syncOptions = new TableSyncOptions();
            var syncer = new CruiseDatabaseSyncer();

            var srcDb = GetDatabase(source);
            var destDb = GetDatabase(dest);

            syncer.Sync(CruiseID, srcDb, destDb, syncOptions);
        }
    }
}