namespace CruiseDAL.V2.Updater.Test.SQL
{
	public static class Create_2_1_1
	{
		public static string CreateCruise =
@"PRAGMA foreign_keys = ON;--is the nessicary? no data is being inserted yet.

--Core Tables--
    CREATE TABLE Sale (
				Sale_CN INTEGER PRIMARY KEY AUTOINCREMENT,
				SaleNumber TEXT NOT NULL,
				Name TEXT,
				Purpose TEXT,
				Region TEXT NOT NULL,
				Forest TEXT NOT NULL,
				District TEXT NOT NULL,
				MeasurementYear TEXT,
				CalendarYear INTEGER Default 0,
				LogGradingEnabled BOOLEAN Default 0,
				Remarks TEXT,
				DefaultUOM TEXT,
				CreatedBy TEXT DEFAULT 'none',
				CreatedDate DateTime DEFAULT (datetime('now')) ,
				ModifiedBy TEXT ,
				ModifiedDate DateTime ,
				RowVersion INTEGER DEFAULT 0,
				UNIQUE (SaleNumber));

    CREATE TABLE CuttingUnit (
				CuttingUnit_CN INTEGER PRIMARY KEY AUTOINCREMENT,
				Code TEXT NOT NULL,
				Area REAL NOT NULL,
				Description TEXT,
				LoggingMethod TEXT,
				PaymentUnit TEXT,
				TallyHistory TEXT,
				CreatedBy TEXT DEFAULT 'none',
				CreatedDate DateTime DEFAULT (datetime('now')) ,
				ModifiedBy TEXT ,
				ModifiedDate DateTime ,
				RowVersion INTEGER DEFAULT 0,
				UNIQUE (Code));

    CREATE TABLE Stratum (
				Stratum_CN INTEGER PRIMARY KEY AUTOINCREMENT,
				Code TEXT NOT NULL,
				Description TEXT,
				Method TEXT NOT NULL,
				BasalAreaFactor REAL Default 0.0,
				FixedPlotSize REAL Default 0.0,
				KZ3PPNT INTEGER Default 0,
				Hotkey TEXT,
				FBSCode TEXT,
				YieldComponent TEXT Default ""CL"",
				Month INTEGER Default 0,
				Year INTEGER Default 0,
				CreatedBy TEXT DEFAULT 'none',
				CreatedDate DateTime DEFAULT (datetime('now')) ,
				ModifiedBy TEXT ,
				ModifiedDate DateTime ,
				RowVersion INTEGER DEFAULT 0,
				UNIQUE (Code));

    CREATE TABLE CuttingUnitStratum (
				CuttingUnit_CN INTEGER REFERENCES CuttingUnit NOT NULL,
				Stratum_CN INTEGER REFERENCES Stratum NOT NULL,
				StratumArea REAL,
				UNIQUE (CuttingUnit_CN, Stratum_CN));

    CREATE TABLE SampleGroup (
				SampleGroup_CN INTEGER PRIMARY KEY AUTOINCREMENT,
				Stratum_CN INTEGER REFERENCES Stratum NOT NULL,
				Code TEXT NOT NULL,
				CutLeave TEXT NOT NULL,
				UOM TEXT NOT NULL,
				PrimaryProduct TEXT NOT NULL,
				SecondaryProduct TEXT,
				BiomassProduct TEXT,
				DefaultLiveDead TEXT,
				SamplingFrequency INTEGER Default 0,
				InsuranceFrequency INTEGER Default 0,
				KZ INTEGER Default 0,
				BigBAF REAL Default 0.0,
				SmallFPS REAL Default 0.0,
				TallyMethod TEXT Default 0,
				Description TEXT,
				SampleSelectorType TEXT,
				SampleSelectorState TEXT,
				MinKPI INTEGER Default 0,
				MaxKPI INTEGER Default 0,
				CreatedBy TEXT DEFAULT 'none',
				CreatedDate DateTime DEFAULT (datetime('now')) ,
				ModifiedBy TEXT ,
				ModifiedDate DateTime ,
				RowVersion INTEGER DEFAULT 0,
				UNIQUE (Stratum_CN, Code));

    CREATE TABLE TreeDefaultValue (
				TreeDefaultValue_CN INTEGER PRIMARY KEY AUTOINCREMENT,
				PrimaryProduct TEXT NOT NULL,
				Species TEXT NOT NULL,
				LiveDead TEXT NOT NULL,
				FIAcode INTEGER Default 0,
				CullPrimary REAL Default 0.0,
				HiddenPrimary REAL Default 0.0,
				CullSecondary REAL Default 0.0,
				HiddenSecondary REAL Default 0.0,
				Recoverable REAL Default 0.0,
				Chargeable TEXT,
				ContractSpecies TEXT,
				TreeGrade TEXT Default ""0"",
				MerchHeightLogLength INTEGER Default 0,
				MerchHeightType TEXT Default ""F"",
				FormClass REAL Default 0.0,
				BarkThicknessRatio REAL Default 0.0,
				AverageZ REAL Default 0.0,
				ReferenceHeightPercent REAL Default 0.0,
				CreatedBy TEXT DEFAULT 'none',
				CreatedDate DateTime DEFAULT (datetime('now')) ,
				ModifiedBy TEXT ,
				ModifiedDate DateTime ,
				RowVersion INTEGER DEFAULT 0,
				UNIQUE (PrimaryProduct, Species, LiveDead, Chargeable));

    CREATE TABLE SampleGroupTreeDefaultValue (
				TreeDefaultValue_CN INTEGER REFERENCES TreeDefaultValue,
				SampleGroup_CN INTEGER REFERENCES SampleGroup,
				UNIQUE (TreeDefaultValue_CN, SampleGroup_CN));

    CREATE TABLE Plot (
				Plot_CN INTEGER PRIMARY KEY AUTOINCREMENT,
				Plot_GUID TEXT,
				Stratum_CN INTEGER REFERENCES Stratum NOT NULL,
				CuttingUnit_CN INTEGER REFERENCES CuttingUnit NOT NULL,
				PlotNumber INTEGER NOT NULL,
				IsEmpty TEXT,
				Slope REAL Default 0.0,
				KPI REAL Default 0.0,
				Aspect REAL Default 0.0,
				Remarks TEXT,
				XCoordinate REAL Default 0.0,
				YCoordinate REAL Default 0.0,
				ZCoordinate REAL Default 0.0,
				MetaData TEXT,
				Blob BLOB,
				CreatedBy TEXT DEFAULT 'none',
				CreatedDate DateTime DEFAULT (datetime('now')) ,
				ModifiedBy TEXT ,
				ModifiedDate DateTime ,
				RowVersion INTEGER DEFAULT 0,
				UNIQUE (Stratum_CN, CuttingUnit_CN, PlotNumber));

    CREATE TABLE Tree (
				Tree_CN INTEGER PRIMARY KEY AUTOINCREMENT,
				Tree_GUID TEXT,
				TreeDefaultValue_CN INTEGER REFERENCES TreeDefaultValue,
				Stratum_CN INTEGER REFERENCES Stratum NOT NULL,
				SampleGroup_CN INTEGER REFERENCES SampleGroup,
				CuttingUnit_CN INTEGER REFERENCES CuttingUnit NOT NULL,
				Plot_CN INTEGER REFERENCES Plot,
				TreeNumber INTEGER NOT NULL,
				Species TEXT,
				CountOrMeasure TEXT,
				TreeCount REAL Default 0.0,
				KPI REAL Default 0.0,
				STM TEXT Default ""N"",
				SeenDefectPrimary REAL Default 0.0,
				SeenDefectSecondary REAL Default 0.0,
				RecoverablePrimary REAL Default 0.0,
				HiddenPrimary REAL Default 0.0,
				Initials TEXT,
				LiveDead TEXT,
				Grade TEXT,
				HeightToFirstLiveLimb REAL Default 0.0,
				PoleLength REAL Default 0.0,
				ClearFace TEXT,
				CrownRatio REAL Default 0.0,
				DBH REAL Default 0.0,
				DRC REAL Default 0.0,
				TotalHeight REAL Default 0.0,
				MerchHeightPrimary REAL Default 0.0,
				MerchHeightSecondary REAL Default 0.0,
				FormClass REAL Default 0.0,
				UpperStemDOB REAL Default 0.0,
				UpperStemDiameter REAL Default 0.0,
				UpperStemHeight REAL Default 0.0,
				DBHDoubleBarkThickness REAL Default 0.0,
				TopDIBPrimary REAL Default 0.0,
				TopDIBSecondary REAL Default 0.0,
				DefectCode TEXT,
				DiameterAtDefect REAL Default 0.0,
				VoidPercent REAL Default 0.0,
				Slope REAL Default 0.0,
				Aspect REAL Default 0.0,
				Remarks TEXT,
				XCoordinate DOUBLE Default 0.0,
				YCoordinate DOUBLE Default 0.0,
				ZCoordinate DOUBLE Default 0.0,
				MetaData TEXT,
				IsFallBuckScale INTEGER Default 0,
				ExpansionFactor REAL Default 0.0,
				TreeFactor REAL Default 0.0,
				PointFactor REAL Default 0.0,
				CreatedBy TEXT DEFAULT 'none',
				CreatedDate DateTime DEFAULT (datetime('now')) ,
				ModifiedBy TEXT ,
				ModifiedDate DateTime ,
				RowVersion INTEGER DEFAULT 0,
				UNIQUE (TreeDefaultValue_CN, Stratum_CN, SampleGroup_CN, CuttingUnit_CN, Plot_CN, TreeNumber));

    CREATE TABLE Log (
				Log_CN INTEGER PRIMARY KEY AUTOINCREMENT,
				Log_GUID TEXT,
				Tree_CN INTEGER REFERENCES Tree NOT NULL,
				LogNumber TEXT NOT NULL,
				Grade TEXT,
				SeenDefect REAL Default 0.0,
				PercentRecoverable REAL Default 0.0,
				Length INTEGER Default 0,
				ExportGrade TEXT,
				SmallEndDiameter REAL Default 0.0,
				LargeEndDiameter REAL Default 0.0,
				GrossBoardFoot REAL Default 0.0,
				NetBoardFoot REAL Default 0.0,
				GrossCubicFoot REAL Default 0.0,
				NetCubicFoot REAL Default 0.0,
				BoardFootRemoved REAL Default 0.0,
				CubicFootRemoved REAL Default 0.0,
				DIBClass REAL Default 0.0,
				BarkThickness REAL Default 0.0,
				CreatedBy TEXT DEFAULT 'none',
				CreatedDate DateTime DEFAULT (datetime('now')) ,
				ModifiedBy TEXT ,
				ModifiedDate DateTime ,
				RowVersion INTEGER DEFAULT 0,
				UNIQUE (Tree_CN, LogNumber));

    CREATE TABLE Stem (
				Stem_CN INTEGER PRIMARY KEY AUTOINCREMENT,
				Stem_GUID TEXT,
				Tree_CN INTEGER REFERENCES Tree,
				Diameter REAL Default 0.0,
				DiameterType TEXT,
				CreatedBy TEXT DEFAULT 'none',
				CreatedDate DateTime DEFAULT (datetime('now')) ,
				ModifiedBy TEXT ,
				ModifiedDate DateTime ,
				RowVersion INTEGER DEFAULT 0,
				UNIQUE (Tree_CN));

    CREATE TABLE CountTree (
				CountTree_CN INTEGER PRIMARY KEY AUTOINCREMENT,
				SampleGroup_CN INTEGER REFERENCES SampleGroup NOT NULL,
				CuttingUnit_CN INTEGER REFERENCES CuttingUnit NOT NULL,
				Tally_CN INTEGER REFERENCES Tally,
				TreeDefaultValue_CN INTEGER REFERENCES TreeDefaultValue,
				Component_CN INTEGER REFERENCES Component,
				TreeCount INTEGER Default 0,
				SumKPI INTEGER Default 0,
				CreatedBy TEXT DEFAULT 'none',
				CreatedDate DateTime DEFAULT (datetime('now')) ,
				ModifiedBy TEXT ,
				ModifiedDate DateTime ,
				RowVersion INTEGER DEFAULT 0,
				UNIQUE (SampleGroup_CN, CuttingUnit_CN, TreeDefaultValue_CN, Component_CN));

    CREATE TABLE Tally (
				Tally_CN INTEGER PRIMARY KEY AUTOINCREMENT,
				Hotkey TEXT NOT NULL,
				Description TEXT NOT NULL,
				IndicatorValue TEXT,
				IndicatorType TEXT);

    CREATE TABLE TreeEstimate (
				TreeEstimate_CN INTEGER PRIMARY KEY AUTOINCREMENT,
				CountTree_CN INTEGER REFERENCES CountTree,
				TreeEstimate_GUID TEXT,
				KPI REAL NOT NULL,
				CreatedBy TEXT DEFAULT 'none',
				CreatedDate DateTime DEFAULT (datetime('now')) ,
				ModifiedBy TEXT ,
				ModifiedDate DateTime );

    CREATE TABLE FixCNTTallyClass (
				FixCNTTallyClass_CN INTEGER PRIMARY KEY AUTOINCREMENT,
				Stratum_CN INTEGER REFERENCES Stratum NOT NULL,
				FieldName INTEGER Default 0);

    CREATE TABLE FixCNTTallyPopulation (
				FixCNTTallyPopulation_CN INTEGER PRIMARY KEY AUTOINCREMENT,
				FixCNTTallyClass_CN INTEGER REFERENCES FixCNTTallyClass NOT NULL,
				SampleGroup_CN INTEGER REFERENCES SampleGroup NOT NULL,
				TreeDefaultValue_CN INTEGER REFERENCES TreeDefaultValue NOT NULL,
				IntervalSize INTEGER Default 0,
				Min INTEGER Default 0,
				Max INTEGER Default 0);

--Processing Tables--
    CREATE TABLE VolumeEquation (
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
				UNIQUE (Species, PrimaryProduct, VolumeEquationNumber));

    CREATE TABLE BiomassEquation (
				Species TEXT NOT NULL,
				Product TEXT NOT NULL,
				Component TEXT NOT NULL,
				LiveDead TEXT NOT NULL,
				FIAcode INTEGER NOT NULL,
				Equation TEXT,
				PercentMoisture REAL Default 0.0,
				PercentRemoved REAL Default 0.0,
				MetaData TEXT,
				WeightFactorPrimary REAL Default 0.0,
				WeightFactorSecondary REAL Default 0.0,
				UNIQUE (Species, Product, Component, LiveDead));

    CREATE TABLE ValueEquation (
				Species TEXT NOT NULL,
				PrimaryProduct TEXT NOT NULL,
				ValueEquationNumber TEXT,
				Grade TEXT,
				Coefficient1 REAL Default 0.0,
				Coefficient2 REAL Default 0.0,
				Coefficient3 REAL Default 0.0,
				Coefficient4 REAL Default 0.0,
				Coefficient5 REAL Default 0.0,
				Coefficient6 REAL Default 0.0,
				UNIQUE (Species, PrimaryProduct, ValueEquationNumber));

    CREATE TABLE QualityAdjEquation (
				Species TEXT NOT NULL,
				QualityAdjEq TEXT,
				Year INTEGER Default 0,
				Grade TEXT,
				Coefficient1 REAL Default 0.0,
				Coefficient2 REAL Default 0.0,
				Coefficient3 REAL Default 0.0,
				Coefficient4 REAL Default 0.0,
				Coefficient5 REAL Default 0.0,
				Coefficient6 REAL Default 0.0,
				UNIQUE (Species, QualityAdjEq));

    CREATE TABLE Reports (
				ReportID TEXT NOT NULL,
				Selected BOOLEAN Default 0,
				Title TEXT,
				UNIQUE (ReportID));

    CREATE TABLE TreeCalculatedValues (
				TreeCalcValues_CN INTEGER PRIMARY KEY AUTOINCREMENT,
				Tree_CN INTEGER REFERENCES Tree NOT NULL,
				TotalCubicVolume REAL Default 0.0,
				GrossBDFTPP REAL Default 0.0,
				NetBDFTPP REAL Default 0.0,
				GrossCUFTPP REAL Default 0.0,
				NetCUFTPP REAL Default 0.0,
				CordsPP REAL Default 0.0,
				GrossBDFTRemvPP REAL Default 0.0,
				GrossCUFTRemvPP REAL Default 0.0,
				GrossBDFTSP REAL Default 0.0,
				NetBDFTSP REAL Default 0.0,
				GrossCUFTSP REAL Default 0.0,
				NetCUFTSP REAL Default 0.0,
				CordsSP REAL Default 0.0,
				GrossCUFTRemvSP REAL Default 0.0,
				NumberlogsMS REAL Default 0.0,
				NumberlogsTPW REAL Default 0.0,
				GrossBDFTRP REAL Default 0.0,
				GrossCUFTRP REAL Default 0.0,
				CordsRP REAL Default 0.0,
				GrossBDFTIntl REAL Default 0.0,
				NetBDFTIntl REAL Default 0.0,
				BiomassMainStemPrimary REAL Default 0.0,
				BiomassMainStemSecondary REAL Default 0.0,
				ValuePP REAL Default 0.0,
				ValueSP REAL Default 0.0,
				ValueRP REAL Default 0.0,
				BiomassProd REAL Default 0.0,
				Biomasstotalstem REAL Default 0.0,
				Biomasslivebranches REAL Default 0.0,
				Biomassdeadbranches REAL Default 0.0,
				Biomassfoliage REAL Default 0.0,
				BiomassTip REAL Default 0.0,
				UNIQUE (Tree_CN));

    CREATE TABLE LCD (
				LCD_CN INTEGER PRIMARY KEY AUTOINCREMENT,
				CutLeave TEXT NOT NULL,
				Stratum TEXT NOT NULL,
				SampleGroup TEXT NOT NULL,
				Species TEXT NOT NULL,
				PrimaryProduct TEXT NOT NULL,
				SecondaryProduct TEXT NOT NULL,
				UOM TEXT NOT NULL,
				LiveDead TEXT NOT NULL,
				Yield TEXT NOT NULL,
				ContractSpecies TEXT NOT NULL,
				TreeGrade TEXT NOT NULL,
				STM TEXT,
				FirstStageTrees DOUBLE Default 0.0,
				MeasuredTrees DOUBLE Default 0.0,
				TalliedTrees DOUBLE Default 0.0,
				SumKPI DOUBLE Default 0.0,
				SumMeasuredKPI DOUBLE Default 0.0,
				SumExpanFactor DOUBLE Default 0.0,
				SumDBHOB DOUBLE Default 0.0,
				SumDBHOBsqrd DOUBLE Default 0.0,
				SumTotHgt DOUBLE Default 0.0,
				SumHgtUpStem DOUBLE Default 0.0,
				SumMerchHgtPrim DOUBLE Default 0.0,
				SumMerchHgtSecond DOUBLE Default 0.0,
				SumLogsMS DOUBLE Default 0.0,
				SumTotCubic DOUBLE Default 0.0,
				SumGBDFT DOUBLE Default 0.0,
				SumNBDFT DOUBLE Default 0.0,
				SumGCUFT DOUBLE Default 0.0,
				SumNCUFT DOUBLE Default 0.0,
				SumGBDFTremv DOUBLE Default 0.0,
				SumGCUFTremv DOUBLE Default 0.0,
				SumCords DOUBLE Default 0.0,
				SumWgtMSP DOUBLE Default 0.0,
				SumValue DOUBLE Default 0.0,
				SumGBDFTtop DOUBLE Default 0.0,
				SumNBDFTtop DOUBLE Default 0.0,
				SumGCUFTtop DOUBLE Default 0.0,
				SumNCUFTtop DOUBLE Default 0.0,
				SumCordsTop DOUBLE Default 0.0,
				SumWgtMSS DOUBLE Default 0.0,
				SumTopValue DOUBLE Default 0.0,
				SumLogsTop DOUBLE Default 0.0,
				SumBDFTrecv DOUBLE Default 0.0,
				SumCUFTrecv DOUBLE Default 0.0,
				SumCordsRecv DOUBLE Default 0.0,
				SumValueRecv DOUBLE Default 0.0,
				BiomassProduct DOUBLE Default 0.0,
				SumWgtBAT DOUBLE Default 0.0,
				SumWgtBBL DOUBLE Default 0.0,
				SumWgtBBD DOUBLE Default 0.0,
				SumWgtBFT DOUBLE Default 0.0,
				SumWgtTip DOUBLE Default 0.0);

    CREATE TABLE POP (
				POP_CN INTEGER PRIMARY KEY AUTOINCREMENT,
				CutLeave TEXT NOT NULL,
				Stratum TEXT NOT NULL,
				SampleGroup TEXT NOT NULL,
				PrimaryProduct TEXT NOT NULL,
				SecondaryProduct TEXT NOT NULL,
				STM TEXT,
				UOM TEXT NOT NULL,
				FirstStageTrees DOUBLE Default 0.0,
				MeasuredTrees DOUBLE Default 0.0,
				TalliedTrees DOUBLE Default 0.0,
				SumKPI DOUBLE Default 0.0,
				SumMeasuredKPI DOUBLE Default 0.0,
				StageOneSamples DOUBLE Default 0.0,
				StageTwoSamples DOUBLE Default 0.0,
				Stg1GrossXPP DOUBLE Default 0.0,
				Stg1GrossXsqrdPP DOUBLE Default 0.0,
				Stg1NetXPP DOUBLE Default 0.0,
				Stg1NetXsqrdPP DOUBLE Default 0.0,
				Stg1ValueXPP DOUBLE Default 0.0,
				Stg1ValueXsqrdPP DOUBLE Default 0.0,
				Stg2GrossXPP DOUBLE Default 0.0,
				Stg2GrossXsqrdPP DOUBLE Default 0.0,
				Stg2NetXPP DOUBLE Default 0.0,
				Stg2NetXsqrdPP DOUBLE Default 0.0,
				Stg2ValueXPP DOUBLE Default 0.0,
				Stg2ValueXsqrdPP DOUBLE Default 0.0,
				Stg1GrossXSP DOUBLE Default 0.0,
				Stg1GrossXsqrdSP DOUBLE Default 0.0,
				Stg1NetXSP DOUBLE Default 0.0,
				Stg1NetXsqrdSP DOUBLE Default 0.0,
				Stg1ValueXSP DOUBLE Default 0.0,
				Stg1ValueXsqrdSP DOUBLE Default 0.0,
				Stg2GrossXSP DOUBLE Default 0.0,
				Stg2GrossXsqrdSP DOUBLE Default 0.0,
				Stg2NetXSP DOUBLE Default 0.0,
				Stg2NetXsqrdSP DOUBLE Default 0.0,
				Stg2ValueXSP DOUBLE Default 0.0,
				Stg2ValueXsqrdSP DOUBLE Default 0.0,
				Stg1GrossXRP DOUBLE Default 0.0,
				Stg1GrossXsqrdRP DOUBLE Default 0.0,
				Stg1NetXRP DOUBLE Default 0.0,
				Stg1NetXRsqrdRP DOUBLE Default 0.0,
				Stg1ValueXRP DOUBLE Default 0.0,
				Stg1ValueXsqrdRP DOUBLE Default 0.0,
				Stg2GrossXRP DOUBLE Default 0.0,
				Stg2GrossXsqrdRP DOUBLE Default 0.0,
				Stg2NetXRP DOUBLE Default 0.0,
				Stg2NetXsqrdRP DOUBLE Default 0.0,
				Stg2ValueXRP DOUBLE Default 0.0,
				Stg2ValueXsqrdRP DOUBLE Default 0.0);

    CREATE TABLE PRO (
				PRO_CN INTEGER PRIMARY KEY AUTOINCREMENT,
				CutLeave TEXT NOT NULL,
				Stratum TEXT NOT NULL,
				CuttingUnit TEXT NOT NULL,
				SampleGroup TEXT NOT NULL,
				PrimaryProduct TEXT NOT NULL,
				SecondaryProduct TEXT NOT NULL,
				UOM TEXT NOT NULL,
				STM TEXT,
				FirstStageTrees DOUBLE Default 0.0,
				MeasuredTrees DOUBLE Default 0.0,
				TalliedTrees DOUBLE Default 0.0,
				SumKPI DOUBLE Default 0.0,
				SumMeasuredKPI DOUBLE Default 0.0,
				ProrationFactor DOUBLE Default 0.0,
				ProratedEstimatedTrees DOUBLE Default 0.0);

    CREATE TABLE LogStock (
				LogStock_CN INTEGER PRIMARY KEY AUTOINCREMENT,
				Tree_CN INTEGER REFERENCES Tree NOT NULL,
				LogNumber TEXT NOT NULL,
				Grade TEXT,
				SeenDefect REAL Default 0.0,
				PercentRecoverable REAL Default 0.0,
				Length INTEGER Default 0,
				ExportGrade TEXT,
				SmallEndDiameter REAL Default 0.0,
				LargeEndDiameter REAL Default 0.0,
				GrossBoardFoot REAL Default 0.0,
				NetBoardFoot REAL Default 0.0,
				GrossCubicFoot REAL Default 0.0,
				NetCubicFoot REAL Default 0.0,
				BoardFootRemoved REAL Default 0.0,
				CubicFootRemoved REAL Default 0.0,
				DIBClass REAL Default 0.0,
				BarkThickness REAL Default 0.0,
				BoardUtil REAL Default 0.0,
				CubicUtil REAL Default 0.0,
				CreatedBy TEXT DEFAULT 'none',
				CreatedDate DateTime DEFAULT (datetime('now')) ,
				ModifiedBy TEXT ,
				ModifiedDate DateTime ,
				UNIQUE (Tree_CN, LogNumber));

    CREATE TABLE SampleGroupStats (
				SampleGroupStats_CN INTEGER PRIMARY KEY AUTOINCREMENT,
				StratumStats_CN INTEGER REFERENCES StratumStats,
				Code TEXT,
				SgSet INTEGER Default 0,
				Description TEXT,
				CutLeave TEXT,
				UOM TEXT,
				PrimaryProduct TEXT,
				SecondaryProduct TEXT,
				DefaultLiveDead TEXT,
				SgError REAL Default 0.0,
				SampleSize1 INTEGER Default 0,
				SampleSize2 INTEGER Default 0,
				CV1 REAL Default 0.0,
				CV2 REAL Default 0.0,
				TreesPerAcre REAL Default 0.0,
				VolumePerAcre REAL Default 0.0,
				TreesPerPlot REAL Default 0.0,
				AverageHeight REAL Default 0.0,
				SamplingFrequency INTEGER Default 0,
				InsuranceFrequency INTEGER Default 0,
				KZ INTEGER Default 0,
				BigBAF REAL Default 0.0,
				BigFIX INTEGER Default 0,
				MinDbh REAL Default 0.0,
				MaxDbh REAL Default 0.0,
				CV_Def INTEGER Default 0,
				CV2_Def INTEGER Default 0,
				TPA_Def INTEGER Default 0,
				VPA_Def INTEGER Default 0,
				ReconPlots INTEGER Default 0,
				ReconTrees INTEGER Default 0,
				UNIQUE (StratumStats_CN, Code, SgSet));

    CREATE TABLE SampleGroupStatsTreeDefaultValue (
				TreeDefaultValue_CN INTEGER REFERENCES TreeDefaultValue,
				SampleGroupStats_CN INTEGER REFERENCES SampleGroupStats,
				UNIQUE (TreeDefaultValue_CN, SampleGroupStats_CN));

    CREATE TABLE StratumStats (
				StratumStats_CN INTEGER PRIMARY KEY AUTOINCREMENT,
				Stratum_CN INTEGER REFERENCES Stratum,
				Code TEXT,
				Description TEXT,
				Method TEXT,
				SgSet INTEGER Default 0,
				SgSetDescription TEXT,
				BasalAreaFactor REAL Default 0.0,
				FixedPlotSize REAL Default 0.0,
				StrError REAL Default 0.0,
				SampleSize1 INTEGER Default 0,
				SampleSize2 INTEGER Default 0,
				WeightedCV1 REAL Default 0.0,
				WeightedCV2 REAL Default 0.0,
				TreesPerAcre REAL Default 0.0,
				VolumePerAcre REAL Default 0.0,
				TotalVolume REAL Default 0.0,
				TotalAcres REAL Default 0.0,
				PlotSpacing INTEGER Default 0,
				Used INTEGER Default 0,
				UNIQUE (Code, Method, SgSet));

    CREATE TABLE Regression (
				Regression_CN INTEGER PRIMARY KEY AUTOINCREMENT,
				rVolume TEXT,
				rVolType TEXT,
				rSpeices TEXT,
				rProduct TEXT,
				rLiveDead TEXT,
				CoefficientA REAL Default 0.0,
				CoefficientB REAL Default 0.0,
				CoefficientC REAL Default 0.0,
				TotalTrees INTEGER Default 0,
				MeanSE REAL Default 0.0,
				Rsquared REAL Default 0.0,
				RegressModel TEXT,
				rMinDbh REAL Default 0.0,
				rMaxDbh REAL Default 0.0);

    CREATE TABLE LogMatrix (
				ReportNumber TEXT,
				GradeDescription TEXT,
				LogSortDescription TEXT,
				Species TEXT,
				LogGrade1 TEXT,
				LogGrade2 TEXT,
				LogGrade3 TEXT,
				LogGrade4 TEXT,
				LogGrade5 TEXT,
				LogGrade6 TEXT,
				SEDlimit TEXT,
				SEDminimum DOUBLE Default 0.0,
				SEDmaximum DOUBLE Default 0.0);

--Settings Tables--
    CREATE TABLE TreeDefaultValueTreeAuditValue (
				TreeAuditValue_CN INTEGER REFERENCES TreeAuditValue NOT NULL,
				TreeDefaultValue_CN INTEGER REFERENCES TreeDefaultValue NOT NULL);

    CREATE TABLE TreeAuditValue (
				TreeAuditValue_CN INTEGER PRIMARY KEY AUTOINCREMENT,
				Field TEXT NOT NULL,
				Min REAL Default 0.0,
				Max REAL Default 0.0,
				ValueSet TEXT,
				Required BOOLEAN Default 0,
				ErrorMessage TEXT);

    CREATE TABLE LogFieldSetup (
				Stratum_CN INTEGER REFERENCES Stratum NOT NULL,
				Field TEXT NOT NULL,
				FieldOrder INTEGER Default 0,
				ColumnType TEXT,
				Heading TEXT,
				Width REAL Default 0.0,
				Format TEXT,
				Behavior TEXT,
				UNIQUE (Stratum_CN, Field));

    CREATE TABLE TreeFieldSetup (
				Stratum_CN INTEGER REFERENCES Stratum NOT NULL,
				Field TEXT NOT NULL,
				FieldOrder INTEGER Default 0,
				ColumnType TEXT,
				Heading TEXT,
				Width REAL Default 0.0,
				Format TEXT,
				Behavior TEXT,
				UNIQUE (Stratum_CN, Field));

    CREATE TABLE LogFieldSetupDefault (
				LogFieldSetupDefault_CN INTEGER PRIMARY KEY AUTOINCREMENT,
				Field TEXT NOT NULL,
				FieldName TEXT,
				FieldOrder INTEGER Default 0,
				ColumnType TEXT,
				Heading TEXT,
				Width REAL Default 0.0,
				Format TEXT,
				Behavior TEXT,
				UNIQUE (Field));

    CREATE TABLE TreeFieldSetupDefault (
				TreeFieldSetupDefault_CN INTEGER PRIMARY KEY AUTOINCREMENT,
				Method TEXT NOT NULL,
				Field TEXT NOT NULL,
				FieldName TEXT,
				FieldOrder INTEGER Default 0,
				ColumnType TEXT,
				Heading TEXT,
				Width REAL Default 0.0,
				Format TEXT,
				Behavior TEXT,
				UNIQUE (Method, Field));

--Lookup Tables--
    CREATE TABLE CruiseMethods (
				CruiseMethods_CN INTEGER PRIMARY KEY AUTOINCREMENT,
				Code TEXT,
				FriendlyValue TEXT,
				UNIQUE (Code));

    CREATE TABLE LoggingMethods (
				LoggingMethods_CN INTEGER PRIMARY KEY AUTOINCREMENT,
				Code TEXT,
				FriendlyValue TEXT,
				UNIQUE (Code));

    CREATE TABLE ProductCodes (
				ProductCodes_CN INTEGER PRIMARY KEY AUTOINCREMENT,
				Code TEXT,
				FriendlyValue TEXT,
				UNIQUE (Code));

    CREATE TABLE UOMCodes (
				UOMCodes_CN INTEGER PRIMARY KEY AUTOINCREMENT,
				Code TEXT,
				FriendlyValue TEXT,
				UNIQUE (Code));

    CREATE TABLE Regions (
				Region_CN INTEGER PRIMARY KEY AUTOINCREMENT,
				Number INTEGER Default 0,
				Name TEXT,
				UNIQUE (Number));

    CREATE TABLE Forests (
				Forest_CN INTEGER PRIMARY KEY AUTOINCREMENT,
				Region_CN INTEGER REFERENCES Regions,
				State TEXT,
				Name TEXT,
				Number INTEGER Default 0);

--Utility Tables--
    CREATE TABLE ErrorLog (
				TableName TEXT NOT NULL,
				CN_Number INTEGER NOT NULL,
				ColumnName TEXT NOT NULL,
				Level TEXT NOT NULL,
				Message TEXT,
				Program TEXT,
				Suppress BOOLEAN Default 0,
				UNIQUE (TableName, CN_Number, ColumnName, Level));

    CREATE TABLE MessageLog (
				Message_CN INTEGER PRIMARY KEY AUTOINCREMENT,
				Program TEXT,
				Message TEXT,
				Date TEXT,
				Time TEXT,
				Level TEXT);

    CREATE TABLE Globals (
				Block TEXT,
				Key TEXT,
				Value TEXT,
				UNIQUE (Block, Key));

    CREATE TABLE Component (
				Component_CN INTEGER PRIMARY KEY AUTOINCREMENT,
				GUID TEXT,
				LastMerge DATETIME,
				FileName TEXT);

CREATE TABLE Util_Tombstone (
RecordID INTEGER ,
RecordGUID TEXT,
TableName TEXT NOT NULL COLLATE NOCASE,
Data TEXT,
DeletedDate DATETIME NON NULL);

CREATE VIEW CountTree_View AS
SELECT Stratum.Code as StratumCode,
Stratum.Method as Method,
SampleGroup.Code as SampleGroupCode,
SampleGroup.PrimaryProduct as PrimaryProduct,
CountTree.*
FROM CountTree JOIN SampleGroup USING (SampleGroup_CN) JOIN Stratum USING (Stratum_CN);

CREATE VIEW StratumAcres_View AS
SELECT CuttingUnit.Code as CuttingUnitCode,
Stratum.Code as StratumCode,
ifnull(Area, CuttingUnit.Area) as Area,
CuttingUnitStratum.*
FROM CuttingUnitStratum
JOIN CuttingUnit USING (CuttingUnit_CN)
JOIN Stratum USING (Stratum_CN);

INSERT INTO Globals (Block, Key, Value) VALUES (""Database"", ""Version"", ""2.1.1"");
PRAGMA user_version = 3;

";

		public static string CreateTriggers =
@"--Core Tables--
--Sale--
CREATE TRIGGER OnInsertedSale
AFTER INSERT
ON Sale
BEGIN
	UPDATE Sale
	SET CreatedDate = datetime(current_timestamp, 'localtime')
	WHERE rowID = new.rowID;
END;

CREATE TRIGGER OnUpdateSale
AFTER UPDATE OF Sale_CN, SaleNumber, Name, Purpose, Region, Forest, District, MeasurementYear, CalendarYear, LogGradingEnabled, Remarks, DefaultUOM
ON Sale
BEGIN
	UPDATE Sale
	SET ModifiedDate = datetime(current_timestamp, 'localtime')
	WHERE rowID = new.rowID;
END;

CREATE TRIGGER OnUpdateSaleRowVersion AFTER UPDATE
OF Sale_CN, SaleNumber, Name, Purpose, Region, Forest, District, MeasurementYear, CalendarYear, LogGradingEnabled, Remarks, DefaultUOM
ON Sale
BEGIN
	UPDATE Sale SET RowVersion = OLD.RowVersion + 1
	WHERE RowID = OLD.RowID;
END;

CREATE TRIGGER OnDeleteSale AFTER DELETE ON Sale
BEGIN
	INSERT INTO Util_Tombstone
	(RecordID, TableName, Data, DeletedDate)
	VALUES
	(OLD.rowID, 'Sale', '(SaleNumber) VALUES (' || quote(OLD.SaleNumber) ||')', datetime(current_timestamp, 'localtime'));
END;

--CuttingUnit--
CREATE TRIGGER OnInsertedCuttingUnit
AFTER INSERT
ON CuttingUnit
BEGIN
	UPDATE CuttingUnit
	SET CreatedDate = datetime(current_timestamp, 'localtime')
	WHERE rowID = new.rowID;
END;

CREATE TRIGGER OnUpdateCuttingUnit
AFTER UPDATE OF CuttingUnit_CN, Code, Area, Description, LoggingMethod, PaymentUnit, TallyHistory
ON CuttingUnit
BEGIN
	UPDATE CuttingUnit
	SET ModifiedDate = datetime(current_timestamp, 'localtime')
	WHERE rowID = new.rowID;
END;

CREATE TRIGGER OnUpdateCuttingUnitRowVersion AFTER UPDATE
OF CuttingUnit_CN, Code, Area, Description, LoggingMethod, PaymentUnit, TallyHistory
ON CuttingUnit
BEGIN
	UPDATE CuttingUnit SET RowVersion = OLD.RowVersion + 1
	WHERE RowID = OLD.RowID;
END;

CREATE TRIGGER OnDeleteCuttingUnit AFTER DELETE ON CuttingUnit
BEGIN
	INSERT INTO Util_Tombstone
	(RecordID, TableName, Data, DeletedDate)
	VALUES
	(OLD.rowID, 'CuttingUnit', '(Code) VALUES (' || quote(OLD.Code) ||')', datetime(current_timestamp, 'localtime'));
END;

--Stratum--
CREATE TRIGGER OnInsertedStratum
AFTER INSERT
ON Stratum
BEGIN
	UPDATE Stratum
	SET CreatedDate = datetime(current_timestamp, 'localtime')
	WHERE rowID = new.rowID;
END;

CREATE TRIGGER OnUpdateStratum
AFTER UPDATE OF Stratum_CN, Code, Description, Method, BasalAreaFactor, FixedPlotSize, KZ3PPNT, Hotkey, FBSCode, YieldComponent, Month, Year
ON Stratum
BEGIN
	UPDATE Stratum
	SET ModifiedDate = datetime(current_timestamp, 'localtime')
	WHERE rowID = new.rowID;
END;

CREATE TRIGGER OnUpdateStratumRowVersion AFTER UPDATE
OF Stratum_CN, Code, Description, Method, BasalAreaFactor, FixedPlotSize, KZ3PPNT, Hotkey, FBSCode, YieldComponent, Month, Year
ON Stratum
BEGIN
	UPDATE Stratum SET RowVersion = OLD.RowVersion + 1
	WHERE RowID = OLD.RowID;
END;

CREATE TRIGGER OnDeleteStratum AFTER DELETE ON Stratum
BEGIN
	INSERT INTO Util_Tombstone
	(RecordID, TableName, Data, DeletedDate)
	VALUES
	(OLD.rowID, 'Stratum', '(Code) VALUES (' || quote(OLD.Code) ||')', datetime(current_timestamp, 'localtime'));
END;

--CuttingUnitStratum--
CREATE TRIGGER OnDeleteCuttingUnitStratum AFTER DELETE ON CuttingUnitStratum
BEGIN
	INSERT INTO Util_Tombstone
	(RecordID, TableName, Data, DeletedDate)
	VALUES
	(OLD.rowID, 'CuttingUnitStratum', '(CuttingUnit_CN, Stratum_CN) VALUES (' || quote(OLD.CuttingUnit_CN) || ',' || quote(OLD.Stratum_CN) ||')', datetime(current_timestamp, 'localtime'));
END;

--SampleGroup--
CREATE TRIGGER OnInsertedSampleGroup
AFTER INSERT
ON SampleGroup
BEGIN
	UPDATE SampleGroup
	SET CreatedDate = datetime(current_timestamp, 'localtime')
	WHERE rowID = new.rowID;
END;

CREATE TRIGGER OnUpdateSampleGroup
AFTER UPDATE OF SampleGroup_CN, Stratum_CN, Code, CutLeave, UOM, PrimaryProduct, SecondaryProduct, BiomassProduct, DefaultLiveDead, SamplingFrequency, InsuranceFrequency, KZ, BigBAF, SmallFPS, TallyMethod, Description, SampleSelectorType, SampleSelectorState, MinKPI, MaxKPI
ON SampleGroup
BEGIN
	UPDATE SampleGroup
	SET ModifiedDate = datetime(current_timestamp, 'localtime')
	WHERE rowID = new.rowID;
END;

CREATE TRIGGER OnUpdateSampleGroupRowVersion AFTER UPDATE
OF SampleGroup_CN, Stratum_CN, Code, CutLeave, UOM, PrimaryProduct, SecondaryProduct, BiomassProduct, DefaultLiveDead, SamplingFrequency, InsuranceFrequency, KZ, BigBAF, SmallFPS, TallyMethod, Description, SampleSelectorType, SampleSelectorState, MinKPI, MaxKPI
ON SampleGroup
BEGIN
	UPDATE SampleGroup SET RowVersion = OLD.RowVersion + 1
	WHERE RowID = OLD.RowID;
END;

CREATE TRIGGER OnDeleteSampleGroup AFTER DELETE ON SampleGroup
BEGIN
	INSERT INTO Util_Tombstone
	(RecordID, TableName, Data, DeletedDate)
	VALUES
	(OLD.rowID, 'SampleGroup', '(Stratum_CN, Code) VALUES (' || quote(OLD.Stratum_CN) || ',' || quote(OLD.Code) ||')', datetime(current_timestamp, 'localtime'));
END;

--TreeDefaultValue--
CREATE TRIGGER OnInsertedTreeDefaultValue
AFTER INSERT
ON TreeDefaultValue
BEGIN
	UPDATE TreeDefaultValue
	SET CreatedDate = datetime(current_timestamp, 'localtime')
	WHERE rowID = new.rowID;
END;

CREATE TRIGGER OnUpdateTreeDefaultValue
AFTER UPDATE OF TreeDefaultValue_CN, PrimaryProduct, Species, LiveDead, FIAcode, CullPrimary, HiddenPrimary, CullSecondary, HiddenSecondary, Recoverable, Chargeable, ContractSpecies, TreeGrade, MerchHeightLogLength, MerchHeightType, FormClass, BarkThicknessRatio, AverageZ, ReferenceHeightPercent
ON TreeDefaultValue
BEGIN
	UPDATE TreeDefaultValue
	SET ModifiedDate = datetime(current_timestamp, 'localtime')
	WHERE rowID = new.rowID;
END;

CREATE TRIGGER OnUpdateTreeDefaultValueRowVersion AFTER UPDATE
OF TreeDefaultValue_CN, PrimaryProduct, Species, LiveDead, FIAcode, CullPrimary, HiddenPrimary, CullSecondary, HiddenSecondary, Recoverable, Chargeable, ContractSpecies, TreeGrade, MerchHeightLogLength, MerchHeightType, FormClass, BarkThicknessRatio, AverageZ, ReferenceHeightPercent
ON TreeDefaultValue
BEGIN
	UPDATE TreeDefaultValue SET RowVersion = OLD.RowVersion + 1
	WHERE RowID = OLD.RowID;
END;

CREATE TRIGGER OnDeleteTreeDefaultValue AFTER DELETE ON TreeDefaultValue
BEGIN
	INSERT INTO Util_Tombstone
	(RecordID, TableName, Data, DeletedDate)
	VALUES
	(OLD.rowID, 'TreeDefaultValue', '(PrimaryProduct, Species, LiveDead, Chargeable) VALUES (' || quote(OLD.PrimaryProduct) || ',' || quote(OLD.Species) || ',' || quote(OLD.LiveDead) || ',' || quote(OLD.Chargeable) ||')', datetime(current_timestamp, 'localtime'));
END;

--SampleGroupTreeDefaultValue--
CREATE TRIGGER OnDeleteSampleGroupTreeDefaultValue AFTER DELETE ON SampleGroupTreeDefaultValue
BEGIN
	INSERT INTO Util_Tombstone
	(RecordID, TableName, Data, DeletedDate)
	VALUES
	(OLD.rowID, 'SampleGroupTreeDefaultValue', '(TreeDefaultValue_CN, SampleGroup_CN) VALUES (' || quote(OLD.TreeDefaultValue_CN) || ',' || quote(OLD.SampleGroup_CN) ||')', datetime(current_timestamp, 'localtime'));
END;

--Plot--
CREATE TRIGGER OnInsertedPlot
AFTER INSERT
ON Plot
BEGIN
	UPDATE Plot
	SET CreatedDate = datetime(current_timestamp, 'localtime')
	WHERE rowID = new.rowID;
END;

CREATE TRIGGER OnUpdatePlot
AFTER UPDATE OF Plot_CN, Plot_GUID, Stratum_CN, CuttingUnit_CN, PlotNumber, IsEmpty, Slope, KPI, Aspect, Remarks, XCoordinate, YCoordinate, ZCoordinate, MetaData, Blob
ON Plot
BEGIN
	UPDATE Plot
	SET ModifiedDate = datetime(current_timestamp, 'localtime')
	WHERE rowID = new.rowID;
END;

CREATE TRIGGER OnUpdatePlotRowVersion AFTER UPDATE
OF Plot_CN, Plot_GUID, Stratum_CN, CuttingUnit_CN, PlotNumber, IsEmpty, Slope, KPI, Aspect, Remarks, XCoordinate, YCoordinate, ZCoordinate, MetaData, Blob
ON Plot
BEGIN
	UPDATE Plot SET RowVersion = OLD.RowVersion + 1
	WHERE RowID = OLD.RowID;
END;

CREATE TRIGGER OnDeletePlot AFTER DELETE ON Plot
BEGIN
	INSERT INTO Util_Tombstone
	(RecordID, RecordGUID, TableName, Data, DeletedDate)
	VALUES
	(OLD.rowID, OLD.Plot_GUID, 'Plot', '(Stratum_CN, CuttingUnit_CN, PlotNumber) VALUES (' || quote(OLD.Stratum_CN) || ',' || quote(OLD.CuttingUnit_CN) || ',' || quote(OLD.PlotNumber) ||')', datetime(current_timestamp, 'localtime'));
END;
--Tree--
CREATE TRIGGER OnInsertedTree
AFTER INSERT
ON Tree
BEGIN
	UPDATE Tree
	SET CreatedDate = datetime(current_timestamp, 'localtime')
	WHERE rowID = new.rowID;
END;

CREATE TRIGGER OnUpdateTree
AFTER UPDATE OF Tree_CN, Tree_GUID, TreeDefaultValue_CN, Stratum_CN, SampleGroup_CN, CuttingUnit_CN, Plot_CN, TreeNumber, Species, CountOrMeasure, TreeCount, KPI, STM, SeenDefectPrimary, SeenDefectSecondary, RecoverablePrimary, HiddenPrimary, Initials, LiveDead, Grade, HeightToFirstLiveLimb, PoleLength, ClearFace, CrownRatio, DBH, DRC, TotalHeight, MerchHeightPrimary, MerchHeightSecondary, FormClass, UpperStemDOB, UpperStemDiameter, UpperStemHeight, DBHDoubleBarkThickness, TopDIBPrimary, TopDIBSecondary, DefectCode, DiameterAtDefect, VoidPercent, Slope, Aspect, Remarks, XCoordinate, YCoordinate, ZCoordinate, MetaData, IsFallBuckScale
ON Tree
BEGIN
	UPDATE Tree
	SET ModifiedDate = datetime(current_timestamp, 'localtime')
	WHERE rowID = new.rowID;
END;

CREATE TRIGGER OnUpdateTreeRowVersion AFTER UPDATE
OF Tree_CN, Tree_GUID, TreeDefaultValue_CN, Stratum_CN, SampleGroup_CN, CuttingUnit_CN, Plot_CN, TreeNumber, Species, CountOrMeasure, TreeCount, KPI, STM, SeenDefectPrimary, SeenDefectSecondary, RecoverablePrimary, HiddenPrimary, Initials, LiveDead, Grade, HeightToFirstLiveLimb, PoleLength, ClearFace, CrownRatio, DBH, DRC, TotalHeight, MerchHeightPrimary, MerchHeightSecondary, FormClass, UpperStemDOB, UpperStemDiameter, UpperStemHeight, DBHDoubleBarkThickness, TopDIBPrimary, TopDIBSecondary, DefectCode, DiameterAtDefect, VoidPercent, Slope, Aspect, Remarks, XCoordinate, YCoordinate, ZCoordinate, MetaData, IsFallBuckScale
ON Tree
BEGIN
	UPDATE Tree SET RowVersion = OLD.RowVersion + 1
	WHERE RowID = OLD.RowID;
END;

CREATE TRIGGER OnDeleteTree AFTER DELETE ON Tree
BEGIN
	INSERT INTO Util_Tombstone
	(RecordID, RecordGUID, TableName, Data, DeletedDate)
	VALUES
	(OLD.rowID, OLD.Tree_GUID, 'Tree', '(TreeDefaultValue_CN, Stratum_CN, SampleGroup_CN, CuttingUnit_CN, Plot_CN, TreeNumber) VALUES (' || quote(OLD.TreeDefaultValue_CN) || ',' || quote(OLD.Stratum_CN) || ',' || quote(OLD.SampleGroup_CN) || ',' || quote(OLD.CuttingUnit_CN) || ',' || quote(OLD.Plot_CN) || ',' || quote(OLD.TreeNumber) ||')', datetime(current_timestamp, 'localtime'));
END;
--Log--
CREATE TRIGGER OnInsertedLog
AFTER INSERT
ON Log
BEGIN
	UPDATE Log
	SET CreatedDate = datetime(current_timestamp, 'localtime')
	WHERE rowID = new.rowID;
END;

CREATE TRIGGER OnUpdateLog
AFTER UPDATE OF Log_CN, Log_GUID, Tree_CN, LogNumber, Grade, SeenDefect, PercentRecoverable, Length, ExportGrade, SmallEndDiameter, LargeEndDiameter, GrossBoardFoot, NetBoardFoot, GrossCubicFoot, NetCubicFoot, BoardFootRemoved, CubicFootRemoved, DIBClass, BarkThickness
ON Log
BEGIN
	UPDATE Log
	SET ModifiedDate = datetime(current_timestamp, 'localtime')
	WHERE rowID = new.rowID;
END;

CREATE TRIGGER OnUpdateLogRowVersion AFTER UPDATE
OF Log_CN, Log_GUID, Tree_CN, LogNumber, Grade, SeenDefect, PercentRecoverable, Length, ExportGrade, SmallEndDiameter, LargeEndDiameter, GrossBoardFoot, NetBoardFoot, GrossCubicFoot, NetCubicFoot, BoardFootRemoved, CubicFootRemoved, DIBClass, BarkThickness
ON Log
BEGIN
	UPDATE Log SET RowVersion = OLD.RowVersion + 1
	WHERE RowID = OLD.RowID;
END;

CREATE TRIGGER OnDeleteLog AFTER DELETE ON Log
BEGIN
	INSERT INTO Util_Tombstone
	(RecordID, RecordGUID, TableName, Data, DeletedDate)
	VALUES
	(OLD.rowID, OLD.Log_GUID, 'Log', '(Tree_CN, LogNumber) VALUES (' || quote(OLD.Tree_CN) || ',' || quote(OLD.LogNumber) ||')', datetime(current_timestamp, 'localtime'));
END;
--Stem--
CREATE TRIGGER OnInsertedStem
AFTER INSERT
ON Stem
BEGIN
	UPDATE Stem
	SET CreatedDate = datetime(current_timestamp, 'localtime')
	WHERE rowID = new.rowID;
END;

CREATE TRIGGER OnUpdateStem
AFTER UPDATE OF Stem_CN, Stem_GUID, Tree_CN, Diameter, DiameterType
ON Stem
BEGIN
	UPDATE Stem
	SET ModifiedDate = datetime(current_timestamp, 'localtime')
	WHERE rowID = new.rowID;
END;

CREATE TRIGGER OnUpdateStemRowVersion AFTER UPDATE
OF Stem_CN, Stem_GUID, Tree_CN, Diameter, DiameterType
ON Stem
BEGIN
	UPDATE Stem SET RowVersion = OLD.RowVersion + 1
	WHERE RowID = OLD.RowID;
END;

CREATE TRIGGER OnDeleteStem AFTER DELETE ON Stem
BEGIN
	INSERT INTO Util_Tombstone
	(RecordID, RecordGUID, TableName, Data, DeletedDate)
	VALUES
	(OLD.rowID, OLD.Stem_GUID, 'Stem', '(Tree_CN) VALUES (' || quote(OLD.Tree_CN) ||')', datetime(current_timestamp, 'localtime'));
END;
--CountTree--
CREATE TRIGGER OnInsertedCountTree
AFTER INSERT
ON CountTree
BEGIN
	UPDATE CountTree
	SET CreatedDate = datetime(current_timestamp, 'localtime')
	WHERE rowID = new.rowID;
END;

CREATE TRIGGER OnUpdateCountTree
AFTER UPDATE OF CountTree_CN, SampleGroup_CN, CuttingUnit_CN, Tally_CN, TreeDefaultValue_CN, Component_CN, TreeCount, SumKPI
ON CountTree
BEGIN
	UPDATE CountTree
	SET ModifiedDate = datetime(current_timestamp, 'localtime')
	WHERE rowID = new.rowID;
END;

CREATE TRIGGER OnUpdateCountTreeRowVersion AFTER UPDATE
OF CountTree_CN, SampleGroup_CN, CuttingUnit_CN, Tally_CN, TreeDefaultValue_CN, Component_CN, TreeCount, SumKPI
ON CountTree
BEGIN
	UPDATE CountTree SET RowVersion = OLD.RowVersion + 1
	WHERE RowID = OLD.RowID;
END;

--Tally--
--TreeEstimate--
CREATE TRIGGER OnInsertedTreeEstimate
AFTER INSERT
ON TreeEstimate
BEGIN
	UPDATE TreeEstimate
	SET CreatedDate = datetime(current_timestamp, 'localtime')
	WHERE rowID = new.rowID;
END;

CREATE TRIGGER OnUpdateTreeEstimate
AFTER UPDATE OF TreeEstimate_CN, CountTree_CN, TreeEstimate_GUID, KPI
ON TreeEstimate
BEGIN
	UPDATE TreeEstimate
	SET ModifiedDate = datetime(current_timestamp, 'localtime')
	WHERE rowID = new.rowID;
END;

CREATE TRIGGER OnDeleteTreeEstimate AFTER DELETE ON TreeEstimate
BEGIN
	INSERT INTO Util_Tombstone
	(RecordID, RecordGUID, TableName, Data, DeletedDate)
	VALUES
	(OLD.rowID, OLD.TreeEstimate_GUID, 'TreeEstimate', '', datetime(current_timestamp, 'localtime'));
END;
--FixCNTTallyClass--
--FixCNTTallyPopulation--
--Processing Tables--
--VolumeEquation--
--BiomassEquation--
--ValueEquation--
--QualityAdjEquation--
--Reports--
--TreeCalculatedValues--
--LCD--
--POP--
--PRO--
--LogStock--
CREATE TRIGGER OnInsertedLogStock
AFTER INSERT
ON LogStock
BEGIN
	UPDATE LogStock
	SET CreatedDate = datetime(current_timestamp, 'localtime')
	WHERE rowID = new.rowID;
END;

CREATE TRIGGER OnUpdateLogStock
AFTER UPDATE OF LogStock_CN, Tree_CN, LogNumber, Grade, SeenDefect, PercentRecoverable, Length, ExportGrade, SmallEndDiameter, LargeEndDiameter, GrossBoardFoot, NetBoardFoot, GrossCubicFoot, NetCubicFoot, BoardFootRemoved, CubicFootRemoved, DIBClass, BarkThickness, BoardUtil, CubicUtil
ON LogStock
BEGIN
	UPDATE LogStock
	SET ModifiedDate = datetime(current_timestamp, 'localtime')
	WHERE rowID = new.rowID;
END;

--SampleGroupStats--
--SampleGroupStatsTreeDefaultValue--
--StratumStats--
--Regression--
--LogMatrix--
--Settings Tables--
--TreeDefaultValueTreeAuditValue--
--TreeAuditValue--
--LogFieldSetup--
--TreeFieldSetup--
--LogFieldSetupDefault--
--TreeFieldSetupDefault--
--Lookup Tables--
--CruiseMethods--
--LoggingMethods--
--ProductCodes--
--UOMCodes--
--Regions--
--Forests--
--Utility Tables--
--ErrorLog--
--MessageLog--
--Globals--
--Component--

";

	}
}