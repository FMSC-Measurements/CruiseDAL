﻿namespace CruiseDAL.Schema
{
    public partial class DDL
    {
        public const string CREATE_TABLE_STRATUMSTATS =
            "CREATE TABLE StratumStats( " +
                "StratumStats_CN INTEGER PRIMARY KEY AUTOINCREMENT, " +
                "Stratum_CN INTEGER NOT NULL, " +
                "Code TEXT, " +
                "Description TEXT, " +
                "Method TEXT, " +
                "SgSet INTEGER Default 0, " +
                "SgSetDescription TEXT, " +
                "BasalAreaFactor REAL Default 0.0, " +
                "FixedPlotSize REAL Default 0.0, " +
                "StrError REAL Default 0.0, " +
                "SampleSize1 INTEGER Default 0, " +
                "SampleSize2 INTEGER Default 0, " +
                "WeightedCV1 REAL Default 0.0, " +
                "WeightedCV2 REAL Default 0.0, " +
                "TreesPerAcre REAL Default 0.0, " +
                "VolumePerAcre REAL Default 0.0, " +
                "TotalVolume REAL Default 0.0, " +
                "TotalAcres REAL Default 0.0, " +
                "PlotSpacing INTEGER Default 0, " +
                "Used INTEGER Default 0, " +
                "UNIQUE (Code, Method, SgSet)," +
                "FOREIGN KEY (Stratum_CN) REFERENCES Stratum (Stratum_CN) ON DELETE CASCADE" +
            ");";

        public const string CREATE_INDEX_StratumStats_Stratum_CN =
            @"CREATE INDEX 'StratumStats_Stratum_CN' ON 'StratumStats'('Stratum_CN');";
    }
}