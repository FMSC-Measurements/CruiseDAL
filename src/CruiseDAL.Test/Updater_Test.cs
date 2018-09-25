using CruiseDAL.DataObjects;
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
    public class Updater_Test : TestBase
    {
        public Updater_Test(ITestOutputHelper output) : base(output)
        {
        }

        [Fact]
        public void Update_FROM_05_30_2013()
        {
            var filePath = Path.Combine(TestTempPath, "TestUpdate.cruise");

            try
            {

                using (var setup = new SQLiteDatastore(filePath))
                {
                    setup.Execute(CruiseDAL.Test.SQL.CRUISECREATE_05_30_2013);
                }

                using (var datastore = new DAL(filePath))
                {
                    var semVerActual = new Version(datastore.DatabaseVersion);
                    var semVerExpected = new Version(DAL.CURENT_DBVERSION);

                    semVerActual.Major.Should().Be(semVerExpected.Major);
                    semVerActual.Minor.Should().Be(semVerExpected.Minor);
                }
            }
            finally
            {
                if(File.Exists(filePath))
                {
                    File.Delete(filePath);
                }
            }
        }

        
    }
}
