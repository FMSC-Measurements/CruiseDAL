using FluentAssertions;
using FMSC.ORM.SQLite;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace CruiseDAL.Tests
{
    public class Updater_Utility_Test : TestBase
    {
        public Updater_Utility_Test(ITestOutputHelper output) : base(output)
        {
        }

        //[Fact]
        //public void CleanUpErrorLog()
        //{
        //    var filePath = Path.Combine(TestTempPath, "TestCleanUpErrorLog.cruise");

        //    try
        //    {

        //        using (var db = new SQLiteDatastore(filePath))
        //        {
        //            var builder = new CruiseDALDatastoreBuilder();
        //            builder.CreateDatastore(db);

        //            db.Insert(new ErrorLogDO() { CN_Number = 101, TableName = "Tree", ColumnName = "something", Level = "e", Message = "something" });

        //            db.GetRowCount("ErrorLog", null).Should().Be(1);

        //            Updater.CleanupErrorLog(db);

        //            db.GetRowCount("ErrorLog", null).Should().Be(0);
        //        }
        //    }
        //    finally
        //    {
        //        if (File.Exists(filePath))
        //        {
        //            try
        //            {
        //                File.Delete(filePath);
        //            }
        //            catch { } //dont stomp on exception
        //        }
        //    }
        //}
    }
}
