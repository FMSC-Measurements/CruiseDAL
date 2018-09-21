using System.Data;

namespace FMSC.ORM.Core
{
    public abstract class DatabaseBuilder
    {
        public virtual void CreateDatastore(DatastoreRedux datastore)
        {
            CreateTables(datastore);
            CreateTriggers(datastore);
        }

        public abstract void CreateTables(DatastoreRedux datastore);

        public abstract void CreateTriggers(DatastoreRedux datastore);

        public abstract void UpdateDatastore(DatastoreRedux datastore);
    }
}