using FMSC.ORM.Core;
using System.IO;

namespace FMSC.ORM.SQLite
{
    public abstract class SQLiteDatabaseBuilder : DatabaseBuilder
    {
        public new SQLiteDatastore Datastore
        {
            get
            {
                return (SQLiteDatastore)base.Datastore;
            }
            set
            {
                base.Datastore = value;
            }
        }

        public override void CreateDatastore()
        {
            if (Datastore.Exists && !Datastore.IsInMemory)
            {
                System.IO.File.Delete(Datastore.Path);
            }

            try
            {
                if (!Datastore.IsInMemory)
                {
                    //This just creates a zero - byte file which SQLite
                    // will turn into a database when the file is opened properly.
                    using (FileStream fs = File.Create(Datastore.Path))
                    {
                        fs.Close();
                    }
                }
                Datastore.BeginTransaction();
                try
                {
                    CreateTables();
                    CreateTriggers();
                    Datastore.CommitTransaction();
                }
                catch
                {
                    Datastore.RollbackTransaction();
                    throw;
                }
            }
            catch
            {
                System.IO.File.Delete(Datastore.Path);
                throw;
            }
        }
    }
}