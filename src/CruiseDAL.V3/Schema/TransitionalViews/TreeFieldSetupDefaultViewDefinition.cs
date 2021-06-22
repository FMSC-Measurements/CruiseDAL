namespace CruiseDAL.Schema
{
    public class TreeFieldSetupDefaultViewDefinition : IViewDefinition
    {
        public string ViewName => "TreeFieldSetupDefault";

        public string CreateView =>
@"CREATE VIEW TreeFieldSetupDefault AS
    SELECT
    0 AS TreeFieldSetupDefault_CN,
    StratumTemplateName || CruiseID AS StratumDefaultID,
    null AS SampleGroupDefaultID,
    Field,
    FieldOrder,
    IsHidden,
    IsLocked,
    -- value type determined by TreeField.DbType
    DefaultValueInt,
    DefaultValueReal,
    DefaultValueBool,
    DefaultValueText
FROM StratumTemplateTreeFieldSetup;";
    }
}