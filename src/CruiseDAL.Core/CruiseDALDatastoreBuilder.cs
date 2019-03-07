using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using FMSC.ORM.Core;
using FMSC.ORM.SQLite;

namespace CruiseDAL
{
    public class CruiseDALDatastoreBuilder : SQLiteDatabaseBuilder
    {

        public string[] CREATE_COMMANDS = new string[]
        {
            //core tables
            CruiseDAL.Schema.Common.DDL.CREATE_TABLE_SALE,
            CruiseDAL.Schema.Common.DDL.CREATE_TRIGGER_SALE_ONUPDATE,

            CruiseDAL.Schema.Common.DDL.CREATE_TABLE_CUTTINGUNIT,
            CruiseDAL.Schema.Common.DDL.CREATE_TRIGGER_CUTTINGUNIT_ONUPDATE,

            CruiseDAL.Schema.Common.DDL.CREATE_TABLE_STRATUM,
            CruiseDAL.Schema.Common.DDL.CREATE_TRIGGER_STRATUM_ONUPDATE,

            CruiseDAL.Schema.V3.DDL.CREATE_TABLE_CUTTINGUNIT_STRATUM,

            CruiseDAL.Schema.V3.DDL.CREATE_TABLE_TREEFIELDSETUP_V3,

            CruiseDAL.Schema.V3.DDL.CREATE_TABLE_SAMPLEGROUP_V3,
            CruiseDAL.Schema.V3.DDL.CREATE_TRIGGER_SAMPLEGROUP_V3_ONUPDATE,

            CruiseDAL.Schema.V3.DDL.CREATE_TABLE_SPECIES,

            CruiseDAL.Schema.V3.DDL.CREATE_TABLE_SUBPOPULATION,

            CruiseDAL.Schema.V3.DDL.CREATE_TABLE_TALLYPOPULATION,

            CruiseDAL.Schema.V3.DDL.CREATE_TABLE_PLOT_V3,
            CruiseDAL.Schema.V3.DDL.CREATE_TRIGGER_PLOT_V3_ONUPDATE,

            CruiseDAL.Schema.V3.DDL.CREATE_TABLE_PLOT_STRATUM,
            CruiseDAL.Schema.V3.DDL.CREATE_TRIGGER_PLOT_STRATUM_ONUPDATE,

            CruiseDAL.Schema.V3.DDL.CREATE_TABLE_TREE_V3,
            CruiseDAL.Schema.V3.DDL.CREATE_TRIGGER_TREE_V3_ONUPDATE,

            CruiseDAL.Schema.V3.DDL.CREATE_TABLE_TREEMEASURMENT,
            CruiseDAL.Schema.V3.DDL.CREATE_TRIGGER_TREEMEASURMENTS_ONUPDATE,

            CruiseDAL.Schema.V3.DDL.CREATE_TABLE_LOG_V3,
            CruiseDAL.Schema.V3.DDL.CREATE_TRIGGER_LOG_V3_ONUPDATE,

            CruiseDAL.Schema.V3.DDL.CREATE_TABLE_LOGFIELDSETUP_V3,

            CruiseDAL.Schema.V3.DDL.CREATE_TABLE_TALLYLEDGER,

            CruiseDAL.Schema.Common.DDL.CREATE_TABLE_FIXCNTTALLYCLASS,
            CruiseDAL.Schema.Common.DDL.CREATE_TABLE_FIXCNTTALLYPOPULATION,
            CruiseDAL.Schema.Common.DDL.CREATE_TABLE_TREEDEFAULTVALUE, 

            //processing tables
            CruiseDAL.Schema.Common.DDL.CREATE_TABLE_BIOMASSEQUATION,
            CruiseDAL.Schema.Common.DDL.CREATE_TABLE_LCD,
            CruiseDAL.Schema.Common.DDL.CREATE_TABLE_LOGMATRIX,
            CruiseDAL.Schema.Common.DDL.CREATE_TABLE_LOGSTOCK,
            CruiseDAL.Schema.Common.DDL.CREATE_TRIGGER_LOGSTOCK_ONUPDATE,
            CruiseDAL.Schema.Common.DDL.CREATE_TABLE_POP,
            CruiseDAL.Schema.Common.DDL.CREATE_TABLE_PRO,
            CruiseDAL.Schema.Common.DDL.CREATE_TABLE_QUALITYADJEQUATION,
            CruiseDAL.Schema.Common.DDL.CREATE_TABLE_REGRESSION,
            CruiseDAL.Schema.Common.DDL.CREATE_TABLE_REPORTS,
            CruiseDAL.Schema.Common.DDL.CREATE_TABLE_STRATUMSTATS,
            CruiseDAL.Schema.Common.DDL.CREATE_TABLE_SAMPLEGROUPSTATS,
            CruiseDAL.Schema.Common.DDL.CREATE_TABLE_SAMPLEGROUPSTATSTREEDEFAULTVALUE,
            CruiseDAL.Schema.Common.DDL.CREATE_TABLE_TREECALCULATEDVALUES,
            CruiseDAL.Schema.Common.DDL.CREATE_TABLE_VALUEEQUATION,
            CruiseDAL.Schema.Common.DDL.CREATE_TABLE_VOLUMEEQUATION,


            //setup tables 
            CruiseDAL.Schema.Common.DDL.CREATE_TABLE_LOGFIELDSETUPDEFAULT,
            CruiseDAL.Schema.Common.DDL.CREATE_TABLE_TREEFIELDSETUPDEFAULT,

            //utility tables
            CruiseDAL.Schema.Common.DDL.CREATE_TABLE_GLOBALS,
            CruiseDAL.Schema.Common.DDL.CREATE_TABLE_MESSAGELOG,

            //validation 
            CruiseDAL.Schema.Common.DDL.CREATE_TABLE_LOGGRADEAUDITRULE,
            CruiseDAL.Schema.Common.DDL.CREATE_TABLE_TREEAUDITVALUE,
            CruiseDAL.Schema.V3.DDL.CREATE_TABLE_TREEDEFAULTVALUE_TREEAUDITVALUE,
            CruiseDAL.Schema.Common.DDL.CREATE_TABLE_ERRORLOG,

            //views 
            CruiseDAL.Schema.V2Backports.DDL.CREATE_VIEW_COUNTTREE,
            CruiseDAL.Schema.V2Backports.DDL.CREATE_VIEW_CUTTINGUNITSTRATUM,
            CruiseDAL.Schema.V2Backports.DDL.CREATE_VIEW_LOG,
            CruiseDAL.Schema.V2Backports.DDL.CREATE_VIEW_PLOT,
            CruiseDAL.Schema.V2Backports.DDL.CREATE_VIEW_SAMPLEGROUP,
            CruiseDAL.Schema.V2Backports.DDL.CREATE_VIEW_SAMPLEGROUPTREEDEFAULTVALUE,
            CruiseDAL.Schema.V2Backports.DDL.CREATE_VIEW_TREE,
            CruiseDAL.Schema.V2Backports.DDL.CREATE_VIEW_TREEDEFAULTVALUETREEAUDITVALUE,
            CruiseDAL.Schema.V2Backports.DDL.CREATE_VIEW_TREEESTIMATE,

            "INSERT INTO Globals (Block, Key, Value) VALUES ('Database', 'Version', '3.0.0'); "
        };

        public override void CreateTables(DatastoreRedux datastore)
        {
            var conn = datastore.OpenConnection();
            CreateDatabase(conn);

            datastore.ReleaseConnection();
        }

        public void CreateDatabase(System.Data.Common.DbConnection conn)
        {
            var transaction = conn.BeginTransaction();
            try
            {
                foreach (var cmd in CREATE_COMMANDS)
                {

                    conn.ExecuteNonQuery(cmd, (object[])null, transaction);

                }

                transaction.Commit();
            }
            catch (Exception e)
            {
                Logger.Log.E(e);
                throw;
            }
        }

        public override void CreateTriggers(DatastoreRedux datastore)
        {
            //datastore.Execute(Schema.Schema.CREATE_TRIGGERS);
        }

        public override void UpdateDatastore(DatastoreRedux datastore)
        {
            Updater.Update((DAL)datastore);
        }
    }
}
