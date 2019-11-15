using FluentAssertions;
using System;
using System.Data.Common;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using Xunit.Abstractions;

namespace CruiseDAL.V2.Test
{
    public class TestBase
    {
        private string _testTempPath;
        protected ITestOutputHelper Output { get; private set; }
        protected DbProviderFactory DbProvider { get; private set; }
        protected Stopwatch _stopwatch;

        public TestBase(ITestOutputHelper output)
        {
            Output = output;
            Output.WriteLine($"CodeBase: {System.Reflection.Assembly.GetExecutingAssembly().CodeBase}");

            var testTempPath = TestTempPath;
            if (!Directory.Exists(testTempPath))
            {
                Directory.CreateDirectory(testTempPath);
            }

#if SYSTEM_DATA_SQLITE
            DbProvider = System.Data.SQLite.SQLiteFactory.Instance;
#elif MICROSOFT_DATA_SQLITE
            DbProvider = Microsoft.Data.Sqlite.SqliteFactory.Instance;
#else

#endif
        }

        public string TestExecutionDirectory
        {
            get
            {
                var codeBase = new Uri(Assembly.GetExecutingAssembly().CodeBase).LocalPath;
                return Path.GetDirectoryName(codeBase);
            }
        }

        public string TestFilesDirectory => Path.Combine(TestExecutionDirectory, "TestFiles");



        public string TestTempPath
        {
            get
            {
                return _testTempPath ?? (_testTempPath = Path.Combine(Path.GetTempPath(), "TestTemp", this.GetType().FullName));
            }
        }

        public string GetTestFile(string fileName)
        {
            var sourceTestFilePath = Path.Combine(TestFilesDirectory, fileName);
            if (File.Exists(sourceTestFilePath) == false) { throw new FileNotFoundException(sourceTestFilePath); }

            var testTemp = TestTempPath;
            var destFilePath = Path.Combine(testTemp, fileName);

            File.Copy(sourceTestFilePath, destFilePath, true);
            return destFilePath;
        }

        public void StartTimer()
        {
            _stopwatch = new Stopwatch();
            Output.WriteLine("Stopwatch Started");
            _stopwatch.Start();
        }

        public void EndTimer()
        {
            _stopwatch.Stop();
            Output.WriteLine("Stopwatch Ended:" + _stopwatch.ElapsedMilliseconds.ToString() + "ms");
        }

        protected void VerifyCommandSyntex(string commandText)
        {
            using (var conn = DbProvider.CreateConnection())
            {
                var command = conn.CreateCommand();
                Output.WriteLine("testing:\r\n" + commandText);
                command.CommandText = "EXPLAIN " + commandText;

#if MICROSOFT_DATA_SQLITE
                var connectionString = "Data Source =:memory:;";
#elif SYSTEM_DATA_SQLITE
                var connectionString = "Data Source =:memory:; Version = 3; New = True;";
#endif
                conn.ConnectionString = connectionString;
                conn.Open();

                command.Connection = conn;

                try
                {
                    command.ExecuteNonQuery();//calling execute should always throw but we check that it isn't a syntax exception
                }
                catch (DbException ex)
                {
                    ex.Message.Should().NotContainEquivalentOf("syntax");
                    //Assert.DoesNotContain("syntax ", ex.Message, StringComparison.InvariantCultureIgnoreCase);
                }
            }
        }
    }
}