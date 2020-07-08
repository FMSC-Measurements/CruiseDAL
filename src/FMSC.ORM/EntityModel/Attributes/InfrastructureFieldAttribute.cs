using System;

namespace FMSC.ORM.EntityModel.Attributes
{
    public abstract class InfrastructureFieldAttribute : FieldAttribute
    {
        public InfrastructureFieldAttribute() : base()
        { }

        public InfrastructureFieldAttribute(string fieldName) : base(fieldName)
        { }

        public abstract Func<object> DefaultValueProvider { get; }
    }
}