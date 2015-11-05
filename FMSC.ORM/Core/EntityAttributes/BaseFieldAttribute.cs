using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace FMSC.ORM.Core.EntityAttributes
{
    [AttributeUsage(AttributeTargets.Property, Inherited = true)]
    public class BaseFieldAttribute : Attribute
    {
        
    }
}
