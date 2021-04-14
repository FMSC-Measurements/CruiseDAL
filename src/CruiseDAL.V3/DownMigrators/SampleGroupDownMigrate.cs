namespace CruiseDAL.DownMigrators
{
    public class SampleGroupDownMigrate : IDownMigrator
    {
        public string CreateCommand(string toDbName, string fromDbName, string cruiseID, string createdBy)
        {
            return
$@"INSERT INTO {toDbName}.SampleGroup (
    SampleGroup_CN,
	Stratum_CN,
	Code,
	CutLeave,
	UOM,
	PrimaryProduct,
	SecondaryProduct,
	BiomassProduct,
	DefaultLiveDead,
	SamplingFrequency,
	InsuranceFrequency,
	KZ,
	BigBAF,
	SmallFPS,
	Description,
	SampleSelectorType,
	SampleSelectorState,
	MinKPI,
	MaxKPI,
	CreatedBy
)
SELECT
    sg.SampleGroup_CN,
    st.Stratum_CN,
    sg.SampleGroupCode AS Code,
    sg.CutLeave,
    ifnull(sg.UOM, (SELECT DefaultUOM FROM {fromDbName}.Cruise WHERE CruiseID = '{cruiseID}')),
    sg.PrimaryProduct,
    sg.SecondaryProduct,
    sg.BiomassProduct,
    sg.DefaultLiveDead,
    sg.SamplingFrequency,
    sg.InsuranceFrequency,
    sg.KZ,
    sg.BigBAF,
    sg.SmallFPS,
    sg.Description,
    NULL AS SampleSelectorType,
    NULL AS SampleSelectorState,
    sg.MinKPI,
    sg.MaxKPI,
    '{createdBy}'
FROM {fromDbName}.SampleGroup AS sg
JOIN {fromDbName}.Stratum AS st USING (StratumCode)
WHERE sg.CruiseID =  '{cruiseID}';
";
        }
    }
}