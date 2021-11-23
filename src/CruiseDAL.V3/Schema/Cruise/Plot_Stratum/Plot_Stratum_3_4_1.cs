using System.Collections.Generic;

namespace CruiseDAL.Schema
{
    public class Plot_StratumTableDefinition_3_4_1 : ITableDefinition
    {
        public string TableName => "Plot_Stratum";

        public string CreateTable => GetCreateTable(TableName);

        public string GetCreateTable(string tableName)
        {
            return $@"CREATE TABLE {tableName} (
    Plot_Stratum_CN INTEGER PRIMARY KEY AUTOINCREMENT,
    PlotNumber INTEGER NOT NULL,
    CruiseID TEXT NOT NULL COLLATE NOCASE,
    CuttingUnitCode TEXT NOT NULL COLLATE NOCASE,
    StratumCode TEXT NOT NULL COLLATE NOCASE,
    IsEmpty BOOLEAN DEFAULT 0,
    CountOrMeasure TEXT COLLATE NOCASE,     -- for 3ppnt cruise methods but might be expanded to other methods
    TreeCount INTEGER Default 0,            -- for 3ppnt cruise method
    AverageHeight REAL Default 0.0,         -- for 3ppnt cruise method
    KPI REAL DEFAULT 0.0,                   -- for 3ppnt cruise method
    ThreePRandomValue INTEGER Default 0,    -- for 3ppnt cruise method
    CreatedBy TEXT DEFAULT 'none',
    Created_TS DATETIME DEFAULT (CURRENT_TIMESTAMP),
    ModifiedBy TEXT,
    Modified_TS DATETIME,

    UNIQUE (PlotNumber, CuttingUnitCode, StratumCode, CruiseID),
    CHECK (IsEmpty IN (0, 1)),
    CHECK (CountOrMeasure IN ('C', 'M') OR CountMeasure IS NULL),
    CHECK ((TreeCount IS 0 AND AverageHeight IS 0 AND KPI IS 0) OR (TreeCount > 0 AND AverageHeight > 0 AND KPI > 0)) 

    FOREIGN KEY (StratumCode,  CruiseID) REFERENCES Stratum (StratumCode, CruiseID) ON DELETE CASCADE ON UPDATE CASCADE,
    FOREIGN KEY (PlotNumber, CuttingUnitCode, CruiseID) REFERENCES Plot (PlotNumber, CuttingUnitCode, CruiseID) ON DELETE CASCADE ON UPDATE CASCADE
);";
        }

        public string InitializeTable => null;

        public string CreateTombstoneTable =>
@"CREATE TABLE Plot_Stratum_Tombstone (
    PlotNumber INTEGER NOT NULL,
    CruiseID TEXT NOT NULL COLLATE NOCASE,
    CuttingUnitCode TEXT NOT NULL COLLATE NOCASE,
    StratumCode TEXT NOT NULL COLLATE NOCASE,
    IsEmpty BOOLEAN,
    CountOrMeasure TEXT COLLATE NOCASE,
    TreeCount INTEGER Default 0,
    AverageHeight REAL Default 0.0,
    KPI REAL,
    ThreePRandomValue INTEGER,
    CreatedBy TEXT,
    Created_TS DATETIME,
    ModifiedBy TEXT,
    Modified_TS DATETIME,
    Deleted_TS DATETIME
);

CREATE INDEX NIX_Plot_Stratum_Tombstone_CruiseID_PlotNumber_CuttingUnitCode_StratumCode ON Plot_Stratum_Tombstone
(CruiseID, PlotNumber, CuttingUnitCode, StratumCode);";

        public string CreateIndexes =>
@"CREATE INDEX NIX_Plot_Stratum_StratumCode_CruiseID ON Plot_Stratum ('StratumCode', 'CruiseID');

CREATE INDEX NIX_Plot_Stratum_PlotNumber_CuttingUnitCode_CruiseID ON Plot_Stratum ('PlotNumber', 'CuttingUnitCode', 'CruiseID');";

        public IEnumerable<string> CreateTriggers => new[] { 
            CREATE_TRIGGER_PLOT_STRATUM_ONUPDATE, 
            CREATE_TRIGGER_Plot_Stratum_OnDelete, 
            CREATE_TRIGGER_Stratum_OnInsert_ClearTombstones 
        };

        public const string CREATE_TRIGGER_PLOT_STRATUM_ONUPDATE =
@"CREATE TRIGGER Plot_Stratum_OnUpdate
AFTER UPDATE OF
    CuttingUnitCode,
    PlotNumber,
    StratumCode,
    IsEmpty,
    CountOrMeasure,
    TreeCount,
    AverageHeight,
    KPI,
    ThreePRandomValue
ON Plot_Stratum
FOR EACH ROW
BEGIN
    UPDATE Plot_Stratum SET Modified_TS = CURRENT_TIMESTAMP WHERE Plot_Stratum_CN = old.Plot_Stratum_CN;
END;";

        public const string CREATE_TRIGGER_Plot_Stratum_OnDelete =
@"CREATE TRIGGER Plot_Stratum_OnDelete
BEFORE DELETE ON Plot_Stratum
FOR EACH ROW
BEGIN
    INSERT OR REPLACE INTO Plot_Stratum_Tombstone (
        PlotNumber,
        CruiseID,
        CuttingUnitCode,
        StratumCode,
        IsEmpty,
        CountOrMeasure,
        TreeCount,
        AverageHeight,
        KPI,
        ThreePRandomValue,
        CreatedBy,
        Created_TS,
        ModifiedBy,
        Modified_TS,
        Deleted_TS
    ) VALUES (
        OLD.PlotNumber,
        OLD.CruiseID,
        OLD.CuttingUnitCode,
        OLD.StratumCode,
        OLD.IsEmpty,
        OLD.CountOrMeasure,
        OLD.TreeCount,
        OLD.AverageHeight,
        OLD.KPI,
        OLD.ThreePRandomValue,
        OLD.CreatedBy,
        OLD.Created_TS,
        OLD.ModifiedBy,
        OLD.Modified_TS,
        CURRENT_TIMESTAMP
    );
END;";


        // the plot_stratum table is somewhat special. Although it does store some values.
        // the primary information stored by the plot_stratum table is whether or not a stratum is included in a plot or not
        // so the existance of a plot_stratum record or the lack of a plot_stratum record is mostly what we are concerned with,
        // and that 'state' should be binary. It shouldn't be possible for a plot_stratum record to exist whild a tombstone exists at the same time
        // we don't have a unique ID for the plot stratum table eithr. 
        // this trigger ensures that if a plot_stratum is re-added that an tombstone record is cleared

        public const string CREATE_TRIGGER_Stratum_OnInsert_ClearTombstones =
@"CREATE TRIGGER Plot_Stratum_OnInsert_ClearTombstones (
AFTER INSERT ON Plot_Stratum 
FOR EACH ROW
BEGIN 
    DELETE FROM Plot_Stratum_Tombstone WHERE 
        CruiseID = NEW.CruiseID 
        AND CuttingUnitCode = NEW.CuttingUnitCode 
        AND StratumCode = NEW.StratumCode 
        AND PlotNumber = NEW.PlotNumber;
END;";
    }
}