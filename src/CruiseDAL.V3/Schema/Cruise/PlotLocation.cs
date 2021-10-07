using System.Collections.Generic;

namespace CruiseDAL.Schema
{
    public class PlotLocationTableDefinition : ITableDefinition
    {
        public string TableName => "PlotLocation";

        public string CreateTable => GetCreateTable(TableName);

        public string GetCreateTable(string tableName)
        {
            return $@"CREATE TABLE {tableName} (
    PlotLocation_CN INTEGER PRIMARY KEY AUTOINCREMENT,
    PlotID TEXT NOT NULL COLLATE NOCASE,
    Latitude REAL NOT NULL,
    Longitude REAL NOT NULL,
    CreatedBy TEXT DEFAULT 'none',
    Created_TS DATETIME DEFAULT (CURRENT_TIMESTAMP),
    ModifiedBy TEXT,
    Modified_TS DATETIME,

    UNIQUE (PlotID),

    CHECK (Latitude BETWEEN -90.0 AND 90.0),
    CHECK (Longitude BETWEEN -180.0 AND 180.0),

    FOREIGN KEY (PlotID) REFERENCES Plot (PlotID) ON DELETE CASCADE
);";
        }

        public string InitializeTable => null;

        public string CreateTombstoneTable =>
@"CREATE TABLE PlotLocation_Tombstone (
    PlotID TEXT NOT NULL COLLATE NOCASE,
    Latitude REAL NOT NULL,
    Longitude REAL NOT NULL,
    CreatedBy TEXT,
    Created_TS DATETIME,
    ModifiedBy TEXT,
    Modified_TS DATETIME,
    Deleted_TS DATETIME
);";

        public string CreateIndexes => null;

        public IEnumerable<string> CreateTriggers => new[]
        {
            CREATE_TRIGGER_PlotLocation_OnUpdate,
            CREATE_TRIGGER_PlotLocation_OnDelete
        };

        public const string CREATE_TRIGGER_PlotLocation_OnUpdate =
@"CREATE TRIGGER PlotLocation_OnUpdate
AFTER UPDATE OF
    PlotID,
    Latitude,
    Longitude
ON PlotLocation
FOR EACH ROW
BEGIN
    UPDATE PlotLocation SET Modified_TS = CURRENT_TIMESTAMP WHERE PlotLocation_CN = old.PlotLocation_CN;
END;";

        public const string CREATE_TRIGGER_PlotLocation_OnDelete =
@"CREATE TRIGGER PlotLocation_OnDelete
BEFORE DELETE ON PlotLocation
FOR EACH ROW
BEGIN
    INSERT OR REPLACE INTO PlotLocation_Tombstone (
        PlotID,
        Latitude,
        Longitude,
        CreatedBy,
        Created_TS,
        ModifiedBy,
        Modified_TS,
        Deleted_TS
    ) VALUES (
        OLD.PlotID,
        OLD.Latitude,
        OLD.Longitude,
        OLD.CreatedBy,
        OLD.Created_TS,
        OLD.ModifiedBy,
        OLD.Modified_TS,
        CURRENT_TIMESTAMP
    );
END;";
    }
}