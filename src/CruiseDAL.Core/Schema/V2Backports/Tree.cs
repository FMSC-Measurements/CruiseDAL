namespace CruiseDAL.Schema
{
    public partial class DDL
    {
        public const string CREATE_VIEW_TREE =
            "CREATE VIEW Tree AS " +

            "SELECT " +
                "t.Tree_CN, " +
                "t.TreeID AS Tree_GUID, " +
                "cu.CuttingUnit_CN, " +
                "st.Stratum_CN, " +
                "sg.SampleGroup_CN, " +
                "tdv.TreeDefaultValue_CN,  " +
                "Plot_Stratum_CN AS Plot_CN, " +
                "t.Species, " +
                "t.LiveDead, " +
                "t.TreeNumber, " +
                "t.CountOrMeasure, " +
                "CAST (" +
                    "(CASE WHEN plt.Plot_Stratum_CN IS NULL " +
                            "THEN 0 " +
                            "ELSE (ifnull(tl.TreeCount, 0)) " +
                    "END) " +
                    "AS REAL) AS TreeCount, " + // in v2 TreeCount and kpi had a type of REAL
                "CAST (ifnull(tl.KPI, 0) AS REAL) AS KPI, " +
                "(CASE ifnull(tl.STM, 0) WHEN 0 THEN 'N' WHEN 1 THEN 'Y' END) AS STM, " +
                "tcv.ExpansionFactor, " +
                "tcv.TreeFactor, " +
                "tcv.PointFactor, " +
                "ifnull(t.XCoordinate, 0.0) AS XCoordinate," +
                "ifnull(t.YCoordinate, 0.0) AS YCoordinate," +
                "ifnull(t.ZCoordinate, 0.0) AS ZCoordinate, " +
                "tm.* " +
            "FROM Tree_V3 AS t " +
            "JOIN SampleGroup_V3 AS sg ON t.StratumCode = sg.StratumCode AND t.SampleGroupCode = sg.SampleGroupCode  " +
            "JOIN Stratum AS st ON t.StratumCode = st.Code " +
            "JOIN CuttingUnit AS cu ON t.CuttingUnitCode = cu.Code " +
            "LEFT JOIN Plot_Stratum AS plt USING (StratumCode, PlotNumber) " +
            "LEFT JOIN TreeCalculatedValues AS tcv USING (Tree_CN) " +
            "LEFT JOIN TreeDefaultValue AS tdv ON " +
                        "tdv.Species = t.Species " +
                        "AND tdv.LiveDead = t.LiveDead " +
                        "AND tdv.PrimaryProduct = sg.PrimaryProduct " +
            "LEFT JOIN TallyLedger AS tl ON tl.TreeID = t.TreeID " +
            "LEFT JOIN TreeMeasurment AS tm ON t.TreeID = tm.TreeID " +
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