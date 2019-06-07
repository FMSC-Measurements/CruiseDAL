using FMSC.ORM.Core;
using FMSC.ORM.SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CruiseDAL
{
    public class CruiseDatastoreBuilder_V2 : SQLiteDatabaseBuilder
    {
        public override void BuildDatabase(Datastore datastore)
        {
            datastore.Execute(Schema.Schema.CREATE_TABLES);
            datastore.Execute(Schema.Schema.CREATE_TRIGGERS);
        }
    }
}
