using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CruiseDAL.Schema
{
    public partial class DDL
    {
        public const string CREATE_VIEW_TallyLedger_Totals =
@"CREATE VIEW TallyLedger_Totals AS
SELECT 
    tl.StratumCode,
    tl.SampleGroupCode,
    tl.Species,
    tl.LiveDead,
    sum(tl.TreeCount) AS TreeCount,
    sum(tl.KPI) AS KPI
FROM TallyLedger AS tl
WHERE IsDeleted = 0
GROUP BY 
    CuttingUnitCode,
    StratumCode,
    SampleGroupCode,
    ifnull(Species, ''),
    ifnull(LiveDead, '');";

        public const string CREATE_VIEW_TallyLedger_Plot_Totals =
@"CREATE VIEW TallyLedger_Plot_Totals AS
SELECT 
    tl.StratumCode,
    tl.SampleGroupCode,
    tl.Species,
    tl.LiveDead,
    tl.PlotNumber,
    sum(tl.TreeCount) AS TreeCount,
    sum(tl.KPI) AS KPI
FROM TallyLedger AS tl
WHERE IsDeleted = 0
GROUP BY 
    CuttingUnitCode,
    StratumCode,
    SampleGroupCode,
    ifnull(Species, ''),
    ifnull(LiveDead, ''),
    ifnull(PlotNumber, -1);";

        public const string CREATE_VIEW_TallyLedger_Tree_Totals =
@"CREATE VIEW TallyLedger_Tree_Totals AS
SELECT 
    tl.TreeID,
    sum(tl.STM) AS STM, 
    sum(tl.TreeCount) AS TreeCount,
    sum(tl.KPI) AS KPI
FROM TallyLedger AS tl
WHERE IsDeleted = 0
    AND TreeID IS NOT NULL
GROUP BY 
    TreeID;";
    }
}
