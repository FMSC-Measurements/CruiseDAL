namespace CruiseDAL.Schema
{
    public partial class DDL
    {
        public const string CREATE_VIEW_TREEAUDITERROR =
@"CREATE VIEW TreeAuditError AS
WITH
-- return just measure trees
measureTrees AS (
    SELECT
        t.Tree_CN,
        t.TreeID,
        t.StratumCode,
        t.Species,
        t.LiveDead,
        sg.PrimaryProduct
    FROM Tree_V3 AS t
    JOIN SampleGroup_V3 AS sg USING (SampleGroupCode, StratumCode)
    WHERE t.CountOrMeasure = 'M'),

-- expands treedefaultValue_TreeAuditValue to include Field, Min, Max
treeDefaultValue_TreeAuditRule_Epanded AS (
    SELECT
        Species,
        LiveDead,
        PrimaryProduct,
        tar.Field,
        Min,
        Max,
        tar.TreeAuditRuleID
    FROM TreeDefaultValue_TreeAuditRule AS tdvtar
    JOIN TreeAuditRule AS tar USING (TreeAuditRuleID))

SELECT
    Tree_CN,
    TreeID,
    TreeAuditRuleID,
    tdvtar.Field AS Field,
    (CASE WHEN res.TreeAuditResolution_CN IS NULL THEN 0 ELSE 1 END)  AS IsResolved, 
    (CASE
    WHEN tdvtar.Min IS NOT NULL AND (tfv.ValueReal < tdvtar.Min) THEN tdvtar.Field || ' must be greater than ' || tdvtar.Min
    WHEN tdvtar.Max IS NOT NULL AND (tfv.ValueReal > tdvtar.Max) THEN tdvtar.Field || ' must be less than ' || tdvtar.Max
    ELSE 'Validation Error' END) AS Message,
    res.Resolution
FROM measureTrees AS t
JOIN TreeFieldSetup_V3 AS tfs USING (StratumCode)
JOIN TreeFieldValue_All AS tfv USING (TreeID, Field)
-- get audit rule
JOIN treeDefaultValue_TreeAuditRule_Epanded tdvtar
        ON (tdvtar.Species IS NULL OR tdvtar.Species = t.Species)
        AND (tdvtar.LiveDead IS NULL OR tdvtar.LiveDead = t.LiveDead)
        AND (tdvtar.PrimaryProduct IS NULL OR tdvtar.PrimaryProduct = t.PrimaryProduct)
        AND tdvtar.Field = tfs.Field
LEFT JOIN TreeAuditResolution AS res USING (TreeAuditRuleID, TreeID)
WHERE
    (tfv.ValueReal IS NOT NULL AND
    (tdvtar.Min IS NOT NULL AND tfv.ValueReal < tdvtar.Min)
    OR (tdvtar.Max IS NOT NULL AND tfv.ValueReal > tdvtar.Max));";
    }
}