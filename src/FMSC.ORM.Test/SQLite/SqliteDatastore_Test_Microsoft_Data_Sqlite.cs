using FluentAssertions;
using FMSC.ORM.SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace FMSC.ORM.SQLite
{
    public class SqliteDatastore_Test_Microsoft_Data_Sqlite : TestBase
    {
        public SqliteDatastore_Test_Microsoft_Data_Sqlite(ITestOutputHelper output) : base(output)
        {
        }

        [Fact]
        public void Execute_with_missing_param()
        {
            using var db = new SQLiteDatastore();
            db.Invoking(x => x.Execute("Select ?;", "'hello world'"))
                .Should().Throw<SQLException>()
                .And.CommandText.Should().NotBeNullOrEmpty();
        }
    }
}
