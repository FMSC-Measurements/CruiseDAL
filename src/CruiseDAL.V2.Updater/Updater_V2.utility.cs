using FMSC.ORM.Core;
using FMSC.ORM.EntityModel.Attributes;
using System;
using System.Data.Common;

namespace CruiseDAL
{
    public partial class Updater_V2
    {
        public static void CleanupErrorLog(Datastore db)
        {
            foreach (var el in db.From<ErrorLog>().Where("CN_Number != 0").Query())
            {
                if (db.GetRowCount(el.TableName, "WHERE rowID = @p1", el.CN_Number) == 0)
                {
                    db.Delete(el);
                }
            }
        }

        public static void SetDatabaseVersion(CruiseDatastore db, string newVersion)
        {
            string command = String.Format("UPDATE Globals SET Value = '{0}' WHERE Block = 'Database' AND Key = 'Version';", newVersion);
            db.Execute(command);
            db.LogMessage($"Updated structure version to {newVersion}");
        }

        public static void SetDatabaseVersion(DbConnection connection, string newVersion, DbTransaction transaction = null)
        {
            string command = String.Format("UPDATE Globals SET Value = '{0}' WHERE Block = 'Database' AND Key = 'Version';", newVersion);
            connection.ExecuteNonQuery(command, transaction: transaction);

            connection.LogMessage($"Updated structure version to {newVersion}", transaction: transaction);
        }

        [Table("ErrorLog")]
        private class ErrorLog
        {
            [PrimaryKeyField(Name = "RowID")]
            public Int64? RowID { get; set; }

            [Field(Name = "TableName")]
            public String TableName { get; set; }

            [Field(Name = "CN_Number")]
            public Int64 CN_Number { get; set; }
        }
    }
}