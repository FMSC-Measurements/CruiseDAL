using FluentAssertions;
using Xunit;
using Xunit.Abstractions;

namespace FMSC.ORM.SQLite
{
    public class SqliteDatastore_Test_Microsoft_Data_Sqlite : TestBase
    {
        public SqliteDatastore_Test_Microsoft_Data_Sqlite(ITestOutputHelper output) : base(output)
        {
        }

#if MICROSOFT_DATA_SQLITE

        [Fact]
        public void Execute_with_param()
        {
            using var db = new SQLiteDatastore();
            db.Invoking(x => x.ExecuteScalar<string>("Select ?;", "'hello world'"))
                .Should().Throw<SQLException>()
                .And.CommandText.Should().NotBeNullOrEmpty();

            // var result = db.ExecuteScalar<string>("Select ?;", "'hello world'");
        }

#endif
    }
}