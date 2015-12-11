using System;
using System.Data;

namespace FMSC.ORM.Core.EntityAttributes
{
    public enum SQLSourceType { Table, View, Query }

    [AttributeUsage(AttributeTargets.Class, Inherited = true)]
    public class SQLEntityAttribute : Attribute
    {
        public string SourceName { get; set; }

        public SQLSourceType SourceType { get; set; }
    }

}