using System.Collections.Generic;

namespace CruiseDAL.Schema
{
    public class TreeFieldSetupDefaultTableDefinition
        : ITableDefinition
    {
        public string TableName => "TreeFieldSetupDefault";

        public string CreateTable =>
@"CREATE TABLE TreeFieldSetupDefault(
    TreeFieldSetupDefault_CN INTEGER PRIMARY KEY AUTOINCREMENT,
    StratumDefaultID TEXT COLLATE NOCASE,
    SampleGroupDefaultID TEXT COLLATE NOCASE,
    Field TEXT NOT NULL COLLATE NOCASE,
    FieldOrder INTEGER Default 0,
    IsHidden BOOLEAN Default 0,
    IsLocked BOOLEAN Default 0,
    -- value type determined by TreeField.DbType
    DefaultValueInt INTEGER, 
    DefaultValueReal REAL,
    DefaultValueBool BOOLEAN,
    DefaultValueText TEXT,

    CHECK (StratumDefaultID NOTNULL OR SampleGroupDefaultID NOTNULL), 
    CHECK (NOT(StratumDefaultID NOTNULL AND SampleGroupDefaultID NOTNULL)),

    FOREIGN KEY (Field) REFERENCES TreeField (Field)
);";

        public string InitializeTable => null;

        public string CreateTombstoneTable => null;

        public string CreateIndexes =>
@"CREATE UNIQUE INDEX UNX_TreeFieldSetupDefault_StratumDefaultID_SampleGroupDefaultID_Field ON TreeFieldSetupDefault
(ifnull(StratumDefaultID, ''), ifnull(SampleGroupDefaultID, ''), Field);";

        public IEnumerable<string> CreateTriggers => null;
    }
}