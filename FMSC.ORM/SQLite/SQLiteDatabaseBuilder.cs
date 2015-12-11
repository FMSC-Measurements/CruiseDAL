using FMSC.ORM.Core;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Text;

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
            if (Datastore.Exists)
            {
                System.IO.File.Delete(Datastore.Path);
            }

            try
            {
                SQLiteConnection.CreateFile(Datastore.Path);
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
