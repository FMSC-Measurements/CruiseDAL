using System.Collections.Generic;
using System;
using System.Text;

namespace FMSC.ORM.EntityModel
{
    public class GenericEntity : Dictionary<string, object>, IDictionary<string, object>
    {
        public GenericEntity(int capacity) : base(capacity, StringComparer.InvariantCultureIgnoreCase)
        {
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            foreach(var kv in this)
            {
                sb.Append(kv.Key).Append(":\t").AppendLine(kv.Value.ToString());
            }
            return sb.ToString();
        }
    }
}