namespace CruiseDAL.Schema.Views
{
    public class StratumDefaultViewDefinition : IViewDefinition
    {
        public string ViewName => "StratumDefault";

        public string CreateView =>
@"CREATE VIEW StratumDefault AS
    SELECT
        0 AS StratumDefault_CN,
        StratumTemplateName || CruiseID AS StratumDefaultID,
        NULL AS Region,
        NULL AS Forest,
        NULL AS District,
        StratumCode,
        StratumTemplateName AS Description,
        Method,
        BasalAreaFactor,
        FixedPlotSize,
        KZ3PPNT,
        SamplingFrequency,
        Hotkey,
        FBSCode,
        YieldComponent,
        FixCNTField
FROM StratumTemplate";
    }
}