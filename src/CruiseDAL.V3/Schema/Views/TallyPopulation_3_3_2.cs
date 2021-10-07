namespace CruiseDAL.Schema
{
    public class TallyPopulationViewDefinition_3_3_2 : IViewDefinition
    {
        public string ViewName => "TallyPopulation";

        public string CreateView =>
@"CREATE VIEW TallyPopulation AS

-- Tally By SubPop tally pops
SELECT
    sp.CruiseID,
    sp.StratumCode,
    sp.SampleGroupCode,
    sp.SpeciesCode,
    sp.LiveDead,
    ifnull(td.Description, '') AS Description,
    ifnull(thk.HotKey, '') AS HotKey
FROM SubPopulation AS sp
JOIN SampleGroup AS sg USING (StratumCode, SampleGroupCode, CruiseID)
JOIN Stratum AS st USING (StratumCode, CruiseID)
LEFT JOIN TallyHotKey AS thk USING (StratumCode, SampleGroupCode, SpeciesCode, LiveDead, CruiseID)
LEFT JOIN TallyDescription AS td USING (StratumCode, SampleGroupCode, SpeciesCode, LiveDead, CruiseID)
WHERE sg.TallyBySubPop != 0  OR st.Method IN ('3P', 'F3P', 'P3P', 'S3P')

UNION ALL

-- Tally by SG tally pops
SELECT
    sg.CruiseID,
    sg.StratumCode,
    sg.SampleGroupCode,
    null AS SpeciesCode,
    null AS LiveDead,
    ifnull(td.Description,'') AS Description,
    ifnull(thk.HotKey, '') AS HotKey
FROM SampleGroup AS sg
JOIN Stratum AS st USING (StratumCode, CruiseID)
LEFT JOIN TallyHotKey AS thk ON
        thk.CruiseID = sg.CruiseID
        AND thk.StratumCode = sg.StratumCode
        AND thk.SampleGroupCode = sg.SampleGroupCode
        AND ifnull(thk.SpeciesCode, '') = ''
        AND ifnull(thk.LiveDead, '') = ''
LEFT JOIN TallyDescription AS td ON
        thk.CruiseID = sg.CruiseID
        AND td.StratumCode = sg.StratumCode
        AND td.SampleGroupCode = sg.SampleGroupCode
        AND ifnull(td.SpeciesCode, '') = ''
        AND ifnull(td.LiveDead, '') = ''
WHERE sg.TallyBySubPop == 0 AND st.Method NOT IN ('3P', 'F3P', 'P3P', 'S3P');";

    }

}