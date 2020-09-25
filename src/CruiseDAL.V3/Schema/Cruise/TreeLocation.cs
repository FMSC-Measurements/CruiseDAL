using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CruiseDAL.Schema
{
    public partial class DDL
    {
        public const string CREATE_TABLE_TreeLocation =
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
    CreatedBy TEXT DEFAULT '',
    CreatedDate DateTime DEFAULT CURRENT_TIMESTAMP,
    ModifiedBy TEXT,
    ModifiedDate DateTime,

    CHECK  ((SS_Latatude IS NOT NULL AND SS_Longitude IS NOT NULL AND Azimuth IS NOT NULL AND Distance IS NOT NULL)
        OR (SS_Latatude IS NULL AND SS_Longitude IS NULL AND Azimuth IS NULL AND Distance IS NULL)),

    UNIQUE (TreeID),

    FOREIGN KEY (TreeID) REFERENCES Tree (TreeID) ON DELETE CASCADE
);";

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
    UPDATE TreeLocation SET ModifiedDate = CURRENT_TIMESTAMP WHERE Tree_CN = old.Tree_CN; 
END; ";

        public const string CREATE_TRIGGER_TreeLocation_OnDelete =
@"CREATE TRIGGER TreeLocation_OnDelete
BEFORE DELETE ON TreeLocation_OnDelete
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
        CreatedDate,
        ModifiedBy,
        ModifiedDate
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
        OLD.CreatedDate,
        OLD.ModifiedBy,
        OLD.ModifiedDate
    );
END;";

        public const string CREATE_TOMBSTONE_TABLE_TreeLocation_Tombstone =
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
    CreatedDate DateTime,
    ModifiedBy TEXT,
    ModifiedDate DateTime
);";
    }
}
