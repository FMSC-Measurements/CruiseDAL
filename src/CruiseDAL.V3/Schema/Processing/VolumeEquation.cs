using System.Collections.Generic;

namespace CruiseDAL.Schema.Processing
{
    public class VolumeEquation : ITableDefinition
    {
        public string TableName => "VolumeEquation";

        public string CreateTable =>
@"CREATE TABLE VolumeEquation (
	VolumeEquation_CN INTEGER PRIMARY KEY AUTOINCREMENT,
	CruiseID TEXT NOT NULL COLLATE NOCASE,
	Species TEXT NOT NULL,
	PrimaryProduct TEXT NOT NULL,
	VolumeEquationNumber TEXT NOT NULL,
	StumpHeight REAL Default 0.0,
	TopDIBPrimary REAL Default 0.0,
	TopDIBSecondary REAL Default 0.0,
	CalcTotal INTEGER Default 0,
	CalcBoard INTEGER Default 0,
	CalcCubic INTEGER Default 0,
	CalcCord INTEGER Default 0,
	CalcTopwood INTEGER Default 0,
	CalcBiomass INTEGER Default 0,
	Trim REAL Default 0.0,
	SegmentationLogic INTEGER Default 0,
	MinLogLengthPrimary REAL Default 0.0,
	MaxLogLengthPrimary REAL Default 0.0,
	MinLogLengthSecondary REAL Default 0.0,
	MaxLogLengthSecondary REAL Default 0.0,
	MinMerchLength REAL Default 0.0,
	Model TEXT,
	CommonSpeciesName TEXT,
	MerchModFlag INTEGER Default 0,
	EvenOddSegment INTEGER Default 0,

    CreatedBy TEXT DEFAULT 'none',
    Created_TS DATETIME DEFAULT (CURRENT_TIMESTAMP),
    ModifiedBy TEXT,
    Modified_TS DATETIME,

	UNIQUE (CruiseID, Species, PrimaryProduct, VolumeEquationNumber)
);";

        public string InitializeTable => null;

        public string CreateTombstoneTable =>
@"CREATE TABLE VolumeEquation_Tombstone (
	CruiseID TEXT NOT NULL COLLATE NOCASE,
	Species TEXT NOT NULL,
	PrimaryProduct TEXT NOT NULL,
	VolumeEquationNumber TEXT NOT NULL,
	StumpHeight REAL,
	TopDIBPrimary REAL,
	TopDIBSecondary REAL,
	CalcTotal INTEGER,
	CalcBoard INTEGER,
	CalcCubic INTEGER,
	CalcCord INTEGER,
	CalcTopwood INTEGER,
	CalcBiomass INTEGER,
	Trim REAL,
	SegmentationLogic INTEGER,
	MinLogLengthPrimary REAL,
	MaxLogLengthPrimary REAL,
	MinLogLengthSecondary REAL,
	MaxLogLengthSecondary REAL,
	MinMerchLength REAL,
	Model TEXT,
	CommonSpeciesName TEXT,
	MerchModFlag INTEGER,
	EvenOddSegment INTEGER,
    CreatedBy TEXT,
    Created_TS DATETIME,
    ModifiedBy TEXT,
    Modified_TS DATETIME,
	Deleted_TS DATETIME
);";

        public string CreateIndexes =>
@"CREATE INDEX NIX_VolumeEquation_CruiseID ON VolumeEquation (CruiseID);";

        public IEnumerable<string> CreateTriggers =>
            new[]
            {
@"CREATE TRIGGER VolumeEquation_OnUpdate
AFTER UPDATE OF
		StumpHeight,
		TopDIBPrimary,
		TopDIBSecondary,
		CalcTotal,
		CalcBoard,
		CalcCubic,
		CalcCord,
		CalcTopwood,
		CalcBiomass,
		Trim,
		SegmentationLogic,
		MinLogLengthPrimary,
		MaxLogLengthPrimary,
		MinLogLengthSecondary,
		MaxLogLengthSecondary,
		MinMerchLength,
		Model,
		CommonSpeciesName,
		MerchModFlag,
		EvenOddSegment
ON VolumeEquation
FOR EACH ROW
BEGIN
	UPDATE VolumeEquation SET Modified_TS = CURRENT_TIMESTAMP WHERE VolumeEquation_CN = OLD.VolumeEquation_CN;
END;",

@"CREATE TRIGGER VolumeEquation_OnDelete
BEFORE DELETE ON VolumeEquation
FOR EACH ROW
BEGIN
	INSERT OR REPLACE INTO VolumeEquation_Tombstone (
		CruiseID,
		Species,
		PrimaryProduct,
		VolumeEquationNumber,
		StumpHeight,
		TopDIBPrimary,
		TopDIBSecondary,
		CalcTotal,
		CalcBoard,
		CalcCubic,
		CalcCord,
		CalcTopwood,
		CalcBiomass,
		Trim,
		SegmentationLogic,
		MinLogLengthPrimary,
		MaxLogLengthPrimary,
		MinLogLengthSecondary,
		MaxLogLengthSecondary,
		MinMerchLength,
		Model,
		CommonSpeciesName,
		MerchModFlag,
		EvenOddSegment,
		CreatedBy,
		Created_TS,
		ModifiedBy,
		Modified_TS,
		Deleted_TS
	) VALUES (
		OLD.CruiseID,
		OLD.Species,
		OLD.PrimaryProduct,
		OLD.VolumeEquationNumber,
		OLD.StumpHeight,
		OLD.TopDIBPrimary,
		OLD.TopDIBSecondary,
		OLD.CalcTotal,
		OLD.CalcBoard,
		OLD.CalcCubic,
		OLD.CalcCord,
		OLD.CalcTopwood,
		OLD.CalcBiomass,
		OLD.Trim,
		OLD.SegmentationLogic,
		OLD.MinLogLengthPrimary,
		OLD.MaxLogLengthPrimary,
		OLD.MinLogLengthSecondary,
		OLD.MaxLogLengthSecondary,
		OLD.MinMerchLength,
		OLD.Model,
		OLD.CommonSpeciesName,
		OLD.MerchModFlag,
		OLD.EvenOddSegment,
		OLD.CreatedBy,
		OLD.Created_TS,
		OLD.ModifiedBy,
		OLD.Modified_TS,
		CURRENT_TIMESTAMP
	);
END;
",
            };
    }
}