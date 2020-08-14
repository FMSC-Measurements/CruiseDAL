namespace CruiseDAL.Schema
{
    public partial class DDL
    {
        public const string CREATE_VIEW_TREEERROR =
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
        tm.TreeID,
        t.CruiseID,
        'e' AS Level,
        'Aleast One Height Parameter Must Be Greater Than 0' AS Message,
        '*' AS Field
    FROM TreeMeasurment AS tm 
    JOIN Tree AS t USING (TreeID)
    WHERE t.CountOrMeasure = 'M' AND tm.TotalHeight <=0 AND tm.MerchHeightPrimary <= 0 AND tm.MerchHeightSecondary <= 0 AND tm.UpperStemHeight <= 0 ),

    treeError_merchHeightSecondary AS (
    SELECT 
        tm.TreeID,
        t.CruiseID,
        'e' AS Level,
        'Merch Height Secondary Must Be Greater Than or Equal Merch Height Primary' AS Message,
        'MerchHeightSecondary' AS Field
    FROM TreeMeasurment AS tm 
    JOIN Tree AS t USING (TreeID)
    WHERE t.CountOrMeasure = 'M' AND tm.MerchHeightSecondary > 0 AND tm.MerchHeightSecondary <= tm.MerchHeightPrimary),

    treeError_upperStemHeight AS (
    SELECT 
        tm.TreeID,
        t.CruiseID,
        'e' AS Level,
        'Upper Stem Height Must Be Greater Than or Equal Merch Height Primary' AS Message,
        'UpperStemHeight' AS Field
    FROM TreeMeasurment AS tm 
    JOIN Tree AS t USING (TreeID)
    WHERE t.CountOrMeasure = 'M' AND tm.UpperStemHeight > 0 AND tm.UpperStemHeight < tm.MerchHeightPrimary),

    treeError_upperStemDiameter AS (
    SELECT 
        tm.TreeID,
        t.CruiseID,
        'e' AS Level,
        'Upper Stem Diameter Must Be Smaller Than DBH' AS Message,
        'UpperStemDiameter' AS Field
    FROM TreeMeasurment AS tm 
    JOIN Tree AS t USING (TreeID)
    WHERE t.CountOrMeasure = 'M' AND tm.UpperStemDiameter > 0 AND tm.UpperStemDiameter >= tm.DBH),

    treeError_topDIBSecondary AS (
    SELECT 
        tm.TreeID,
        t.CruiseID,
        'e' AS Level,
        'Top DIB Secondary must be less Top DIB Primary' AS Message,
        'TopDIBSecondary' AS Field
    FROM TreeMeasurment AS tm 
    JOIN Tree AS t USING (TreeID)
    WHERE t.CountOrMeasure = 'M' AND tm.TopDIBSecondary > 0 AND tm.TopDIBSecondary > tm.TopDIBPrimary),

    treeError_seenDefectPrimary AS (
    SELECT 
        tm.TreeID,
        t.CruiseID,
        'e' AS Level,
        'Seen Defect Primary must be greater than Recoverable Primary' AS Message,
        'SeenDefectPrimary' AS Field
    FROM TreeMeasurment AS tm 
    JOIN Tree AS t USING (TreeID)
    WHERE t.CountOrMeasure = 'M' AND tm.SeenDefectPrimary > 0 AND tm.SeenDefectPrimary < tm.RecoverablePrimary)

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

        // union view for creating errors
    }
}