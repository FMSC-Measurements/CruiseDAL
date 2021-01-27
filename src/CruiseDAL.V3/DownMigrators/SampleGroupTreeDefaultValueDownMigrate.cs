namespace CruiseDAL.DownMigrators
{
    public class SampleGroupTreeDefaultValueDownMigrate : IDownMigrator
    {
        public string CreateCommand(string toDbName, string fromDbName, string cruiseID, string createdBy)
        {
            return
$@"INSERT INTO {toDbName}.SampleGroupTreeDefaultValue (
    SampleGroup_CN,
    TreeDefaultValue_CN
)
SELECT
    sg.SampleGroup_CN,
    tdv.TreeDefaultValue_CN
FROM {fromDbName}.SubPopulation AS subpop
JOIN {fromDbName}.SampleGroup AS sg USING (SampleGroupCode, StratumCode)
JOIN {toDbName}.TreeDefaultValue AS tdv ON sg.PrimaryProduct = tdv.PrimaryProduct AND subpop.SpeciesCode = tdv.Species AND subpop.LiveDead = tdv.LiveDead
;";
        }
    }
}