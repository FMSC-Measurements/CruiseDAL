using CruiseDAL.TestCommon;
using CruiseDAL.Update;
using FluentAssertions;
using FMSC.ORM.Core;
using FMSC.ORM.SQLite;
using System.Linq;
using Xunit;
using Xunit.Abstractions;

namespace CruiseDAL.V3.Test.Update
{
    public class DbUpdateBase_Test : TestBase
    {
        public DbUpdateBase_Test(ITestOutputHelper output) : base(output)
        {
        }

        [Fact]
        public void ListFieldsIntersect()
        {
            using (var db = new SQLiteDatastore())
            {
                using (var conn = db.OpenConnection())
                {
                    conn.ExecuteNonQuery("CREATE TABLE A ( f1 TEXT, f2 TEXT );");

                    conn.ExecuteNonQuery("CREATE TABLE B ( f2 TEXT, f3 TEXT );");

                    var tables = DbUpdateBase.ListFieldsIntersect(conn, "A", "B");
                    tables.Single().Should().Be("\"f2\"");
                }
            }
        }
    }
}