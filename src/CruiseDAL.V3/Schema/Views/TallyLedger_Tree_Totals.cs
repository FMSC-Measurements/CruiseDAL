namespace CruiseDAL.Schema
{
    public class TallyLedger_Tree_TotalsViewDefinition : IViewDefinition
    {
        public string ViewName => "TallyLedger_Tree_Totals";

        public string CreateView =>
@"CREATE VIEW TallyLedger_Tree_Totals AS
SELECT
    tl.CruiseID,
    tl.TreeID,
    max(tl.STM) AS STM,
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