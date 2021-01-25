namespace FMSC.ORM.EntityModel.Attributes
{
    public class PrimaryKeyFieldAttribute : FieldAttribute
    {
        public PrimaryKeyFieldAttribute() : base()
        { 
            PersistanceFlags = PersistanceFlags.OnInsert; 
        }

        public PrimaryKeyFieldAttribute(string fieldName) : base(fieldName)
        {
            PersistanceFlags = PersistanceFlags.OnInsert;
        }
    }
}