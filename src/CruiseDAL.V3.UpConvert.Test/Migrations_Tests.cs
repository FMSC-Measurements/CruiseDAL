using FluentAssertions;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Xunit;
using Xunit.Abstractions;
using System.Reflection;
using CruiseDAL.Migrators;
using System.Runtime.CompilerServices;
using CruiseDAL.TestCommon;
using CruiseDAL.UpConvert;

#if NET452 || NET461
using MoreLinq;
#endif

namespace CruiseDAL.V3.Test
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
            crz3File = new Migrator().MigrateFromV2ToV3(orgFile, true);

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
            var comparer = new DictionaryComparar(fields, ignore);
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
        public void ContainsAllMigrators()
        {
            var allMigratorTypes = Assembly.GetAssembly(typeof(Migrator))
                .GetTypes().Where(x => typeof(IMigrator).IsAssignableFrom(x) && x != typeof(IMigrator)).ToArray();

            Migrator.MIGRATORS.Select(x => x.GetType()).ToArray()
                .Should().Contain(allMigratorTypes);
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

        [SkippableTheory]
        [ClassData(typeof(TableNamesTestDataProvider))]
        public void RunDelete_WithConvertedFile(string tableName)
        {
            var skipTables = new[]
      {
                    "LK_CruiseMethod",
                    "LK_Product",
                    "LK_Purpose",
                    "LK_Region",
                    "LK_UOM",
                    "LK_TallyEntryType",
                    "Species",
                    "TreeField",
                };
            // some tables don't have cascading deletes so we need to skip them
            Skip.If(skipTables.Contains(tableName));

            var testFile = "MultiTest.2014.10.31.cruise";
            var (orgFile, crz3) = SetUpTestFile(testFile);

            var initializer = new DatabaseInitializer();
            using (var database = new CruiseDatastore_V3(crz3))
            {
                //database.OpenConnection();
                //database.Execute("PRAGMA foreign_keys=0;");
                database.Invoking(x => x.Execute($"DELETE FROM {tableName};"))
                        .Should().NotThrow();
                //var fKeyErrors = database.QueryGeneric("PRAGMA foreign_key_check;");
                //Output.WriteLine(string.Join("|\r\n",fKeyErrors.Select(x=> x.ToString()).ToArray()));
                //fKeyErrors.Should().BeEmpty();
            }
        }

        private (string, string) SetUpTestFile(string fileName, [CallerMemberName] string testName = null)
        {
            var filePath = Path.Combine(TestFilesDirectory, fileName);

            var baseFileName = Path.GetFileName(fileName);
            var orgFile = Path.Combine(TestTempPath, testName + fileName);
            var crz3File = (string)null;

            // create copy of base file
            if (File.Exists(orgFile) == false)
            {
                File.Copy(filePath, orgFile);
            }
            crz3File = new Migrator().MigrateFromV2ToV3(orgFile, true);

            return (orgFile, crz3File);
        }

        public class DictionaryComparar : IEqualityComparer<IDictionary<string, object>>
        {
            public IEnumerable<string> MatchKeys { get; set; }
            public IEnumerable<string> IgnoreKeys { get; set; }

            public DictionaryComparar(IEnumerable<string> matchKeys = null, IEnumerable<string> ignoreKeys = null)
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