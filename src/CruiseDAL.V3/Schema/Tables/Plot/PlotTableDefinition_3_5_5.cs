using System.Collections.Generic;

namespace CruiseDAL.Schema
{
    //added checks to slope and aspect
    public class PlotTableDefinition_3_5_5 : ITableDefinition
    {
        public string TableName => "Plot";

        public string CreateTable => GetCreateTable(TableName);

        public string GetCreateTable(string tableName)
        {
            return $@"CREATE TABLE {tableName} (
    Plot_CN INTEGER PRIMARY KEY AUTOINCREMENT,
    PlotID TEXT NOT NULL,
    PlotNumber INTEGER NOT NULL,
    CruiseID TEXT NOT NULL COLLATE NOCASE,
    CuttingUnitCode TEXT NOT NULL COLLATE NOCASE,
    Slope REAL Default 0.0,
    Aspect REAL Default 0.0,
    Remarks TEXT,
    CreatedBy TEXT DEFAULT 'none',
    Created_TS DATETIME DEFAULT (CURRENT_TIMESTAMP),
    ModifiedBy TEXT,
    Modified_TS DATETIME,

    CHECK (PlotID LIKE '________-____-____-____-____________'),
    CHECK (PlotNumber > 0),
    CHECK (Slope IS NULL OR Slope BETWEEN 0.0 AND 200.0), -- value of 200 is about 64°, a somewhat arbitrary ceiling but thats what we're going with
    CHECK (Aspect IS NULL OR Aspect BETWEEN 0.0 AND 360.0),

    UNIQUE (PlotID),
    UNIQUE (PlotNumber, CuttingUnitCode, CruiseID),

    FOREIGN KEY (CuttingUnitCode, CruiseID) REFERENCES CuttingUnit (CuttingUnitCode, CruiseID) ON DELETE CASCADE ON UPDATE CASCADE
);";
        }

        public string InitializeTable => null;

        public string CreateTombstoneTable =>
@"CREATE TABLE Plot_Tombstone (
    PlotID TEXT NOT NULL,
    PlotNumber INTEGER NOT NULL,
    CruiseID TEXT NOT NULL COLLATE NOCASE,
    CuttingUnitCode TEXT NOT NULL COLLATE NOCASE,
    Slope REAL,
    Aspect REAL,
    Remarks TEXT,
    CreatedBy TEXT,
    Created_TS DATETIME,
    ModifiedBy TEXT,
    Modified_TS DATETIME,
    Deleted_TS DATETIME
);

CREATE INDEX NIX_Plot_Tombstone_PlotID ON Plot_Tombstone
(PlotID);

CREATE INDEX NIX_Plot_Tombstone_CruiseID_PlotNumber_CuttingUnitCode ON Plot_Tombstone
(CruiseID, PlotNumber, CuttingUnitCode);";

        public string CreateIndexes =>
@"CREATE INDEX NIX_Plot_CuttingUnitCode_CruiseID ON Plot (CuttingUnitCode, CruiseID);

CREATE INDEX NIX_Plot_PlotNumber_CruiseID ON Plot (PlotNumber, CruiseID);";

        public IEnumerable<string> CreateTriggers => new[] { CREATE_TRIGGER_PLOT_ONUPDATE, CREATE_TRIGGER_Plot_OnDelete };

        public const string CREATE_TRIGGER_PLOT_ONUPDATE =
@"CREATE TRIGGER Plot_OnUpdate
AFTER UPDATE OF
    CuttingUnitCode,
    Slope,
    Aspect,
    Remarks
ON Plot
FOR EACH ROW
BEGIN
    UPDATE Plot SET Modified_TS = CURRENT_TIMESTAMP WHERE Plot_CN = old.Plot_CN;
END;";

        public const string CREATE_TRIGGER_Plot_OnDelete =
@"CREATE TRIGGER Plot_OnDelete
BEFORE DELETE ON Plot
FOR EACH ROW
BEGIN
    INSERT OR REPLACE INTO Plot_Tombstone (
        PlotID,
        PlotNumber,
        CruiseID,
        CuttingUnitCode,
        Slope,
        Aspect,
        Remarks,
        CreatedBy,
        Created_TS,
        ModifiedBy,
        Modified_TS,
        Deleted_TS
    ) VALUES (
        OLD.PlotID,
        OLD.PlotNumber,
        OLD.CruiseID,
        OLD.CuttingUnitCode,
        OLD.Slope,
        OLD.Aspect,
        OLD.Remarks,
        OLD.CreatedBy,
        OLD.Created_TS,
        OLD.ModifiedBy,
        OLD.Modified_TS,
        CURRENT_TIMESTAMP
    );
END;";
    }
}