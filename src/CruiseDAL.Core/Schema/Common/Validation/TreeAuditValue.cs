namespace CruiseDAL.Schema
{
    public partial class DDL
    {
        public const string CREATE_TABLE_TREEAUDITVALUE =
            "CREATE TABLE TreeAuditValue ( " +
                "TreeAuditValue_CN INTEGER PRIMARY KEY AUTOINCREMENT, " +
                "TreeAuditValueID TEXT NOT NULL," +
                "Field TEXT NOT NULL COLLATE NOCASE, " +
                "Min REAL Default 0.0, " +
                "Max REAL Default 0.0, " +
                "ValueSet TEXT, " +
                "Required BOOLEAN DEFAULT 0, " +
                //"ErrorMessage TEXT, " +
                "UNIQUE (TreeAuditValueID) " +
            ");";
    }

    public partial class Migrations
    {
        public const string MIGRATE_TREEAUDITVALUE_FORMAT_STR =
                "INSERT INTO {0}.TreeAuditValue ( " +
                    "TreeAuditValue_CN, " +
                    "TreeAuditValueID, " +
                    "Field, " +
                    "Min, " +
                    "Max, " +
                    "ValueSet, " +
                    "Required" +
                ") " +
                "SELECT " +
                    "TreeAuditValue_CN, " +
                    "'updateToV3-' || TreeAuditValue_CN AS TreeAuditValueID, " +
                    "Field, " +
                    "Min, " +
                    "Max, " +
                    "ValueSet, " +
                    "Required " +
                "FROM {1}.TreeAuditValue;";
    }

    //public partial class Updater
    //{
    //    public const string UPDATE_TREEAUDITVALUE_TO_V3 =
    //        "CREATE TABLE new_TreeAuditValue ( " +
    //            "TreeAuditValue_CN INTEGER PRIMARY KEY AUTOINCREMENT, " +
    //            "TreeAuditValueID TEXT NOT NULL," +
    //            "Field TEXT NOT NULL COLLATE NOCASE, " +
    //            "Min REAL Default 0.0, " +
    //            "Max REAL Default 0.0, " +
    //            "ValueSet TEXT, " +
    //            "Required BOOLEAN DEFAULT 0, " +
    //            //"ErrorMessage TEXT, " +
    //            "UNIQUE (TreeAuditValueID) " +
    //        ");" +
    //        "INSERT INTO new_TreeAuditValue " +
    //        "SELECT " +
    //            "TreeAuditValue_CN, " +
    //            "'updateToV3-' || TreeAuditValue_CN AS TreeAuditValueID, " +
    //            "Field, " +
    //            "Min, " +
    //            "Max, " +
    //            "ValueSet, " +
    //            "Required " +
    //        //"ErrorMessage " +
    //        "FROM TreeAuditValue;" +
    //        "DROP TABLE TreeAuditValue; " +
    //        "ALTER TABLE new_TreeAuditValue RENAME TO TreeAuditValue; ";
    //}
}