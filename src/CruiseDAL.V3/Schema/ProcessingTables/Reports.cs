﻿using System.Collections.Generic;
using System.Linq;

namespace CruiseDAL.Schema
{
    public class ReportsTableDefinition : ITableDefinition
    {
        public string TableName => "Reports";

        public string CreateTable => GetCreateTable(TableName);


        public string GetCreateTable(string tableName)
        {
return $@"CREATE TABLE {tableName} ( 
    ReportID TEXT NOT NULL COLLATE NOCASE,
    CruiseID TEXT NOT NULL COLLATE NOCASE,
    Selected BOOLEAN Default 0,
    Title TEXT,
    CreatedBy TEXT DEFAULT 'none',
    Created_TS DATETIME DEFAULT (CURRENT_TIMESTAMP),
    ModifiedBy TEXT,
    Modified_TS DATETIME,

    FOREIGN KEY (CruiseID) REFERENCES Cruise (CruiseID) ON DELETE CASCADE,

    UNIQUE (ReportID, CruiseID)
);";
        }

        public string InitializeTable => null;

        public string CreateTombstoneTable =>
@"CREATE TABLE Reports_Tombstone (
    ReportID TEXT NOT NULL COLLATE NOCASE,
    CruiseID TEXT NOT NULL COLLATE NOCASE,
    Selected BOOLEAN,
    Title TEXT,

    CreatedBy TEXT,
    Created_TS DATETIME,
    ModifiedBy TEXT,
    Modified_TS DATETIME,
    Deleted_TS DATETIME
);

CREATE INDEX Reports_Tombstone_ReportID_CruiseID ON Reports_Tombstone
(ReportID, CruiseID);
";

        public string CreateIndexes => null;

        public IEnumerable<string> CreateTriggers => new[] { 
            CREATE_TRIGGER_Reports_OnDelete, 
            CREATE_TRIGGER_Reports_OnInsert_ClearTombstone 
        };

        public string CREATE_TRIGGER_Reports_OnDelete =
@"CREATE TRIGGER Reports_OnDelete 
BEFORE DELETE ON Reports 
FOR EACH ROW 
BEGIN 
    INSERT OR REPLACE INTO Reports_Tombstone (
        ReportID,
        CruiseID,
        Selected,
        Title,
        CreatedBy,
        Created_TS,
        ModifiedBy,
        Modified_TS,
        Deleted_TS
    ) VALUES (
        OLD.ReportID,
        OLD.CruiseID,
        OLD.Selected,
        OLD.Title,
        OLD.CreatedBy,
        OLD.Created_TS,
        OLD.ModifiedBy,
        OLD.Modified_TS,
        CURRENT_TIMESTAMP
    );
END;
";
        public const string CREATE_TRIGGER_Reports_OnInsert_ClearTombstone =
@"CREATE TRIGGER Reports_OnInsert_ClearTombstone
AFTER INSERT ON Reports
FOR EACH ROW
BEGIN
    DELETE FROM Reports_Tombstone 
        WHERE CruiseID = NEW.CruiseID
        AND ReportID = NEW.ReportID;
END;    
";
    }

}