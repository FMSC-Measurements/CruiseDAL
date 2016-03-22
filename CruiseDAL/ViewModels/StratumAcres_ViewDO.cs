using FMSC.ORM.EntityModel.Attributes;

namespace CruiseDAL.DataObjects
{
    public class StratumAcres_View 
    {
        [Field(Name="CuttingUnitCode")]
        public string CuttingUnitCode { get; set; }
        [Field(Name="StratumCode")]
        public string StratumCode { get; set; }
        [Field(Name="Area")]
        public float Area { get; set; }
        [Field(Name="CuttingUnit_CN")]
        public long CuttingUnit_CN { get; set; }
        [Field(Name="Stratum_CN")]
        public long Stratum_CN { get; set; }
    }
}
