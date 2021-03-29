namespace CruiseDAL.DownMigrators
{
    public class PlotDownMigrator : IDownMigrator
    {
        public string CreateCommand(string toDbName, string fromDbName, string cruiseID, string createdBy)
        {
            return
$@"INSERT INTO {toDbName}.Plot (
				Plot_CN,
				Plot_GUID,
				Stratum_CN,
				CuttingUnit_CN,
				PlotNumber,
				IsEmpty,
				Slope,
				KPI,
				Aspect,
				Remarks,
				ThreePRandomValue
)
SELECT
	Plot_Stratum_CN AS Plot_CN,
	null AS Plot_GUID,
	st.Stratum_CN,
	cu.CuttingUnit_CN,
	ps.PlotNumber,
	(CASE ps.IsEmpty WHEN 0 THEN 'False' ELSE 'True' END) AS IsEmpty,
	p.Slope,
    ps.KPI,
	p.Aspect,
    p.Remarks,
	ps.ThreePRandomValue
FROM {fromDbName}.Plot_Stratum AS ps
JOIN {fromDbName}.Plot AS p  USING (PlotNumber, CuttingUnitCode)
JOIN {fromDbName}.Stratum AS st USING (StratumCode)
JOIN {fromDbName}.CuttingUnit AS cu USING (CuttingUnitCode)
WHERE ps.CruiseID = '{cruiseID}'
;";
        }
    }
}