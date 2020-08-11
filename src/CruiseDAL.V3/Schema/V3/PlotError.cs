namespace CruiseDAL.Schema
{
    public partial class DDL
    {
        public const string CREATE_VIEW_PLOTERROR =
// note: if we add anymore log audits you will need to update the RowID 
// value returned by the ErrorLog view when reading from PlotError

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
    JOIN Plot_V3 AS p USING (CuttingUnitCode, PlotNumber, CruiseID)
    WHERE IsEmpty != 0
        AND EXISTS (SELECT * FROM Tree_V3
            WHERE CuttingUnitCode = ps.CuttingUnitCode
            AND PlotNumber = ps.PlotNumber AND StratumCode = ps.StratumCode AND CruiseID = ps.CruiseID);";
    }
}