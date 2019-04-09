namespace CruiseDAL.Schema
{
    public partial class DDL
    {
        public const string CREATE_TABLE_PLOT_V3 =
            "CREATE TABLE Plot_V3 ( " +
                "Plot_CN INTEGER PRIMARY KEY AUTOINCREMENT, " +
                "PlotID TEXT NOT NULL, " +
                "PlotNumber INTEGER NOT NULL, " +
                "CuttingUnitCode TEXT NOT NULL COLLATE NOCASE, " +
                "Slope REAL Default 0.0, " +
                "Aspect REAL Default 0.0, " +
                "Remarks TEXT, " +
                "XCoordinate REAL Default 0.0, " +
                "YCoordinate REAL Default 0.0, " +
                "ZCoordinate REAL Default 0.0, " +
                "CreatedBy TEXT DEFAULT '', " +
                "CreatedDate DATETIME DEFAULT (datetime('now', 'localtime')), " +
                "ModifiedBy TEXT COLLATE NOCASE, " +
                "ModifiedDate DATETIME, " +
                "RowVersion, " +

                "UNIQUE (PlotID), " +
                "UNIQUE (PlotNumber, CuttingUnitCode)," +

                "FOREIGN KEY (CuttingUnitCode) REFERENCES CuttingUnit (Code) ON DELETE CASCADE ON UPDATE CASCADE " +
            ");";

        public const string CREATE_INDEX_Plot_V3_CuttingUnitCode =
            @"CREATE INDEX 'Plot_V3_CuttingUnitCode' ON 'Plot_V3'('CuttingUnitCode');";

        public const string CREATE_TRIGGER_PLOT_V3_ONUPDATE =
            "CREATE TRIGGER Plot_V3_OnUpdate " +
            "AFTER UPDATE OF " +
                "CuttingUnitCode, " +
                "Slope, " +
                "Aspect, " +
                "Remarks, " +
                "XCoordinate, " +
                "YCoordinate, " +
                "ZCoordinate " +
            "ON Plot_V3 " +
            "FOR EACH ROW " +
            "BEGIN " +
                "UPDATE Plot_V3 SET ModifiedDate = datetime('now', 'localtime') WHERE Plot_CN = old.Plot_CN; " +
                "UPDATE Plot_V3 SET RowVersion = old.RowVersion WHERE Plot_CN = old.Plot_CN; " +
            "END;";
    }

    public partial class Migrations
    {
        public const string MIGRATE_PLOT_V3_FROM_PLOT_FORMAT_STR =
            "INSERT INTO {0}.Plot_V3 ( " +
                    "Plot_CN, " +
                    "PlotID, " +
                    "PlotNumber, " +
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
                    "ifnull(Plot_GUID, 'migrateFormPlot-' || p.Plot_CN) AS PlotID, " +
                    "p.PlotNumber, " +
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