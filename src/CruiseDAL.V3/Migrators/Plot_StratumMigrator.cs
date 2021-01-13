using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CruiseDAL.Migrators
{
    public class Plot_StratumMigrator : IMigrator
    {
        public string MigrateToV3(string toDbName, string fromDbName, string cruiseID, string saleID)
        {
            return
$@"INSERT INTO {toDbName}.Plot_Stratum (
        Plot_Stratum_CN,
        CruiseID,
        CuttingUnitCode,
        PlotNumber,
        StratumCode,
        IsEmpty,
        KPI,
        ThreePRandomValue,
        CreatedBy,
        Created_TS,
        ModifiedBy,
        Modified_TS
    )
    SELECT
        p.Plot_CN AS Plot_Stratum_CN,
        '{cruiseID}',
        cu.Code AS CuttingUnitCode,
        p.PlotNumber,
        st.Code AS StratumCode,
        (CASE p.IsEmpty WHEN 'True' THEN 1 ELSE 0 END) AS IsEmpty,
        p.KPI,
        p.ThreePRandomValue,
        p.CreatedBy,
        p.CreatedDate,
        p.ModifiedBy,
        p.ModifiedDate
    FROM {fromDbName}.Plot AS p
    JOIN {fromDbName}.CuttingUnit AS cu USING (CuttingUnit_CN)
    JOIN {fromDbName}.Stratum AS st USING (Stratum_CN);";
        }
    }
}
