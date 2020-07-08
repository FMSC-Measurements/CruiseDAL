using FMSC.ORM.EntityModel.Support;
using System;

namespace FMSC.ORM.EntityModel.Attributes
{
    [AttributeUsage(AttributeTargets.Property, Inherited = true)]
    public abstract class BaseFieldAttribute : Attribute
    {
        
    }
}