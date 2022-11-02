using System;
using System.Collections.Generic;

namespace CruiseDAL.Schema.Tables.CrusieLog
{
    public class CruiseLogTableDefinition_3_6_0 : ITableDefinition
    {
        public string TableName => "CruiseLog";

        public string CreateTable => GetCreateTable(TableName);

        public string InitializeTable => null;

        public string CreateTombstoneTable => null;

        public string CreateIndexes =>
@"CREATE INDEX NIX_CruiseLog_CruiseID ON CruiseLog (CruiseID);
CREATE INDEX NIX_CruiseLog_TimeStamp ON CruiseLog (TimeStamp);";

        public IEnumerable<string> CreateTriggers => null;

        public string GetCreateTable(string tableName)
        {
            return $@"CREATE TABLE {tableName} (
    CruiseLog_CN INTEGER PRIMARY KEY AUTOINCREMENT,
    CruiseLogID TEXT NOT NULL COLLATE NOCASE DEFAULT (hex( randomblob(4)) || '-' || hex( randomblob(2)) 
                                                        || '-' || '4' || substr(hex(randomblob(2)), 2) || '-' 
                                                        || substr('AB89', 1 + (abs(random()) % 4), 1) || 
                                                        substr(hex(randomblob(2)), 2) || '-' || hex(randomblob(6))),
    CruiseID TEXT NOT NULL COLLATE NOCASE,
    CuttingUnitID TEXT COLLATE NOCASE,
    StratumID TEXT COLLATE NOCASE,
    SampleGroupID TEXT COLLATE NOCASE,
    PlotID TEXT COLLATE NOCASE,
    TallyLedgerID TEXT COLLATE NOCASE,
    TreeID  TEXT COLLATE NOCASE,
    LogID  TEXT COLLATE NOCASE,

    Message TEXT,

    TableName TEXT COLLATE NOCASE,
    Field TEXT COLLATE NOCASE,

    Program TEXT COLLATE NOCASE,
    TimeStamp DATETIME NOT NULL DEFAULT (CURRENT_TIMESTAMP),
    Level TEXT COLLATE NOCASE DEFAULT 'N',

    UNIQUE (CruiseLogID), 

    FOREIGN KEY (CruiseID) REFERENCES Cruise (CruiseID) ON DELETE CASCADE,

    CHECK (CruiseLogID LIKE '________-____-____-____-____________')
);";
        }
    }
}