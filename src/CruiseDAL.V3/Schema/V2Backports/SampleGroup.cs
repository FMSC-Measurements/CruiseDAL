namespace CruiseDAL.Schema
{
    public partial class DDL
    {
        public const string CREATE_VIEW_SAMPLEGROUP =
            "CREATE VIEW SampleGroup AS " +
            "SELECT " +
                "sg.SampleGroup_CN, " +
                "sg.SampleGroupCode AS Code, " +
                "st.Stratum_CN, " +
                "sg.CutLeave, " +
                "sg.UOM, " +
                "sg.PrimaryProduct, " +
                "sg.SecondaryProduct, " +
                "sg.BiomassProduct, " +
                "sg.DefaultLiveDead, " +
                "sg.SamplingFrequency, " +
                "sg.InsuranceFrequency, " +
                "sg.KZ, " +
                "sg.BigBAF, " +
                "sg.TallyBySubPop, " +
                "sg.TallyMethod, " +
                "sg.Description, " +
                "NULL AS SampleSelectorType, " +
                "NULL AS SampleSelectorState, " +
                "sg.MinKPI, " +
                "sg.MaxKPI, " +
                "sg.SmallFPS, " +
                "sg.CreatedBy, " +
                "sg.CreatedDate, " +
                "sg.ModifiedBy, " +
                "sg.ModifiedDate, " +
                "sg.RowVersion " +
            "FROM SampleGroup_V3 AS sg " +
            "JOIN Stratum AS st ON sg.StratumCode = st.Code " +
            ";";
    }
}