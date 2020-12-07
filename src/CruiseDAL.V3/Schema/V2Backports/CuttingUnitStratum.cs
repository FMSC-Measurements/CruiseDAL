namespace CruiseDAL.Schema.V2Backports
{
    public class CuttingUnitStratum_V2_ViewDefinition : IViewDefinition
    {
        public string ViewName => "CuttingUnitStratum_V2";

        public string CreateView =>
@"CREATE VIEW CuttingUnitStratum_V2
AS
    SELECT cu.CuttingUnit_CN,
    st.Stratum_CN,
    StratumArea
    FROM CuttingUnit_Stratum AS cust
    JOIN CuttingUnit AS cu ON cust.CuttingUnitCode = cu.Code
    JOIN Stratum AS st ON cust.StratumCode = st.Code;";
    }
}