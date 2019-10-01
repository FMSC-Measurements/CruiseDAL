namespace CruiseDAL
{
    public class Updater_V3 : IUpdater
    {
        public void Update(CruiseDatastore datastore)
        {
            if(datastore.DatabaseVersion == "3.0.0")
            {
                UpdateTo_3_0_1(datastore);
            }
        }

        public static void UpdateTo_3_0_1(CruiseDatastore ds)
        {
            // create an in-memory database
            // to migrate into
            using (var newDatastore = new CruiseDatastore_V3())
            {
                // migrate contents of old db into new in-memory database
                Migrator.Migrate(ds, newDatastore);

                // use back up rutine to replace old database with 
                // migrated contents
                newDatastore.BackupDatabase(ds);
            }
        }
    }
}