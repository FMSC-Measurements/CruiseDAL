namespace CruiseDAL.Schema
{
    public partial class DDL
    {
        public const string CREATE_VIEW_SAMPLEGROUPTREEDEFAULTVALUE =
            "CREATE VIEW SampleGroupTreeDefaultValue_V2 AS " +
            "SELECT " +
                "sg.SampleGroup_CN, " +
                "tdv.TreeDefaultValue_CN " +
            "FROM SubPopulation AS subpop " +
            "JOIN SampleGroup AS sg USING (SampleGroupCode, StratumCode) " +
            "JOIN TreeDefaultValue AS tdv ON sg.PrimaryProduct = tdv.PrimaryProduct AND subpop.SpeciesCode = tdv.SpeciesCode AND subpop.LiveDead = tdv.LiveDead " +
            ";";
    }
}