using FMSC.ORM.Core;
using System.IO;

namespace FMSC.ORM.SQLite
{
    public abstract class SQLiteDatabaseBuilder : DatabaseBuilder
    {
        public override void CreateDatastore(DatastoreRedux datastore)
        {
            var sqliteDatastore = (SQLiteDatastore)datastore;

            if (!sqliteDatastore.IsInMemory)
            {
                if (sqliteDatastore.Exists)
                {
                    System.IO.File.Delete(sqliteDatastore.Path);
                }

                //This just creates a zero - byte file which SQLite
                // will turn into a database when the file is opened properly.
                using (FileStream fs = File.Create(sqliteDatastore.Path))
                {
                    fs.Close();
                }
            }

            try
            {
                base.CreateDatastore(datastore);
            }
            catch
            {
                if (!sqliteDatastore.IsInMemory)
                {
                    System.IO.File.Delete(sqliteDatastore.Path);
                }
                throw;
            }
        }
    }
}