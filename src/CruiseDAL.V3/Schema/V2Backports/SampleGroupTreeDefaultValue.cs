namespace CruiseDAL.Schema.V2Backports
{
    public class SampleGroupTreeDefaultValue_V2_ViewDefinition : IViewDefinition
    {
        public string ViewName => "SampleGroupTreeDefaultValue_V2";

        public string CreateView =>
@"CREATE VIEW SampleGroupTreeDefaultValue_V2 AS
SELECT
    sg.SampleGroup_CN,
    tdv.TreeDefaultValue_CN
FROM SubPopulation AS subpop
JOIN SampleGroup AS sg USING (SampleGroupCode, StratumCode)
JOIN TreeDefaultValue AS tdv ON sg.PrimaryProduct = tdv.PrimaryProduct AND subpop.SpeciesCode = tdv.SpeciesCode AND subpop.LiveDead = tdv.LiveDead
;";
    }
}