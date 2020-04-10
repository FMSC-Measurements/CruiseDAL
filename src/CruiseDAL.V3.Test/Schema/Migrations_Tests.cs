using CruiseDAL.DataObjects;
using CruiseDAL.V3.Tests;
using FluentAssertions;
using FMSC.ORM.SQLite;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Xunit;
using Xunit.Abstractions;

#if NET452 || NET461
using MoreLinq;
#endif

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

            var baseFileName = Path.GetFileName(fileName);
            var orgFile = Path.Combine(TestTempPath, fileName);
            var crz3File = (string)null;

            // create copy of base file
            if (File.Exists(orgFile) == false)
            {
                File.Copy(filePath, orgFile);
            }
            crz3File = Migrator.MigrateFromV2ToV3(orgFile, true);

            return (orgFile, crz3File);
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
                var rightItem = aRight[i];
                var doesHas = comparer.Equals(leftItem, rightItem);
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
        [InlineData("0432 C53East TS.cruise")]
        [InlineData("Whipple Timbersale.cruise")]
        [InlineData("Albee.M_20151014_1758.cruise")]
        [InlineData("dupTallyHotkey.cruise")]
        public void Migrate(string fileName)
        {
            var (origFile, testFile) = SetUpTestFile(fileName);
        }

        [Fact]
        public void Migrate_LogAuditRules()
        {
            var fromFile = GetTempFilePath(".cruise");
            var toFile = GetTempFilePath(".crz3");

            RegesterFileForCleanUp(fromFile);
            RegesterFileForCleanUp(toFile);

            using (var fromDb = new CruiseDatastore(fromFile, true, null, null))
            using (var toDb = new CruiseDatastore_V3(toFile, true))
            {
                fromDb.Execute(@"CREATE TABLE LogGradeAuditRule (
				Species TEXT,
				DefectMax REAL Default 0.0,
				ValidGrades TEXT);");

                var testLGARValues = new[]
                {
                    new {Sp = "ANY", DefectMax = 1.0, ValidGrades = "1"},
                    new {Sp = "ANY", DefectMax = 2.0, ValidGrades = "1,2"},
                    new {Sp = "ANY", DefectMax = 3.0, ValidGrades = "1,2, "},
                    new {Sp = "ANY", DefectMax = 4.0, ValidGrades = ",1 ,2"},
                };

                foreach (var value in testLGARValues)
                {
                    fromDb.Execute2("INSERT INTO LogGradeAuditRule (Species, DefectMax, ValidGrades) VALUES (@Sp, @DefectMax, @ValidGrades);",
                        value);
                }


                toDb.AttachDB(fromDb, "v2");
                toDb.Execute(String.Format(CruiseDAL.Schema.Migrations.MIGRATE_LOGGRADEAUDITRULE_V3, "main", "v2"));

                var results = toDb.From<CruiseDAL.V3.Models.LogGradeAuditRule_V3>().Query().ToArray();
                results.Should().NotBeEmpty();

                foreach(var value in testLGARValues)
                {
                    var grades = value.ValidGrades.Trim().Split(',').Where(x => string.IsNullOrEmpty(x) == false)
                                                  .ToArray();
                    foreach(var g in grades)
                    {
                        results.Should().Contain(x => x.DefectMax == value.DefectMax && x.Grade == g.Trim());
                    }
                }

            }
        
        }

        [Theory]
        [InlineData("7Wolf.cruise")]
        public void Migrate_Sale_Test(string fileName)
        {
            var (origFile, testFile) = SetUpTestFile(fileName);
            using (var origDatastore = new DAL(origFile))
            using (var newDatastore = new CruiseDatastore_V3(testFile))
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
            using (var newDatastore = new CruiseDatastore_V3(testFile))
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
            using (var newDatastore = new CruiseDatastore_V3(testFile))
            {
                var query = "SELECT * FROM Tree " +
                    "ORDER BY CuttingUnit_CN, Plot_CN, TreeNumber; ";

                var treesOrig = origDatastore.Query<TreeDO>(query).ToArray();

                var treesAfter = newDatastore.Query<TreeDO>(query).ToArray();

                treesAfter.Should().HaveSameCount(treesOrig);

                int len = treesAfter.Length;
                for(int i = 0; i < len; i++)
                {
                    var before = treesOrig[i];
                    var after = treesAfter[i];

                    after.Should().BeEquivalentTo(before, conf => 
                    conf.ExcludingNestedObjects()
                    .ExcludingFields()
                    .Excluding(x => x.Self)
                    .Excluding(x => x.DAL)
                    .Excluding(x => x.TreeDefaultValue)
                    .Excluding(x => x.CuttingUnit)
                    .Excluding(x => x.Stratum)
                    .Excluding(x => x.SampleGroup)
                    .Excluding(x => x.Plot)
                    .Excluding(x => x.TreeValidator)
                    .Excluding(x => x.Error)
                    .Excluding(x => x.IsValidated)
                    .Excluding(x => x.Validator)
                    .Excluding(x => x.Tree_GUID)
                    .Excluding(x => x.ModifiedBy)
                    .Excluding(x => x.ExpansionFactor)
                    .Excluding(x => x.TreeFactor)
                    .Excluding(x => x.PointFactor)
                    );
                }
            }
        }

        [Fact]
        public void Migrate_Tree_With_byteArray_treeid()
        {
            // in the old cruise files tree_guid would sometimes be stored as a blob
            // we need to make sure that when migrated that the value of treeid is always a string


            var v2File = GetTempFilePath(".cruise");
            //RegesterFileForCleanUp(v2File);

            using(var v2db = new DAL(v2File, true))
            {
                var unit = new CuttingUnitDO()
                {
                    Code = "u1"
                };
                v2db.Insert(unit);
                var stratum = new StratumDO()
                {
                    Code = "s1",
                    Method = "100",
                };
                v2db.Insert(stratum);
                var sg = new SampleGroupDO()
                {
                    Code = "sg1",
                    CutLeave = "",
                    UOM = "",
                    PrimaryProduct = "",
                    Stratum_CN = stratum.Stratum_CN,
                };
                v2db.Insert(sg);

                v2db.Execute("INSERT INTO Tree (CuttingUnit_CN, Stratum_CN, SampleGroup_CN, Tree_GUID, TreeNumber) " +
                    "VALUES (1, 1, 1, randomblob(16), 1);");
            }

            var v3File = GetTempFilePath(".crz3");
            RegesterFileForCleanUp(v3File);

            Migrator.MigrateFromV2ToV3(v2File, v3File);

            using(var v3db = new CruiseDatastore(v3File))
            {
                var treeid = v3db.ExecuteScalar("SELECT TreeID FROM Tree_V3 LIMIT 1;");
                treeid.Should().BeOfType<string>();
            }
        }

        [Theory]
        [InlineData("7Wolf.cruise")]
        public void Migrate_TreeDefaultValueTreeAuditvalue(string fileName)
        {
            var (origFile, testFile) = SetUpTestFile(fileName);
            using (var origDatastore = new DAL(origFile))
            using (var newDatastore = new CruiseDatastore_V3(testFile))
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
            using (var newDatastore = new CruiseDatastore_V3(testFile))
            {
                // note: program and message may be different so we wont read those fields

                var erroLogOrig = origDatastore.QueryGeneric(
                    "SELECT TableName, CN_Number, ColumnName, Level, cast (Suppress as bool) FROM ErrorLog " +
                    "ORDER BY TableName, CN_Number, ColumnName;").ToArray();

                //var treeAuditErrors = newDatastore.QueryGeneric(
                //    "SELECT * FROM TreeAuditError;").ToArray();

                //var treeErrors = newDatastore.QueryGeneric(
                //    "SELECT * FROM TreeError;").ToArray();

                var errorLogAfter = newDatastore.QueryGeneric(
                    "SELECT TableName, CN_Number, ColumnName, Level, cast (Suppress as bool) FROM ErrorLog " +
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
            using (var newDatastore = new CruiseDatastore_V3(testFile))
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
            using (var newDatastore = new CruiseDatastore_V3(testFile))
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