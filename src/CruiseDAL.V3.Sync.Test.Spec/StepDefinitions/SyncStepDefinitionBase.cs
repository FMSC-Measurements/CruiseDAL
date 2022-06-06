
using CruiseDAL.V3.Models;
using System.Reflection;
using TechTalk.SpecFlow.Infrastructure;

namespace CruiseDAL.V3.Sync.Test.Spec.StepDefinitions
{
    public class SyncStepDefinitionBase
    {
        private string _testTempPath;

        public SyncStepDefinitionBase(ISpecFlowOutputHelper output, ScenarioContext senarioContext, FeatureContext featureContext)
        {
            Output = output ?? throw new ArgumentNullException(nameof(output));
            SenarioContext = senarioContext ?? throw new ArgumentNullException(nameof(senarioContext));
            FeatureContext = featureContext ?? throw new ArgumentNullException(nameof(featureContext));

            
        }

        [BeforeScenario]
        public void SetupSenario()
        {
            //var logger = new TestLogger(Output);
            //FMSC.ORM.Logging.LoggerProvider.Register(logger);

            IDLookup = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            DatabaseLookup = new Dictionary<string, CruiseDatastore_V3>(StringComparer.OrdinalIgnoreCase);

            // setup Scenario Artifact Directory
            var featureName = FeatureContext.FeatureInfo.Title.Replace(" ", "");
            var senarioName = SenarioContext.ScenarioInfo.Title.Replace(" ", "");
            SenarioArtifactDir = Path.Combine(TestTempPath, featureName, senarioName);
            if (!Directory.Exists(SenarioArtifactDir))
            {
                Directory.CreateDirectory(SenarioArtifactDir);
                Output.WriteLine("Artifact Dir at " + SenarioArtifactDir);
            }


            // Initialize Cruise and Sale records 
            CruiseID = Guid.NewGuid().ToString();

            var saleID = Guid.NewGuid().ToString();
            var sale = DefaultSale = new Sale
            {
                SaleID = saleID,
                SaleNumber = "12345",
            };

            var cruise = DefaultCruise = new Cruise
            {
                CruiseID = CruiseID,
                CruiseNumber = "12345",
                SaleNumber = sale.SaleNumber,
                SaleID = saleID,
            };
        }

        protected IDictionary<string, string> IDLookup
        {
            get => SenarioContext.Get<IDictionary<string, string>>(nameof(IDLookup));
            set => SenarioContext.Set(value, nameof(IDLookup));
        }

        protected IDictionary<string, CruiseDatastore_V3> DatabaseLookup
        {
            get => SenarioContext.Get<IDictionary<string, CruiseDatastore_V3>>(nameof(DatabaseLookup));
            set => SenarioContext.Set(value, nameof(DatabaseLookup));
        }

        protected string CruiseID
        {
            get => SenarioContext.Get<string>(nameof(CruiseID));
            set => SenarioContext.Set(value, nameof(CruiseID));
        }

        protected ConflictResolutionOptions ConflictResults
        {
            get => SenarioContext.Get<ConflictResolutionOptions>(nameof(ConflictResults));
            set => SenarioContext.Set(value, nameof(ConflictResults));
        }

        protected Cruise DefaultCruise
        {
            get => SenarioContext.Get<Cruise>(nameof(DefaultCruise));
            set => SenarioContext.Set(value, nameof(DefaultCruise));
        }

        protected Sale DefaultSale
        {
            get => SenarioContext.Get<Sale>(nameof(DefaultSale));
            set => SenarioContext.Set(value, nameof(DefaultSale));
        }

        protected ISpecFlowOutputHelper Output { get; }

        protected ScenarioContext SenarioContext { get; }
        protected FeatureContext FeatureContext { get; }

        protected string TestExecutionDirectory
        {
            get
            {
                var codeBase = new Uri(Assembly.GetExecutingAssembly().CodeBase).LocalPath;
                return Path.GetDirectoryName(codeBase);
            }
        }

        protected string TestTempPath => _testTempPath ??= Path.Combine(Path.GetTempPath(), "TestTemp", this.GetType().FullName);

        protected string SenarioArtifactDir { get; private set; }

        protected string GetTempFilePath(string fileName)
        {
            Output.WriteLine("Generated Temp File Path: " + fileName);

            return Path.Combine(SenarioArtifactDir, fileName);
        }

        protected string GetOrGenerateRecordID(string alias)
        {
            var idLookup = IDLookup;
            if (idLookup.ContainsKey(alias))
            {
                return idLookup[alias];
            }
            else
            {
                var id = Guid.NewGuid().ToString();
                Output.WriteLine($"Generated ID: {alias}=>{id}");
                idLookup.Add(alias, id);
                return id;
            }
        }

        protected CruiseDatastore_V3 GetDatabase(string alias)
        {
            if (string.IsNullOrEmpty(alias)) { throw new ArgumentException($"'{nameof(alias)}' cannot be null or empty.", nameof(alias)); }

            return DatabaseLookup[alias];
        }

        protected string GetRedordID(string alias)
        {
            return IDLookup[alias];
        }
    }
}