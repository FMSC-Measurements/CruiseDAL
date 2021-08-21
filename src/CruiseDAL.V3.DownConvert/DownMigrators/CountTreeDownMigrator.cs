namespace CruiseDAL.DownMigrators
{
    public class CountTreeDownMigrator : IDownMigrator
    {
        public string CreateCommand(string toDbName, string fromDbName, string cruiseID, string createdBy)
        {
            return
$@"
WITH tallyLedgerGrouped AS (
    SELECT CruiseID, CuttingUnitCode, StratumCode, SampleGroupCode, ifnull(SpeciesCode, '') AS SpeciesCode, ifnull(LiveDead, '') AS LiveDead,
    sum(TreeCount) AS TreeCount, sum(KPI) AS SumKPI,
    min(TallyLedger_CN) AS CountTree_CN
    FROM {fromDbName}.TallyLedger
    WHERE CruiseID = '{cruiseID}'
    GROUP BY CruiseID, CuttingUnitCode, StratumCode, SampleGroupCode, SpeciesCode, LiveDead
),

 tallyPopulationTallyLedger AS (
    SELECT
    tp.CruiseID,
    tl.CountTree_CN,
    cust.CuttingUnitCode,
    tp.StratumCode,
    tp.SampleGroupCode,
    tp.SpeciesCode,
    tp.LiveDead,
    ifnull(tl.TreeCount, 0) AS TreeCount, ifnull(tl.SumKPI, 0) AS SumKPI
    FROM TallyPopulation AS tp
    JOIN CuttingUnit_Stratum AS cust USING (StratumCode, CruiseID)
    LEFT JOIN tallyLedgerGrouped AS tl ON cust.CuttingUnitCode = tl.CuttingUnitCode
        AND cust.CruiseID = tl.CruiseID
        AND tl.StratumCode = tp.StratumCode
        AND tl.SampleGroupCode = tp.SampleGroupCode
        AND tl.SpeciesCode = ifnull(tp.SpeciesCode,'')
        AND tl.LiveDead = ifnull(tp.LiveDead, '')
)

INSERT INTO {toDbName}.CountTree (
	--CountTree_CN,
    CuttingUnit_CN,
	SampleGroup_CN,
    TreeDefaultValue_CN,
	Tally_CN,
	Component_CN,
	TreeCount,
	SumKPI
)
SELECT
    --tptl.CountTree_CN AS CountTree_CN,
    cu.CuttingUnit_CN,
    sg.SampleGroup_CN,
    tdv.TreeDefaultValue_CN,
    0 AS Tally_CN,
    null AS Component_CN,
    tptl.TreeCount,
    tptl.SumKPI
FROM tallyPopulationTallyLedger AS tptl
JOIN {fromDbName}.CuttingUnit AS cu USING (CuttingUnitCode, CruiseID)
JOIN {fromDbName}.SampleGroup AS sg USING (SampleGroupCode, StratumCode, CruiseID)
LEFT JOIN {toDbName}.TreeDefaultValue AS tdv ON
    tptl.SpeciesCode = tdv.Species
    AND tptl.LiveDead = tdv.LiveDead
    AND tdv.PrimaryProduct = sg.PrimaryProduct
WHERE tptl.CruiseID = '{cruiseID}'
;";
        }
    }
}