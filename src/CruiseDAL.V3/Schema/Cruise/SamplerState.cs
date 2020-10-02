using System.Collections.Generic;

namespace CruiseDAL.Schema
{
    public class SamplerStateTableDefinition : ITableDefinition
    {
        public string TableName => "SamplerState";

        public string CreateTable =>
@"CREATE TABLE SamplerState (
    SamplerState_CN INTEGER PRIMARY KEY AUTOINCREMENT,
    DeviceID TEXT NOT NULL COLLATE NOCASE,
    CruiseID TEXT NOT NULL COLLATE NOCASE,
    StratumCode TEXT NOT NULL COLLATE NOCASE,
    SampleGroupCode TEXT NOT NULL COLLATE NOCASE,
    SampleSelectorType TEXT COLLATE NOCASE,
    BlockState TEXT,
    SystematicIndex INTEGER DEFAULT 0,
    Counter INTEGER DEFAULT 0,
    InsuranceIndex DEFAULT -1,
    InsuranceCounter DEFAULT -1,
    ModifiedDate DateTime,

    UNIQUE (CruiseID, DeviceID, StratumCode, SampleGroupCode),

    FOREIGN KEY (DeviceID) REFERENCES Device (DeviceID) ON DELETE CASCADE,
    FOREIGN KEY (StratumCode, SampleGroupCode, CruiseID) REFERENCES SampleGroup (StratumCode, SampleGroupCode, CruiseID) ON DELETE CASCADE ON UPDATE CASCADE
);";

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
    UPDATE SamplerState SET ModifiedDate = datetime('now', 'localtime') WHERE SamplerState_CN = old.SamplerState_CN;
END; ";

    }
}