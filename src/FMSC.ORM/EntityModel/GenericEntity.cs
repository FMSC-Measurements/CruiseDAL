using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FMSC.ORM.EntityModel
{
    public class GenericEntity : Dictionary<string, object>, IDictionary<string, object>
    {
        public GenericEntity(int capacity) : base(capacity, StringComparer.InvariantCultureIgnoreCase)
        {
        }
    }
}
