﻿using System.Collections.Generic;

namespace CruiseDAL.Schema
{
    public class Plot_StratumTableDefinition : ITableDefinition
    {
        public string TableName => "Plot_Stratum";

        public string CreateTable =>
@"CREATE TABLE Plot_Stratum (
    Plot_Stratum_CN INTEGER PRIMARY KEY AUTOINCREMENT,
    PlotNumber INTEGER NOT NULL,
    CruiseID TEXT NOT NULL COLLATE NOCASE,
    CuttingUnitCode TEXT NOT NULL COLLATE NOCASE,
    StratumCode TEXT NOT NULL COLLATE NOCASE,
    IsEmpty BOOLEAN DEFAULT 0,
    KPI REAL DEFAULT 0.0,
    ThreePRandomValue INTEGER Default 0,
    CreatedBy TEXT DEFAULT '',
    CreatedDate DATETIME DEFAULT (datetime('now', 'localtime')),
    ModifiedBy TEXT,
    ModifiedDate DATETIME,
    RowVersion INTEGER DEFAULT 0,

    UNIQUE (PlotNumber, CuttingUnitCode, StratumCode, CruiseID),

    FOREIGN KEY (StratumCode,  CruiseID) REFERENCES Stratum (StratumCode, CruiseID) ON DELETE CASCADE ON UPDATE CASCADE,
    FOREIGN KEY (PlotNumber, CuttingUnitCode, CruiseID) REFERENCES Plot (PlotNumber, CuttingUnitCode, CruiseID) ON DELETE CASCADE ON UPDATE CASCADE
);";

        public string InitializeTable => null;

        public string CreateTombstoneTable =>
@"CREATE TABLE Plot_Stratum_Tombstone (
    PlotNumber INTEGER NOT NULL,
    CruiseID TEXT NOT NULL COLLATE NOCASE,
    CuttingUnitCode TEXT NOT NULL COLLATE NOCASE,
    StratumCode TEXT NOT NULL COLLATE NOCASE,
    IsEmpty BOOLEAN,
    KPI REAL,
    ThreePRandomValue INTEGER,
    CreatedBy TEXT,
    CreatedDate DATETIME,
    ModifiedBy TEXT,
    ModifiedDate DATETIME,
    RowVersion INTEGER DEFAULT 0,

    UNIQUE (PlotNumber, CuttingUnitCode, StratumCode, CruiseID)
);";

        public string CreateIndexes =>
@"CREATE INDEX 'Plot_Stratum_StratumCode_CruiseID' ON 'Plot_Stratum'('StratumCode', 'CruiseID');

CREATE INDEX 'Plot_Stratum_PlotNumber_CuttingUnitCode_CruiseID' ON 'Plot_Stratum' ('PlotNumber', 'CuttingUnitCode', 'CruiseID');";

        public IEnumerable<string> CreateTriggers => new[] { CREATE_TRIGGER_PLOT_STRATUM_ONUPDATE, CREATE_TRIGGER_Plot_Stratum_OnDelete };

        public const string CREATE_TRIGGER_PLOT_STRATUM_ONUPDATE =
            "CREATE TRIGGER Plot_Stratum_OnUpdate " +
            "AFTER UPDATE OF " +
                "CuttingUnitCode, " +
                "PlotNumber, " +
                "StratumCode, " +
                "IsEmpty, " +
                "KPI, " +
                "ThreePRandomValue " +
            "ON Plot_Stratum " +
            "FOR EACH ROW " +
            "BEGIN " +
                "UPDATE Plot_Stratum SET ModifiedDate = datetime('now', 'localtime') WHERE Plot_Stratum_CN = old.Plot_Stratum_CN; " +
                "UPDATE Plot_Stratum SET RowVersion = old.RowVersion + 1 WHERE Plot_Stratum_CN = old.Plot_Stratum_CN; " +
            "END;";

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
        KPI,
        ThreePRandomValue,
        CreatedBy,
        CreatedDate,
        ModifiedBy,
        ModifiedDate,
        RowVersion
    ) VALUES (
        OLD.PlotNumber,
        OLD.CruiseID,
        OLD.CuttingUnitCode,
        OLD.StratumCode,
        OLD.IsEmpty,
        OLD.KPI,
        OLD.ThreePRandomValue,
        OLD.CreatedBy,
        OLD.CreatedDate,
        OLD.ModifiedBy,
        OLD.ModifiedDate,
        OLD.RowVersion
    );
END;";
    }
}