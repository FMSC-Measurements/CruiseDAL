using System.Reflection;
using TechTalk.SpecFlow.Infrastructure;

namespace CruiseDAL.V3.Sync.Test.Spec.StepDefinitions
{
    public class BindingDefinitionBase
    {
        private string _testTempPath;

        public BindingDefinitionBase(ISpecFlowOutputHelper output, ScenarioContext senarioContext, FeatureContext featureContext)
        {
            Output = output ?? throw new ArgumentNullException(nameof(output));
            SenarioContext = senarioContext ?? throw new ArgumentNullException(nameof(senarioContext));
            FeatureContext = featureContext ?? throw new ArgumentNullException(nameof(featureContext));

            var featureName = FeatureContext.FeatureInfo.Title.Replace(" ", "");
            var senarioName = SenarioContext.ScenarioInfo.Title.Replace(" ", "");
            SenarioArtifactDir = Path.Combine(TestTempPath, featureName, senarioName);
            if (!Directory.Exists(SenarioArtifactDir))
            {
                Directory.CreateDirectory(SenarioArtifactDir);
                Output.WriteLine("Artifact Dir at " + SenarioArtifactDir);
            }

        }

        protected ISpecFlowOutputHelper Output { get; }

        protected ScenarioContext SenarioContext { get; }
        public FeatureContext FeatureContext { get; }

        protected string TestExecutionDirectory
        {
            get
            {
                var codeBase = new Uri(Assembly.GetExecutingAssembly().CodeBase).LocalPath;
                return Path.GetDirectoryName(codeBase);
            }
        }

        protected string TestTempPath => _testTempPath ??= Path.Combine(Path.GetTempPath(), "TestTemp", this.GetType().FullName);

        public string SenarioArtifactDir { get; }

        public string GetTempFilePath(string fileName)
        {
            Output.WriteLine("Generated Temp File Path: " + fileName);

            return Path.Combine(SenarioArtifactDir, fileName);
        }
    }
}