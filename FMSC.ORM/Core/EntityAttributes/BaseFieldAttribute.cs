using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace FMSC.ORM.Core.EntityAttributes
{
    [AttributeUsage(AttributeTargets.Property, Inherited = true)]
    public class BaseFieldAttribute : Attribute
    {        
    }
}
