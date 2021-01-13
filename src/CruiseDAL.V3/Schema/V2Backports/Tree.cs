namespace CruiseDAL.Schema.V2Backports
{
    public class Tree_V2_ViewDefinition : IViewDefinition
    {
        public string ViewName => "Tree_V2";

        public string CreateView =>
@"CREATE VIEW Tree_V2 AS

SELECT
    t.Tree_CN,
    t.TreeID AS Tree_GUID,
    cu.CuttingUnit_CN,
    st.Stratum_CN,
    sg.SampleGroup_CN,
    tdv.TreeDefaultValue_CN,
    Plot_Stratum_CN AS Plot_CN,
    t.SpeciesCode AS Species,
    t.LiveDead,
    t.TreeNumber,
    t.CountOrMeasure,
    CAST (
        (CASE WHEN plt.Plot_Stratum_CN IS NULL
                THEN 0
                ELSE (ifnull(tl.TreeCount, 0))
        END)
        AS REAL) AS TreeCount, -- in v2 TreeCount and kpi had a type of REAL
    CAST (ifnull(tl.KPI, 0) AS REAL) AS KPI,
    (CASE ifnull(tl.STM, 0) WHEN 0 THEN 'N' WHEN 1 THEN 'Y' END) AS STM,
    tcv.ExpansionFactor,
    tcv.TreeFactor,
    tcv.PointFactor,
    ifnull(t.XCoordinate, 0.0) AS XCoordinate,
    ifnull(t.YCoordinate, 0.0) AS YCoordinate,
    ifnull(t.ZCoordinate, 0.0) AS ZCoordinate,
    tm.*
FROM Tree AS t
JOIN SampleGroup AS sg ON t.StratumCode = sg.StratumCode AND t.SampleGroupCode = sg.SampleGroupCode
JOIN Stratum AS st ON t.StratumCode = st.Code
JOIN CuttingUnit AS cu ON t.CuttingUnitCode = cu.Code
LEFT JOIN Plot_Stratum AS plt USING (CuttingUnitCode, StratumCode, PlotNumber)
LEFT JOIN TreeCalculatedValues AS tcv USING (Tree_CN)
LEFT JOIN TreeDefaultValue AS tdv ON
            tdv.SpeciesCode = t.SpeciesCode
            AND tdv.LiveDead = t.LiveDead
            AND tdv.PrimaryProduct = sg.PrimaryProduct
LEFT JOIN TallyLedger_Tree_Totals AS tl ON tl.TreeID = t.TreeID
LEFT JOIN TreeMeasurment AS tm ON t.TreeID = tm.TreeID
;";
    }
}