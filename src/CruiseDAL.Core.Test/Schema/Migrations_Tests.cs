using FluentAssertions;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Xunit;
using Xunit.Abstractions;

namespace CruiseDAL.Tests.Schema
{
    public class Migrations_Tests : TestBase
    {
        public string OrigFile { get; }
        public string TestFile { get; }

        public Migrations_Tests(ITestOutputHelper output) : base(output)
        {
            foreach (var file in Directory.GetFiles(TestTempPath))
            {
                try
                {
                    File.Delete(file);
                }
                catch
                { }
            }
        }

        public IEnumerable<object[]> TestFiles
        {
            get
            {
                return Directory.GetFiles(TestFilesDirectory)
                    .Where(x => x.EndsWith(".cruise"))
                    .Select(x => new object[] { x });
            }
        }

        private (string, string) SetUpTestFile(string fileName)
        {
            var filePath = Path.Combine(TestFilesDirectory, fileName);

            var baseFileName = Path.GetFileNameWithoutExtension(fileName);
            var orgFile = Path.Combine(TestTempPath, baseFileName + "_test_orig.cruise");
            var testFile = Path.Combine(TestTempPath, baseFileName + "_test.cruise");

            // create copy of base file
            if (File.Exists(orgFile) == false)
            {
                File.Copy(filePath, orgFile);
            }
            if (File.Exists(testFile) == false)
            {
                File.Copy(filePath, testFile);

                using (var datastore = new DAL(testFile))
                {
                    CruiseDAL.Updater.Migrate_From_V2(datastore);
                }
            }

            return (orgFile, testFile);
        }

        private void Compare(IDictionary<string, object> x, IDictionary<string, object> y)
        {
            foreach (var key in x.Keys)
            {
                x[key].Should().Be(y[key], key);
            }
        }

        private void Compare(IEnumerable<IDictionary<string, object>> left, IEnumerable<IDictionary<string, object>> right, IEnumerable<string> fields = null, IEnumerable<string> ignore = null)
        {
            var comparer = new DirectoryComparar(fields, ignore);
            var aRight = right.ToArray();

            left.Should().HaveSameCount(aRight);

            var results = new bool[left.Count()];
            foreach (var (leftItem, i) in left.Select((x, i) => (x, i)))
            {
                var doesHas = comparer.Equals(leftItem, aRight[i]);
                results[i] = doesHas;

                Output.WriteLine(leftItem.ToString() + " - " + ((doesHas) ? "found" : "not found"));
            }

            results.Should().NotContain(false);
        }

        [Fact]
        public void GetMigrateCommands_Contains_All_Public_Static_String_commands()
        {
            var commandsLookup = CruiseDAL.Schema.Migrations.GetMigrateCommands("to", "from").ToDictionary(x => x);

            var type = typeof(CruiseDAL.Schema.Migrations);

            var fields = type.GetFields(System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public)
                .Where(x => x.FieldType == typeof(string));

            foreach (var field in fields)
            {
                var commandFormat = (string)field.GetValue(null);
                var command = string.Format(commandFormat, "to", "from");
                commandsLookup.ContainsKey(command).Should().BeTrue(field.Name);
            }

            commandsLookup.Count().Should().Be(fields.Count());
        }

        [Theory]
        [InlineData("7Wolf.cruise")]
        [InlineData("MultiTest.2014.10.31.cruise")]
        [InlineData("test_FixCNT.cruise")]
        public void Migrate(string fileName)
        {
            var (origFile, testFile) = SetUpTestFile(fileName);
        }


        [Theory]
        [InlineData("7Wolf.cruise")]
        public void Migrate_Sale_Test(string fileName)
        {
            var (origFile, testFile) = SetUpTestFile(fileName);
            using (var origDatastore = new DAL(origFile))
            using (var newDatastore = new DAL(testFile))
            {
                //CruiseDAL.Updater.CheckNeedsMajorUpdate(datastore).Should().BeTrue();

                var sales = origDatastore.QueryGeneric("SELECT * FROM Sale;").Single();

                var salesAfter = newDatastore.QueryGeneric("SELECT * FROM Sale;").Single();

                Compare(salesAfter, sales);
            }
        }

        [Theory]
        [InlineData("7Wolf.cruise")]
        public void Migrate_TallyPopulation_Test(string fileName)
        {
            var (origFile, testFile) = SetUpTestFile(fileName);
            using (var origDatastore = new DAL(origFile))
            using (var newDatastore = new DAL(testFile))
            {
                var tallyPops = origDatastore.QueryGeneric("SELECT DISTINCT " +
                    "st.Code AS StratumCode," +
                    "sg.Code AS SampleGroupCode, " +
                    "tdv.Species, " +
                    "tdv.LiveDead, " +
                    "t.Hotkey, " +
                    "t.Description " +
                    "FROM CountTree " +
                    "JOIN SampleGroup AS sg USING (SampleGroup_CN) " +
                    "JOIN Stratum AS st USING (Stratum_CN) " +
                    "LEFT JOIN TreeDefaultValue AS tdv USING (TreeDefaultValue_CN) " +
                    "JOIN Tally AS t USING (Tally_CN) " +
                    "Order by st.Code, sg.Code, tdv.Species, tdv.LiveDead" +
                    ";").ToArray();

                var tallyPopsAfter = newDatastore.QueryGeneric("SELECT " +
                    "StratumCode, " +
                    "SampleGroupCode, " +
                    "Species, " +
                    "LiveDead, " +
                    "Hotkey, " +
                    "Description " +
                    "FROM TallyPopulation " +
                    "Order by StratumCode, SampleGroupCode, Species, LiveDead" +
                    ";").ToArray();

                Compare(tallyPopsAfter, tallyPops);
            }
        }

        [Theory]
        [InlineData("7Wolf.cruise")]
        public void Migrate_Tree_Test(string fileName)
        {
            var (origFile, testFile) = SetUpTestFile(fileName);
            using (var origDatastore = new DAL(origFile))
            using (var newDatastore = new DAL(testFile))
            {
                var treeOrig = origDatastore.QueryGeneric(
                    "SELECT * FROM Tree " +
                    "ORDER BY CuttingUnit_CN, Plot_CN, TreeNumber; ");

                var treeAfter = newDatastore.QueryGeneric(
                    "SELECT * FROM Tree " +
                    "ORDER BY CuttingUnit_CN, Plot_CN, TreeNumber; ");

                var ignore = new string[] { "Tree_GUID", "TreeID", "ModifiedDate", "ExpansionFactor", "TreeFactor", "PointFactor", "TreeMeasurment_CN" };
                if(fileName == "7Wolf.cruise")
                { ignore = ignore.Prepend("Species").ToArray(); }

                Compare(treeAfter, treeOrig, ignore: ignore);
            }
        }

        [Theory]
        [InlineData("7Wolf.cruise")]
        public void Migrate_TreeDefaultValueTreeAuditvalue(string fileName)
        {
            var (origFile, testFile) = SetUpTestFile(fileName);
            using (var origDatastore = new DAL(origFile))
            using (var newDatastore = new DAL(testFile))
            {
                var tdvtavOrig = origDatastore.QueryGeneric(
                    "SELECT * FROM TreeDefaultValueTreeAuditValue " +
                    "ORDER BY TreeDefaultValue_CN, TreeAuditValue_CN;");

                var tdvtavAfter = newDatastore.QueryGeneric(
                    "SELECT * FROM TreeDefaultValueTreeAuditValue " +
                    "ORDER BY TreeDefaultValue_CN, TreeAuditValue_CN;");

                Compare(tdvtavAfter, tdvtavOrig);
            }
        }

        [Theory]
        [InlineData("7Wolf.cruise")]
        public void Migrate_ErrorLog_Test(string fileName)
        {
            var (origFile, testFile) = SetUpTestFile(fileName);
            using (var origDatastore = new DAL(origFile))
            using (var newDatastore = new DAL(testFile))
            {
                // note: program and message may be different so we wont read those fields

                var erroLogOrig = origDatastore.QueryGeneric(
                    "SELECT TableName, CN_Number, ColumnName, Level, Suppress FROM ErrorLog " +
                    "ORDER BY TableName, CN_Number, ColumnName;").ToArray();

                //var treeAuditErrors = newDatastore.QueryGeneric(
                //    "SELECT * FROM TreeAuditError;").ToArray();

                //var treeErrors = newDatastore.QueryGeneric(
                //    "SELECT * FROM TreeError;").ToArray();

                var errorLogAfter = newDatastore.QueryGeneric(
                    "SELECT TableName, CN_Number, ColumnName, Level, Suppress FROM ErrorLog " +
                    "ORDER BY TableName, CN_Number, ColumnName;").ToArray();

                Compare(errorLogAfter, erroLogOrig);
            }
        }

        [Theory]
        [InlineData("7Wolf.cruise")]
        public void Migrate_Plot_Test(string fileName)
        {
            var (origFile, testFile) = SetUpTestFile(fileName);
            using (var origDatastore = new DAL(origFile))
            using (var newDatastore = new DAL(testFile))
            {
                var plotOrig = origDatastore.QueryGeneric(
                    "SELECT * FROM Plot " +
                    "ORDER BY CuttingUnit_CN, Plot_CN;");

                var plotAfter = newDatastore.QueryGeneric(
                    "SELECT * FROM Plot " +
                    "ORDER BY CuttingUnit_CN, Plot_CN;");

                Compare(plotAfter, plotOrig);
            }
        }

        [Theory]
        [InlineData("7Wolf.cruise")]
        public void Migrate_CountTree_Test(string fileName)
        {
            var (origFile, testFile) = SetUpTestFile(fileName);
            using (var origDatastore = new DAL(origFile))
            using (var newDatastore = new DAL(testFile))
            {
                var countTreeOrig = origDatastore.QueryGeneric(
                    "SELECT " +
                        "CuttingUnit_CN, " +
                        "SampleGroup_CN, " +
                        "TreeDefaultValue_CN, " +
                        "sum(TreeCount) AS TreeCount, " +
                        "sum(SumKPI) AS SumKPI " +
                    "FROM CountTree " +
                    "GROUP BY CuttingUnit_CN, SampleGroup_CN, ifnull(TreeDefaultValue_CN, 0) " +
                    "ORDER BY CuttingUnit_CN, SampleGroup_CN, ifnull(TreeDefaultValue_CN, 0);").ToArray();

                var countTreeAfter = newDatastore.QueryGeneric(
                    "SELECT " +
                        "CuttingUnit_CN, " +
                        "SampleGroup_CN, " +
                        "TreeDefaultValue_CN, " +
                        "TreeCount, " +
                        "SumKPI " +
                    "FROM CountTree " +
                    //"GROUP BY CuttingUnit_CN, SampleGroup_CN, ifnull(TreeDefaultValue_CN, 0) " +
                    "ORDER BY CuttingUnit_CN, SampleGroup_CN, ifnull(TreeDefaultValue_CN, 0);").ToArray();

                Compare(countTreeAfter, countTreeOrig);
            }
        }

        public class DirectoryComparar : IEqualityComparer<IDictionary<string, object>>
        {
            public IEnumerable<string> MatchKeys { get; set; }
            public IEnumerable<string> IgnoreKeys { get; set; }

            public DirectoryComparar(IEnumerable<string> matchKeys = null, IEnumerable<string> ignoreKeys = null)
            {
                MatchKeys = matchKeys;
                IgnoreKeys = ignoreKeys;
            }

            public bool Equals(IDictionary<string, object> x, IDictionary<string, object> y)
            {
                var matchKeys = MatchKeys ?? x.Keys;

                foreach (var key in matchKeys)
                {
                    if (IgnoreKeys != null
                        && IgnoreKeys.Contains(key))
                    { continue; }

                    if (!x.ContainsKey(key))
                    { return false; }
                    var xValue = x[key];

                    if (!y.ContainsKey(key))
                    { return false; }
                    var yValue = y[key];

                    if (!xValue.Equals(yValue))
                    {
                        return false;
                    }
                }

                return true;
            }

            public int GetHashCode(IDictionary<string, object> obj)
            {
                var matchKeys = MatchKeys ?? obj.Keys;

                int hash = 0;

                foreach (var key in matchKeys)
                {
                    var value = obj[key];

                    hash ^= key.GetHashCode();
                    hash ^= key.ToString().GetHashCode();
                }

                return hash;
            }
        }
    }
}