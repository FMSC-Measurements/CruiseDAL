namespace CruiseDAL.Schema
{
    public partial class DDL
    {
        public const string CREATE_VIEW_CUTTINGUNITSTRATUM =
            "CREATE VIEW CuttingUnitStratum_V2 " +
            "AS " +
                "SELECT cu.CuttingUnit_CN, " +
                "st.Stratum_CN, " +
                "StratumArea " +
                "FROM CuttingUnit_Stratum AS cust " +
                "JOIN CuttingUnit AS cu ON cust.CuttingUnitCode = cu.Code " +
                "JOIN Stratum AS st ON cust.StratumCode = st.Code;";
    }
}