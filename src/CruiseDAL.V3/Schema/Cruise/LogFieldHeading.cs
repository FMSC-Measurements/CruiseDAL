using System.Collections.Generic;

namespace CruiseDAL.Schema
{
    public class LogFieldHeading : ITableDefinition
    {
        public string TableName => "LogFieldHeading";

        public string CreateTable =>
@"CREATE TABLE LogFieldHeading (
    LogFieldHeading_CN INTEGER PRIMARY KEY AUTOINCREMENT,
    CruiseID TEXT NOT NULL COLLATE NOCASE,
    Field TEXT NOT NULL COLLATE NOCASE,
    Heading TEXT NOT NULL COLLATE NOCASE,
    CreatedBy TEXT DEFAULT 'none',
    Created_TS DATETIME DEFAULT (CURRENT_TIMESTAMP),
    ModifiedBy TEXT,
    Modified_TS DATETIME,

    UNIQUE (CruiseID, Field),

    FOREIGN KEY (Field) REFERENCES LogField (Field) ON DELETE CASCADE,
    FOREIGN KEY (CruiseID) REFERENCES Cruise (CruiseID) ON DELETE CASCADE
);";

        public string InitializeTable => null;

        public string CreateTombstoneTable => null;

        public string CreateIndexes => null;

        public string Create_Trigger_LogFieldHeading_OnUpdate =
@"CREATE TRIGGER LogFieldHeading_OnUpdate
AFTER UPDATE OF
    Heading
ON LogFieldHeading
FOR EACH ROW
BEGIN
    UPDATE LogFieldHeading SET Modified_TS = CURRENT_TIMESTAMP WHERE LogFieldHeading_CN = OLD.LogFieldHeading_CN;
END;";

        public IEnumerable<string> CreateTriggers =>
            new[]
            {
                Create_Trigger_LogFieldHeading_OnUpdate,
            };
    }
}