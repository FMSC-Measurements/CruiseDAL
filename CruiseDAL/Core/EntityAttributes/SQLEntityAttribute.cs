using System;
using System.Data;

namespace CruiseDAL
{
    public enum SQLSourceType { Table, View, Query }

    [AttributeUsage(AttributeTargets.Class, Inherited = true)]
    public class SQLEntityAttribute : Attribute
    {
        public string SourceName { get; set; }

        public SQLSourceType SourceType { get; set; }
    }

}