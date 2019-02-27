using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using FMSC.ORM.Core;
using FMSC.ORM.SQLite;

namespace CruiseDAL
{
    public class CruiseDALDatastoreBuilder : SQLiteDatabaseBuilder
    {

        public override void CreateTables(DatastoreRedux datastore)
        {
            datastore.Execute(Schema.Schema.CREATE_TABLES);
        }

        public override void CreateTriggers(DatastoreRedux datastore)
        {
            datastore.Execute(Schema.Schema.CREATE_TRIGGERS);
        }

        public override void UpdateDatastore(DatastoreRedux datastore)
        {
            Updater.Update((DAL)datastore);
        }
    }
}
