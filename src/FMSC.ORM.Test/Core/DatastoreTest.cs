using FluentAssertions;
using FMSC.ORM.SQLite;
using FMSC.ORM.Test;
using FMSC.ORM.TestSupport;
using Xunit;
using Xunit.Abstractions;

namespace FMSC.ORM.Core
{
    public class DatastoreTest : TestBase
    {
        public DatastoreTest(ITestOutputHelper output) : base(output)
        { }

        [Fact]
        public void CommitTransaction()
        {
            using var db = new SQLiteDatastore();

            db.BeginTransaction();
            db.TransactionDepth.Should().Be(1);
            db.CurrentTransaction.Should().NotBeNull();

            db.Execute(TestDBBuilder.CREATE_TABLEONE);
            db.CheckTableExists("TableOne").Should().BeTrue();


            db.CommitTransaction();
            db.TransactionDepth.Should().Be(0);
            db.CurrentTransaction.Should().BeNull();

            db.CheckTableExists("TableOne").Should().BeTrue();
        }


        [Fact]
        public void RollbackTransaction()
        {
            using var db = new SQLiteDatastore();

            db.BeginTransaction();
            db.TransactionDepth.Should().Be(1);
            db.CurrentTransaction.Should().NotBeNull();

            db.Execute(TestDBBuilder.CREATE_TABLEONE);
            db.CheckTableExists("TableOne").Should().BeTrue();


            db.RollbackTransaction();
            db.TransactionDepth.Should().Be(0);
            db.CurrentTransaction.Should().BeNull();

            db.CheckTableExists("TableOne").Should().BeFalse();
        }
    }
}