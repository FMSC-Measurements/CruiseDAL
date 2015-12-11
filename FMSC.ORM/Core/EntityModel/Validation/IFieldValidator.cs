namespace FMSC.ORM.Core.EntityModel
{
    public interface IFieldValidator
    {
        string Field { get; }

        string TableName { get; }

        string ErrorMessage { get; }

        ErrorLevel Level { get; }

        bool Validate(object sender, object value);
    }
}
