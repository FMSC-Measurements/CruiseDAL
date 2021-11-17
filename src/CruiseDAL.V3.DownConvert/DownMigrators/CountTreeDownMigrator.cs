namespace CruiseDAL.DownMigrators
{
    public class CountTreeDownMigrator : IDownMigrator
    {
        public string CreateCommand(string toDbName, string fromDbName, string cruiseID, string createdBy)
        {
            return
$@"
WITH 

 tallyPopulationTallyLedger AS (
    SELECT
    tp.CruiseID,
    cust.CuttingUnitCode,
    tp.StratumCode,
    tp.SampleGroupCode,
    tp.SpeciesCode,
    tp.LiveDead,
    ifnull(sum(tl.TreeCount), 0)  AS TreeCount, 
    ifnull(sum(tl.KPI), 0) AS SumKPI
    --ifnull(tl.TreeCount, 0)  AS TreeCount, 
    --ifnull(tl.KPI, 0) AS SumKPI
    FROM {fromDbName}.TallyPopulation AS tp
    JOIN {fromDbName}.Stratum AS st USING (StratumCode, CruiseID)
    JOIN {fromDbName}.CuttingUnit_Stratum AS cust USING (StratumCode, CruiseID)
    JOIN {fromDbName}.LK_CruiseMethod AS cm USING (Method)
    LEFT JOIN {fromDbName}.TallyLedger AS tl ON 
        cust.CuttingUnitCode = tl.CuttingUnitCode
        AND tp.CruiseID = tl.CruiseID
        AND tl.StratumCode = tp.StratumCode
        AND tl.SampleGroupCode = tp.SampleGroupCode
        AND (tp.SpeciesCode IS NULL OR ifnull(tp.SpeciesCode,'') = ifnull(tl.SpeciesCode,''))
        AND (tp.LiveDead IS NULL OR ifnull(tp.LiveDead, '') = ifnull(tl.LiveDead, ''))
    WHERE cm.IsPlotMethod IS FALSE
    GROUP BY tp.CruiseID, cust.CuttingUnitCode, tp.StratumCode, tp.SampleGroupCode, tp.SpeciesCode, tp.LiveDead

    UNION ALL

    SELECT
    tp.CruiseID,
    cust.CuttingUnitCode,
    tp.StratumCode,
    tp.SampleGroupCode,
    tp.SpeciesCode,
    tp.LiveDead,
    0  AS TreeCount, 
    0 AS SumKPI
    FROM {fromDbName}.TallyPopulation AS tp
    JOIN {fromDbName}.Stratum AS st USING (StratumCode, CruiseID)
    JOIN {fromDbName}.CuttingUnit_Stratum AS cust USING (StratumCode, CruiseID)
    JOIN {fromDbName}.LK_CruiseMethod AS cm USING (Method)
    WHERE cm.IsPlotMethod IS TRUE
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