namespace CruiseDAL.Schema.V2Backports
{
    public class Plot_V2_ViewDefinition : IViewDefinition
    {
        public const string CREATE_VIEW_PLOT =
            "CREATE VIEW Plot_V2 AS " +
            "SELECT " +
                "null AS Plot_GUID, " +
                "ps.Plot_Stratum_CN AS Plot_CN, " +
                "st.Stratum_CN, " +
                "cu.CuttingUnit_CN, " +
                "ps.PlotNumber, " +
                "(CASE ps.IsEmpty WHEN 0 THEN 'False' ELSE 'True' END) AS IsEmpty, " +
                "p.Slope, " +
                "ps.KPI, " +
                "ps.ThreePRandomValue, " +
                "p.Aspect, " +
                "p.Remarks, " +
                "p.XCoordinate, " +
                "p.YCoordinate, " +
                "p.ZCoordinate, " +
                "null AS MetaData, " +
                "null AS Blob, " +
                "ps.CreatedBy, " +
                "ps.Created_TS AS CreatedDate, " +
                "ps.ModifiedBy, " +
                "ps.Modified_TS AS ModifiedDate, " +
                "0 AS RowVersion " +
            "FROM Plot_Stratum AS ps " +
            "JOIN Plot AS p  USING (PlotNumber, CuttingUnitCode)" +
            "JOIN Stratum AS st ON ps.StratumCode = st.Code " +
            "JOIN CuttingUnit AS cu ON p.CuttingUnitCode = cu.Code" +
            ";";

        public string ViewName => "Plot_V2";

        public string CreateView => "CREATE VIEW Plot_V2 AS " +
@"SELECT
    null AS Plot_GUID,
    ps.Plot_Stratum_CN AS Plot_CN,
    st.Stratum_CN,
    cu.CuttingUnit_CN,
    ps.PlotNumber,
    (CASE ps.IsEmpty WHEN 0 THEN 'False' ELSE 'True' END) AS IsEmpty,
    p.Slope,
    ps.KPI,
    ps.ThreePRandomValue,
    p.Aspect,
    p.Remarks,
    p.XCoordinate,
    p.YCoordinate,
    p.ZCoordinate,
    null AS MetaData,
    null AS Blob,
    ps.CreatedBy,
    ps.Created_TS AS CreatedDate,
    ps.ModifiedBy,
    ps.Modified_TS AS ModifiedDate,
    0 AS RowVersion
FROM Plot_Stratum AS ps
JOIN Plot AS p  USING (PlotNumber, CuttingUnitCode)
JOIN Stratum AS st ON ps.StratumCode = st.Code
JOIN CuttingUnit AS cu ON p.CuttingUnitCode = cu.Code
;";
    }
}