using CruiseDAL.V3.Models;
using System.Reflection;
using TechTalk.SpecFlow.Infrastructure;

namespace CruiseDAL.V3.Sync.Test.Spec.StepDefinitions
{
    [Binding]
    public class SyncHooks : SyncStepDefinitionBase
    {
        public SyncHooks(ISpecFlowOutputHelper output, ScenarioContext senarioContext, FeatureContext featureContext) : base(output, senarioContext, featureContext)
        {
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

            var testTempPath = Path.Combine(Path.GetTempPath(), "TestTemp", Assembly.GetExecutingAssembly().GetName().Name);
            SenarioArtifactDir = Path.Combine(testTempPath, featureName, senarioName);
            if (!Directory.Exists(SenarioArtifactDir))
            {
                Directory.CreateDirectory(SenarioArtifactDir);
                Output.WriteLine("Artifact Dir at " + SenarioArtifactDir);
            }

            // Test Execution Dir
            // in .net 5 and later codeBase throws exception. This is because CodeBase is
            // depreciated and replaced with "Location" property. However Location returns 
            // the location of the assembly after shadow-copy of assemblies where CodeBase
            // returns location before shadow-copy. since the testing framework uses shadow
            // copy and we want the before shadow-copy location. 
            var codeBase = new Uri(Assembly.GetExecutingAssembly().CodeBase).LocalPath;
            TestExecutionDirectory = Path.GetDirectoryName(codeBase);


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

        [AfterScenario]
        public void AfterSenario()
        {
            var output = Output;

            var idLookup = IDLookup;
            if (idLookup != null)
            {
                output.WriteLine("ID alias values:");
                foreach (var item in IDLookup)
                {
                    output.WriteLine($"{item.Key.ToString().PadRight(10)}: {item.Value}");
                }
            }
        }
    }
}