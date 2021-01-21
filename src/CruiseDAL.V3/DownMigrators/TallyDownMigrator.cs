namespace CruiseDAL.DownMigrators
{
    public class TallyDownMigrator : IDownMigrator
    {
        public string CreateCommand(string toDbName, string fromDbName, string cruiseID, string createdBy)
        {
            return
$@"INSERT INTO {toDbName}.Tally (
	Hotkey,
	Description
)
SELECT
    thk.HotKey,
    ifnull(tds.Description, tp.SampleGroupCode || ' ' || tp.SpeciesCode) AS Description
FROM {fromDbName}.TallyPopulation AS tp
JOIN {fromDbName}.TallyHotKey AS thk USING (StratumCode, SampleGroupCode, SpeciesCode, LiveDead)
JOIN {fromDbName}.TallyDescription AS tds USING (StratumCode, SampleGroupCode, SpeciesCode, LiveDead);
";
        }
    }
}