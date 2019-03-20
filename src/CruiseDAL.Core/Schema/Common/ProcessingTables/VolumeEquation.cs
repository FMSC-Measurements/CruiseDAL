namespace CruiseDAL.Schema
{
    public partial class DDL
    {
        public const string CREATE_TABLE_VOLUMEEQUATION =
            "CREATE TABLE VolumeEquation( " +
                "Species TEXT NOT NULL, " +
                "PrimaryProduct TEXT NOT NULL, " +
                "VolumeEquationNumber TEXT NOT NULL, " +
                "StumpHeight REAL Default 0.0, " +
                "TopDIBPrimary REAL Default 0.0, " +
                "TopDIBSecondary REAL Default 0.0, " +
                "CalcTotal INTEGER Default 0, " +
                "CalcBoard INTEGER Default 0, " +
                "CalcCubic INTEGER Default 0, " +
                "CalcCord INTEGER Default 0, " +
                "CalcTopwood INTEGER Default 0, " +
                "CalcBiomass INTEGER Default 0, " +
                "Trim REAL Default 0.0, " +
                "SegmentationLogic INTEGER Default 0, " +
                "MinLogLengthPrimary REAL Default 0.0, " +
                "MaxLogLengthPrimary REAL Default 0.0, " +
                "MinLogLengthSecondary REAL Default 0.0, " +
                "MaxLogLengthSecondary REAL Default 0.0, " +
                "MinMerchLength REAL Default 0.0, " +
                "Model TEXT, " +
                "CommonSpeciesName TEXT, " +
                "MerchModFlag INTEGER Default 0, " +
                "EvenOddSegment INTEGER Default 0, " +
                "UNIQUE (Species, PrimaryProduct, VolumeEquationNumber)" +
            ");";
    }
}