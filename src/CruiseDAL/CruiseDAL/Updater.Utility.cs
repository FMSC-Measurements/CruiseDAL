using CruiseDAL.DataObjects;
using CruiseDAL.Schema;
using FMSC.ORM.Core;
using System;

namespace CruiseDAL
{
    public static partial class Updater
    {
        public static void SetDatabaseVersion(DAL db, string newVersion)
        {
            string command = String.Format("UPDATE Globals SET Value = '{0}' WHERE Block = 'Database' AND Key = 'Version';", newVersion);
            db.Execute(command);
            db.LogMessage(String.Format("Updated structure version to {0}", newVersion), "I");
            db.DatabaseVersion = newVersion;
        }

        public static void CleanupErrorLog(DatastoreRedux db)
        {
            foreach (ErrorLogDO el in db.From<ErrorLogDO>().Where("CN_Number != 0").Query())
            {
                if (db.GetRowCount(el.TableName, "WHERE rowID = @p1", el.CN_Number) == 0)
                {
                    el.Delete();
                }
            }
        }

        public static void FixTreeAuditValueFKeyErrors(DatastoreRedux db)
        {
            if (db.HasForeignKeyErrors(TREEDEFAULTVALUETREEAUDITVALUE._NAME))
            {
                db.BeginTransaction();
                try
                {
                    db.Execute("DELETE FROM TreeDefaultValueTreeAuditValue WHERE TreeDefaultValue_CN NOT IN (Select TreeDefaultValue_CN FROM TreeDefaultValue);");
                    db.Execute("DELETE FROM TreeDefaultValueTreeAuditValue WHERE TreeAuditValue_CN NOT IN (SELECT TreeAuditValue_CN FROM TreeAuditValue);");
                    db.CommitTransaction();
                }
                catch
                {
                    db.RollbackTransaction();
                }
            }
        }

        private static String[] ListTriggers(DatastoreRedux db)
        {
            var result = db.ExecuteScalar("SELECT group_concat(name,',') FROM sqlite_master WHERE type LIKE 'trigger';") as string;
            if (string.IsNullOrEmpty(result)) { return new string[0]; }
            else
            {
                return result.Split(',');
            }
        }

        public static void RebuildTable(DatastoreRedux db, String tableName, String newTableDef, String columnList)
        {
            //get all triggers associated with table so we can recreate them later
            var getTriggers = String.Format("SELECT group_concat(sql,';\r\n') FROM sqlite_master WHERE tbl_name LIKE '{0}' and type LIKE 'trigger';", tableName);
            var triggers = db.ExecuteScalar(getTriggers) as string;
            db.BeginTransaction();
            try
            {
                db.Execute("PRAGMA foreign_keys = off;");
                db.Execute("ALTER TABLE " + tableName + " RENAME TO " + tableName + "temp;");

                //create rebuilt table
                db.Execute(newTableDef + ";");

                //copy data from existing table to rebuilt table
                db.Execute(
                    "INSERT INTO " + tableName +
                    " ( " + columnList + ") " +
                    "SELECT " + columnList + " FROM " + tableName + "temp;");

                db.Execute("DROP TABLE " + tableName + "temp;");

                //recreate triggers
                if (triggers != null)
                {
                    db.Execute(triggers);
                }

                db.Execute("PRAGMA foreign_keys = on;");
                db.CommitTransaction();
            }
            catch
            {
                db.RollbackTransaction();
                throw;
            }
        }
    }
}