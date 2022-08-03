using System.Collections.Generic;

namespace CruiseDAL.Schema
{
    // fixed missing types on InsuranceIndex and InsuranceCounter
    public class SamplerStateTableDefinition_3_5_5 : ITableDefinition
    {
        public string TableName => "SamplerState";

        public string CreateTable =>
@"CREATE TABLE SamplerState (
    SamplerState_CN INTEGER PRIMARY KEY AUTOINCREMENT,
    DeviceID TEXT NOT NULL COLLATE NOCASE,
    CruiseID TEXT NOT NULL COLLATE NOCASE,
    StratumCode TEXT NOT NULL COLLATE NOCASE,
    SampleGroupCode TEXT NOT NULL COLLATE NOCASE,
    SampleSelectorType TEXT COLLATE NOCASE, -- should not change after record creation
    BlockState TEXT,
    SystematicIndex INTEGER DEFAULT 0,      -- should not change after record creation
    Counter INTEGER DEFAULT 0,
    InsuranceIndex INTEGER DEFAULT -1,              -- should not change after record creation
    InsuranceCounter INTEGER DEFAULT -1,
    CreatedBy TEXT DEFAULT 'none',
    Created_TS DATETIME DEFAULT (CURRENT_TIMESTAMP),
    ModifiedBy TEXT,
    Modified_TS DATETIME,

    UNIQUE (CruiseID, DeviceID, StratumCode, SampleGroupCode),

    FOREIGN KEY (DeviceID, CruiseID) REFERENCES Device (DeviceID, CruiseID) ON DELETE CASCADE,
    FOREIGN KEY (StratumCode, SampleGroupCode, CruiseID) REFERENCES SampleGroup (StratumCode, SampleGroupCode, CruiseID) ON DELETE CASCADE ON UPDATE CASCADE
);";

        public string GetCreateTable(string tableName)
        {
            return $@"CREATE TABLE {tableName} (
    SamplerState_CN INTEGER PRIMARY KEY AUTOINCREMENT,
    DeviceID TEXT NOT NULL COLLATE NOCASE,
    CruiseID TEXT NOT NULL COLLATE NOCASE,
    StratumCode TEXT NOT NULL COLLATE NOCASE,
    SampleGroupCode TEXT NOT NULL COLLATE NOCASE,
    SampleSelectorType TEXT COLLATE NOCASE, -- should not change after record creation
    BlockState TEXT,
    SystematicIndex INTEGER DEFAULT 0,      -- should not change after record creation
    Counter INTEGER DEFAULT 0,
    InsuranceIndex DEFAULT -1,              -- should not change after record creation
    InsuranceCounter DEFAULT -1,
    CreatedBy TEXT DEFAULT 'none',
    Created_TS DATETIME DEFAULT (CURRENT_TIMESTAMP),
    ModifiedBy TEXT,
    Modified_TS DATETIME,

    UNIQUE (CruiseID, DeviceID, StratumCode, SampleGroupCode),

    FOREIGN KEY (DeviceID, CruiseID) REFERENCES Device (DeviceID, CruiseID) ON DELETE CASCADE,
    FOREIGN KEY (StratumCode, SampleGroupCode, CruiseID) REFERENCES SampleGroup (StratumCode, SampleGroupCode, CruiseID) ON DELETE CASCADE ON UPDATE CASCADE
);";
        }

        public string InitializeTable => null;

        public string CreateTombstoneTable => null;

        public string CreateIndexes => null;

        public IEnumerable<string> CreateTriggers => new[] { CREATE_TRIGGER_SAMPLERSTATE_ONUPDATE };

        public const string CREATE_TRIGGER_SAMPLERSTATE_ONUPDATE =
@"CREATE TRIGGER SamplerState_OnUpdate
AFTER UPDATE OF
    BlockState,
    Counter,
    InsuranceCounter
ON SamplerState
FOR EACH ROW
BEGIN
    UPDATE SamplerState SET Modified_TS = CURRENT_TIMESTAMP WHERE SamplerState_CN = old.SamplerState_CN;
END; ";
    }
}