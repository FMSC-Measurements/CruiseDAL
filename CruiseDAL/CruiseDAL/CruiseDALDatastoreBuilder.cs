using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using FMSC.ORM.SQLite;

namespace CruiseDAL
{
    public class CruiseDALDatastoreBuilder : SQLiteDatabaseBuilder
    {
        public new DAL Datastore
        {
            get { return (DAL)base.Datastore; }
            set { base.Datastore = value; }
        }

        public override void CreateTables()
        {
            String createSQL = GetCreateSQL();
            Datastore.Execute(createSQL);
        }

        public override void CreateTriggers()
        {
            String createTriggers = GetCreateTriggers();
            Datastore.Execute(createTriggers);
        }

        public override void UpdateDatastore()
        {
            Updater.Update(Datastore);
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
