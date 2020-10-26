using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CruiseDAL.Schema
{
    public class TreeLocationTableDefinition : ITableDefinition
    {
        public string TableName => "TreeLocation";

        public string CreateTable =>
@"CREATE TABLE TreeLocation (
    TreeLocation_CN INTEGER PRIMARY KEY AUTOINCREMENT,
    TreeID TEXT NOT NULL COLLATE NOCASE,
    Latitude REAL NOT NULL,
    Longitude REAL NOT NULL,
    SS_Latatude REAL, --side shot latatude
    SS_Longitude REAL, --side shot longitude
    Azimuth REAL,
    Distance REAL,
    IsEstimate BOOLEAN DEFAULT 0,
    CreatedBy TEXT DEFAULT 'none',
    Created_TS DATETIME DEFAULT (CURRENT_TIMESTAMP),
    ModifiedBy TEXT,
    Modified_TS DATETIME,

    CHECK  ((SS_Latatude IS NOT NULL AND SS_Longitude IS NOT NULL AND Azimuth IS NOT NULL AND Distance IS NOT NULL)
        OR (SS_Latatude IS NULL AND SS_Longitude IS NULL AND Azimuth IS NULL AND Distance IS NULL)),

    UNIQUE (TreeID),

    FOREIGN KEY (TreeID) REFERENCES Tree (TreeID) ON DELETE CASCADE
);";

        public string InitializeTable => null;

        public string CreateTombstoneTable =>
@"CREATE TABLE TreeLocation_Tombstone (
    TreeID TEXT NOT NULL COLLATE NOCASE,
    Latitude REAL NOT NULL,
    Longitude REAL NOT NULL,
    SS_Latatude REAL, --side shot latatude
    SS_Longitude REAL, --side shot longitude
    Azimuth REAL,
    Distance REAL,
    IsEstimate BOOLEAN,
    CreatedBy TEXT,
    Created_TS DATETIME,
    ModifiedBy TEXT,
    Modified_TS DATETIME,
    Deleted_TS DATETIME

);";

        public string CreateIndexes => null;

        public IEnumerable<string> CreateTriggers => new[] { CREATE_TRIGGER_TreeLocation_ONUPDATE, CREATE_TRIGGER_TreeLocation_OnDelete };

        public const string CREATE_TRIGGER_TreeLocation_ONUPDATE =
@"CREATE TRIGGER TreeLocation_OnUpdate 
AFTER UPDATE OF 
    Latitude,
    Longitude,
    SS_Latatude,
    SS_Longitude,
    Azimuth,
    Distance,
    IsEstimate
ON TreeLocation
FOR EACH ROW 
BEGIN 
    UPDATE TreeLocation SET Modified_TS = CURRENT_TIMESTAMP WHERE Tree_CN = old.Tree_CN; 
END; ";

        public const string CREATE_TRIGGER_TreeLocation_OnDelete =
@"CREATE TRIGGER TreeLocation_OnDelete
BEFORE DELETE ON TreeLocation
FOR EACH ROW
BEGIN
    INSERT OR REPLACE INTO TreeLocation_Tombstone (
        TreeID,
        Latitude,
        Longitude,
        SS_Latatude, --side shot latatude
        SS_Longitude, --side shot longitude
        Azimuth,
        Distance,
        IsEstimate,

        CreatedBy,
        Created_TS,
        ModifiedBy,
        Modified_TS,
        Deleted_TS
    ) VALUES (
        OLD.TreeID,
        OLD.Latitude,
        OLD.Longitude,
        OLD.SS_Latatude, --side shot latatude
        OLD.SS_Longitude, --side shot longitude
        OLD.Azimuth,
        OLD.Distance,
        OLD.IsEstimate,

        OLD.CreatedBy,
        OLD.Created_TS,
        OLD.ModifiedBy,
        OLD.Modified_TS,
        CURRENT_TIMESTAMP
    );
END;";

    }
}
