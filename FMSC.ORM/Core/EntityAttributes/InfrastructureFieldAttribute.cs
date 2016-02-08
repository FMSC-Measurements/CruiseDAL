using System;

namespace FMSC.ORM.Core.EntityAttributes
{
    //public enum InfrastructureFieldType { RowVersion, CreatedBy, CreatedDate, ModifiedBy, ModifiedDate }
    [Flags]
    public enum PersistMode {Undefinded = 0, Never, OnInsert, OnUpdate }


    public class InfrastructureFieldAttribute : FieldAttribute
    {
        public InfrastructureFieldAttribute() : base()
        { }

        public InfrastructureFieldAttribute(string fieldName) : base(fieldName)
        { }

        public PersistMode PersistMode { get; set; }

        //public InfrastructureFieldType FieldType { get; set; }

        public bool ReadOnly { get; set; }

        //public object DefaultValue { get; set; }

    }
}
