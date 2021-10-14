namespace CruiseDAL.Schema.Views
{
    public class Tree_TreeDefaultValue : IViewDefinition
    {
        public string ViewName => "Tree_TreeDefaultValue";

        public string CreateView =>
@"CREATE VIEW Tree_TreeDefaultValue AS
SELECT
    t.TreeID,
    (SELECT TreeDefaultValue_CN FROM TreeDefaultValue AS tdv
        WHERE  tdv.CruiseID = t.CruiseID
            AND SpeciesCode = t.SpeciesCode OR SpeciesCode IS NULL
            AND PrimaryProduct = sg.PrimaryProduct OR PrimaryProduct IS NULL
        ORDER BY PrimaryProduct DESC, SpeciesCode DESC
        LIMIT 1
    ) AS TreeDefaultValue_CN
FROM Tree AS t
JOIN SampleGroup AS sg USING (SampleGroupCode, StratumCode, CruiseID);   -- join sample group to get primary product
";
    }
}