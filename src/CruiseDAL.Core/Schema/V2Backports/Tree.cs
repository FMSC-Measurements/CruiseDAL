namespace CruiseDAL.Schema
{
    public partial class DDL
    {
        public const string CREATE_VIEW_TREE =
            "WITH plotTrees AS ( " +
            "SELECT " +
                "t.Tree_CN, " +
                "t.TreeID AS Tree_GUID, " +
                "cu.CuttingUnit_CN, " +
                "st.Stratum_CN, " +
                "sg.SampleGroup_CN, " +
                "tdv.TreeDefaultValue_CN,  " +
                "plt.Plot_Stratum_CN AS Plot_CN, " +
                "t.Species, " +
                "t.LiveDead, " +
                "t.TreeNumber, " +
                "t.CountOrMeasure, " +
                "ifnull(tl.TreeCount, 0) AS TreeCount, " + // in v2 TreeCount and kpi had a type of REAL
                "ifnull(tl.KPI, 0) AS KPI, " +
                "ifnull(tl.STM, 'N') AS STM, " +
                "tcv.ExpansionFactor, " +
                "tcv.TreeFactor, " +
                "tcv.PointFactor, " +
                "tm.* " +
            "FROM Tree_V3 AS t " +
            "JOIN SampleGroup_V3 AS sg ON t.StratumCode = sg.StratumCode AND t.SampleGroupCode = sg.SampleGroupCode  " +
            "JOIN Stratum AS st ON t.StratumCode = st.Code " +
            "JOIN CuttingUnit AS cu ON t.CuttingUnitCode = cu.Code " +
            "LEFT JOIN TallyLedger AS tl USING (TreeID) " +
            "LEFT JOIN TreeMeasurment AS tm USING (TreeID) " +
            "LEFT JOIN TreeCalculatedValues AS tcv USING (Tree_CN) " +
            "LEFT JOIN TreeDefaultValue AS tdv ON " +
                        "tdv.Species = t.Species " +
                        "AND tdv.LiveDead = t.LiveDead " +
                        "AND tdv.PrimaryProduct = sg.PrimaryProduct " +
            "LEFT JOIN Plot_Stratum AS plt USING (StratumCode, PlotNumber) " +
            "), " +
            "nonPlotTrees AS ( " +
            "SELECT " +
                "t.Tree_CN, " +
                "t.TreeID AS Tree_GUID, " +
                "cu.CuttingUnit_CN, " +
                "st.Stratum_CN, " +
                "sg.SampleGroup_CN, " +
                "tdv.TreeDefaultValue_CN,  " +
                "null AS Plot_CN, " +
                "t.Species, " +
                "t.LiveDead, " +
                "t.TreeNumber, " +
                "t.CountOrMeasure, " +
                "0 AS TreeCount, " + 
                "ifnull(tl.KPI, 0) AS KPI, " +
                "ifnull(tl.STM, 'N') AS STM, " +
                "tcv.ExpansionFactor, " +
                "tcv.TreeFactor, " +
                "tcv.PointFactor, " +
                "tm.* " +
            "FROM Tree_V3 AS t " +
            "JOIN SampleGroup_V3 AS sg ON t.StratumCode = sg.StratumCode AND t.SampleGroupCode = sg.SampleGroupCode  " +
            "JOIN Stratum AS st ON t.StratumCode = st.Code " +
            "JOIN CuttingUnit AS cu ON t.CuttingUnitCode = cu.Code " +
            "LEFT JOIN TallyLedger AS tl USING (TreeID) " +
            
            "LEFT JOIN TreeCalculatedValues AS tcv USING (Tree_CN) " +
            "LEFT JOIN TreeDefaultValue AS tdv ON " +
                        "tdv.Species = t.Species " +
                        "AND tdv.LiveDead = t.LiveDead " +
                        "AND tdv.PrimaryProduct = sg.PrimaryProduct " +
            "), " +
            "" +
            "CREATE VIEW Tree AS " +
            "SELECT " +
                "Tree_CN, " +
                "Tree_GUID, " +
                "CuttingUnit_CN, " +
                "Stratum_CN, " +
                "SampleGroup_CN, " +
                "TreeDefaultValue_CN,  " +
                "Plot_Stratum_CN AS Plot_CN, " +
                "Species, " +
                "LiveDead, " +
                "TreeNumber, " +
                "CountOrMeasure, " +                
                "CAST (ifnull(s1.TreeCount, 0) AS REAL) AS TreeCount, " + // in v2 TreeCount and kpi had a type of REAL
                "CAST (ifnull(s1.KPI, 0) AS REAL) AS KPI, " +
                "ifnull(s1.STM, 'N') AS STM, " +
                "ExpansionFactor, " +
                "TreeFactor, " +
                "PointFactor, " +
                "tm.* " +
            "FROM (" +
            "SELECT * FROM plotTrees " +
            "UNION ALL " +
            "SELECT * FROM nonPlotTrees " +
            ") AS s1" +
            "LEFT JOIN TreeMeasurment AS tm ON s1.Tree_GUID = tm.TreeID " +
            ";";

        public const string CTEATE_TRIGGER_TREE_ONUPDATE =
            "CREATE TRIGGER TREE_ONUPDATE_PROCESSING " +
            "INSTEAD OF UPDATE OF ExpansionFactor, TreeFactor, PointFactor ON Tree " +
            "FOR EACH ROW " +
            "BEGIN " +
            "UPDATE TreeCalculatedValues SET ExpansionFactor = new.ExpansionFactor, TreeFactor = new.TreeFactor, PointFactor = new.PointFactor WHERE Tree_CN = new.Tree_CN; " +
            "END;";
    }
}