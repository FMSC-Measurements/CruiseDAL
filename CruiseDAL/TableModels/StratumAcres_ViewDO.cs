using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CruiseDAL.DataObjects
{
    public class StratumAcres_View 
    {
        [Field(FieldName="CuttingUnitCode")]
        public string CuttingUnitCode { get; set; }
        [Field(FieldName="StratumCode")]
        public string StratumCode { get; set; }
        [Field(FieldName="Area")]
        public float Area { get; set; }
        [Field(FieldName="CuttingUnit_CN")]
        public long CuttingUnit_CN { get; set; }
        [Field(FieldName="Stratum_CN")]
        public long Stratum_CN { get; set; }
    }
}
