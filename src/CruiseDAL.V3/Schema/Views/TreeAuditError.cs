namespace CruiseDAL.Schema
{
    public class TreeAuditErrorViewDefinition : IViewDefinition
    {
        public string ViewName => "TreeAuditError";

        public string CreateView => CREATE_VIEW_3_5_1;

        public const string CREATE_VIEW_3_5_1 =
@"CREATE VIEW TreeAuditError AS
WITH
-- return just measure trees
measureTrees AS (
    SELECT
        t.Tree_CN,
        t.CruiseID,
        t.TreeID,
        t.StratumCode,
        t.SpeciesCode,
        t.LiveDead,
        sg.PrimaryProduct
    FROM Tree AS t
    JOIN SampleGroup AS sg USING (SampleGroupCode, StratumCode, CruiseID)
    WHERE t.CountOrMeasure = 'M'),

-- expands TreeAuditRuleSelector to include Field, Min, Max
treeAuditRuleSelector_Epanded AS (
    SELECT
        tar.CruiseID,
        SpeciesCode,
        LiveDead,
        PrimaryProduct,
        tar.Field,
        Min,
        Max,
        tar.TreeAuditRuleID
    FROM TreeAuditRuleSelector AS tars
    JOIN TreeAuditRule AS tar USING (TreeAuditRuleID))

SELECT
    Tree_CN,
    t.CruiseID,
    TreeID,
    TreeAuditRuleID,
    tar.Field AS Field,
    (CASE WHEN res.TreeAuditResolution_CN IS NULL THEN 0 ELSE 1 END)  AS IsResolved,
    (CASE
    WHEN tar.Min IS NOT NULL AND (tfv.ValueReal < tar.Min) THEN tar.Field || ' must be greater than ' || tar.Min
    WHEN tar.Max IS NOT NULL AND (tfv.ValueReal > tar.Max) THEN tar.Field || ' must be less than ' || tar.Max
    ELSE 'Validation Error' END) AS Message,
    res.Resolution
FROM measureTrees AS t
JOIN TreeFieldSetup AS tfs USING (StratumCode, CruiseID)
JOIN TreeFieldValue_All AS tfv USING (TreeID, Field)
-- get audit rule
JOIN TreeAuditRule AS tar
    ON tar.Field = tfs.Field AND tar.CruiseID = t.CruiseID
        AND EXISTS (SELECT * FROM TreeAuditRuleSelector AS tars WHERE
            (tars.SpeciesCode IS NULL OR tars.SpeciesCode = t.SpeciesCode)
            AND (tars.LiveDead IS NULL OR tars.LiveDead = t.LiveDead)
            AND (tars.PrimaryProduct IS NULL OR tars.PrimaryProduct = t.PrimaryProduct)
            AND tars.TreeAuditRuleID = tar.TreeAuditRuleID
            AND tars.CruiseID = t.CruiseID)
LEFT JOIN TreeAuditResolution AS res USING (TreeAuditRuleID, TreeID)
WHERE
    (tfv.ValueReal IS NOT NULL AND
    (tar.Min IS NOT NULL AND tfv.ValueReal < tar.Min)
    OR (tar.Max IS NOT NULL AND tfv.ValueReal > tar.Max));";
    }
}