using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CruiseDAL.Schema.V3
{
    public partial class DDL
    {
        public const string CREATE_TABLE_PLOT_STRATUM =
            "CREATE TABLE Plot_Stratum (" +
                "Plot_Stratum_CN INTEGER PRIMARY KEY AUTOINCREMENT, " +
                "CuttingUnitCode TEXT NOT NULL COLLATE NOCASE, " +
                "PlotNumber INTEGER NOT NULL, " +
                "StratumCode TEXT NOT NULL COLLATE NOCASE, " +
                "IsEmpty BOOLEAN DEFAULT 0, " +
                "KPI REAL DEFAULT 0.0, " +
                "ThreePRandomValue INTEGER Default 0, " +
                "CreatedBy TEXT DEFAULT '', " +
                "CreatedDate DATETIME, " +
                "ModifiedBy TEXT DEFAULT '', " +
                "ModifiedDate DATETIME, " +
                "RowVersion INTEGER DEFAULT 0, " +
                "UNIQUE (CuttingUnitCode, PlotNumber, StratumCode), " +
                "FOREIGN KEY (CuttingUnitCode) REFERENCES CuttingUnit (Code), " +
                "FOREIGN KEY (PlotNumber, CuttingUnitCode) REFERENCES Plot_V3 (PlotNumber, CuttingUnitCode) ON DELETE CASCADE ON UPDATE CASCADE" +
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

    public partial class Updater
    {
        public const string INITIALIZE_PLOT_STRATUM_FROM_PLOT =
            "INSERT INTO Plot_Stratum " +
            "SELECT " +
            "cu.Code AS CuttingUnitCode, " +
            "PlotNumber, " +
            "st.Code AS StratumCode, " +
            "(CASE IsEmpty WHEN 'True' THEN 1 ELSE 0) AS IsEmpty, " +
            "KPI, " +
            "ThreePRandomValue, " +
            "CreatedBy, " +
            "CreatedDate, " +
            "ModifiedBy, " +
            "ModifiedDate, " +
            "RowVersion " +
            "FROM Plot " +
            "JOIN CuttingUnit AS cu USING (CuttingUnit_CN) " +
            "JOIN Stratum AS st USING (Stratum_CN);";
    }
}
