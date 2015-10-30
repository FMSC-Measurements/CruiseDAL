using System;
using System.Xml.Serialization;

using CruiseDAL.Core.EntityAttributes;

namespace CruiseDAL.DataObjects
{
    public class CountTree_View
    {
        [Field(FieldName="StratumCode")]
        public string StratumCode { get; set; }

        [Field(FieldName = "Method")]
        public string Method { get; set; }

        [Field(FieldName= "SampleGroupCode")]
        public string SampleGroupCode { get; set; }

        [Field(FieldName="PrimaryProduct")]
        public string PrimaryProduct { get; set; }

        [XmlIgnore]
        [Field(FieldName = "SampleGroup_CN")]
        public virtual long? SampleGroup_CN { get; set; }

        [XmlIgnore]
        [Field(FieldName = "CuttingUnit_CN")]
        public virtual long? CuttingUnit_CN { get; set; }
       
        [XmlIgnore]
        [Field(FieldName = "TreeDefaultValue_CN")]
        public virtual long? TreeDefaultValue_CN { get; set; }

        [XmlIgnore]
        [Field(FieldName = "Component_CN")]
        public virtual long? Component_CN { get; set; }
        
        [XmlElement]
        [Field(FieldName = "TreeCount")]
        public virtual Int64 TreeCount { get; set; }
       
        [XmlElement]
        [Field(FieldName = "SumKPI")]
        public virtual Int64 SumKPI { get; set; }

    }
}
