namespace CruiseDAL.Schema
{
    public partial class DDL
    {
        public const string CREATE_TABLE_TREEDEFAULTVALUE =
            "CREATE TABLE TreeDefaultValue ( " +
                "TreeDefaultValue_CN INTEGER PRIMARY KEY AUTOINCREMENT, " +
                "PrimaryProduct TEXT NOT NULL COLLATE NOCASE, " +
                "Species TEXT NOT NULL COLLATE NOCASE, " +
                "LiveDead TEXT NOT NULL COLLATE NOCASE, " +
                "FIAcode INTEGER DEFAULT 0, " +
                "CullPrimary REAL DEFAULT 0.0, " +
                "HiddenPrimary REAL DEFAULT 0.0, " +
                "CullSecondary REAL DEFAULT 0.0, " +
                "HiddenSecondary REAL DEFAULT 0.0, " +
                "Recoverable REAL DEFAULT 0.0, " +
                "ContractSpecies TEXT, " +
                "TreeGrade TEXT DEFAULT '0' COLLATE NOCASE, " +
                "MerchHeightLogLength INTEGER DEFAULT 0, " +
                "MerchHeightType TEXT DEFAULT 'F' , " +
                "FormClass REAL DEFAULT 0.0, " +
                "BarkThicknessRatio REAL DEFAULT 0.0, " +
                "AverageZ REAL DEFAULT 0.0, " +
                "ReferenceHeightPercent REAL DEFAULT 0.0, " +
                "CreatedBy TEXT DEFAULT 'none', " +
                "CreatedDate DateTime DEFAULT (datetime( 'now', 'localtime')), " +
                "ModifiedBy TEXT, " +
                "ModifiedDate DateTime, " +
                "RowVersion INTEGER DEFAULT 0, " +
                "UNIQUE (PrimaryProduct, Species, LiveDead), " +
                "FOREIGN KEY (Species) REFERENCES Species (Species) ON UPDATE CASCADE ON DELETE CASCADE" +
            ");";

        public const string CREATE_TRIGGER_TREEDEFAULTVALUE_ONUPDATE =
            "CREATE TRIGGER TreeDefaultValue_OnUpdate " +
            "AFTER UPDATE OF " +
                "PrimaryProduct, " +
                "Species, " +
                "LiveDead, " +
                "FIAcode, " +
                "CullPrimary, " +
                "HiddenPrimary, " +
                "CullSecondary, " +
                "HiddenSecondary, " +
                "Recoverable, " +
                "ContractSpecies, " +
                "TreeGrade, " +
                "MerchHeightLogLength, " +
                "MerchHeightType, " +
                "FormClass, " +
                "BarkThicknessRatio, " +
                "AverageZ, " +
                "ReferenceHeightPercent " +
            "ON TreeDefaultValue " +
            "FOR EACH ROW " +
            "BEGIN " +
                "UPDATE TreeDefaultValue SET ModifiedDate = datetime( 'now', 'localtime') WHERE TreeDefaultValue_CN = new.TreeDefaultValue_CN; " +
                "UPDATE TreeDefaultValue SET RowVersion = old.RowVersion + 1 WHERE TreeDefaultValue_CN = old.TreeDefaultValue_CN; " +
            "END; ";
    }

    public partial class Migrations
    {
        public const string MIGRATE_TREEDEFAULTVALUE_FORMAT_STR =
            "INSERT INTO {0}.TreeDefaultValue ( " +
                    "TreeDefaultValue_CN, " +
                    "PrimaryProduct, " +
                    "Species, " +
                    "LiveDead, " +
                    "FIAcode, " +
                    "CullPrimary, " +
                    "HiddenPrimary, " +
                    "CullSecondary, " +
                    "HiddenSecondary, " +
                    "Recoverable, " +
                    "ContractSpecies, " +
                    "TreeGrade, " +
                    "MerchHeightLogLength, " +
                    "MerchHeightType, " +
                    "FormClass, " +
                    "BarkThicknessRatio, " +
                    "AverageZ, " +
                    "ReferenceHeightPercent, " +
                    "CreatedBy, " +
                    "CreatedDate, " +
                    "ModifiedBy, " +
                    "ModifiedDate, " +
                    "RowVersion " +
                ") " +
                "SELECT " +
                    "TreeDefaultValue_CN, " +
                    "PrimaryProduct, " +
                    "Species, " +
                    "LiveDead, " +
                    "FIAcode, " +
                    "CullPrimary, " +
                    "HiddenPrimary, " +
                    "CullSecondary, " +
                    "HiddenSecondary, " +
                    "Recoverable, " +
                    "ContractSpecies, " +
                    "TreeGrade, " +
                    "MerchHeightLogLength, " +
                    "MerchHeightType, " +
                    "FormClass, " +
                    "BarkThicknessRatio, " +
                    "AverageZ, " +
                    "ReferenceHeightPercent, " +
                    "CreatedBy, " +
                    "CreatedDate, " +
                    "ModifiedBy, " +
                    "ModifiedDate, " +
                    "RowVersion " +
                "FROM {1}.TreeDefaultValue;";
    }


    //public partial class Updater
    //{
    //    public const string UPDATE_TREEDEFAULTVALUE_TO_V3 =
    //        "CREATE TABLE new_TreeDefaultValue ( " +
    //            "TreeDefaultValue_CN INTEGER PRIMARY KEY AUTOINCREMENT, " +
    //            "PrimaryProduct TEXT NOT NULL COLLATE NOCASE, " +
    //            "Species TEXT NOT NULL COLLATE NOCASE, " +
    //            "LiveDead TEXT NOT NULL COLLATE NOCASE, " +
    //            "FIAcode INTEGER DEFAULT 0, " +
    //            "CullPrimary REAL DEFAULT 0.0, " +
    //            "HiddenPrimary REAL DEFAULT 0.0, " +
    //            "CullSecondary REAL DEFAULT 0.0, " +
    //            "HiddenSecondary REAL DEFAULT 0.0, " +
    //            "Recoverable REAL DEFAULT 0.0, " +
    //            "ContractSpecies TEXT, " +
    //            "TreeGrade TEXT DEFAULT '0' COLLATE NOCASE, " +
    //            "MerchHeightLogLength INTEGER DEFAULT 0, " +
    //            "MerchHeightType TEXT DEFAULT 'F' , " +
    //            "FormClass REAL DEFAULT 0.0, " +
    //            "BarkThicknessRatio REAL DEFAULT 0.0, " +
    //            "AverageZ REAL DEFAULT 0.0, " +
    //            "ReferenceHeightPercent REAL DEFAULT 0.0, " +
    //            "CreatedBy TEXT DEFAULT 'none', " +
    //            "CreatedDate DateTime DEFAULT (datetime( 'now', 'localtime')), " +
    //            "ModifiedBy TEXT, " +
    //            "ModifiedDate DateTime , " +
    //            "RowVersion INTEGER DEFAULT 0, " +
    //            "UNIQUE(PrimaryProduct, Species, LiveDead)" +
    //        ");" +
    //        "INSERT OR IGNORE INTO new_TreeDefaultValue ( " +
    //            "TreeDefaultValue_CN, " +
    //            "PrimaryProduct, " +
    //            "Species, " +
    //            "LiveDead, " +
    //            "FIAcode, " +
    //            "CullPrimary, " +
    //            "HiddenPrimary, " +
    //            "CullSecondary, " +
    //            "HiddenSecondary, " +
    //            "Recoverable, " +
    //            "ContractSpecies, " +
    //            "TreeGrade, " +
    //            "MerchHeightLogLength, " +
    //            "MerchHeightType, " +
    //            "FormClass, " +
    //            "BarkThicknessRatio, " +
    //            "AverageZ, " +
    //            "ReferenceHeightPercent, " +
    //            "CreatedBy, " +
    //            "CreatedDate, " +
    //            "ModifiedBy, " +
    //            "ModifiedDate, " +
    //            "RowVersion " +
    //        ") " +
    //        "SELECT " +
    //            "TreeDefaultValue_CN, " +
    //            "PrimaryProduct, " +
    //            "Species, " +
    //            "LiveDead, " +
    //            "FIAcode, " +
    //            "CullPrimary, " +
    //            "HiddenPrimary, " +
    //            "CullSecondary, " +
    //            "HiddenSecondary, " +
    //            "Recoverable, " +
    //            "ContractSpecies, " +
    //            "TreeGrade, " +
    //            "MerchHeightLogLength, " +
    //            "MerchHeightType, " +
    //            "FormClass, " +
    //            "BarkThicknessRatio, " +
    //            "AverageZ, " +
    //            "ReferenceHeightPercent, " +
    //            "CreatedBy, " +
    //            "CreatedDate, " +
    //            "ModifiedBy, " +
    //            "ModifiedDate, " +
    //            "RowVersion " +
    //        "FROM TreeDefaultValue ; " +
    //        "DROP TABLE TreeDefaultValue; " +
    //        "ALTER TABLE new_TreeDefaultValue RENAME TO TreeDefaultValue;";
    //}
}