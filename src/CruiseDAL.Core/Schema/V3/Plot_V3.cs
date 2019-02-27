using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CruiseDAL.Schema.V3
{
    public class Plot_V3
    {
        public const string CREATE_TABLE_PLOT_V3 =
            "CREATE TABLE Plot_V3 (" +
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
                "UNIQUE (CuttingUnitCode, PlotNumber)" +
            ");";
    }
}
