using FMSC.ORM.Core;
using FMSC.ORM.SQLite;

namespace FMSC.ORM.TestSupport
{
	public class TestDBBuilder : SQLiteDatabaseBuilder
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

        public override void BuildDatabase(Datastore datastore)
        {
            datastore.Execute(CREATE_MULTIPROPTABLE);
            datastore.Execute(CREATE_AUTOINCREMENT_TABLE);
        }
    }
}