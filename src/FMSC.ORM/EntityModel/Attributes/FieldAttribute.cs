using System;

namespace FMSC.ORM.EntityModel.Attributes
{
    public enum SepcialFieldType { None = 0, CreatedBy, CreatedDate, ModifiedBy, ModifiedDate, RowVersion };

    [AttributeUsage(AttributeTargets.Property, Inherited = true)]
    public class FieldAttribute : BaseFieldAttribute
    {
        public string Name { get; set; }
        public PersistanceFlags PersistanceFlags { get; set; }
        public string Alias { get; set; }
        public string SQLExpression { get; set; }

        public FieldAttribute()
        {
            PersistanceFlags = PersistanceFlags.Always;
        }

        public FieldAttribute(string fieldName) : this()
        {
            this.Name = fieldName;
        }
        public override string ToString()
        {
            return String.Format("EntityFieldInfo  FieldName({0}), Expression({1})",
                  Name ?? "null", SQLExpression ?? "null");
        }
    }
}