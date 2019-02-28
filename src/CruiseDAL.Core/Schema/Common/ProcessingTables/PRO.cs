using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CruiseDAL.Schema.Common
{
    public partial class DDL
    {
        public const string CREATE_TABLE_PRO =
            "CREATE TABLE PRO( " +
                "PRO_CN INTEGER PRIMARY KEY AUTOINCREMENT, " +
                "CutLeave TEXT NOT NULL, " +
                "Stratum TEXT NOT NULL, " +
                "CuttingUnit TEXT NOT NULL, " +
                "SampleGroup TEXT NOT NULL, " +
                "PrimaryProduct TEXT NOT NULL, " +
                "SecondaryProduct TEXT NOT NULL, " +
                "UOM TEXT NOT NULL, " +
                "STM TEXT, " +
                "FirstStageTrees DOUBLE Default 0.0, " +
                "MeasuredTrees DOUBLE Default 0.0, " +
                "TalliedTrees DOUBLE Default 0.0, " +
                "SumKPI DOUBLE Default 0.0, " +
                "SumMeasuredKPI DOUBLE Default 0.0, " +
                "ProrationFactor DOUBLE Default 0.0, " +
                "ProratedEstimatedTrees DOUBLE Default 0.0);";
    }
}
