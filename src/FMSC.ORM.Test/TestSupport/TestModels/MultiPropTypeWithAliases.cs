using FMSC.ORM.EntityModel.Attributes;

namespace FMSC.ORM.TestSupport.TestModels
{
    public class MultiPropTypeWithAliases : POCOMultiTypeObject
    {
        [Field(Alias = "AliasForStringField", SQLExpression = "StringField")]
        public string AliasForStringField { get; set; }
    }
}