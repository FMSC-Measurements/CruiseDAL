using System;
using System.Xml.Serialization;

using FMSC.ORM.EntityModel.Attributes;

namespace CruiseDAL.DataObjects
{
    public class CountTree_View
    {
        [Field(Name="StratumCode")]
        public string StratumCode { get; set; }

        [Field(Name = "Method")]
        public string Method { get; set; }

        [Field(Name= "SampleGroupCode")]
        public string SampleGroupCode { get; set; }

        [Field(Name="PrimaryProduct")]
        public string PrimaryProduct { get; set; }

        [XmlIgnore]
        [Field(Name = "SampleGroup_CN")]
        public virtual long? SampleGroup_CN { get; set; }

        [XmlIgnore]
        [Field(Name = "CuttingUnit_CN")]
        public virtual long? CuttingUnit_CN { get; set; }
       
        [XmlIgnore]
        [Field(Name = "TreeDefaultValue_CN")]
        public virtual long? TreeDefaultValue_CN { get; set; }

        [XmlIgnore]
        [Field(Name = "Component_CN")]
        public virtual long? Component_CN { get; set; }
        
        [XmlElement]
        [Field(Name = "TreeCount")]
        public virtual Int64 TreeCount { get; set; }
       
        [XmlElement]
        [Field(Name = "SumKPI")]
        public virtual Int64 SumKPI { get; set; }

    }
}
