using System.Collections.Generic;
using System.Linq;

namespace CruiseDAL.Schema
{
    public class ReportsTableDefinition : ITableDefinition
    {
        public string TableName => "Reports";

        public string CreateTable =>
@"CREATE TABLE Reports ( 
    ReportID TEXT NOT NULL COLLATE NOCASE,
    CruiseID TEXT NOT NULL COLLATE NOCASE,
    Selected BOOLEAN Default 0,
    Title TEXT,
    CreatedBy TEXT DEFAULT 'none',
    Created_TS DATETIME DEFAULT (CURRENT_TIMESTAMP),
    ModifiedBy TEXT,
    Modified_TS DATETIME,

    FOREIGN KEY (CruiseID) REFERENCES Cruise (CruiseID) ON DELETE CASCADE,

    UNIQUE (ReportID)
);";

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

CREATE INDEX Reports_Tombstone_ReportID ON Reports_Tombstone
(ReportID);
";

        public string CreateIndexes => null;

        public IEnumerable<string> CreateTriggers => new[] { CREATE_TRIGGER_Reports_OnDelete };

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
    }

}