﻿using System.Collections.Generic;

namespace CruiseDAL.Schema
{
    public class TreeDefaultValueTableDefinition : ITableDefinition
    {
        public string TableName => "TreeDefaultValue";

        public string CreateTable =>
@"CREATE TABLE TreeDefaultValue (
    TreeDefaultValue_CN INTEGER PRIMARY KEY AUTOINCREMENT,
    CruiseID TEXT NOT NULL COLLATE NOCASE,
    SpeciesCode TEXT COLLATE NOCASE,
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

    FOREIGN KEY (CruiseID) REFERENCES Cruise (CruiseID) ON DELETE CASCADE,
    FOREIGN KEY (SpeciesCode, CruiseID) REFERENCES Species (SpeciesCode, CruiseID) ON UPDATE CASCADE ON DELETE CASCADE
);";

        public string InitializeTable => null;

        public string CreateTombstoneTable =>
@"CREATE TABLE TreeDefaultValue_Tombstone (
    CruiseID TEXT NOT NULL COLLATE NOCASE,
    SpeciesCode TEXT COLLATE NOCASE,
    PrimaryProduct TEXT COLLATE NOCASE,
    CullPrimary REAL,
    CullPrimaryDead REAL,
    HiddenPrimary REAL,
    HiddenPrimaryDead REAL,
    TreeGrade TEXT COLLATE NOCASE,
    TreeGradeDead TEXT COLLATE NOCASE,
    CullSecondary REAL,
    HiddenSecondary REAL,
    Recoverable REAL,
    MerchHeightLogLength INTEGER,
    MerchHeightType TEXT,
    FormClass REAL,
    BarkThicknessRatio REAL,
    AverageZ REAL,
    ReferenceHeightPercent REAL,
    CreatedBy TEXT,
    CreatedDate DateTime,
    ModifiedBy TEXT,
    ModifiedDate DateTime,
    RowVersion INTEGER
);";

        public string CreateIndexes =>
@"CREATE UNIQUE INDEX TreeDefaultValue_SpeciesCode_PrimaryProduct ON TreeDefaultValue (CruiseID, ifnull(SpeciesCode, '') COLLATE NOCASE, ifnull(PrimaryProduct, '') COLLATE NOCASE);

CREATE INDEX 'TreeDefaultValue_SpeciesCode' ON 'TreeDefaultValue' ('SpeciesCode');

CREATE INDEX 'TreeDefaultValue_CruiseID' ON 'TreeDefaultValue' ('CruiseID');";

        public IEnumerable<string> CreateTriggers => new[] { CREATE_TRIGGER_TREEDEFAULTVALUE_ONUPDATE, CREATE_TRIGGER_TreeDefaultValue_OnDelete };

        public const string CREATE_TRIGGER_TREEDEFAULTVALUE_ONUPDATE =
@"CREATE TRIGGER TreeDefaultValue_OnUpdate
AFTER UPDATE OF
    SpeciesCode,
    PrimaryProduct,
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

        public const string CREATE_TRIGGER_TreeDefaultValue_OnDelete =
@"CREATE TRIGGER TreeDefaultValue_OnDelete
BEFORE DELETE ON TreeDefaultValue
FOR EACH ROW
BEGIN
    INSERT OR REPLACE INTO TreeDefaultValue_Tombstone (
        CruiseID,
        SpeciesCode,
        PrimaryProduct,
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
    ) VALUES (
        OLD.CruiseID,
        OLD.SpeciesCode,
        OLD.PrimaryProduct,
        OLD.CullPrimary,
        OLD.CullPrimaryDead,
        OLD.HiddenPrimary,
        OLD.HiddenPrimaryDead,
        OLD.TreeGrade,
        OLD.TreeGradeDead,
        OLD.CullSecondary,
        OLD.HiddenSecondary,
        OLD.Recoverable,
        OLD.MerchHeightLogLength,
        OLD.MerchHeightType,
        OLD.FormClass,
        OLD.BarkThicknessRatio,
        OLD.AverageZ,
        OLD.ReferenceHeightPercent,
        OLD.CreatedBy,
        OLD.CreatedDate,
        OLD.ModifiedBy,
        OLD.ModifiedDate,
        OLD.RowVersion
    );
END;";
    }
}