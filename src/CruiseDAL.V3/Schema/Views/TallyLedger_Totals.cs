using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CruiseDAL.Schema
{
    public class TallyLedgerViewDefinition : IViewDefinition
    {
        public string ViewName => "TallyLedger_Totals";

        public string CreateView => CREATE_VIEW_TallyLedger_Totals + CREATE_VIEW_TallyLedger_Tree_Totals + CREATE_VIEW_TallyLedger_Plot_Totals;

        public const string CREATE_VIEW_TallyLedger_Totals =
@"CREATE VIEW TallyLedger_Totals AS
SELECT 
    tl.CruiseID,
    tl.StratumCode,
    tl.SampleGroupCode,
    tl.SpeciesCode,
    tl.LiveDead,
    sum(tl.TreeCount) AS TreeCount,
    sum(tl.KPI) AS KPI
FROM TallyLedger AS tl
GROUP BY 
    CruiseID, 
    CuttingUnitCode,
    StratumCode,
    SampleGroupCode,
    ifnull(SpeciesCode, ''),
    ifnull(LiveDead, '')
;";

        public const string CREATE_VIEW_TallyLedger_Plot_Totals =
@"CREATE VIEW TallyLedger_Plot_Totals AS
SELECT 
    tl.CruiseID,
    tl.StratumCode,
    tl.SampleGroupCode,
    tl.SpeciesCode,
    tl.LiveDead,
    tl.PlotNumber,
    sum(tl.TreeCount) AS TreeCount,
    sum(tl.KPI) AS KPI
FROM TallyLedger AS tl
GROUP BY 
    CruiseID,
    CuttingUnitCode,
    StratumCode,
    SampleGroupCode,
    ifnull(SpeciesCode, ''),
    ifnull(LiveDead, ''),
    ifnull(PlotNumber, -1)
;";

        public const string CREATE_VIEW_TallyLedger_Tree_Totals =
@"CREATE VIEW TallyLedger_Tree_Totals AS
SELECT 
    tl.CruiseID,
    tl.TreeID,
    sum(tl.STM) AS STM, 
    sum(tl.TreeCount) AS TreeCount,
    sum(tl.KPI) AS KPI
FROM TallyLedger AS tl
WHERE  TreeID IS NOT NULL
GROUP BY 
    CruiseID,
    TreeID
;";


    }
}
