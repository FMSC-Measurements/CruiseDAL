using FMSC.ORM.SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
	DateTimeField DATETIME,
	PartialyPublicField TEXT,
	PrivateField TEXT,
	CreatedBy TEXT,
	ModifiedBy TEXT
)";


        public override void CreateTables()
        {
            this.Datastore.Execute(CREATE_MULTIPROPTABLE);
                
        }

        public override void CreateTriggers()
        {
            
        }

        public override void UpdateDatastore()
        {
            
        }
    }
}
