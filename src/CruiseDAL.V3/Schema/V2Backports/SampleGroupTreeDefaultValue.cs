namespace CruiseDAL.Schema
{
    public partial class DDL
    {
        public const string CREATE_VIEW_SAMPLEGROUPTREEDEFAULTVALUE =
            "CREATE VIEW SampleGroupTreeDefaultValue AS " +
            "SELECT " +
                "sg.SampleGroup_CN, " +
                "tdv.TreeDefaultValue_CN " +
            "FROM SubPopulation AS subpop " +
            "JOIN SampleGroup_V3 AS sg USING (SampleGroupCode, StratumCode) " +
            "JOIN TreeDefaultValue AS tdv ON sg.PrimaryProduct = tdv.PrimaryProduct AND subpop.Species = tdv.Species AND subpop.LiveDead = tdv.LiveDead " +
            ";";
    }
}