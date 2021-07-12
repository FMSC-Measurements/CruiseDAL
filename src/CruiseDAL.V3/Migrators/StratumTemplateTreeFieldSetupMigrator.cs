namespace CruiseDAL.Migrators
{
    public class StratumTemplateTreeFieldSetupMigrator : IMigrator
    {
        public string MigrateToV3(string toDbName, string fromDbName, string cruiseID, string saleID, string deviceID)
        {
            return
$@"INSERT INTO {toDbName}.StratumTemplate (
    StratumTemplateName,
    CruiseID,
    Method
)
SELECT
    Method,
    '{cruiseID}',
    Method
FROM {fromDbName}.TreeFieldSetupDefault
GROUP BY Method;

INSERT INTO { toDbName}.StratumTemplateTreeFieldSetup (
    StratumTemplateName,
    CruiseID,
    Field,
    FieldOrder
)
SELECT
    tfsd.Method AS StratumTemplateName,
    '{cruiseID}' AS CruiseID,
    tfsd.Field,
    tfsd.FieldOrder
FROM { fromDbName}.TreeFieldSetupDefault AS tfsd
JOIN {toDbName}.TreeField AS tf USING (Field); -- join with TreeField so that we only import v3 supported tree fields";
        }
    }
}