using FluentAssertions;
using FMSC.ORM.Core;
using FMSC.ORM.TestSupport;
using FMSC.ORM.TestSupport.TestModels;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace FMSC.ORM.SQLite
{
    // tests that apply for all sqlite libraries 
    public class Sqlite_Tests_All : TestBase
    {
        public Sqlite_Tests_All(ITestOutputHelper output) : base(output)
        {
        }

        // verify that turning off ForeignKeys disables cascading 
        [Fact]
        public void Disabling_Fkeys_Disables_Cascading_Deletes()
        {
            using (var db = new SQLiteDatastore())
            {
                db.CreateDatastore(new TestDBBuilder());


                var t1 = new TableOne
                {
                    Data = "im a table one",
                };
                db.Insert(t1);

                var t3 = new TableThree
                {
                    TableOne_ID = t1.TableOne_ID,
                    Data = "im a table three",
                };
                db.Insert(t3);

                db.From<TableThree>().Count().Should().Be(1);

                db.OpenConnection();
                try
                {
                    db.Execute("PRAGMA foreign_keys=off;"); // note: Fkeys are off by default, but will just make sure they are off
                    db.Execute("DELETE FROM TableOne;");

                    db.From<TableThree>().Count().Should().Be(1);
                }
                finally
                {
                    db.ReleaseConnection();
                }
            }
        }

        [Fact]
        public void CreateTableWithMissingFKeyTable()
        {

            // create table track that references missing table artist
            using (var conn = GetOpenConnection())
            {
                conn.Invoking(x => x.ExecuteNonQuery(@"CREATE TABLE track(
  trackid     INTEGER,
  trackname   TEXT,
  trackartist INTEGER,
  FOREIGN KEY(trackartist) REFERENCES artist (artistid)
); ")).Should().NotThrow();

                conn.Invoking(x => x.ExecuteNonQuery(@"INSERT INTO artist VALUES(3, 'Sammy Davis Jr.');"))
                .Should().Throw<DbException>();
            }
        }


        //https://sqlite.org/forum/forumpost/483cdbb61a
        [Fact]
        public void AllowsCheckConstraintsWithOutCommas()
        {
            using (var conn = GetOpenConnection())
            {
                conn.Invoking(x => x.ExecuteNonQuery(@"CREATE TABLE track(
  trackid     INTEGER,
  trackname   TEXT,
  trackartist INTEGER,

  CHECK (length(trackname) > 0)
  CHECK (trackartist > 0)  
); ")).Should().NotThrow();

                conn.Invoking(x => x.ExecuteNonQuery(@"INSERT INTO artist VALUES(3, 'Sammy Davis Jr.');"))
                .Should().Throw<DbException>();
            }
        }


        // see if defering fkeys defers cascading deletes
        //[Fact]
        //public void Defering_Fkeys_Defers_Cascading_Deletes()
        //{
        //    using (var db = new SQLiteDatastore())
        //    {
        //        db.CreateDatastore(new TestDBBuilder());


        //        var t1 = new TableOne
        //        {
        //            Data = "im a table one",
        //        };
        //        db.Insert(t1);

        //        var t3 = new TableThree
        //        {
        //            TableOne_ID = t1.TableOne_ID,
        //            Data = "im a table three",
        //        };
        //        db.Insert(t3);

        //        db.From<TableThree>().Count().Should().Be(1);

        //        db.OpenConnection();
        //        try
        //        {
        //            db.Execute("PRAGMA foreign_keys=on;"); // note: Fkeys are off by default, but we need to make sure they are on for the connection


        //            db.BeginTransaction();
        //            db.Execute("PRAGMA defer_foreign_keys=on;");
        //            db.Execute("DELETE FROM TableOne;");

        //            db.From<TableThree>().Count().Should().Be(1);
        //            db.CommitTransaction();
        //            db.From<TableThree>().Count().Should().Be(0);
        //        }
        //        finally
        //        {
        //            db.ReleaseConnection();
        //        }
        //    }
        //}
    }
}
