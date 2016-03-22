namespace FMSC.ORM.EntityModel.Attributes
{
    public enum KeyType { None = 0, RowID, GUID}

    public class PrimaryKeyFieldAttribute : FieldAttribute
    {
        public KeyType KeyType { get; set; }
    }
}
