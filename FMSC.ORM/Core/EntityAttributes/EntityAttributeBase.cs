using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FMSC.ORM.Core.EntityAttributes
{
    [AttributeUsage(AttributeTargets.Class, Inherited = true)]
    public class EntityAttributeBase : Attribute
    {
    }
}
