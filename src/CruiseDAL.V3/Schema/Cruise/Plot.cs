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

    public partial class Migrations
    {
        public const string MIGRATE_PLOT_FROM_PLOT_FORMAT_STR =
            "INSERT INTO {0}.Plot ( " +
                    "Plot_CN, " +
                    "PlotID, " +
                    "PlotNumber, " +
                    "CruiseID, " +
                    "CuttingUnitCode, " +
                    "Slope, " +
                    "Aspect, " +
                    "Remarks, " +
                    "XCoordinate, " +
                    "YCoordinate, " +
                    "ZCoordinate, " +
                    "CreatedBy, " +
                    "CreatedDate, " +
                    "ModifiedBy, " +
                    "ModifiedDate, " +
                    "RowVersion " +
                ") " +
                "SELECT " +
                    "p.Plot_CN, " +
                    "ifnull( " +
                        "(CASE typeof(Plot_GUID) COLLATE NOCASE " + // ckeck the type of Plot_GUID
                            "WHEN 'TEXT' THEN " + // if text
                                "(CASE WHEN Plot_GUID LIKE '________-____-____-____-____________' " + // check to see if it is a properly formated guid
                                    "THEN nullif(Plot_GUID, '00000000-0000-0000-0000-000000000000') " + // if not a empty guid return that value otherwise return null for now
                                    "ELSE NULL END) " + // if it is not a properly formatted guid return Tree_GUID
                            "ELSE NULL END)" + // if value is not a string return null
                        ", (hex( randomblob(4)) || '-' || hex( randomblob(2)) " +
                            "|| '-' || '4' || substr(hex(randomblob(2)), 2) || '-' " +
                            "|| substr('AB89', 1 + (abs(random()) % 4), 1) || " +
                            "substr(hex(randomblob(2)), 2) || '-' || hex(randomblob(6)))) AS PlotID, " +
                    "p.PlotNumber, " +
                    "'{3}', " +
                    "cu.Code AS CuttingUnitCode, " +
                    "p.Slope, " +
                    "p.Aspect, " +
                    "group_concat(p.Remarks) AS Remarks, " +
                    "p.XCoordinate, " +
                    "p.YCoordinate, " +
                    "p.ZCoordinate, " +
                    "p.CreatedBy, " +
                    "p.CreatedDate, " +
                    "p.ModifiedBy, " +
                    "p.ModifiedDate, " +
                    "p.RowVersion " +
                "FROM {1}.Plot AS p " +
                "JOIN {1}.CuttingUnit AS cu USING (CuttingUnit_CN) " +
                "GROUP BY cu.Code, PlotNumber;";
    }

    //public partial class Updater
    //{
    //    public const string INITIALIZE_PLOT_V3_FROM_PLOT =
    //        "INSERT INTO Plot_V3 " +
    //        "SELECT " +
    //        "p.Plot_CN, " +
    //        "p.PlotNumber, " +
    //        "cu.Code AS CuttingUnitCode, " +
    //        "p.Slope, " +
    //        "p.Aspect, " +
    //        "p.Remarks, " +
    //        "p.XCoordinate, " +
    //        "p.YCoordinate, " +
    //        "p.ZCoordinate, " +
    //        "p.CreatedBy, " +
    //        "p.CreatedDate, " +
    //        "p.ModifiedBy, " +
    //        "p.ModifiedDate, " +
    //        "p.RowVersion " +
    //        "FROM Plot AS p " +
    //        "JOIN CuttingUnit AS cu USING (CuttingUnit_CN) " +
    //        "GROUP BY cu.Code, PlotNumber;";
    //}
}