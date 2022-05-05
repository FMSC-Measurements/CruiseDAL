using FluentAssertions;
using FMSC.ORM.SQLite;
using FMSC.ORM.TestSupport;
using FMSC.ORM.TestSupport.TestModels;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace FMSC.ORM.Core
{
    public class ConnectionExtentions_Tests
    {
        [Fact]
        public void GetLastInsertRowID()
        {
            using (var ds = new SQLiteDatastore())
            {
                using (var connection = ds.OpenConnection())
                {
                    connection.ExecuteNonQuery("CREATE TABLE tbl (id INTEGER PRIMARY KEY AUTOINCREMENT, col1 TEXT);", null, null);

                    connection.ExecuteNonQuery("INSERT INTO tbl (col1) VALUES ('something');", null, null);

                    connection.GetLastInsertRowID().Should().Be(1);
                }
            }
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void Query_Test(bool nulls)
        {
            using (var ds = new SQLiteDatastore())
            {
                ds.CreateDatastore(new TestDBBuilder());

                var poco = CreateRandomPoco(nulls);
                ds.Insert(poco);

                using(var connection = ds.OpenConnection())
                {
                    var query = connection.Query<POCOMultiTypeObject>("SELECT * FROM MultiPropTable;");

                    var results = query.ToArray();
                    results.Should().HaveCount(1);

                    var result = query.SingleOrDefault();

                    result.Should().NotBeNull();

                    result.Should().BeEquivalentTo(poco);
                }
            }
        }

        [Theory(Skip = "reading two fields with the same name from seperate tables not supported")]
        [InlineData(true)]
        [InlineData(false)]
        public void Query_MultiType_Test(bool nulls)
        {
            using (var ds = new SQLiteDatastore())
            {
                ds.CreateDatastore(new TestDBBuilder());

                var t1 = new TableOne
                { Data = "something" };
                ds.Insert(t1);

                var t2 = new TableTwo
                {
                    Data = "somethingElse",
                    TableOne_ID = t1.TableOne_ID,
                };
                ds.Insert(t2);

                using (var connection = ds.OpenConnection())
                {
                    var query = connection.Query<TableTwo, TableOne>("SELECT t2.*, t1.* FROM TableTwo AS t2 JOIN TableOne AS t1 USING (TableOne_ID);");

                    var results = query.ToArray();
                    results.Should().HaveCount(1);

                    var (t2Again, t1Again) = query.SingleOrDefault();

                    t2Again.Should().NotBeNull();
                    t1Again.Should().NotBeNull();

                    t2Again.Should().BeEquivalentTo(t2);
                    t1Again.Should().BeEquivalentTo(t1);
                }
            }
        }

        private POCOMultiTypeObject CreateRandomPoco(bool nullableSetNull = false)
        {
            var randomizer = new Bogus.Randomizer();

            var poco = new POCOMultiTypeObject()
            {
                BoolField = randomizer.Bool(),
                DateTimeField = DateTime.Now,
                NDateTimeField = (nullableSetNull) ? (DateTime?)null : DateTime.Now,
                StrDateTime = (nullableSetNull) ? (string)null : DateTime.Now.ToShortTimeString(),
                DoubleField = randomizer.Double(),
                FloatField = randomizer.Float(),
                GuidField = randomizer.Guid(),
                NGuidField = (nullableSetNull) ? (Guid?)null : randomizer.Guid(),
                ID = randomizer.Int(),
                IntField = randomizer.Int(),
                LongField = randomizer.Long(),
                NBoolField = (nullableSetNull) ? (bool?)null : randomizer.Bool(),
                NDoubleField = (nullableSetNull) ? (double?)null : randomizer.Double(),
                NFloatField = (nullableSetNull) ? (float?)null : randomizer.Float(),
                NIntField = (nullableSetNull) ? (int?)null : randomizer.Int(),
                NLongField = (nullableSetNull) ? (long?)null : randomizer.Long(),
                //RowID = randomizer.Int(),
                StringField = randomizer.String2(16),
            };
            return poco;
        }
    }
}
