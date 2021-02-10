using FMSC.ORM.Core;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;

namespace CruiseDAL.V3.Sync
{
    public class CruiseAnalizer
    {
        public static DateTime GetUnitNonPlotTreeModified(DbConnection db, string cruiseID, string unitCode)
        {
            return db.ExecuteScalar<DateTime>(
@" SELECT max(mod) FROM (
        SELECT max(Created_TS) AS mod FROM TallyLedger WHERE CruiseID = @p1 AND CuttingUnitCode = @p2 AND PlotNumber IS NULL
        UNION ALL 
        SELECT max( max(Created_TS), max(Modified_TS)) AS mod FROM Tree WHERE CruiseID = @p1 AND CuttingUnitCode = @p2 AND PlotNumber IS NULL 
        UNION ALL 
        SELECT max( max(tm.Created_TS), max(tm.Modified_TS)) AS mod FROM TreeMeasurment AS tm JOIN Tree AS t USING (TreeID) WHERE t.CruiseID = @p1 AND t.CuttingUnitCode = @p2 AND t.PlotNumber IS NULL
        SELECT max( max(tfv.Created_TS), max(tfv.Modified_TS)) AS mod FROM TreeFieldValue AS tfv JOIN Tree AS t USING (TreeID) WHERE t.CruiseID = @p1 AND t.CuttingUnitCode = @p2 AND t.PlotNumber IS NULL
);", parameters: new[] { cruiseID, unitCode });
        }

        public static DateTime GetPlotTreeModified(DbConnection db, string cruiseID, string unitCode, int plotNumber)
        {
            return db.ExecuteScalar<DateTime>("SELECT  max( max(Created_TS), max(Modified_TS)) FROM Tree WHERE CruiseID = @p1 CuttingUnitCode = @p2 AND PlotNumber = @p3",
                        new object[] { cruiseID, unitCode, plotNumber });
        }

        public static DateTime? GetTreeDataModified(DbConnection db, string treeID)
        {
            return db.ExecuteScalar<DateTime>(
@" SELECT max(mod) FROM (
        SELECT max( max(tm.Created_TS), max(tm.Modified_TS)) AS mod FROM TreeMeasurment AS tm JOIN Tree AS t USING (TreeID) WHERE t.TreeID = @p1
        UNION ALL
        SELECT max( max(tfv.Created_TS), max(tfv.Modified_TS)) AS mod FROM TreeFieldValue AS tfv JOIN Tree AS t USING (TreeID) WHERE t.TreeID = @p1
);", parameters: new[] { treeID });
        }
    }
}
