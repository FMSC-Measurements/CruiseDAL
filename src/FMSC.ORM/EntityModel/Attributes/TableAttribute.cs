using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
