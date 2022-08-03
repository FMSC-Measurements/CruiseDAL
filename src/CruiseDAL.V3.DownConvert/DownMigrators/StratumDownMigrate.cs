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
    ifnull(BasalAreaFactor, 0.0) AS BasalAreaFactor,
    ifnull(FixedPlotSize, 0.0) AS FixedPlotSize,
    ifnull(KZ3PPNT, 0) AS KZ3PPNT,
    ifnull(SamplingFrequency, 0) AS SamplingFrequency,
    HotKey,
    FBSCode,
    YieldComponent,
    (SELECT strftime('%m', min(Created_TS)) FROM {fromDbName}.TallyLedger WHERE CruiseID =  '{cruiseID}' AND StratumCode = st.StratumCode AND EntryType = 'tally'),
    (SELECT strftime('%Y', min(Created_TS)) FROM {fromDbName}.TallyLedger WHERE CruiseID =  '{cruiseID}' AND StratumCode = st.StratumCode AND EntryType = 'tally'),
    '{createdBy}'
FROM {fromDbName}.Stratum AS st
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
