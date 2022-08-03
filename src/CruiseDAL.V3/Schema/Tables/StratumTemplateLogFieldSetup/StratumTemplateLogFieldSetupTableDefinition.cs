using System.Collections.Generic;

namespace CruiseDAL.Schema
{
    public class StratumTemplateLogFieldSetupTableDefinition : ITableDefinition
    {
        public string TableName => "StratumTemplateLogFieldSetup";

        public string CreateTable => GetCreateTable(TableName);


        public string GetCreateTable(string tableName)
        {
return $@"CREATE TABLE {tableName} (
    StratumTemplateLogFieldSetup_CN INTEGER PRIMARY KEY AUTOINCREMENT,
    StratumTemplateName TEXT NOT NULL COLLATE NOCASE,
    CruiseID TEXT NOT NULL COLLATE NOCASE,
    Field TEXT NOT NULL COLLATE NOCASE,
    FieldOrder INTEGER Default 0,

    CreatedBy TEXT DEFAULT 'none',
    Created_TS DATETIME DEFAULT (CURRENT_TIMESTAMP),
    ModifiedBy TEXT,
    Modified_TS DATETIME,

    UNIQUE (StratumTemplateName, CruiseID, Field),    

    FOREIGN KEY (StratumTemplateName, CruiseID) REFERENCES StratumTemplate (StratumTemplateName, CruiseID) ON DELETE CASCADE ON UPDATE CASCADE,
    FOREIGN KEY (Field) REFERENCES LogField (Field)
);";
        }

        public string InitializeTable => null;

        public string CreateTombstoneTable =>
@"CREATE TABLE StratumTemplateLogFieldSetup_Tombstone (
    StratumTemplateName TEXT NOT NULL COLLATE NOCASE,
    CruiseID TEXT NOT NULL COLLATE NOCASE,
    Field TEXT NOT NULL COLLATE NOCASE,
    FieldOrder INTEGER,
    CreatedBy TEXT,
    Created_TS DATETIME,
    ModifiedBy TEXT,
    Modified_TS DATETIME,
    Deleted_TS DATETIME
);

CREATE INDEX NIX_StratumTemplateLogFieldSetup_Tombstone_StratumTemplateName_Field ON StratumTemplateLogFieldSetup_Tombstone
(StratumTemplateName, CruiseID, Field);
";

        public string CreateIndexes => 
@"CREATE INDEX NIX_StratumTemplateLogFileSetup_StratumTemplateName_CruiseID ON StratumTemplateLogFieldSetup
(StratumTemplateName, CruiseID);";

        public IEnumerable<string> CreateTriggers => new[] { CREATE_TRIGGER_StratumTemplateLogFieldSetup_OnUpdate, CREATE_TRIGGER_StratumTemplateLogFieldSetup_OnDelete, };

        public const string CREATE_TRIGGER_StratumTemplateLogFieldSetup_OnUpdate =
@"CREATE TRIGGER StratumTemplateLogFieldSetup_OnUpdate
AFTER UPDATE OF 
    FieldOrder
ON StratumTemplateLogFieldSetup
FOR EACH ROW
BEGIN
    UPDATE StratumTemplateLogFieldSetup SET Modified_TS = CURRENT_TIMESTAMP WHERE StratumTemplateLogFieldSetup_CN = OLD.StratumTemplateLogFieldSetup_CN;
END;
";
        public const string CREATE_TRIGGER_StratumTemplateLogFieldSetup_OnDelete =
@"CREATE TRIGGER StratumTemplateLogFieldSetup_OnDelete
BEFORE DELETE ON StratumTemplateLogFieldSetup
FOR EACH ROW
BEGIN
    INSERT OR REPLACE INTO StratumTemplateLogFieldSetup_Tombstone (
        StratumTemplateName,
        CruiseID,
        Field,
        FieldOrder,
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