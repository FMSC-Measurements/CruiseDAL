namespace CruiseDAL.Schema
{
    public class TallyLedger_Plot_TotalsViewDefinition : IViewDefinition
    {
        public string ViewName => "TallyLedger_Plot_Totals";

        public string CreateView =>
@"CREATE VIEW TallyLedger_Plot_Totals AS
SELECT
    tl.CruiseID,
    tl.CuttingUnitCode,
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
    }
}