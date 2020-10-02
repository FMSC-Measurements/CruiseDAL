namespace CruiseDAL.Schema
{
    public class PlotErrorViewDefinition : IViewDefinition
    {
// note: if we add anymore log audits you will need to update the RowID 
// value returned by the ErrorLog view when reading from PlotError

        public string ViewName => "PlotError";

        public string CreateView =>
@"CREATE VIEW PlotError AS
    SELECT
        p.PlotID, 
        p.CruiseID,
        ps.CuttingUnitCode,
        ps.PlotNumber,
        ps.StratumCode,
        ps.Plot_Stratum_CN,
        'Unit:' || ps.CuttingUnitCode || ' Plot:' || ps.PlotNumber || ' St:' || ps.StratumCode || ' contains trees but is marked as empty' AS Message,
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
            AND PlotNumber = ps.PlotNumber AND StratumCode = ps.StratumCode AND CruiseID = ps.CruiseID);";
    }
}