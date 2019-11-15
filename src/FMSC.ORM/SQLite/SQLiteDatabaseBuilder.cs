using FMSC.ORM.Core;
using System;
using System.IO;

namespace FMSC.ORM.SQLite
{
    public abstract class SQLiteDatabaseBuilder : IDatastoreBuilder
    {
        public static void CreateEmptyFile(string path, bool overwrite = true)
        {
            if(File.Exists(path))
            {
                if (overwrite)
                {
                    File.Delete(path);
                }
                else
                {
                    return;
                }
            }

            //This just creates a zero - byte file which SQLite
            // will turn into a database when the file is opened properly.
            using (FileStream fs = File.Create(path))
            {
                fs.Close();
            }
        }

        public void CreateDatastore(Datastore datastore)
        {
            var sqliteDatastore = (SQLiteDatastore)datastore;

            string path = null;
            if (!sqliteDatastore.IsInMemory)
            {
                path = sqliteDatastore.Path;
                CreateEmptyFile(path);
            }

            try
            {
                BuildDatabase(datastore);
            }
            catch 
            {
                if (!sqliteDatastore.IsInMemory)
                {
                    try
                    {
                        if(path != null)
                        { File.Delete(path); }
                    }
                    catch
                    {
                        // don't stomp on original exception
                    }
                }
                throw;
            }
        }

        public abstract void BuildDatabase(Datastore datastore);
    }
}