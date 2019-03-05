using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CruiseDAL.Schema.Common
{
    public partial class DDL
    {
        public const string CREATE_TABLE_POP =
            "CREATE TABLE POP( " +
                "POP_CN INTEGER PRIMARY KEY AUTOINCREMENT, " +
                "CutLeave TEXT NOT NULL, " +
                "Stratum TEXT NOT NULL, " +
                "SampleGroup TEXT NOT NULL, " +
                "PrimaryProduct TEXT NOT NULL, " +
                "SecondaryProduct TEXT NOT NULL, " +
                "STM TEXT, " +
                "UOM TEXT NOT NULL, " +
                "FirstStageTrees DOUBLE Default 0.0, " +
                "MeasuredTrees DOUBLE Default 0.0, " +
                "TalliedTrees DOUBLE Default 0.0, " +
                "SumKPI DOUBLE Default 0.0, " +
                "SumMeasuredKPI DOUBLE Default 0.0, " +
                "StageOneSamples DOUBLE Default 0.0, " +
                "StageTwoSamples DOUBLE Default 0.0, " +
                "Stg1GrossXPP DOUBLE Default 0.0, " +
                "Stg1GrossXsqrdPP DOUBLE Default 0.0, " +
                "Stg1NetXPP DOUBLE Default 0.0, " +
                "Stg1NetXsqrdPP DOUBLE Default 0.0, " +
                "Stg1ValueXPP DOUBLE Default 0.0, " +
                "Stg1ValueXsqrdPP DOUBLE Default 0.0, " +
                "Stg2GrossXPP DOUBLE Default 0.0, " +
                "Stg2GrossXsqrdPP DOUBLE Default 0.0, " +
                "Stg2NetXPP DOUBLE Default 0.0, " +
                "Stg2NetXsqrdPP DOUBLE Default 0.0, " +
                "Stg2ValueXPP DOUBLE Default 0.0, " +
                "Stg2ValueXsqrdPP DOUBLE Default 0.0, " +
                "Stg1GrossXSP DOUBLE Default 0.0, " +
                "Stg1GrossXsqrdSP DOUBLE Default 0.0, " +
                "Stg1NetXSP DOUBLE Default 0.0, " +
                "Stg1NetXsqrdSP DOUBLE Default 0.0, " +
                "Stg1ValueXSP DOUBLE Default 0.0, " +
                "Stg1ValueXsqrdSP DOUBLE Default 0.0, " +
                "Stg2GrossXSP DOUBLE Default 0.0, " +
                "Stg2GrossXsqrdSP DOUBLE Default 0.0, " +
                "Stg2NetXSP DOUBLE Default 0.0, " +
                "Stg2NetXsqrdSP DOUBLE Default 0.0, " +
                "Stg2ValueXSP DOUBLE Default 0.0, " +
                "Stg2ValueXsqrdSP DOUBLE Default 0.0, " +
                "Stg1GrossXRP DOUBLE Default 0.0, " +
                "Stg1GrossXsqrdRP DOUBLE Default 0.0, " +
                "Stg1NetXRP DOUBLE Default 0.0, " +
                "Stg1NetXRsqrdRP DOUBLE Default 0.0, " +
                "Stg1ValueXRP DOUBLE Default 0.0, " +
                "Stg1ValueXsqrdRP DOUBLE Default 0.0, " +
                "Stg2GrossXRP DOUBLE Default 0.0, " +
                "Stg2GrossXsqrdRP DOUBLE Default 0.0, " +
                "Stg2NetXRP DOUBLE Default 0.0, " +
                "Stg2NetXsqrdRP DOUBLE Default 0.0, " +
                "Stg2ValueXRP DOUBLE Default 0.0, " +
                "Stg2ValueXsqrdRP DOUBLE Default 0.0" +
            ");";
    }
}
