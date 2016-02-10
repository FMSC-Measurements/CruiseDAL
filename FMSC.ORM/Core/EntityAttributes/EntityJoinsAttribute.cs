using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FMSC.ORM.Core.EntityAttributes
{
    public class EntityJoinsAttribute : EntityAttributeBase
    {
        public string JoinSouce { get; set; }
        public string Constraint { get; set; }
        public string Alias { get; set; }
    }
}
