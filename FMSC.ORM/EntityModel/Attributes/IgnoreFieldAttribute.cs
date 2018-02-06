namespace FMSC.ORM.EntityModel.Attributes
{
    public class IgnoreFieldAttribute : BaseFieldAttribute
    {
        public override string GetResultColumnExpression()
        {
            return null;
        }
    }
}