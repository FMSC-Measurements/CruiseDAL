﻿using System.Collections.Generic;

namespace CruiseDAL.Schema
{
    public class TreeFieldSetupTableDefinition : ITableDefinition
    {
        public string TableName => "TreeFieldSetup";

        public string CreateTable =>
@"CREATE TABLE TreeFieldSetup (
    CruiseID TEXT NOT NULL COLLATE NOCASE,
    StratumCode TEXT NOT NULL COLLATE NOCASE,
    SampleGroupCode TEXT COLLATE NOCASE,
    Field TEXT NOT NULL COLLATE NOCASE,
    FieldOrder INTEGER Default 0,
    IsHidden BOOLEAN Default 0,
    IsLocked BOOLEAN Default 0,
    -- value type determined by TreeField.DbType
    DefaultValueInt INTEGER, 
    DefaultValueReal REAL,
    DefaultValueBool BOOLEAN,
    DefaultValueText TEXT,

    CHECK (IsHidden IN (0, 1)),
    CHECK (IsLocked IN (0, 1)),

    FOREIGN KEY (StratumCode, CruiseID) REFERENCES Stratum (StratumCode, CruiseID) ON DELETE CASCADE ON UPDATE CASCADE,
    FOREIGN KEY (SampleGroupCode, StratumCode, CruiseID) REFERENCES SampleGroup (SampleGroupCode, StratumCode, CruiseID) ON DELETE CASCADE ON UPDATE CASCADE,
    FOREIGN KEY (Field) REFERENCES TreeField (Field)
);";

        public string InitializeTable => null;

        public string CreateTombstoneTable =>
@"CREATE TABLE TreeFieldSetup_Tombstone (
    CruiseID TEXT NOT NULL COLLATE NOCASE,
    StratumCode TEXT NOT NULL COLLATE NOCASE,
    SampleGroupCode TEXT COLLATE NOCASE,
    Field TEXT NOT NULL COLLATE NOCASE,
    FieldOrder INTEGER,
    DefaultValueInt INTEGER,
    DefaultValueReal REAL,
    DefaultValueBool BOOLEAN,
    DefaultValueText TEXT
);";

        public string CreateIndexes =>
@"CREATE INDEX NIX_TreeFieldSetup_Field ON TreeFieldSetup (Field);

CREATE INDEX NIX_TreeFieldSetup_StratumCode_CruiseID ON TreeFieldSetup (StratumCode, CruiseID);

CREATE INDEX NIX_TreeFieldSetup_SampleGroupCode_StratumCode_CruiseID ON TreeFieldSetup (SampleGroupCode, StratumCode, CruiseID); 

CREATE UNIQUE INDEX UIX_TreeFieldSetup_SampleGroupCode_StratumCode_Field_CruiseID ON TreeFieldSetup
    (ifnull(SampleGroupCode, ''), StratumCode, Field, CruiseID);";

        public IEnumerable<string> CreateTriggers => new[] { CREATE_TRIGGER_TreeFieldSetup_OnDelete };

    public const string CREATE_TRIGGER_TreeFieldSetup_OnDelete =
@"CREATE TRIGGER TreeFieldSetup_OnDelete
BEFORE DELETE ON TreeFieldSetup
BEGIN
    INSERT OR REPLACE INTO TreeFieldSetup_Tombstone (
        CruiseID,
        StratumCode,
        SampleGroupCode,
        Field,
        FieldOrder,
        DefaultValueInt,
        DefaultValueReal,
        DefaultValueBool,
        DefaultValueText
    ) VALUES (
        OLD.CruiseID,
        OLD.StratumCode,
        OLD.SampleGroupCode,
        OLD.Field,
        OLD.FieldOrder,
        OLD.DefaultValueInt,
        OLD.DefaultValueReal,
        OLD.DefaultValueBool,
        OLD.DefaultValueText
    );
END;";
    }
}