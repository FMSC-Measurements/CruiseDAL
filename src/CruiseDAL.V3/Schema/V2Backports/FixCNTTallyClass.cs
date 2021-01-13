namespace CruiseDAL.Schema.V2Backports
{
    public class FixCNTTallyClass_V2_ViewDefinition : IViewDefinition
    {
        public string ViewName => "FixCNTTallyClass_V2";

        public string CreateView =>
@"CREATE VIEW FixCNTTallyClass_V2 AS
    SELECT Stratum_CN, FixCNTField AS Field
    FROM Stratum;";
    }
}