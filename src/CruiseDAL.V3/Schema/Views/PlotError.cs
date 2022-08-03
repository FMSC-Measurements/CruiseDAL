namespace CruiseDAL.Schema
{
    public class PlotErrorViewDefinition : IViewDefinition
    {
// note: if we add anymore log audits you will need to update the RowID 
// value returned by the ErrorLog view when reading from PlotError

        public string ViewName => "PlotError";

        public string CreateView =>
@"





CREATE VIEW PlotError AS

WITH 
    plotError_null_with_trees AS (
    SELECT
        p.PlotID, 
        p.CruiseID,
        ps.CuttingUnitCode,
        ps.PlotNumber,
        ps.StratumCode,
        ps.Plot_Stratum_CN,
        'Unit:' || ps.CuttingUnitCode || ' Plot:' || ps.PlotNumber || ' St:' || ps.StratumCode || ' contains trees but is marked as null plot' AS Message,
        'IsEmpty' AS Field,
        'E' AS Level,
        0 AS IsResolved,
        null AS Resolution,
        null AS ResolutionInitials
    FROM Plot_Stratum AS ps
    JOIN Plot AS p USING (CuttingUnitCode, PlotNumber, CruiseID)
    WHERE IsEmpty != 0
        AND EXISTS (SELECT * FROM Tree
            WHERE CuttingUnitCode = ps.CuttingUnitCode
            AND PlotNumber = ps.PlotNumber AND StratumCode = ps.StratumCode AND CruiseID = ps.CruiseID)
    ),

    plotError_nonNull_no_trees AS  (
    SELECT
        p.PlotID, 
        p.CruiseID,
        ps.CuttingUnitCode,
        ps.PlotNumber,
        ps.StratumCode,
        ps.Plot_Stratum_CN,
        'Unit:' || ps.CuttingUnitCode || ' Plot:' || ps.PlotNumber || ' St:' || ps.StratumCode || ' contains no trees but is not marked as null plot' AS Message,
        'IsEmpty' AS Field,
        'E' AS Level,
        0 AS IsResolved,
        null AS Resolution,
        null AS ResolutionInitials
    FROM Plot_Stratum AS ps
    JOIN Plot AS p USING (CuttingUnitCode, PlotNumber, CruiseID)
    WHERE IsEmpty == 0
        AND NOT EXISTS (SELECT * FROM Tree
            WHERE CuttingUnitCode = ps.CuttingUnitCode
            AND PlotNumber = ps.PlotNumber AND StratumCode = ps.StratumCode AND CruiseID = ps.CruiseID)
    ),

    plotError_no_plotStratum AS  (
    SELECT
        p.PlotID, 
        p.CruiseID,
        p.CuttingUnitCode,
        p.PlotNumber,
        ps.StratumCode,
        ps.Plot_Stratum_CN,
        'Unit:' || p.CuttingUnitCode || ' Plot:' || p.PlotNumber ||  ' no strata in plot ' AS Message,
        NULL AS Field,
        'E' AS Level,
        0 AS IsResolved,
        null AS Resolution,
        null AS ResolutionInitials
    FROM Plot AS p
    LEFT JOIN Plot_Stratum as ps USING (CruiseID, CuttingUnitCode, PlotNumber)
    WHERE ps.RowID IS NULL
    )
    


SELECT 
    pe.PlotID,
    pe.CruiseID, 
    pe.CuttingUnitCode,
    pe.PlotNumber,
    pe.StratumCode,
    pe.Plot_Stratum_CN,
    pe.Message,
    pe.Field,
    pe.Level,
    pe.IsResolved,
    pe.Resolution,
    pe.ResolutionInitials
FROM (
    SELECT * FROM plotError_null_with_trees
    UNION ALL
    SELECT * FROM plotError_nonNull_no_trees
    UNION ALL
    SELECT * FROM plotError_no_plotStratum
) AS pe;";
    }
}