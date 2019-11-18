using System.Collections.Generic;
using System;

namespace FMSC.ORM.EntityModel
{
    public class GenericEntity : Dictionary<string, object>, IDictionary<string, object>
    {
        public GenericEntity(int capacity) : base(capacity, StringComparer.InvariantCultureIgnoreCase)
        {
        }
    }
}