namespace CruiseDAL.Schema.Views
{
    public class Tree_TreeDefaultValue : IViewDefinition
    {
        public string ViewName => "Tree_TreeDefaultValue";

        public string CreateView => CREATE_VIEW_3_5_1;

        public const string CREATE_VIEW_3_5_1 =
@"CREATE VIEW Tree_TreeDefaultValue AS
SELECT
    t.TreeID,
    (SELECT TreeDefaultValue_CN FROM TreeDefaultValue AS tdv
        WHERE  tdv.CruiseID = t.CruiseID
            AND (tdv.SpeciesCode = t.SpeciesCode OR SpeciesCode IS NULL)
            AND (tdv.PrimaryProduct = sg.PrimaryProduct OR tdv.PrimaryProduct IS NULL)
        ORDER BY tdv.PrimaryProduct DESC, tdv.SpeciesCode DESC
        LIMIT 1
    ) AS TreeDefaultValue_CN
FROM Tree AS t
JOIN SampleGroup AS sg USING (SampleGroupCode, StratumCode, CruiseID);   -- join sample group to get primary product
";
    }
}