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
    }
}
