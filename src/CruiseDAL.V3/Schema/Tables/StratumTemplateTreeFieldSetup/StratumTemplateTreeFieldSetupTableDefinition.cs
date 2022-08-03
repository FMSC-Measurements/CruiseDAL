using System.Collections.Generic;

namespace CruiseDAL.Schema
{
    public class StratumTemplateTreeFieldSetupTableDefinition : ITableDefinition
    {
        public string TableName => "StratumTemplateTreeFieldSetup";

        public string CreateTable => GetCreateTable(TableName);


        public string GetCreateTable(string tableName)
        {
return $@"CREATE TABLE {tableName} (
    StratumTemplateTreeFieldSetup_CN INTEGER PRIMARY KEY AUTOINCREMENT,
    StratumTemplateName TEXT NOT NULL COLLATE NOCASE,
    CruiseID TEXT NOT NULL COLLATE NOCASE,
    Field TEXT NOT NULL COLLATE NOCASE,
    FieldOrder INTEGER Default 0,
    IsHidden BOOLEAN Default 0,
    IsLocked BOOLEAN Default 0,
    -- value type determined by TreeField.DbType
    DefaultValueInt INTEGER,
    DefaultValueReal REAL,
    DefaultValueBool BOOLEAN,
    DefaultValueText TEXT,

    CreatedBy TEXT DEFAULT 'none',
    Created_TS DATETIME DEFAULT (CURRENT_TIMESTAMP),
    ModifiedBy TEXT,
    Modified_TS DATETIME,

    UNIQUE (Field, StratumTemplateName, CruiseID),

    FOREIGN KEY (StratumTemplateName, CruiseID) REFERENCES StratumTemplate (StratumTemplateName, CruiseID) ON DELETE CASCADE ON UPDATE CASCADE,
    FOREIGN KEY (Field) REFERENCES TreeField (Field)
);";
        }

        public string InitializeTable => null;

        public string CreateTombstoneTable =>
@"CREATE TABLE StratumTemplateTreeFieldSetup_Tombstone (
    StratumTemplateName TEXT NOT NULL COLLATE NOCASE,
    CruiseID TEXT NOT NULL COLLATE NOCASE,
    Field TEXT NOT NULL COLLATE NOCASE,
    FieldOrder INTEGER,
    IsHidden BOOLEAN,
    IsLocked BOOLEAN,
    -- value type determined by TreeField.DbType
    DefaultValueInt INTEGER,
    DefaultValueReal REAL,
    DefaultValueBool BOOLEAN,
    DefaultValueText TEXT,

    CreatedBy TEXT,
    Created_TS DATETIME,
    ModifiedBy TEXT,
    Modified_TS DATETIME,
    Deleted_TS DATETIME
);

CREATE INDEX NIX_StratumTemplateTreeFieldSetup_Tombstone_StratumTemplateName_CruiseID_Field ON StratumTemplateTreeFieldSetup_Tombstone
(StratumTemplateName, CruiseID, Field);
";

        public string CreateIndexes =>
@"CREATE INDEX NIX_StratumTemplateTreeFieldSetup_StratumTemplateName_CruiseID ON StratumTemplateTreeFieldSetup (StratumTemplateName, CruiseID);";

        public IEnumerable<string> CreateTriggers => new[] { CREATE_TRIGGER_StratumTemplateTreeFieldSetup_OnUpdate, CREATE_TRIGGER_StratumTemplateTreeFieldSetup_OnDelete, };

        public const string CREATE_TRIGGER_StratumTemplateTreeFieldSetup_OnUpdate =
@"CREATE TRIGGER StratumTemplateTreeFieldSetup_OnUpdate
AFTER UPDATE OF
    FieldOrder,
    IsHidden,
    IsLocked,
    DefaultValueInt,
    DefaultValueReal,
    DefaultValueBool,
    DefaultValueText
ON StratumTemplateTreeFieldSetup
FOR EACH ROW
BEGIN
    UPDATE StratumTemplateTreeFieldSetup SET Modified_TS = CURRENT_TIMESTAMP WHERE StratumTemplateTreeFieldSetup_CN = OLD.StratumTemplateTreeFieldSetup_CN;
END;
";

        public const string CREATE_TRIGGER_StratumTemplateTreeFieldSetup_OnDelete =
@"CREATE TRIGGER StratumTemplateTreeFieldSetup_OnDelete
BEFORE DELETE ON StratumTemplateTreeFieldSetup
FOR EACH ROW
BEGIN
    INSERT OR REPLACE INTO StratumTemplateTreeFieldSetup_Tombstone (
        StratumTemplateName,
        CruiseID,
        Field,
        FieldOrder,
        IsHidden,
        IsLocked,
        DefaultValueInt,
        DefaultValueReal,
        DefaultValueBool,
        DefaultValueText,
        CreatedBy,
        Created_TS,
        ModifiedBy,
        Modified_TS,
        Deleted_TS
    ) VALUES (
        OLD.StratumTemplateName,
        OLD.CruiseID,
        OLD.Field,
        OLD.FieldOrder,
        OLD.IsHidden,
        OLD.IsLocked,
        OLD.DefaultValueInt,
        OLD.DefaultValueReal,
        OLD.DefaultValueBool,
        OLD.DefaultValueText,
        OLD.CreatedBy,
        OLD.Created_TS,
        OLD.ModifiedBy,
        OLD.Modified_TS,
        CURRENT_TIMESTAMP
    );
END;
";
    }
}