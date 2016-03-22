using FMSC.ORM.Core.SQL;
using System;
using System.Data;

namespace FMSC.ORM.EntityModel.Attributes
{
    //public enum SQLSourceType { Table, View, Query }

    
    public class EntitySourceAttribute : EntityAttributeBase
    {
        public EntitySourceAttribute()
        {
        }

        public EntitySourceAttribute(string sourceName)
        {
            SourceName = sourceName;
        }

        public string SourceName
        {
            get;
            set;
        }

        public string JoinCommands { get; set; }

        public string Alias { get; set; }

        //public SQLSourceType SourceType { get; set; }
    }

}