using System;

namespace FMSC.ORM.Core
{
    public abstract class DatabaseBuilder
    {
        public DatastoreRedux Datastore { get; set; }

        public abstract void CreateDatastore();

        public abstract void CreateTables();


        public abstract void CreateTriggers();

        public abstract void UpdateDatastore();
    }
}
