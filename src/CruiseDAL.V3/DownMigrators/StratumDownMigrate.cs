namespace CruiseDAL.DownMigrators
{
    public class StratumDownMigrate : IDownMigrator
    {
        public string CreateCommand(string toDbName, string fromDbName, string cruiseID, string createdBy)
        {
            return
$@"INSERT INTO {toDbName}.Stratum (
    Stratum_CN,
    Code,
    Description,
    Method,
    BasalAreaFactor,
    FixedPlotSize,
    KZ3PPNT,
    SamplingFrequency,
    Hotkey,
    FBSCode,
    YieldComponent,
    Month,
    Year,
    CreatedBy
)
SELECT 
    Stratum_CN,
    StratumCode,
    Description,
    Method,
    BasalAreaFactor,
    FixedPlotSize,
    KZ3PPNT,
    SamplingFrequency,
    HotKey,
    FBSCode,
    YieldComponent,
    Month,
    Year,
    '{createdBy}'
FROM {fromDbName}.Stratum
WHERE CruiseID = '{cruiseID}';

INSERT INTO {toDbName}.FixCNTTallyClass (
    Stratum_CN,
    FieldName
)
SELECT 
    Stratum_CN, 
    FixCNTField
FROM {fromDbName}.Stratum
WHERE CruiseID = '{cruiseID}' AND FixCNTField IS NOT NULL AND Method = 'FIXCNT';
";
        }
    }
}
