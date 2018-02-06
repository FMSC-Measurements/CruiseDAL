namespace FMSC.ORM.EntityModel
{
    public interface IValidatable
    {
        void AddError(string fieldName, string message);

        void RemoveError(string fieldName, string message);
    }
}