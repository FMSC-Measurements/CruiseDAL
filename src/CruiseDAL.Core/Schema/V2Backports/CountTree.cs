namespace CruiseDAL.Schema
{
    public partial class DDL
    {
        public const string CREATE_VIEW_COUNTTREE =
            "CREATE VIEW CountTree AS " +
            "WITH tallyLedgerGrouped AS (" +
                "SELECT CuttingUnitCode, StratumCode, SampleGroupCode, ifnull(Species, '') AS Species, ifnull(LiveDead, '') AS LiveDead, " +
                "sum(TreeCount) AS TreeCount, sum(KPI) AS SumKPI, " +
                "min(TallyLedger_CN) AS CountTree_CN " +
                "FROM TallyLedger " +
                "WHERE IsDeleted = 0" +
                "GROUP BY CuttingUnitCode, StratumCode, SampleGroupCode, Species, LiveDead " +
            ") " +

            ", tallyPopulationTallyLedger AS (" +
                "SELECT " +
                "tl.CountTree_CN, " +
                "cust.CuttingUnitCode, " +
                "tp.StratumCode, " +
                "tp.SampleGroupCode, " +
                "tp.Species, tp.LiveDead, " +
                "ifnull(tl.TreeCount, 0) AS TreeCount, ifnull(tl.SumKPI, 0) AS SumKPI " +
                "FROM TallyPopulation AS tp " +
                "JOIN CuttingUnit_Stratum AS cust USING (StratumCode) " +
                "LEFT JOIN tallyLedgerGrouped AS tl ON cust.CuttingUnitCode = tl.CuttingUnitCode " +
                    "AND tl.StratumCode = tp.StratumCode " +
                    "AND tl.SampleGroupCode = tp.SampleGroupCode " +
                    "AND tl.Species = ifnull(tp.Species,'') " +
                    "AND tl.LiveDead = ifnull(tp.LiveDead, '') " +
            ") " +

            "SELECT " +
                "tptl.CountTree_CN AS CountTree_CN, " +
                "cu.CuttingUnit_CN, " +
                "sg.SampleGroup_CN, " +
                "tdv.TreeDefaultValue_CN, " +
                "0 AS Tally_CN, null AS Component_CN, " +
                "tptl.TreeCount, " +
                "tptl.SumKPI, " +
                "'' AS CreatedBy, " +
                "'' AS CreatedDate, " +
                "null AS ModifiedBy, " +
                "null AS ModifiedDate, " +
                "0 AS RowVersion " +
            "FROM tallyPopulationTallyLedger AS tptl " +
            "JOIN CuttingUnit AS cu ON tptl.CuttingUnitCode = cu.Code " +
            "JOIN SampleGroup_V3 AS sg USING (SampleGroupCode, StratumCode) " +
            "LEFT JOIN TreeDefaultValue AS tdv ON tptl.Species = tdv.Species AND tptl.LiveDead = tdv.LiveDead AND tdv.PrimaryProduct = sg.PrimaryProduct " +
            "; ";
    }
}