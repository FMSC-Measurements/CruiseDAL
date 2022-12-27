using FMSC.ORM.EntityModel.Attributes;

namespace CruiseDAL.V3.Models
{
    public class LogEx : Log
    {
        [Field(Alias = "CuttingUnitCode", SQLExpression = "Tree.CuttingUnitCode", PersistanceFlags = PersistanceFlags.Never)]
        public string CuttingUnitCode { get; set; }

        [Field(Alias = "PlotNumber", SQLExpression = "Tree.PlotNumber", PersistanceFlags = PersistanceFlags.Never)]
        public int? PlotNumber { get; set; }

        [Field(Alias = "TreeNumber", SQLExpression = "Tree.TreeNumber", PersistanceFlags = PersistanceFlags.Never)]
        public int? TreeNumber { get; set; }
    }
}