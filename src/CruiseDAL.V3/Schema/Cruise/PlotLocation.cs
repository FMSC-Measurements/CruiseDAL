using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CruiseDAL.Schema
{
    public partial class DDL
    {
        public const string CREATE_TABLE_PlotLocation =
@"CREATE TABLE PlotLocation (
    PlotLocation_CN INTEGER PRIMARY KEY AUTOINCREMENT,
    PlotID TEXT NOT NULL COLLATE NOCASE, 
    Latitude REAL NOT NULL,
    Longitude REAL NOT NULL,

    UNIQUE (PlotID),
    
    FOREIGN KEY (PlotID) REFERENCES Plot (PlotID) ON DELETE CASCADE
);";

        public const string CREATE_TRIGGER_PlotLocation_OnDelete =
@"CREATE TRIGGER PlotLocation_OnDelete 
BEFORE DELETE ON PlotLocation
FOR EACH ROW
BEGIN
    INSERT OR REPLACE INTO PlotLocation_Tombstone (
        PlotID,
        Latitude,
        Longitude
    ) VALUES (
        OLD.PlotID,
        OLD.Latitude,
        OLD.Longitude
    );
END;";

        public const string CREATE_TOMBSTONE_TABLE_PlotLocation_Tombstone =
@"CREATE TABLE PlotLocation_Tombstone (
    PlotID TEXT NOT NULL COLLATE NOCASE, 
    Latitude REAL NOT NULL,
    Longitude REAL NOT NULL
);";
    }
}
