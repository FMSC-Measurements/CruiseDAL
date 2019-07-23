using System;

namespace FMSC.ORM.EntityModel.Attributes
{
    [Obsolete("Use Table intead")]
    public class EntitySourceAttribute : TableAttribute
    {
        public EntitySourceAttribute()
        {
        }

        public EntitySourceAttribute(string sourceName) : base(sourceName)
        { }
        

        public string SourceName
        {
            get { return base.Name; }
            set { base.Name = value; }
        }

        public string JoinCommands { get; set; }

        public string Alias { get; set; }

        //public SQLSourceType SourceType { get; set; }
    }
}