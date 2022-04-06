using FMSC.ORM.Core;
using System.Data.Common;

namespace FMSC.ORM.TestSupport
{
    public class TestDBBuilder : IDatastoreBuilder
    {
        public const string CREATE_MULTIPROPTABLE =
@"CREATE TABLE MultiPropTable
(
	ID INTEGER Primary Key AUTOINCREMENT,
	StringField TEXT,
	IntField INTEGER,
	NIntField INTEGER,
	LongField INTEGER,
	NLongField INTEGER,
	FloatField REAL,
	NFloatField REAL,
	DoubleField REAL,
	NDoubleField REAL,
	BoolField BOOLEAN,
	NBoolField BOOLEAN,
	GuidField TEXT,
	NGuidField TEXT,
	DateTimeField DATETIME,
	NDateTimeField DATETIME,
	StrDateTimeField DATETIME,
    EnumField TEXT,
	PartialyPublicField TEXT,
	ImplicitNamedField TEXT,
    AutomaticStringField TEXT,
    PartialyPublicAutomaticField TEXT,
	PrivateField TEXT,
	CreatedBy TEXT,
	ModifiedBy TEXT
);";

        public const string CREATE_AUTOINCREMENT_TABLE =
@"CREATE TABLE AutoIncrementTable
(
    ID INTEGER Primary Key AUTOINCREMENT,
    Data TEXT
);";

		public const string CREATE_TABLEONE =
@"CREATE TABLE TableOne
(
	TableOne_ID INTEGER Primary Key AUTOINCREMENT,
	Data TEXT
);";

		public const string CREATE_TABLETWO =
@"CREATE TABLE TableTwo
(
	TableTwo_ID INTEGER Primary Key AUTOINCREMENT,
	TableOne_ID INTEGER FORIEGN KEY REFERENCES TableOne (TableOne_ID),
	Data TEXT
)";


		public void BuildDatabase(DbConnection connection, DbTransaction transaction, IExceptionProcessor exceptionProcessor = null)
        {
            connection.ExecuteNonQuery(CREATE_MULTIPROPTABLE, transaction: transaction, exceptionProcessor: exceptionProcessor);
            connection.ExecuteNonQuery(CREATE_AUTOINCREMENT_TABLE, transaction: transaction, exceptionProcessor: exceptionProcessor);
			connection.ExecuteNonQuery(CREATE_TABLEONE, transaction: transaction, exceptionProcessor: exceptionProcessor);
			connection.ExecuteNonQuery(CREATE_TABLETWO, transaction: transaction, exceptionProcessor: exceptionProcessor);

		}
    }
}