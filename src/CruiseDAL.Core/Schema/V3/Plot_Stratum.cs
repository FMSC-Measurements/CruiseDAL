namespace CruiseDAL.Schema
{
    public partial class DDL
    {
        public static readonly string[] PLOT_STRATUM = new string[]
        {
            CREATE_TABLE_PLOT_STRATUM,
            CREATE_TRIGGER_PLOT_STRATUM_ONUPDATE
        };

        public const string CREATE_TABLE_PLOT_STRATUM =
            "CREATE TABLE Plot_Stratum (" +
                "Plot_Stratum_CN INTEGER PRIMARY KEY AUTOINCREMENT, " +
                "PlotNumber INTEGER NOT NULL, " +
                "CuttingUnitCode TEXT NOT NULL COLLATE NOCASE, " +
                "StratumCode TEXT NOT NULL COLLATE NOCASE, " +
                "IsEmpty BOOLEAN DEFAULT 0, " +
                "KPI REAL DEFAULT 0.0, " +
                "ThreePRandomValue INTEGER Default 0, " +
                "CreatedBy TEXT DEFAULT '', " +
                "CreatedDate DATETIME DEFAULT (datetime('now', 'localtime')), " +
                "ModifiedBy TEXT, " +
                "ModifiedDate DATETIME, " +
                "RowVersion INTEGER DEFAULT 0, " +

                "UNIQUE (PlotNumber, CuttingUnitCode, StratumCode), " +

                //"FOREIGN KEY (CuttingUnitCode) REFERENCES CuttingUnit (Code) ON DELETE CASCADE ON UPDATE CASCADE, " +
                "FOREIGN KEY (StratumCode) REFERENCES Stratum (Code) ON DELETE CASCADE ON UPDATE CASCADE, " +
                "FOREIGN KEY (PlotNumber, CuttingUnitCode) REFERENCES Plot_V3 (PlotNumber, CuttingUnitCode) ON DELETE CASCADE " +
            ");";

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
    }

    public partial class Migrations
    {
        public const string MIGRATE_PLOT_STRATUM_FROM_PLOT_FORMAT_STR =
            "INSERT INTO {0}.Plot_Stratum ( " +
                    "Plot_Stratum_CN, " +
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