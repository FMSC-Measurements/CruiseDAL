﻿using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using Xunit.Abstractions;

namespace FMSC.ORM
{
    public class TestBase
    {
        public ITestOutputHelper Output { get; }
        private string _testTempPath;
        private Stopwatch _stopwatch;

        protected DbProviderFactory DbProvider { get; }

        private List<string> FilesToBeDeleted { get; } = new List<string>();

        public TestBase(ITestOutputHelper output)
        {
            Output = output;

#if MICROSOFT_DATA_SQLITE
            DbProvider = Microsoft.Data.Sqlite.SqliteFactory.Instance;
            Output.WriteLine("using Microsoft.Data.Sqlite");
#elif SYSTEM_DATA_SQLITE
            DbProvider = System.Data.SQLite.SQLiteFactory.Instance;
            Output.WriteLine("using System.Data.Sqlite");
#endif

            var testTempPath = TestTempPath;
            if (!Directory.Exists(testTempPath))
            {
                Directory.CreateDirectory(testTempPath);
            }
        }

        ~TestBase()
        {
            foreach (var file in FilesToBeDeleted)
            {
                try
                {
                    File.Delete(file);
                }
                catch
                {
                    // do nothing
                }
            }
        }

        public string TestExecutionDirectory
        {
            get
            {
                var codeBase = new Uri(Assembly.GetExecutingAssembly().CodeBase).LocalPath;
                return Path.GetDirectoryName(codeBase);
            }
        }

        public string TestTempPath => _testTempPath ?? (_testTempPath = Path.Combine(Path.GetTempPath(), "TestTemp", this.GetType().FullName));
        public string TestFilesDirectory => Path.Combine(TestExecutionDirectory, "TestFiles");
        public string ResourceDirectory => Path.Combine(TestExecutionDirectory, "Resources");

        public string GetTempFilePath(string extention, string fileName = null)
        {
            return Path.Combine(TestTempPath, (fileName ?? Guid.NewGuid().ToString()) + extention);
        }

        public void RegesterFileForCleanUp(string path)
        {
            FilesToBeDeleted.Add(path);
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

        public DbConnection GetOpenConnection()
        {
            var conn = DbProvider.CreateConnection();
            conn.ConnectionString = "Data Source=:memory:";
            conn.Open();

#if SYSTEM_DATA_SQLITE
                ((System.Data.SQLite.SQLiteConnection) conn).Flags |= System.Data.SQLite.SQLiteConnectionFlags.NoVerifyTypeAffinity;
#endif

            return conn;
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