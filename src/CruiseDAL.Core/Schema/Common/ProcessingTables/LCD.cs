using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CruiseDAL.Schema.Common
{
    public partial class DDL
    {
        public const string CREATE_TABLE_LCD =
            "CREATE TABLE LCD( " +
                "LCD_CN INTEGER PRIMARY KEY AUTOINCREMENT, " +
                "CutLeave TEXT NOT NULL, " +
                "Stratum TEXT NOT NULL, " +
                "SampleGroup TEXT NOT NULL, " +
                "Species TEXT NOT NULL, " +
                "PrimaryProduct TEXT NOT NULL, " +
                "SecondaryProduct TEXT NOT NULL, " +
                "UOM TEXT NOT NULL, " +
                "LiveDead TEXT NOT NULL, " +
                "Yield TEXT NOT NULL, " +
                "ContractSpecies TEXT NOT NULL, " +
                "TreeGrade TEXT NOT NULL, " +
                "STM TEXT, " +
                "FirstStageTrees DOUBLE Default 0.0, " +
                "MeasuredTrees DOUBLE Default 0.0, " +
                "TalliedTrees DOUBLE Default 0.0, " +
                "SumKPI DOUBLE Default 0.0, " +
                "SumMeasuredKPI DOUBLE Default 0.0, " +
                "SumExpanFactor DOUBLE Default 0.0, " +
                "SumDBHOB DOUBLE Default 0.0, " +
                "SumDBHOBsqrd DOUBLE Default 0.0, " +
                "SumTotHgt DOUBLE Default 0.0, " +
                "SumHgtUpStem DOUBLE Default 0.0, " +
                "SumMerchHgtPrim DOUBLE Default 0.0, " +
                "SumMerchHgtSecond DOUBLE Default 0.0, " +
                "SumLogsMS DOUBLE Default 0.0, " +
                "SumTotCubic DOUBLE Default 0.0, " +
                "SumGBDFT DOUBLE Default 0.0, " +
                "SumNBDFT DOUBLE Default 0.0, " +
                "SumGCUFT DOUBLE Default 0.0, " +
                "SumNCUFT DOUBLE Default 0.0, " +
                "SumGBDFTremv DOUBLE Default 0.0, " +
                "SumGCUFTremv DOUBLE Default 0.0, " +
                "SumCords DOUBLE Default 0.0, " +
                "SumWgtMSP DOUBLE Default 0.0, " +
                "SumValue DOUBLE Default 0.0, " +
                "SumGBDFTtop DOUBLE Default 0.0, " +
                "SumNBDFTtop DOUBLE Default 0.0, " +
                "SumGCUFTtop DOUBLE Default 0.0, " +
                "SumNCUFTtop DOUBLE Default 0.0, " +
                "SumCordsTop DOUBLE Default 0.0, " +
                "SumWgtMSS DOUBLE Default 0.0, " +
                "SumTopValue DOUBLE Default 0.0, " +
                "SumLogsTop DOUBLE Default 0.0, " +
                "SumBDFTrecv DOUBLE Default 0.0, " +
                "SumCUFTrecv DOUBLE Default 0.0, " +
                "SumCordsRecv DOUBLE Default 0.0, " +
                "SumValueRecv DOUBLE Default 0.0, " +
                "BiomassProduct DOUBLE Default 0.0, " +
                "SumWgtBAT DOUBLE Default 0.0, " +
                "SumWgtBBL DOUBLE Default 0.0, " +
                "SumWgtBBD DOUBLE Default 0.0, " +
                "SumWgtBFT DOUBLE Default 0.0, " +
                "SumWgtTip DOUBLE Default 0.0, " +
                "SumTipwood DOUBLE Default 0.0" +
            ");";
    }
}
