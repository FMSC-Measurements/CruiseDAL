namespace CruiseDAL.Schema
{
    public class TallyLedger_TotalsViewDefinition : IViewDefinition
    {
        public string ViewName => "TallyLedger_Totals";

        public string CreateView => CREATE_VIEW_TallyLedger_Totals;

        public const string CREATE_VIEW_TallyLedger_Totals =
@"CREATE VIEW TallyLedger_Totals AS
SELECT
    tl.CruiseID,
    tl.CuttingUnitCode,
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
    }
}