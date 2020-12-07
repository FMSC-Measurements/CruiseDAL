namespace CruiseDAL.Schema.V2Backports
{
    public class TreeDefaultValueTreeAuditValue_V2_ViewDefinition : IViewDefinition
    {
        public string ViewName => "TreeDefaultValueTreeAuditValue_V2";

        public string CreateView =>
@"CREATE VIEW TreeDefaultValueTreeAuditValue_V2 AS
SELECT
    tdv.TreeDefaultValue_CN,
    tar.TreeAuditRule_CN AS TreeAuditValue_CN
FROM TreeAuditRuleSelector tars
JOIN TreeDefaultValue AS tdv USING (SpeciesCode, LiveDead, PrimaryProduct)
JOIN TreeAuditRule AS tar USING (TreeAuditRuleID)";
    }
}