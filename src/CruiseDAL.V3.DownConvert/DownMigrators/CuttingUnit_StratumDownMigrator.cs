
namespace CruiseDAL.DownMigrators
{
    public class CuttingUnit_StratumDownMigrator : IDownMigrator
    {
        public string CreateCommand(string toDbName, string fromDbName, string cruiseID, string createdBy)
        {
            return
$@"INSERT INTO {toDbName}.CuttingUnitStratum (
    CuttingUnit_CN, 
    Stratum_CN
)
SELECT 
    cu.CuttingUnit_CN,
    st.Stratum_CN
FROM {fromDbName}.CuttingUnit_Stratum AS cust
JOIN {fromDbName}.CuttingUnit AS cu USING (CuttingUnitCode)
JOIN {fromDbName}.Stratum AS st USING (StratumCode)
WHERE cust.CruiseID = '{cruiseID}';";
        }
    }
}
