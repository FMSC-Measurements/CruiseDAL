namespace CruiseDAL.Schema
{
    public partial class DDL
    {
        public const string CREATE_VIEW_PLOTERROR =
@"CREATE VIEW PlotError AS
    SELECT
        ps.CuttingUnitCode,
        ps.PlotNumber,
        ps.StratumCode,
        ps.Plot_Stratum_CN,
        'Unit:' || ps.CuttingUnitCode || ' Plot:' || ps.PlotNumber || ' contains no trees but is not marked as empty' AS Message,
        'IsEmpty' AS Field,
        'E' AS Level,
        null AS Resolution,
        null AS ResolutionInitials
    FROM Plot_Stratum AS ps
    WHERE IsEmpty = 0
        AND EXISTS (SELECT * FROM Tree_V3
            WHERE CuttingUnitCode = ps.CuttingUnitCode
            AND PlotNumber = ps.PlotNumber AND StratumCode = ps.StratumCode);";
    }
}