namespace CruiseDAL.Migrators
{
    public class StratumTemplateLogFieldSetupMigrator : IMigrator
    {
        public string MigrateToV3(string toDbName, string fromDbName, string cruiseID, string saleID, string deviceID)
        {
            return
$@"
INSERT INTO {toDbName}.StratumTemplate (
    StratumTemplateName,
    CruiseID
) VALUES (
    'Default Log Field Setup Template',
    '{cruiseID}'
);

INSERT INTO {toDbName}.StratumTemplateLogFieldSetup (
    StratumTemplateName,
    CruiseID,
    Field,
    FieldOrder
)
SELECT
    'Default Log Field Setup Template' AS StratumTemplateName,
    '{cruiseID}',
    Field,
    FieldOrder
FROM {fromDbName}.LogFieldSetupDefault
JOIN {toDbName}.LogField USING (Field); -- join with LogField so that we only import V3 supported log fields";
        }
    }
}