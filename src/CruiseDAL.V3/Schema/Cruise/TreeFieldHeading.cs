using System;
using System.Collections.Generic;

namespace CruiseDAL.Schema
{
    public class TreeFieldHeading : ITableDefinition
    {
        public string TableName => "TreeFieldHeading";

        public string CreateTable =>
@"CREATE TABLE TreeFieldHeading (
    TreeFieldHeading_CN INTEGER PRIMARY KEY AUTOINCREMENT,
    CruiseID TEXT NOT NULL COLLATE NOCASE,
    Field TEXT NOT NULL COLLATE NOCASE,
    Heading TEXT NOT NULL, 
    CreatedBy TEXT DEFAULT 'none',
    Created_TS DATETIME DEFAULT (CURRENT_TIMESTAMP),
    ModifiedBy TEXT,
    Modified_TS DATETIME,

    UNIQUE (CruiseID, Field),

    FOREIGN KEY (CruiseID) REFERENCES Cruise (CruiseID) ON DELETE CASCADE,
    FOREIGN KEY (Field) REFERENCES TreeField (Field) ON DELETE CASCADE
);";

        public string InitializeTable => null;

        public string CreateTombstoneTable => null;

        public string CreateIndexes => null;

        public IEnumerable<string> CreateTriggers => new[] { CREATE_TRIGGER_TreeFieldHeading_OnUpdate };

        public string CREATE_TRIGGER_TreeFieldHeading_OnUpdate =
@"CREATE TRIGGER TreeFieldHeading_OnUpdate 
AFTER UPDATE OF
    Heading
ON TreeFieldHeading 
FOR EACH ROW
BEGIN
    UPDATE TreeFieldHeading SET Modified_TS = CURRENT_TIMESTAMP WHERE TreeFieldHeading_CN = new.TreeFieldHeading_CN;
END; ";
    }
}
