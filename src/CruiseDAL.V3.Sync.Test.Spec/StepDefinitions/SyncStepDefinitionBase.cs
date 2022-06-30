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

        protected string SenarioArtifactDir
        {
            get => SenarioContext.Get<string>(nameof(SenarioArtifactDir));
            set => SenarioContext.Set(value, nameof(SenarioArtifactDir));
        }

        protected string TestExecutionDirectory
        {
            get => SenarioContext.Get<string>(nameof(TestExecutionDirectory));
            set => SenarioContext.Set(value, nameof(TestExecutionDirectory));
        }

        public string TestFilesDirectory => Path.Combine(TestExecutionDirectory, "TestFiles");

        protected ISpecFlowOutputHelper Output { get; }

        protected ScenarioContext SenarioContext { get; }
        protected FeatureContext FeatureContext { get; }


        public string GetTemplateFile(string fileName)
        {
            var templateFilePath = GetTestFile(fileName);

            var ext = Path.GetExtension(templateFilePath).ToLower();

            if (ext != ".cut" || ext != ".crz3t")
            { throw new InvalidOperationException("invalid file extension"); }

            if (ext == ".cut")
            {
                var v3TemplatePath = templateFilePath + ".crz3t";

                var upconverter = new UpConvert.Migrator();
                upconverter.MigrateFromV2ToV3(templateFilePath, v3TemplatePath);

                templateFilePath = v3TemplatePath;
            }

            return templateFilePath;
        }

        public string GetTestFile(string fileName) => InitializeTestFile(fileName);

        public string InitializeTestFile(string fileName)
        {
            var sourcePath = Path.Combine(TestFilesDirectory, fileName);
            if (File.Exists(sourcePath) == false) { throw new FileNotFoundException(sourcePath); }

            var targetPath = Path.Combine(SenarioArtifactDir, fileName);

            File.Copy(sourcePath, targetPath, true);
            return targetPath;
        }

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