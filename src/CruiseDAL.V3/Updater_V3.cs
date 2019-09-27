namespace CruiseDAL
{
    public class Updater_V3 : IUpdater
    {
        public void Update(CruiseDatastore datastore)
        {
            // create an in-memory database
            // to migrate into
            using (var newDatastore = new CruiseDatastore_V3())
            {
                // migrate contents of old db into new in-memory database
                Migrator.Migrate(datastore, newDatastore);

                // use back up rutine to replace old database with 
                // migrated contents
                newDatastore.BackupDatabase(datastore.Path);
            }
        }
    }
}