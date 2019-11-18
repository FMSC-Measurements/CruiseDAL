using System;

namespace FMSC.ORM.EntityModel.Attributes
{
    public class TableAttribute : EntityAttributeBase
    {
        public TableAttribute()
        {
        }

        public TableAttribute(string name)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
        }

        public string Name { get; set; }
    }
}