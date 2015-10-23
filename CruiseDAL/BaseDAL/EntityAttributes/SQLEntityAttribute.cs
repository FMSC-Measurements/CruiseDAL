using System;
using System.Data;

namespace CruiseDAL
{
    public enum SQLSourceType { Table, View, SubQuery }

    [AttributeUsage(AttributeTargets.Class, Inherited = true)]
    public class SQLEntityAttribute : Attribute
    {
        public string Source { get; set; }

        public SQLSourceType SourceType { get; set; }
    }

}