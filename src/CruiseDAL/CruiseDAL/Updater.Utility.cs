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

        public static bool CheckNeedsMajorUpdate(DAL dal)
        {
            var version = dal.DatabaseVersion;

            if (version.StartsWith("2"))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static bool CheckCanUpdate(DAL dal)
        {
            var version = dal.DatabaseVersion;

            if (version.StartsWith("2")
                || version.StartsWith("3."))
            {
                return true;
            }
            else
            {
                return false;
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

        public static string GetTriggerDDL(DatastoreRedux db, string tableName)
        {
            var getTriggers = String.Format("SELECT group_concat(sql,';\r\n') FROM sqlite_master WHERE tbl_name LIKE '{0}' and type LIKE 'trigger';", tableName);
            return db.ExecuteScalar(getTriggers) as string;
        }
    }
}