using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CruiseDAL.Schema.V3
{
    public partial class DDL
    {
        public const string CREATE_TABLE_PLOT_V3 =
            "CREATE TABLE Plot_V3 ( " +
                "Plot_CN INTEGER PRIMARY KEY AUTOINCREMENT, " +
                "PlotNumber INTEGER NOT NULL, " +
                "CuttingUnitCode TEXT NOT NULL COLLATE NOCASE, " +
                "Slope REAL Default 0.0, " +
                "Aspect REAL Default 0.0, " +
                "Remarks TEXT, " +
                "XCoordinate REAL Default 0.0, " +
                "YCoordinate REAL Default 0.0, " +
                "ZCoordinate REAL Default 0.0, " +
                "CreatedBy TEXT DEFAULT '', " +
                "CreatedDate DATETIME (datetime('now', 'localtime')), " +
                "ModifiedBy TEXT COLLATE NOCASE, " +
                "ModifiedDate DATETIME, " +
                "RowVersion, " +
                "UNIQUE (CuttingUnitCode, PlotNumber)" +
            ");";

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

    public partial class Updater
    {
        public const string INITIALIZE_PLOT_V3_FROM_PLOT =
            "INSERT INTO Plot_V3 " +
            "SELECT " +
            "PlotNumber, " +
            "cu.Code AS CuttingUnitCode, " +
            "Slope, " +
            "Aspect, " +
            "Remarks, " +
            "XCoordinate, " +
            "YCoordinate, " +
            "ZCoordinate, " +
            "CreatedBy, " +
            "CreatedDate " +
            "FROM Plot AS p " +
            "JOIN CuttingUnit AS cu USING (CuttingUnit_CN) " +
            "GROUP BY cu.Code, PlotNumber;";
    }
}
