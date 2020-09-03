namespace CruiseDAL.Schema
{
    public partial class DDL
    {
        public const string CREATE_TABLE_TREEDEFAULTVALUE =
@"CREATE TABLE TreeDefaultValue ( 
    TreeDefaultValue_CN INTEGER PRIMARY KEY AUTOINCREMENT, 
    CruiseID TEXT NOT NULL COLLATE NOCASE,
    SpeciesCode TEXT COLLATE NOCASE,
    SampleGroupPattern TEXT COLLATE NOCASE,
    PrimaryProduct TEXT COLLATE NOCASE,
    CullPrimary REAL DEFAULT 0.0, 
    CullPrimaryDead REAL DEFAULT 0.0,
    HiddenPrimary REAL DEFAULT 0.0,
    HiddenPrimaryDead REAL DEFAULT 0.0,
    TreeGrade TEXT DEFAULT '0' COLLATE NOCASE,
    TreeGradeDead TEXT DEFAULT '0' COLLATE NOCASE,
    CullSecondary REAL DEFAULT 0.0, 
    HiddenSecondary REAL DEFAULT 0.0,
    Recoverable REAL DEFAULT 0.0, 
    MerchHeightLogLength INTEGER DEFAULT 0,
    MerchHeightType TEXT DEFAULT 'F' , 
    FormClass REAL DEFAULT 0.0, 
    BarkThicknessRatio REAL DEFAULT 0.0, 
    AverageZ REAL DEFAULT 0.0, 
    ReferenceHeightPercent REAL DEFAULT 0.0, 
    CreatedBy TEXT DEFAULT 'none', 
    CreatedDate DateTime DEFAULT (datetime( 'now', 'localtime')), 
    ModifiedBy TEXT, 
    ModifiedDate DateTime, 
    RowVersion INTEGER DEFAULT 0, 
    UNIQUE (CruiseID, PrimaryProduct, SpeciesCode), 

    CHECK (LiveDead IN ('L', 'D')),

    FOREIGN KEY (CruiseID) REFERENCES Cruise (CruiseID) ON DELETE CASCADE,
    FOREIGN KEY (SpeciesCode, CruiseID) REFERENCES Species (SpeciesCode, CruiseID) ON UPDATE CASCADE ON DELETE CASCADE
);";

        public const string CREATE_UNIQUE_INDEX_TreeDefaultValue_SpeciesCode_SampleGroupPattern_PrimaryProduct =
@"CREATE UNIQUE INDEX TreeDefaultValue_SpeciesCode_SampleGroupPattern_PrimaryProduct ON TreeDefaultValue (ifnull(SpeciesCode, ''), ifnull(SampleGroupPattern, ''), ifnull(PrimaryProduct, ''));";

        public const string CREATE_INDEX_TreeDefaultValue_SpeciesCode =
            @"CREATE INDEX 'TreeDefaultValue_SpeciesCode' ON 'TreeDefaultValue' ('SpeciesCode');";

        public const string CREATE_INDEX_TreeDefaultValue_CruiseID =
            @"CREATE INDEX 'TreeDefaultValue_CruiseID' ON 'TreeDefaultValue' ('CruiseID');";

        public const string CREATE_TRIGGER_TREEDEFAULTVALUE_ONUPDATE =
@"CREATE TRIGGER TreeDefaultValue_OnUpdate 
AFTER UPDATE OF 
    SpeciesCode, 
    PrimaryProduct, 
    SampleGroupPattern, 
    CullPrimary,
    CullPrimaryDead,
    HiddenPrimary,
    HiddenPrimaryDead,
    TreeGrade,
    TreeGradeDead,
    CullSecondary, 
    HiddenSecondary, 
    Recoverable, 
    MerchHeightLogLength, 
    MerchHeightType, 
    FormClass, 
    BarkThicknessRatio, 
    AverageZ, 
    ReferenceHeightPercent 
ON TreeDefaultValue 
FOR EACH ROW 
BEGIN 
    UPDATE TreeDefaultValue SET ModifiedDate = datetime( 'now', 'localtime') WHERE TreeDefaultValue_CN = new.TreeDefaultValue_CN; 
    UPDATE TreeDefaultValue SET RowVersion = old.RowVersion + 1 WHERE TreeDefaultValue_CN = old.TreeDefaultValue_CN; 
END; ";
    }

    public partial class Migrations
    {
        public const string MIGRATE_TREEDEFAULTVALUE_FORMAT_STR =
@"INSERT INTO {0}.TreeDefaultValue (
        TreeDefaultValue_CN,
        CruiseID,
        PrimaryProduct,
        SpeciesCode,
        CullPrimary,
        CullPrimaryDead,
        HiddenPrimary,
        HiddenPrimaryDead,
        TreeGrade,
        TreeGradeDead,
        CullSecondary,
        HiddenSecondary,
        Recoverable,
        MerchHeightLogLength,
        MerchHeightType,
        FormClass,
        BarkThicknessRatio,        
        AverageZ, 
        ReferenceHeightPercent,
        CreatedBy,
        CreatedDate,
        ModifiedBy,
        ModifiedDate,
        RowVersion
    )
    SELECT
        tdvl.TreeDefaultValue_CN,
        '{3}',
        tdvl.PrimaryProduct,
        tdvl.Species,
        tdvl.CullPrimary,
        tdvd.CullPrimaryDead,
        tdvl.HiddenPrimary,
        tdvd.HiddenPrimaryDead,
        tdvl.TreeGrade,
        tdvd.TreeGradeDead,
        tdvl.CullSecondary,
        tdvl.HiddenSecondary,
        tdvl.Recoverable,
        tdvl.MerchHeightLogLength,
        tdvl.MerchHeightType,
        tdvl.FormClass,
        tdvl.BarkThicknessRatio,
        tdvl.AverageZ,
        tdvl.ReferenceHeightPercent,
        tdvl.CreatedBy,
        tdvl.CreatedDate,
        tdvl.ModifiedBy,
        tdvl.ModifiedDate,
        tdvl.RowVersion
    FROM (SELECT * FROM {1}.TreeDefaultValue WHERE LiveDead = 'L') AS tdvl 
    LEFT JOIN (SELECT * FROM {1}.TreeDefaultValue WHERE LiveDead = 'D') AS tdvd USING (Species, PrimaryProduct);

    UPDATE TreeDefaultValue SET 
    CullPrimaryDead = 
";
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