using FMSC.ORM.Core;
using System;
using System.Collections.Generic;
using System.Text;

#if Mono
using Mono.Data.Sqlite;
using SQLiteConnection = Mono.Data.Sqlite.SqliteConnection;
#else
using System.Data.SQLite;
#endif

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
                    SQLiteConnection.CreateFile(Datastore.Path);
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
