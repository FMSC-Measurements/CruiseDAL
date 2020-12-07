namespace CruiseDAL.Schema.V2Backports
{
    public class SampleGroup_V2_ViewDefinition : IViewDefinition
    {
        public string ViewName => "SampleGroup_V2";

        public string CreateView =>
@"CREATE VIEW SampleGroup_V2 AS
SELECT
    sg.SampleGroup_CN,
    sg.SampleGroupCode AS Code,
    st.Stratum_CN,
    sg.CutLeave,
    sg.UOM,
    sg.PrimaryProduct,
    sg.SecondaryProduct,
    sg.BiomassProduct,
    sg.DefaultLiveDead,
    sg.SamplingFrequency,
    sg.InsuranceFrequency,
    sg.KZ,
    sg.BigBAF,
    sg.TallyBySubPop,
    sg.TallyMethod,
    sg.Description,
    NULL AS SampleSelectorType,
    NULL AS SampleSelectorState,
    sg.MinKPI,
    sg.MaxKPI,
    sg.SmallFPS,
    sg.CreatedBy,
    sg.Created_TS AS CreatedDate,
    sg.ModifiedBy,
    sg.Modified_TS AS ModifiedDate,
    0 AS RowVersion
FROM SampleGroup AS sg
JOIN Stratum AS st ON sg.StratumCode = st.Code
;";
    }
}