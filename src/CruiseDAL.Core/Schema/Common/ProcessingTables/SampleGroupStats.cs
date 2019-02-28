using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CruiseDAL.Schema.Common
{
    public partial class DDL
    {
        public const string CREATE_TABLE_SAMPLEGROUPSTATS =
            "CREATE TABLE SampleGroupStats( " +
                "SampleGroupStats_CN INTEGER PRIMARY KEY AUTOINCREMENT, " +
                "StratumStats_CN INTEGER, " +
                "Code TEXT, " +
                "SgSet INTEGER Default 0, " +
                "Description TEXT, " +
                "CutLeave TEXT, " +
                "UOM TEXT, " +
                "PrimaryProduct TEXT, " +
                "SecondaryProduct TEXT, " +
                "DefaultLiveDead TEXT, " +
                "SgError REAL Default 0.0, " +
                "SampleSize1 INTEGER Default 0, " +
                "SampleSize2 INTEGER Default 0, " +
                "CV1 REAL Default 0.0, " +
                "CV2 REAL Default 0.0, " +
                "TreesPerAcre REAL Default 0.0, " +
                "VolumePerAcre REAL Default 0.0, " +
                "TreesPerPlot REAL Default 0.0, " +
                "AverageHeight REAL Default 0.0, " +
                "SamplingFrequency INTEGER Default 0, " +
                "InsuranceFrequency INTEGER Default 0, " +
                "KZ INTEGER Default 0, " +
                "BigBAF REAL Default 0.0, " +
                "BigFIX INTEGER Default 0, " +
                "MinDbh REAL Default 0.0, " +
                "MaxDbh REAL Default 0.0, " +
                "CV_Def INTEGER Default 0, " +
                "CV2_Def INTEGER Default 0, " +
                "TPA_Def INTEGER Default 0, " +
                "VPA_Def INTEGER Default 0, " +
                "ReconPlots INTEGER Default 0, " +
                "ReconTrees INTEGER Default 0, " +
                "UNIQUE (StratumStats_CN, Code, SgSet), " +
            "FOREIGN KEY (StratumStats_CN) REFERENCES StratumStats (StratumStats_CN) ON DELETE CASCADE" +
            ");";
    }
}
