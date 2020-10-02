using System.Collections.Generic;

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

    public partial class Migrations
    {
        public const string MIGRATE_PLOT_STRATUM_FROM_PLOT_FORMAT_STR =
            "INSERT INTO {0}.Plot_Stratum ( " +
                    "Plot_Stratum_CN, " +
                    "CruiseID, " +
                    "CuttingUnitCode, " +
                    "PlotNumber, " +
                    "StratumCode, " +
                    "IsEmpty, " +
                    "KPI, " +
                    "ThreePRandomValue, " +
                    "CreatedBy, " +
                    "CreatedDate, " +
                    "ModifiedBy, " +
                    "ModifiedDate, " +
                    "RowVersion " +
                ") " +
                "SELECT " +
                    "p.Plot_CN AS Plot_Stratum_CN, " +
                    "'{3}', " +
                    "cu.Code AS CuttingUnitCode, " +
                    "p.PlotNumber, " +
                    "st.Code AS StratumCode, " +
                    "(CASE p.IsEmpty WHEN 'True' THEN 1 ELSE 0 END) AS IsEmpty, " +
                    "p.KPI, " +
                    "p.ThreePRandomValue, " +
                    "p.CreatedBy, " +
                    "p.CreatedDate, " +
                    "p.ModifiedBy, " +
                    "p.ModifiedDate, " +
                    "p.RowVersion " +
                "FROM {1}.Plot AS p " +
                "JOIN {1}.CuttingUnit AS cu USING (CuttingUnit_CN) " +
                "JOIN {1}.Stratum AS st USING (Stratum_CN);";
    }

    //public partial class Updater
    //{
    //    public const string INITIALIZE_PLOT_STRATUM_FROM_PLOT =
    //        "INSERT INTO Plot_Stratum " +
    //        "SELECT " +
    //        "null AS Plot_Stratum_CN," +
    //        "cu.Code AS CuttingUnitCode, " +
    //        "p.PlotNumber, " +
    //        "st.Code AS StratumCode, " +
    //        "(CASE p.IsEmpty WHEN 'True' THEN 1 ELSE 0 END) AS IsEmpty, " +
    //        "p.KPI, " +
    //        "p.ThreePRandomValue, " +
    //        "p.CreatedBy, " +
    //        "p.CreatedDate, " +
    //        "p.ModifiedBy, " +
    //        "p.ModifiedDate, " +
    //        "p.RowVersion " +
    //        "FROM Plot AS p " +
    //        "JOIN CuttingUnit AS cu USING (CuttingUnit_CN) " +
    //        "JOIN Stratum AS st USING (Stratum_CN);";
    //}
}