using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CruiseDAL.BaseDAL.EntityAttributes
{
    [AttributeUsage(AttributeTargets.Property, Inherited = true)]
    public class FieldValidationRuleAttribut : Attribute
    {
        public int Min { get; set; }
        public int Max { get; set; }
        public string[] Values { get; set; }
        public bool NotNull { get; set; }
    }
}
