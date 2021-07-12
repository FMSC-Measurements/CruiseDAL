namespace CruiseDAL.Schema
{
    public class LogFieldSetupDefaultViewDefinition : IViewDefinition
    {
        public string ViewName => "LogFieldSetupDefault";

        public string CreateView =>
@"CREATE VIEW LogFieldSetupDefault AS
SELECT
    StratumTemplateLogFieldSetup_CN AS LogFieldSetupDefault_CN,
    StratumTemplateName || CruiseID AS StratumDefaultID,
    Field,
    FieldOrder
FROM StratumTemplateLogFieldSetup;";
    }
}