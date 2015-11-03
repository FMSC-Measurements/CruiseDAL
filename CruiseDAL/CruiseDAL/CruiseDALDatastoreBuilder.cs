using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using FMSC.ORM.Core;


namespace CruiseDAL
{
    public class CruiseDALDatastoreBuilder : DatastoreBuilder
    {
        public new DALRedux DataStore
        {
            get { return (DALRedux)base.DataStore; }
            set { base.DataStore = value; }
        }

        public override void CreateDatastore()
        {
            if(DataStore.Exists)
            {
                File.Delete(DataStore.Path);
            }

            try
            {
                SQLiteConnection.CreateFile(DataStore.Path);
                DataStore.BeginTransaction();
                try
                {
                    CreateTables();
                    CreateTriggers();
                    DataStore.CommitTransaction();
                }
                catch
                {
                    DataStore.RollbackTransaction();
                    throw;
                }
            }
            catch
            {
                File.Delete(DataStore.Path);
                throw;
            }
        }

        public override void CreateTables()
        {
            String createSQL = GetCreateSQL();
            DataStore.Execute(createSQL);
        }

        public override void CreateTriggers()
        {
            String createTriggers = GetCreateTriggers();
            DataStore.Execute(createTriggers);
        }

        public override void UpdateDatastore()
        {
            Updater.Update(DataStore);
        }

        public static String GetCreateSQL()
        {
            return CruiseDAL.Properties.Resources.CruiseCreate;
        }

        public static string GetCreateTriggers()
        {
            return CruiseDAL.Properties.Resources.CreateTriggers;
        }
    }
}
