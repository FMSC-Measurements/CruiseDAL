namespace FMSC.ORM.EntityModel.Attributes
{
    public enum KeyType { None = 0, RowID, GUID }

    public class PrimaryKeyFieldAttribute : FieldAttribute
    {
        public PrimaryKeyFieldAttribute() : base()
        { }

        public PrimaryKeyFieldAttribute(string fieldName) : base(fieldName)
        {
        }

        public KeyType KeyType { get; set; }
    }
}