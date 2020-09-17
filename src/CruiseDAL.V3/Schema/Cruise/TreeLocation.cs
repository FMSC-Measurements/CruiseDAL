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
    }
}
