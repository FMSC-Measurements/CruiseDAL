namespace CruiseDAL.Schema
{
    public class TreeErrorViewDefinition : IViewDefinition
    {
        public string ViewName => "TreeError";

        public string CreateView => v3_4_3;

        public const string v3_4_3 =
@"CREATE VIEW TreeError AS

WITH
    treeError_speciesCode AS (
    SELECT
        t.TreeID,
        t.CruiseID,
        'e' AS Level,
        'Species Code Is Missing' AS Message,
        'SpeciesCode' AS Field
    FROM Tree AS t
    WHERE t.SpeciesCode IS NULL OR t.SpeciesCode = ''),

    treeError_liveDead AS (
    SELECT
        t.TreeID,
        t.CruiseID,
        'e' AS Level,
        'Live/Dead Value Is Missing' AS Message,
        'LiveDead' AS Field
    FROM Tree AS t
    WHERE t.LiveDead IS NULL OR t.LiveDead = ''),

    treeError_heights AS (
    SELECT
        t.TreeID,
        t.CruiseID,
        'e' AS Level,
        'At Least One Height Parameter Must Be Greater Than 0' AS Message,
        'heights' AS Field
    FROM Tree AS t
    LEFT JOIN TreeMeasurment AS tm USING (TreeID)
    WHERE t.CountOrMeasure = 'M'  
        AND ifnull(tm.TotalHeight, 0) <=0 
        AND ifnull(tm.MerchHeightPrimary, 0) <= 0 
        AND ifnull(tm.MerchHeightSecondary, 0) <= 0 
        AND ifnull(tm.UpperStemHeight, 0) <= 0),

    treeError_diameters AS (
    SELECT
        t.TreeID,
        t.CruiseID,
        'e' AS Level,
        'DBH or DRC must be greater than 0' AS Message,
        'diameters' AS Field
    FROM Tree AS t
    LEFT JOIN TreeMeasurment AS tm USING (TreeID)
    WHERE t.CountOrMeasure = 'M' 
        AND ifnull(tm.DBH, 0) <=0 
        AND ifnull(tm.DRC, 0) <=0),

    treeError_merchHeightSecondary AS (
    SELECT
        t.TreeID,
        t.CruiseID,
        'e' AS Level,
        'Merch Height Secondary Must Be Greater Than or Equal Merch Height Primary' AS Message,
        'MerchHeightSecondary' AS Field
    FROM Tree AS t
    JOIN TreeMeasurment AS tm USING (TreeID)
    WHERE t.CountOrMeasure = 'M' 
        AND tm.MerchHeightSecondary > 0 
        AND tm.MerchHeightSecondary <= tm.MerchHeightPrimary),

    treeError_upperStemHeight AS (
    SELECT
        t.TreeID,
        t.CruiseID,
        'e' AS Level,
        'Upper Stem Height Must Be Greater Than or Equal Merch Height Primary' AS Message,
        'UpperStemHeight' AS Field
    FROM Tree AS t
    JOIN TreeMeasurment AS tm USING (TreeID)
    WHERE t.CountOrMeasure = 'M' 
        AND tm.UpperStemHeight > 0 
        AND tm.UpperStemHeight < tm.MerchHeightPrimary),

    treeError_upperStemDiameter AS (
    SELECT
        t.TreeID,
        t.CruiseID,
        'e' AS Level,
        'Upper Stem Diameter Must Be Smaller Than DBH' AS Message,
        'UpperStemDiameter' AS Field
    FROM Tree AS t
    JOIN TreeMeasurment AS tm USING (TreeID)
    WHERE t.CountOrMeasure = 'M' 
        AND tm.UpperStemDiameter > 0 
        AND tm.UpperStemDiameter >= tm.DBH),

    treeError_topDIBSecondary AS (
    SELECT
        t.TreeID,
        t.CruiseID,
        'e' AS Level,
        'Top DIB Secondary must be less Top DIB Primary' AS Message,
        'TopDIBSecondary' AS Field
    FROM Tree AS t
    JOIN TreeMeasurment AS tm USING (TreeID)
    WHERE t.CountOrMeasure = 'M' 
        AND tm.TopDIBSecondary > 0 
        AND tm.TopDIBSecondary > tm.TopDIBPrimary),

    treeError_seenDefectPrimary AS (
    SELECT
        t.TreeID,
        t.CruiseID,
        'e' AS Level,
        'Seen Defect Primary must be greater than Recoverable Primary' AS Message,
        'SeenDefectPrimary' AS Field
    FROM Tree AS t
    JOIN TreeMeasurment AS tm USING (TreeID)
    WHERE t.CountOrMeasure = 'M' 
        AND tm.SeenDefectPrimary > 0 
        AND tm.SeenDefectPrimary < tm.RecoverablePrimary)

SELECT
	tae.TreeID,
    tae.CruiseID,
	tae.TreeAuditRuleID,
	'W' AS Level,
	tae.Message,
	tae.Field,
    tae.IsResolved,
	tar.Resolution,
	tar.Initials AS ResolutionInitials
FROM TreeAuditError AS tae
LEFT JOIN TreeAuditResolution AS tar USING (TreeAuditRuleID, TreeID)
UNION ALL
SELECT
    te.TreeID,
    te.CruiseID,
    null AS TreeAuditRuleID,
    'E' AS Level,
    te.Message,
    te.Field,
    0 AS IsResolved,
    null AS Resolution,
    null AS ResolutionInitials
FROM (
    SELECT * FROM treeError_speciesCode
    UNION ALL
    SELECT * FROM treeError_liveDead
    UNION ALL
    SELECT * FROM treeError_heights
    UNION ALL
    SELECT * FROM treeError_diameters
    UNION ALL
    SELECT * FROM treeError_merchHeightSecondary
    UNION ALL
    SELECT * FROM treeError_upperStemHeight
    UNION ALL
    SELECT * FROM treeError_upperStemDiameter
    UNION ALL
    SELECT * FROM treeError_topDIBSecondary
    UNION ALL
    SELECT * FROM treeError_seenDefectPrimary
) AS te
;";
    }
}