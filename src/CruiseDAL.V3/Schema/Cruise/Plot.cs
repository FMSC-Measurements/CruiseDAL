using System.Collections.Generic;

namespace CruiseDAL.Schema
{
    public class PlotTableDefinition : ITableDefinition
    {
        public string TableName => "Plot";

        public string CreateTable =>
@"CREATE TABLE Plot (
    Plot_CN INTEGER PRIMARY KEY AUTOINCREMENT,
    PlotID TEXT NOT NULL,
    PlotNumber INTEGER NOT NULL,
    CruiseID TEXT NOT NULL COLLATE NOCASE,
    CuttingUnitCode TEXT NOT NULL COLLATE NOCASE,
    Slope REAL Default 0.0,
    Aspect REAL Default 0.0,
    Remarks TEXT,
    CreatedBy TEXT DEFAULT '',
    CreatedDate DATETIME DEFAULT (datetime('now', 'localtime')),
    ModifiedBy TEXT COLLATE NOCASE,
    ModifiedDate DATETIME,
    RowVersion INTEGER DEFAULT 0,

    CHECK (PlotID LIKE '________-____-____-____-____________'),

    UNIQUE (PlotID),
    UNIQUE (PlotNumber, CuttingUnitCode, CruiseID),

    FOREIGN KEY (CuttingUnitCode, CruiseID) REFERENCES CuttingUnit (Code, CruiseID) ON DELETE CASCADE ON UPDATE CASCADE
);";

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
    CreatedDate DATETIME,
    ModifiedBy TEXT COLLATE NOCASE,
    ModifiedDate DATETIME,
    RowVersion INTEGER
);";

        public string CreateIndexes =>
@"CREATE INDEX Plot_CuttingUnitCode_CruiseID ON Plot (CuttingUnitCode, CruiseID);

CREATE INDEX Plot_PlotNumber_CruiseID ON Plot (PlotNumber, CruiseID);";

        public IEnumerable<string> CreateTriggers => new[] { CREATE_TRIGGER_PLOT_ONUPDATE, CREATE_TRIGGER_Plot_OnDelete };

        public const string CREATE_TRIGGER_PLOT_ONUPDATE =
@"CREATE TRIGGER Plot_OnUpdate
AFTER UPDATE OF
    CuttingUnitCode,
    Slope,
    Aspect,
    Remarks,
    XCoordinate,
    YCoordinate,
    ZCoordinate
ON Plot
FOR EACH ROW
BEGIN
    UPDATE Plot SET ModifiedDate = datetime('now', 'localtime') WHERE Plot_CN = old.Plot_CN;
    UPDATE Plot SET RowVersion = old.RowVersion WHERE Plot_CN = old.Plot_CN;
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
        CreatedDate,
        ModifiedBy,
        ModifiedDate,
        RowVersion
    ) VALUES (
        OLD.PlotID,
        OLD.PlotNumber,
        OLD.CruiseID,
        OLD.CuttingUnitCode,
        OLD.Slope,
        OLD.Aspect,
        OLD.Remarks,
        OLD.CreatedBy,
        OLD.CreatedDate,
        OLD.ModifiedBy,
        OLD.ModifiedDate,
        OLD.RowVersion
    );
END;";
    }
}