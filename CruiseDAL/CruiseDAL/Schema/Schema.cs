 

using System;
using CruiseDAL;

namespace CruiseDAL.Schema
{
	#region Core Tables
	public static class SALE
	{
		public const string _NAME = "Sale";
		public const string SALE_CN = "Sale_CN";
		public const string SALENUMBER = "SaleNumber";
		public const string NAME = "Name";
		public const string PURPOSE = "Purpose";
		public const string REGION = "Region";
		public const string FOREST = "Forest";
		public const string DISTRICT = "District";
		public const string MEASUREMENTYEAR = "MeasurementYear";
		public const string CALENDARYEAR = "CalendarYear";
		public const string LOGGRADINGENABLED = "LogGradingEnabled";
		public const string REMARKS = "Remarks";
		public const string DEFAULTUOM = "DefaultUOM";
		public static string[] _ALL = { SALE_CN, SALENUMBER, NAME, PURPOSE, REGION, FOREST, DISTRICT, MEASUREMENTYEAR, CALENDARYEAR, LOGGRADINGENABLED, REMARKS, DEFAULTUOM };
		public enum SALE_FIELDS { Sale_CN, SaleNumber, Name, Purpose, Region, Forest, District, MeasurementYear, CalendarYear, LogGradingEnabled, Remarks, DefaultUOM };
	}

	public static class CUTTINGUNIT
	{
		public const string _NAME = "CuttingUnit";
		public const string CUTTINGUNIT_CN = "CuttingUnit_CN";
		public const string CODE = "Code";
		public const string AREA = "Area";
		public const string DESCRIPTION = "Description";
		public const string LOGGINGMETHOD = "LoggingMethod";
		public const string PAYMENTUNIT = "PaymentUnit";
		public const string TALLYHISTORY = "TallyHistory";
		public static string[] _ALL = { CUTTINGUNIT_CN, CODE, AREA, DESCRIPTION, LOGGINGMETHOD, PAYMENTUNIT, TALLYHISTORY };
		public enum CUTTINGUNIT_FIELDS { CuttingUnit_CN, Code, Area, Description, LoggingMethod, PaymentUnit, TallyHistory };
	}

	public static class STRATUM
	{
		public const string _NAME = "Stratum";
		public const string STRATUM_CN = "Stratum_CN";
		public const string CODE = "Code";
		public const string DESCRIPTION = "Description";
		public const string METHOD = "Method";
		public const string BASALAREAFACTOR = "BasalAreaFactor";
		public const string FIXEDPLOTSIZE = "FixedPlotSize";
		public const string KZ3PPNT = "KZ3PPNT";
		public const string HOTKEY = "Hotkey";
		public const string FBSCODE = "FBSCode";
		public const string YIELDCOMPONENT = "YieldComponent";
		public const string MONTH = "Month";
		public const string YEAR = "Year";
		public static string[] _ALL = { STRATUM_CN, CODE, DESCRIPTION, METHOD, BASALAREAFACTOR, FIXEDPLOTSIZE, KZ3PPNT, HOTKEY, FBSCODE, YIELDCOMPONENT, MONTH, YEAR };
		public enum STRATUM_FIELDS { Stratum_CN, Code, Description, Method, BasalAreaFactor, FixedPlotSize, KZ3PPNT, Hotkey, FBSCode, YieldComponent, Month, Year };
	}

	public static class CUTTINGUNITSTRATUM
	{
		public const string _NAME = "CuttingUnitStratum";
		public const string CUTTINGUNIT_CN = "CuttingUnit_CN";
		public const string STRATUM_CN = "Stratum_CN";
		public const string STRATUMAREA = "StratumArea";
		public static string[] _ALL = { CUTTINGUNIT_CN, STRATUM_CN, STRATUMAREA };
		public enum CUTTINGUNITSTRATUM_FIELDS { CuttingUnit_CN, Stratum_CN, StratumArea };
	}

	public static class SAMPLEGROUP
	{
		public const string _NAME = "SampleGroup";
		public const string SAMPLEGROUP_CN = "SampleGroup_CN";
		public const string STRATUM_CN = "Stratum_CN";
		public const string CODE = "Code";
		public const string CUTLEAVE = "CutLeave";
		public const string UOM = "UOM";
		public const string PRIMARYPRODUCT = "PrimaryProduct";
		public const string SECONDARYPRODUCT = "SecondaryProduct";
		public const string BIOMASSPRODUCT = "BiomassProduct";
		public const string DEFAULTLIVEDEAD = "DefaultLiveDead";
		public const string SAMPLINGFREQUENCY = "SamplingFrequency";
		public const string INSURANCEFREQUENCY = "InsuranceFrequency";
		public const string KZ = "KZ";
		public const string BIGBAF = "BigBAF";
		public const string SMALLFPS = "SmallFPS";
		public const string TALLYMETHOD = "TallyMethod";
		public const string DESCRIPTION = "Description";
		public const string SAMPLESELECTORTYPE = "SampleSelectorType";
		public const string SAMPLESELECTORSTATE = "SampleSelectorState";
		public const string MINKPI = "MinKPI";
		public const string MAXKPI = "MaxKPI";
		public static string[] _ALL = { SAMPLEGROUP_CN, STRATUM_CN, CODE, CUTLEAVE, UOM, PRIMARYPRODUCT, SECONDARYPRODUCT, BIOMASSPRODUCT, DEFAULTLIVEDEAD, SAMPLINGFREQUENCY, INSURANCEFREQUENCY, KZ, BIGBAF, SMALLFPS, TALLYMETHOD, DESCRIPTION, SAMPLESELECTORTYPE, SAMPLESELECTORSTATE, MINKPI, MAXKPI };
		public enum SAMPLEGROUP_FIELDS { SampleGroup_CN, Stratum_CN, Code, CutLeave, UOM, PrimaryProduct, SecondaryProduct, BiomassProduct, DefaultLiveDead, SamplingFrequency, InsuranceFrequency, KZ, BigBAF, SmallFPS, TallyMethod, Description, SampleSelectorType, SampleSelectorState, MinKPI, MaxKPI };
	}

	public static class TREEDEFAULTVALUE
	{
		public const string _NAME = "TreeDefaultValue";
		public const string TREEDEFAULTVALUE_CN = "TreeDefaultValue_CN";
		public const string PRIMARYPRODUCT = "PrimaryProduct";
		public const string SPECIES = "Species";
		public const string LIVEDEAD = "LiveDead";
		public const string FIACODE = "FIAcode";
		public const string CULLPRIMARY = "CullPrimary";
		public const string HIDDENPRIMARY = "HiddenPrimary";
		public const string CULLSECONDARY = "CullSecondary";
		public const string HIDDENSECONDARY = "HiddenSecondary";
		public const string RECOVERABLE = "Recoverable";
		public const string CHARGEABLE = "Chargeable";
		public const string CONTRACTSPECIES = "ContractSpecies";
		public const string TREEGRADE = "TreeGrade";
		public const string MERCHHEIGHTLOGLENGTH = "MerchHeightLogLength";
		public const string MERCHHEIGHTTYPE = "MerchHeightType";
		public const string FORMCLASS = "FormClass";
		public const string BARKTHICKNESSRATIO = "BarkThicknessRatio";
		public const string AVERAGEZ = "AverageZ";
		public const string REFERENCEHEIGHTPERCENT = "ReferenceHeightPercent";
		public static string[] _ALL = { TREEDEFAULTVALUE_CN, PRIMARYPRODUCT, SPECIES, LIVEDEAD, FIACODE, CULLPRIMARY, HIDDENPRIMARY, CULLSECONDARY, HIDDENSECONDARY, RECOVERABLE, CHARGEABLE, CONTRACTSPECIES, TREEGRADE, MERCHHEIGHTLOGLENGTH, MERCHHEIGHTTYPE, FORMCLASS, BARKTHICKNESSRATIO, AVERAGEZ, REFERENCEHEIGHTPERCENT };
		public enum TREEDEFAULTVALUE_FIELDS { TreeDefaultValue_CN, PrimaryProduct, Species, LiveDead, FIAcode, CullPrimary, HiddenPrimary, CullSecondary, HiddenSecondary, Recoverable, Chargeable, ContractSpecies, TreeGrade, MerchHeightLogLength, MerchHeightType, FormClass, BarkThicknessRatio, AverageZ, ReferenceHeightPercent };
	}

	public static class SAMPLEGROUPTREEDEFAULTVALUE
	{
		public const string _NAME = "SampleGroupTreeDefaultValue";
		public const string TREEDEFAULTVALUE_CN = "TreeDefaultValue_CN";
		public const string SAMPLEGROUP_CN = "SampleGroup_CN";
		public static string[] _ALL = { TREEDEFAULTVALUE_CN, SAMPLEGROUP_CN };
		public enum SAMPLEGROUPTREEDEFAULTVALUE_FIELDS { TreeDefaultValue_CN, SampleGroup_CN };
	}

	public static class PLOT
	{
		public const string _NAME = "Plot";
		public const string PLOT_CN = "Plot_CN";
		public const string PLOT_GUID = "Plot_GUID";
		public const string STRATUM_CN = "Stratum_CN";
		public const string CUTTINGUNIT_CN = "CuttingUnit_CN";
		public const string PLOTNUMBER = "PlotNumber";
		public const string ISEMPTY = "IsEmpty";
		public const string SLOPE = "Slope";
		public const string KPI = "KPI";
		public const string ASPECT = "Aspect";
		public const string REMARKS = "Remarks";
		public const string XCOORDINATE = "XCoordinate";
		public const string YCOORDINATE = "YCoordinate";
		public const string ZCOORDINATE = "ZCoordinate";
		public const string METADATA = "MetaData";
		public const string BLOB = "Blob";
		public static string[] _ALL = { PLOT_CN, PLOT_GUID, STRATUM_CN, CUTTINGUNIT_CN, PLOTNUMBER, ISEMPTY, SLOPE, KPI, ASPECT, REMARKS, XCOORDINATE, YCOORDINATE, ZCOORDINATE, METADATA, BLOB };
		public enum PLOT_FIELDS { Plot_CN, Plot_GUID, Stratum_CN, CuttingUnit_CN, PlotNumber, IsEmpty, Slope, KPI, Aspect, Remarks, XCoordinate, YCoordinate, ZCoordinate, MetaData, Blob };
	}

	public static class TREE
	{
		public const string _NAME = "Tree";
		public const string TREE_CN = "Tree_CN";
		public const string TREE_GUID = "Tree_GUID";
		public const string TREEDEFAULTVALUE_CN = "TreeDefaultValue_CN";
		public const string STRATUM_CN = "Stratum_CN";
		public const string SAMPLEGROUP_CN = "SampleGroup_CN";
		public const string CUTTINGUNIT_CN = "CuttingUnit_CN";
		public const string PLOT_CN = "Plot_CN";
		public const string TREENUMBER = "TreeNumber";
		public const string SPECIES = "Species";
		public const string COUNTORMEASURE = "CountOrMeasure";
		public const string TREECOUNT = "TreeCount";
		public const string KPI = "KPI";
		public const string STM = "STM";
		public const string SEENDEFECTPRIMARY = "SeenDefectPrimary";
		public const string SEENDEFECTSECONDARY = "SeenDefectSecondary";
		public const string RECOVERABLEPRIMARY = "RecoverablePrimary";
		public const string HIDDENPRIMARY = "HiddenPrimary";
		public const string INITIALS = "Initials";
		public const string LIVEDEAD = "LiveDead";
		public const string GRADE = "Grade";
		public const string HEIGHTTOFIRSTLIVELIMB = "HeightToFirstLiveLimb";
		public const string POLELENGTH = "PoleLength";
		public const string CLEARFACE = "ClearFace";
		public const string CROWNRATIO = "CrownRatio";
		public const string DBH = "DBH";
		public const string DRC = "DRC";
		public const string TOTALHEIGHT = "TotalHeight";
		public const string MERCHHEIGHTPRIMARY = "MerchHeightPrimary";
		public const string MERCHHEIGHTSECONDARY = "MerchHeightSecondary";
		public const string FORMCLASS = "FormClass";
		public const string UPPERSTEMDOB = "UpperStemDOB";
		public const string UPPERSTEMDIAMETER = "UpperStemDiameter";
		public const string UPPERSTEMHEIGHT = "UpperStemHeight";
		public const string DBHDOUBLEBARKTHICKNESS = "DBHDoubleBarkThickness";
		public const string TOPDIBPRIMARY = "TopDIBPrimary";
		public const string TOPDIBSECONDARY = "TopDIBSecondary";
		public const string DEFECTCODE = "DefectCode";
		public const string DIAMETERATDEFECT = "DiameterAtDefect";
		public const string VOIDPERCENT = "VoidPercent";
		public const string SLOPE = "Slope";
		public const string ASPECT = "Aspect";
		public const string REMARKS = "Remarks";
		public const string XCOORDINATE = "XCoordinate";
		public const string YCOORDINATE = "YCoordinate";
		public const string ZCOORDINATE = "ZCoordinate";
		public const string METADATA = "MetaData";
		public const string ISFALLBUCKSCALE = "IsFallBuckScale";
		public const string EXPANSIONFACTOR = "ExpansionFactor";
		public const string TREEFACTOR = "TreeFactor";
		public const string POINTFACTOR = "PointFactor";
		public static string[] _ALL = { TREE_CN, TREE_GUID, TREEDEFAULTVALUE_CN, STRATUM_CN, SAMPLEGROUP_CN, CUTTINGUNIT_CN, PLOT_CN, TREENUMBER, SPECIES, COUNTORMEASURE, TREECOUNT, KPI, STM, SEENDEFECTPRIMARY, SEENDEFECTSECONDARY, RECOVERABLEPRIMARY, HIDDENPRIMARY, INITIALS, LIVEDEAD, GRADE, HEIGHTTOFIRSTLIVELIMB, POLELENGTH, CLEARFACE, CROWNRATIO, DBH, DRC, TOTALHEIGHT, MERCHHEIGHTPRIMARY, MERCHHEIGHTSECONDARY, FORMCLASS, UPPERSTEMDOB, UPPERSTEMDIAMETER, UPPERSTEMHEIGHT, DBHDOUBLEBARKTHICKNESS, TOPDIBPRIMARY, TOPDIBSECONDARY, DEFECTCODE, DIAMETERATDEFECT, VOIDPERCENT, SLOPE, ASPECT, REMARKS, XCOORDINATE, YCOORDINATE, ZCOORDINATE, METADATA, ISFALLBUCKSCALE, EXPANSIONFACTOR, TREEFACTOR, POINTFACTOR };
		public enum TREE_FIELDS { Tree_CN, Tree_GUID, TreeDefaultValue_CN, Stratum_CN, SampleGroup_CN, CuttingUnit_CN, Plot_CN, TreeNumber, Species, CountOrMeasure, TreeCount, KPI, STM, SeenDefectPrimary, SeenDefectSecondary, RecoverablePrimary, HiddenPrimary, Initials, LiveDead, Grade, HeightToFirstLiveLimb, PoleLength, ClearFace, CrownRatio, DBH, DRC, TotalHeight, MerchHeightPrimary, MerchHeightSecondary, FormClass, UpperStemDOB, UpperStemDiameter, UpperStemHeight, DBHDoubleBarkThickness, TopDIBPrimary, TopDIBSecondary, DefectCode, DiameterAtDefect, VoidPercent, Slope, Aspect, Remarks, XCoordinate, YCoordinate, ZCoordinate, MetaData, IsFallBuckScale, ExpansionFactor, TreeFactor, PointFactor };
	}

	public static class LOG
	{
		public const string _NAME = "Log";
		public const string LOG_CN = "Log_CN";
		public const string LOG_GUID = "Log_GUID";
		public const string TREE_CN = "Tree_CN";
		public const string LOGNUMBER = "LogNumber";
		public const string GRADE = "Grade";
		public const string SEENDEFECT = "SeenDefect";
		public const string PERCENTRECOVERABLE = "PercentRecoverable";
		public const string LENGTH = "Length";
		public const string EXPORTGRADE = "ExportGrade";
		public const string SMALLENDDIAMETER = "SmallEndDiameter";
		public const string LARGEENDDIAMETER = "LargeEndDiameter";
		public const string GROSSBOARDFOOT = "GrossBoardFoot";
		public const string NETBOARDFOOT = "NetBoardFoot";
		public const string GROSSCUBICFOOT = "GrossCubicFoot";
		public const string NETCUBICFOOT = "NetCubicFoot";
		public const string BOARDFOOTREMOVED = "BoardFootRemoved";
		public const string CUBICFOOTREMOVED = "CubicFootRemoved";
		public const string DIBCLASS = "DIBClass";
		public const string BARKTHICKNESS = "BarkThickness";
		public static string[] _ALL = { LOG_CN, LOG_GUID, TREE_CN, LOGNUMBER, GRADE, SEENDEFECT, PERCENTRECOVERABLE, LENGTH, EXPORTGRADE, SMALLENDDIAMETER, LARGEENDDIAMETER, GROSSBOARDFOOT, NETBOARDFOOT, GROSSCUBICFOOT, NETCUBICFOOT, BOARDFOOTREMOVED, CUBICFOOTREMOVED, DIBCLASS, BARKTHICKNESS };
		public enum LOG_FIELDS { Log_CN, Log_GUID, Tree_CN, LogNumber, Grade, SeenDefect, PercentRecoverable, Length, ExportGrade, SmallEndDiameter, LargeEndDiameter, GrossBoardFoot, NetBoardFoot, GrossCubicFoot, NetCubicFoot, BoardFootRemoved, CubicFootRemoved, DIBClass, BarkThickness };
	}

	public static class STEM
	{
		public const string _NAME = "Stem";
		public const string STEM_CN = "Stem_CN";
		public const string STEM_GUID = "Stem_GUID";
		public const string TREE_CN = "Tree_CN";
		public const string DIAMETER = "Diameter";
		public const string DIAMETERTYPE = "DiameterType";
		public static string[] _ALL = { STEM_CN, STEM_GUID, TREE_CN, DIAMETER, DIAMETERTYPE };
		public enum STEM_FIELDS { Stem_CN, Stem_GUID, Tree_CN, Diameter, DiameterType };
	}

	public static class COUNTTREE
	{
		public const string _NAME = "CountTree";
		public const string COUNTTREE_CN = "CountTree_CN";
		public const string SAMPLEGROUP_CN = "SampleGroup_CN";
		public const string CUTTINGUNIT_CN = "CuttingUnit_CN";
		public const string TALLY_CN = "Tally_CN";
		public const string TREEDEFAULTVALUE_CN = "TreeDefaultValue_CN";
		public const string COMPONENT_CN = "Component_CN";
		public const string TREECOUNT = "TreeCount";
		public const string SUMKPI = "SumKPI";
		public static string[] _ALL = { COUNTTREE_CN, SAMPLEGROUP_CN, CUTTINGUNIT_CN, TALLY_CN, TREEDEFAULTVALUE_CN, COMPONENT_CN, TREECOUNT, SUMKPI };
		public enum COUNTTREE_FIELDS { CountTree_CN, SampleGroup_CN, CuttingUnit_CN, Tally_CN, TreeDefaultValue_CN, Component_CN, TreeCount, SumKPI };
	}

	public static class TALLY
	{
		public const string _NAME = "Tally";
		public const string TALLY_CN = "Tally_CN";
		public const string HOTKEY = "Hotkey";
		public const string DESCRIPTION = "Description";
		public const string INDICATORVALUE = "IndicatorValue";
		public const string INDICATORTYPE = "IndicatorType";
		public static string[] _ALL = { TALLY_CN, HOTKEY, DESCRIPTION, INDICATORVALUE, INDICATORTYPE };
		public enum TALLY_FIELDS { Tally_CN, Hotkey, Description, IndicatorValue, IndicatorType };
	}

	public static class TREEESTIMATE
	{
		public const string _NAME = "TreeEstimate";
		public const string TREEESTIMATE_CN = "TreeEstimate_CN";
		public const string COUNTTREE_CN = "CountTree_CN";
		public const string TREEESTIMATE_GUID = "TreeEstimate_GUID";
		public const string KPI = "KPI";
		public static string[] _ALL = { TREEESTIMATE_CN, COUNTTREE_CN, TREEESTIMATE_GUID, KPI };
		public enum TREEESTIMATE_FIELDS { TreeEstimate_CN, CountTree_CN, TreeEstimate_GUID, KPI };
	}

	public static class FIXCNTTALLYCLASS
	{
		public const string _NAME = "FixCNTTallyClass";
		public const string FIXCNTTALLYCLASS_CN = "FixCNTTallyClass_CN";
		public const string STRATUM_CN = "Stratum_CN";
		public const string FIELDNAME = "FieldName";
		public static string[] _ALL = { FIXCNTTALLYCLASS_CN, STRATUM_CN, FIELDNAME };
		public enum FIXCNTTALLYCLASS_FIELDS { FixCNTTallyClass_CN, Stratum_CN, FieldName };
	}

	public static class FIXCNTTALLYPOPULATION
	{
		public const string _NAME = "FixCNTTallyPopulation";
		public const string FIXCNTTALLYPOPULATION_CN = "FixCNTTallyPopulation_CN";
		public const string FIXCNTTALLYCLASS_CN = "FixCNTTallyClass_CN";
		public const string SAMPLEGROUP_CN = "SampleGroup_CN";
		public const string TREEDEFAULTVALUE_CN = "TreeDefaultValue_CN";
		public const string INTERVALSIZE = "IntervalSize";
		public const string MIN = "Min";
		public const string MAX = "Max";
		public static string[] _ALL = { FIXCNTTALLYPOPULATION_CN, FIXCNTTALLYCLASS_CN, SAMPLEGROUP_CN, TREEDEFAULTVALUE_CN, INTERVALSIZE, MIN, MAX };
		public enum FIXCNTTALLYPOPULATION_FIELDS { FixCNTTallyPopulation_CN, FixCNTTallyClass_CN, SampleGroup_CN, TreeDefaultValue_CN, IntervalSize, Min, Max };
	}

	#endregion
	#region Processing Tables
	public static class VOLUMEEQUATION
	{
		public const string _NAME = "VolumeEquation";
		public const string SPECIES = "Species";
		public const string PRIMARYPRODUCT = "PrimaryProduct";
		public const string VOLUMEEQUATIONNUMBER = "VolumeEquationNumber";
		public const string STUMPHEIGHT = "StumpHeight";
		public const string TOPDIBPRIMARY = "TopDIBPrimary";
		public const string TOPDIBSECONDARY = "TopDIBSecondary";
		public const string CALCTOTAL = "CalcTotal";
		public const string CALCBOARD = "CalcBoard";
		public const string CALCCUBIC = "CalcCubic";
		public const string CALCCORD = "CalcCord";
		public const string CALCTOPWOOD = "CalcTopwood";
		public const string CALCBIOMASS = "CalcBiomass";
		public const string TRIM = "Trim";
		public const string SEGMENTATIONLOGIC = "SegmentationLogic";
		public const string MINLOGLENGTHPRIMARY = "MinLogLengthPrimary";
		public const string MAXLOGLENGTHPRIMARY = "MaxLogLengthPrimary";
		public const string MINLOGLENGTHSECONDARY = "MinLogLengthSecondary";
		public const string MAXLOGLENGTHSECONDARY = "MaxLogLengthSecondary";
		public const string MINMERCHLENGTH = "MinMerchLength";
		public const string MODEL = "Model";
		public const string COMMONSPECIESNAME = "CommonSpeciesName";
		public const string MERCHMODFLAG = "MerchModFlag";
		public const string EVENODDSEGMENT = "EvenOddSegment";
		public static string[] _ALL = { SPECIES, PRIMARYPRODUCT, VOLUMEEQUATIONNUMBER, STUMPHEIGHT, TOPDIBPRIMARY, TOPDIBSECONDARY, CALCTOTAL, CALCBOARD, CALCCUBIC, CALCCORD, CALCTOPWOOD, CALCBIOMASS, TRIM, SEGMENTATIONLOGIC, MINLOGLENGTHPRIMARY, MAXLOGLENGTHPRIMARY, MINLOGLENGTHSECONDARY, MAXLOGLENGTHSECONDARY, MINMERCHLENGTH, MODEL, COMMONSPECIESNAME, MERCHMODFLAG, EVENODDSEGMENT };
		public enum VOLUMEEQUATION_FIELDS { Species, PrimaryProduct, VolumeEquationNumber, StumpHeight, TopDIBPrimary, TopDIBSecondary, CalcTotal, CalcBoard, CalcCubic, CalcCord, CalcTopwood, CalcBiomass, Trim, SegmentationLogic, MinLogLengthPrimary, MaxLogLengthPrimary, MinLogLengthSecondary, MaxLogLengthSecondary, MinMerchLength, Model, CommonSpeciesName, MerchModFlag, EvenOddSegment };
	}

	public static class BIOMASSEQUATION
	{
		public const string _NAME = "BiomassEquation";
		public const string SPECIES = "Species";
		public const string PRODUCT = "Product";
		public const string COMPONENT = "Component";
		public const string LIVEDEAD = "LiveDead";
		public const string FIACODE = "FIAcode";
		public const string EQUATION = "Equation";
		public const string PERCENTMOISTURE = "PercentMoisture";
		public const string PERCENTREMOVED = "PercentRemoved";
		public const string METADATA = "MetaData";
		public const string WEIGHTFACTORPRIMARY = "WeightFactorPrimary";
		public const string WEIGHTFACTORSECONDARY = "WeightFactorSecondary";
		public static string[] _ALL = { SPECIES, PRODUCT, COMPONENT, LIVEDEAD, FIACODE, EQUATION, PERCENTMOISTURE, PERCENTREMOVED, METADATA, WEIGHTFACTORPRIMARY, WEIGHTFACTORSECONDARY };
		public enum BIOMASSEQUATION_FIELDS { Species, Product, Component, LiveDead, FIAcode, Equation, PercentMoisture, PercentRemoved, MetaData, WeightFactorPrimary, WeightFactorSecondary };
	}

	public static class VALUEEQUATION
	{
		public const string _NAME = "ValueEquation";
		public const string SPECIES = "Species";
		public const string PRIMARYPRODUCT = "PrimaryProduct";
		public const string VALUEEQUATIONNUMBER = "ValueEquationNumber";
		public const string GRADE = "Grade";
		public const string COEFFICIENT1 = "Coefficient1";
		public const string COEFFICIENT2 = "Coefficient2";
		public const string COEFFICIENT3 = "Coefficient3";
		public const string COEFFICIENT4 = "Coefficient4";
		public const string COEFFICIENT5 = "Coefficient5";
		public const string COEFFICIENT6 = "Coefficient6";
		public static string[] _ALL = { SPECIES, PRIMARYPRODUCT, VALUEEQUATIONNUMBER, GRADE, COEFFICIENT1, COEFFICIENT2, COEFFICIENT3, COEFFICIENT4, COEFFICIENT5, COEFFICIENT6 };
		public enum VALUEEQUATION_FIELDS { Species, PrimaryProduct, ValueEquationNumber, Grade, Coefficient1, Coefficient2, Coefficient3, Coefficient4, Coefficient5, Coefficient6 };
	}

	public static class QUALITYADJEQUATION
	{
		public const string _NAME = "QualityAdjEquation";
		public const string SPECIES = "Species";
		public const string QUALITYADJEQ = "QualityAdjEq";
		public const string YEAR = "Year";
		public const string GRADE = "Grade";
		public const string COEFFICIENT1 = "Coefficient1";
		public const string COEFFICIENT2 = "Coefficient2";
		public const string COEFFICIENT3 = "Coefficient3";
		public const string COEFFICIENT4 = "Coefficient4";
		public const string COEFFICIENT5 = "Coefficient5";
		public const string COEFFICIENT6 = "Coefficient6";
		public static string[] _ALL = { SPECIES, QUALITYADJEQ, YEAR, GRADE, COEFFICIENT1, COEFFICIENT2, COEFFICIENT3, COEFFICIENT4, COEFFICIENT5, COEFFICIENT6 };
		public enum QUALITYADJEQUATION_FIELDS { Species, QualityAdjEq, Year, Grade, Coefficient1, Coefficient2, Coefficient3, Coefficient4, Coefficient5, Coefficient6 };
	}

	public static class REPORTS
	{
		public const string _NAME = "Reports";
		public const string REPORTID = "ReportID";
		public const string SELECTED = "Selected";
		public const string TITLE = "Title";
		public static string[] _ALL = { REPORTID, SELECTED, TITLE };
		public enum REPORTS_FIELDS { ReportID, Selected, Title };
	}

	public static class TREECALCULATEDVALUES
	{
		public const string _NAME = "TreeCalculatedValues";
		public const string TREECALCVALUES_CN = "TreeCalcValues_CN";
		public const string TREE_CN = "Tree_CN";
		public const string TOTALCUBICVOLUME = "TotalCubicVolume";
		public const string GROSSBDFTPP = "GrossBDFTPP";
		public const string NETBDFTPP = "NetBDFTPP";
		public const string GROSSCUFTPP = "GrossCUFTPP";
		public const string NETCUFTPP = "NetCUFTPP";
		public const string CORDSPP = "CordsPP";
		public const string GROSSBDFTREMVPP = "GrossBDFTRemvPP";
		public const string GROSSCUFTREMVPP = "GrossCUFTRemvPP";
		public const string GROSSBDFTSP = "GrossBDFTSP";
		public const string NETBDFTSP = "NetBDFTSP";
		public const string GROSSCUFTSP = "GrossCUFTSP";
		public const string NETCUFTSP = "NetCUFTSP";
		public const string CORDSSP = "CordsSP";
		public const string GROSSCUFTREMVSP = "GrossCUFTRemvSP";
		public const string NUMBERLOGSMS = "NumberlogsMS";
		public const string NUMBERLOGSTPW = "NumberlogsTPW";
		public const string GROSSBDFTRP = "GrossBDFTRP";
		public const string GROSSCUFTRP = "GrossCUFTRP";
		public const string CORDSRP = "CordsRP";
		public const string GROSSBDFTINTL = "GrossBDFTIntl";
		public const string NETBDFTINTL = "NetBDFTIntl";
		public const string BIOMASSMAINSTEMPRIMARY = "BiomassMainStemPrimary";
		public const string BIOMASSMAINSTEMSECONDARY = "BiomassMainStemSecondary";
		public const string VALUEPP = "ValuePP";
		public const string VALUESP = "ValueSP";
		public const string VALUERP = "ValueRP";
		public const string BIOMASSPROD = "BiomassProd";
		public const string BIOMASSTOTALSTEM = "Biomasstotalstem";
		public const string BIOMASSLIVEBRANCHES = "Biomasslivebranches";
		public const string BIOMASSDEADBRANCHES = "Biomassdeadbranches";
		public const string BIOMASSFOLIAGE = "Biomassfoliage";
		public const string BIOMASSTIP = "BiomassTip";
		public static string[] _ALL = { TREECALCVALUES_CN, TREE_CN, TOTALCUBICVOLUME, GROSSBDFTPP, NETBDFTPP, GROSSCUFTPP, NETCUFTPP, CORDSPP, GROSSBDFTREMVPP, GROSSCUFTREMVPP, GROSSBDFTSP, NETBDFTSP, GROSSCUFTSP, NETCUFTSP, CORDSSP, GROSSCUFTREMVSP, NUMBERLOGSMS, NUMBERLOGSTPW, GROSSBDFTRP, GROSSCUFTRP, CORDSRP, GROSSBDFTINTL, NETBDFTINTL, BIOMASSMAINSTEMPRIMARY, BIOMASSMAINSTEMSECONDARY, VALUEPP, VALUESP, VALUERP, BIOMASSPROD, BIOMASSTOTALSTEM, BIOMASSLIVEBRANCHES, BIOMASSDEADBRANCHES, BIOMASSFOLIAGE, BIOMASSTIP };
		public enum TREECALCULATEDVALUES_FIELDS { TreeCalcValues_CN, Tree_CN, TotalCubicVolume, GrossBDFTPP, NetBDFTPP, GrossCUFTPP, NetCUFTPP, CordsPP, GrossBDFTRemvPP, GrossCUFTRemvPP, GrossBDFTSP, NetBDFTSP, GrossCUFTSP, NetCUFTSP, CordsSP, GrossCUFTRemvSP, NumberlogsMS, NumberlogsTPW, GrossBDFTRP, GrossCUFTRP, CordsRP, GrossBDFTIntl, NetBDFTIntl, BiomassMainStemPrimary, BiomassMainStemSecondary, ValuePP, ValueSP, ValueRP, BiomassProd, Biomasstotalstem, Biomasslivebranches, Biomassdeadbranches, Biomassfoliage, BiomassTip };
	}

	public static class LCD
	{
		public const string _NAME = "LCD";
		public const string LCD_CN = "LCD_CN";
		public const string CUTLEAVE = "CutLeave";
		public const string STRATUM = "Stratum";
		public const string SAMPLEGROUP = "SampleGroup";
		public const string SPECIES = "Species";
		public const string PRIMARYPRODUCT = "PrimaryProduct";
		public const string SECONDARYPRODUCT = "SecondaryProduct";
		public const string UOM = "UOM";
		public const string LIVEDEAD = "LiveDead";
		public const string YIELD = "Yield";
		public const string CONTRACTSPECIES = "ContractSpecies";
		public const string TREEGRADE = "TreeGrade";
		public const string STM = "STM";
		public const string FIRSTSTAGETREES = "FirstStageTrees";
		public const string MEASUREDTREES = "MeasuredTrees";
		public const string TALLIEDTREES = "TalliedTrees";
		public const string SUMKPI = "SumKPI";
		public const string SUMMEASUREDKPI = "SumMeasuredKPI";
		public const string SUMEXPANFACTOR = "SumExpanFactor";
		public const string SUMDBHOB = "SumDBHOB";
		public const string SUMDBHOBSQRD = "SumDBHOBsqrd";
		public const string SUMTOTHGT = "SumTotHgt";
		public const string SUMHGTUPSTEM = "SumHgtUpStem";
		public const string SUMMERCHHGTPRIM = "SumMerchHgtPrim";
		public const string SUMMERCHHGTSECOND = "SumMerchHgtSecond";
		public const string SUMLOGSMS = "SumLogsMS";
		public const string SUMTOTCUBIC = "SumTotCubic";
		public const string SUMGBDFT = "SumGBDFT";
		public const string SUMNBDFT = "SumNBDFT";
		public const string SUMGCUFT = "SumGCUFT";
		public const string SUMNCUFT = "SumNCUFT";
		public const string SUMGBDFTREMV = "SumGBDFTremv";
		public const string SUMGCUFTREMV = "SumGCUFTremv";
		public const string SUMCORDS = "SumCords";
		public const string SUMWGTMSP = "SumWgtMSP";
		public const string SUMVALUE = "SumValue";
		public const string SUMGBDFTTOP = "SumGBDFTtop";
		public const string SUMNBDFTTOP = "SumNBDFTtop";
		public const string SUMGCUFTTOP = "SumGCUFTtop";
		public const string SUMNCUFTTOP = "SumNCUFTtop";
		public const string SUMCORDSTOP = "SumCordsTop";
		public const string SUMWGTMSS = "SumWgtMSS";
		public const string SUMTOPVALUE = "SumTopValue";
		public const string SUMLOGSTOP = "SumLogsTop";
		public const string SUMBDFTRECV = "SumBDFTrecv";
		public const string SUMCUFTRECV = "SumCUFTrecv";
		public const string SUMCORDSRECV = "SumCordsRecv";
		public const string SUMVALUERECV = "SumValueRecv";
		public const string BIOMASSPRODUCT = "BiomassProduct";
		public const string SUMWGTBAT = "SumWgtBAT";
		public const string SUMWGTBBL = "SumWgtBBL";
		public const string SUMWGTBBD = "SumWgtBBD";
		public const string SUMWGTBFT = "SumWgtBFT";
		public const string SUMWGTTIP = "SumWgtTip";
		public static string[] _ALL = { LCD_CN, CUTLEAVE, STRATUM, SAMPLEGROUP, SPECIES, PRIMARYPRODUCT, SECONDARYPRODUCT, UOM, LIVEDEAD, YIELD, CONTRACTSPECIES, TREEGRADE, STM, FIRSTSTAGETREES, MEASUREDTREES, TALLIEDTREES, SUMKPI, SUMMEASUREDKPI, SUMEXPANFACTOR, SUMDBHOB, SUMDBHOBSQRD, SUMTOTHGT, SUMHGTUPSTEM, SUMMERCHHGTPRIM, SUMMERCHHGTSECOND, SUMLOGSMS, SUMTOTCUBIC, SUMGBDFT, SUMNBDFT, SUMGCUFT, SUMNCUFT, SUMGBDFTREMV, SUMGCUFTREMV, SUMCORDS, SUMWGTMSP, SUMVALUE, SUMGBDFTTOP, SUMNBDFTTOP, SUMGCUFTTOP, SUMNCUFTTOP, SUMCORDSTOP, SUMWGTMSS, SUMTOPVALUE, SUMLOGSTOP, SUMBDFTRECV, SUMCUFTRECV, SUMCORDSRECV, SUMVALUERECV, BIOMASSPRODUCT, SUMWGTBAT, SUMWGTBBL, SUMWGTBBD, SUMWGTBFT, SUMWGTTIP };
		public enum LCD_FIELDS { LCD_CN, CutLeave, Stratum, SampleGroup, Species, PrimaryProduct, SecondaryProduct, UOM, LiveDead, Yield, ContractSpecies, TreeGrade, STM, FirstStageTrees, MeasuredTrees, TalliedTrees, SumKPI, SumMeasuredKPI, SumExpanFactor, SumDBHOB, SumDBHOBsqrd, SumTotHgt, SumHgtUpStem, SumMerchHgtPrim, SumMerchHgtSecond, SumLogsMS, SumTotCubic, SumGBDFT, SumNBDFT, SumGCUFT, SumNCUFT, SumGBDFTremv, SumGCUFTremv, SumCords, SumWgtMSP, SumValue, SumGBDFTtop, SumNBDFTtop, SumGCUFTtop, SumNCUFTtop, SumCordsTop, SumWgtMSS, SumTopValue, SumLogsTop, SumBDFTrecv, SumCUFTrecv, SumCordsRecv, SumValueRecv, BiomassProduct, SumWgtBAT, SumWgtBBL, SumWgtBBD, SumWgtBFT, SumWgtTip };
	}

	public static class POP
	{
		public const string _NAME = "POP";
		public const string POP_CN = "POP_CN";
		public const string CUTLEAVE = "CutLeave";
		public const string STRATUM = "Stratum";
		public const string SAMPLEGROUP = "SampleGroup";
		public const string PRIMARYPRODUCT = "PrimaryProduct";
		public const string SECONDARYPRODUCT = "SecondaryProduct";
		public const string STM = "STM";
		public const string UOM = "UOM";
		public const string FIRSTSTAGETREES = "FirstStageTrees";
		public const string MEASUREDTREES = "MeasuredTrees";
		public const string TALLIEDTREES = "TalliedTrees";
		public const string SUMKPI = "SumKPI";
		public const string SUMMEASUREDKPI = "SumMeasuredKPI";
		public const string STAGEONESAMPLES = "StageOneSamples";
		public const string STAGETWOSAMPLES = "StageTwoSamples";
		public const string STG1GROSSXPP = "Stg1GrossXPP";
		public const string STG1GROSSXSQRDPP = "Stg1GrossXsqrdPP";
		public const string STG1NETXPP = "Stg1NetXPP";
		public const string STG1NETXSQRDPP = "Stg1NetXsqrdPP";
		public const string STG1VALUEXPP = "Stg1ValueXPP";
		public const string STG1VALUEXSQRDPP = "Stg1ValueXsqrdPP";
		public const string STG2GROSSXPP = "Stg2GrossXPP";
		public const string STG2GROSSXSQRDPP = "Stg2GrossXsqrdPP";
		public const string STG2NETXPP = "Stg2NetXPP";
		public const string STG2NETXSQRDPP = "Stg2NetXsqrdPP";
		public const string STG2VALUEXPP = "Stg2ValueXPP";
		public const string STG2VALUEXSQRDPP = "Stg2ValueXsqrdPP";
		public const string STG1GROSSXSP = "Stg1GrossXSP";
		public const string STG1GROSSXSQRDSP = "Stg1GrossXsqrdSP";
		public const string STG1NETXSP = "Stg1NetXSP";
		public const string STG1NETXSQRDSP = "Stg1NetXsqrdSP";
		public const string STG1VALUEXSP = "Stg1ValueXSP";
		public const string STG1VALUEXSQRDSP = "Stg1ValueXsqrdSP";
		public const string STG2GROSSXSP = "Stg2GrossXSP";
		public const string STG2GROSSXSQRDSP = "Stg2GrossXsqrdSP";
		public const string STG2NETXSP = "Stg2NetXSP";
		public const string STG2NETXSQRDSP = "Stg2NetXsqrdSP";
		public const string STG2VALUEXSP = "Stg2ValueXSP";
		public const string STG2VALUEXSQRDSP = "Stg2ValueXsqrdSP";
		public const string STG1GROSSXRP = "Stg1GrossXRP";
		public const string STG1GROSSXSQRDRP = "Stg1GrossXsqrdRP";
		public const string STG1NETXRP = "Stg1NetXRP";
		public const string STG1NETXRSQRDRP = "Stg1NetXRsqrdRP";
		public const string STG1VALUEXRP = "Stg1ValueXRP";
		public const string STG1VALUEXSQRDRP = "Stg1ValueXsqrdRP";
		public const string STG2GROSSXRP = "Stg2GrossXRP";
		public const string STG2GROSSXSQRDRP = "Stg2GrossXsqrdRP";
		public const string STG2NETXRP = "Stg2NetXRP";
		public const string STG2NETXSQRDRP = "Stg2NetXsqrdRP";
		public const string STG2VALUEXRP = "Stg2ValueXRP";
		public const string STG2VALUEXSQRDRP = "Stg2ValueXsqrdRP";
		public static string[] _ALL = { POP_CN, CUTLEAVE, STRATUM, SAMPLEGROUP, PRIMARYPRODUCT, SECONDARYPRODUCT, STM, UOM, FIRSTSTAGETREES, MEASUREDTREES, TALLIEDTREES, SUMKPI, SUMMEASUREDKPI, STAGEONESAMPLES, STAGETWOSAMPLES, STG1GROSSXPP, STG1GROSSXSQRDPP, STG1NETXPP, STG1NETXSQRDPP, STG1VALUEXPP, STG1VALUEXSQRDPP, STG2GROSSXPP, STG2GROSSXSQRDPP, STG2NETXPP, STG2NETXSQRDPP, STG2VALUEXPP, STG2VALUEXSQRDPP, STG1GROSSXSP, STG1GROSSXSQRDSP, STG1NETXSP, STG1NETXSQRDSP, STG1VALUEXSP, STG1VALUEXSQRDSP, STG2GROSSXSP, STG2GROSSXSQRDSP, STG2NETXSP, STG2NETXSQRDSP, STG2VALUEXSP, STG2VALUEXSQRDSP, STG1GROSSXRP, STG1GROSSXSQRDRP, STG1NETXRP, STG1NETXRSQRDRP, STG1VALUEXRP, STG1VALUEXSQRDRP, STG2GROSSXRP, STG2GROSSXSQRDRP, STG2NETXRP, STG2NETXSQRDRP, STG2VALUEXRP, STG2VALUEXSQRDRP };
		public enum POP_FIELDS { POP_CN, CutLeave, Stratum, SampleGroup, PrimaryProduct, SecondaryProduct, STM, UOM, FirstStageTrees, MeasuredTrees, TalliedTrees, SumKPI, SumMeasuredKPI, StageOneSamples, StageTwoSamples, Stg1GrossXPP, Stg1GrossXsqrdPP, Stg1NetXPP, Stg1NetXsqrdPP, Stg1ValueXPP, Stg1ValueXsqrdPP, Stg2GrossXPP, Stg2GrossXsqrdPP, Stg2NetXPP, Stg2NetXsqrdPP, Stg2ValueXPP, Stg2ValueXsqrdPP, Stg1GrossXSP, Stg1GrossXsqrdSP, Stg1NetXSP, Stg1NetXsqrdSP, Stg1ValueXSP, Stg1ValueXsqrdSP, Stg2GrossXSP, Stg2GrossXsqrdSP, Stg2NetXSP, Stg2NetXsqrdSP, Stg2ValueXSP, Stg2ValueXsqrdSP, Stg1GrossXRP, Stg1GrossXsqrdRP, Stg1NetXRP, Stg1NetXRsqrdRP, Stg1ValueXRP, Stg1ValueXsqrdRP, Stg2GrossXRP, Stg2GrossXsqrdRP, Stg2NetXRP, Stg2NetXsqrdRP, Stg2ValueXRP, Stg2ValueXsqrdRP };
	}

	public static class PRO
	{
		public const string _NAME = "PRO";
		public const string PRO_CN = "PRO_CN";
		public const string CUTLEAVE = "CutLeave";
		public const string STRATUM = "Stratum";
		public const string CUTTINGUNIT = "CuttingUnit";
		public const string SAMPLEGROUP = "SampleGroup";
		public const string PRIMARYPRODUCT = "PrimaryProduct";
		public const string SECONDARYPRODUCT = "SecondaryProduct";
		public const string UOM = "UOM";
		public const string STM = "STM";
		public const string FIRSTSTAGETREES = "FirstStageTrees";
		public const string MEASUREDTREES = "MeasuredTrees";
		public const string TALLIEDTREES = "TalliedTrees";
		public const string SUMKPI = "SumKPI";
		public const string SUMMEASUREDKPI = "SumMeasuredKPI";
		public const string PRORATIONFACTOR = "ProrationFactor";
		public const string PRORATEDESTIMATEDTREES = "ProratedEstimatedTrees";
		public static string[] _ALL = { PRO_CN, CUTLEAVE, STRATUM, CUTTINGUNIT, SAMPLEGROUP, PRIMARYPRODUCT, SECONDARYPRODUCT, UOM, STM, FIRSTSTAGETREES, MEASUREDTREES, TALLIEDTREES, SUMKPI, SUMMEASUREDKPI, PRORATIONFACTOR, PRORATEDESTIMATEDTREES };
		public enum PRO_FIELDS { PRO_CN, CutLeave, Stratum, CuttingUnit, SampleGroup, PrimaryProduct, SecondaryProduct, UOM, STM, FirstStageTrees, MeasuredTrees, TalliedTrees, SumKPI, SumMeasuredKPI, ProrationFactor, ProratedEstimatedTrees };
	}

	public static class LOGSTOCK
	{
		public const string _NAME = "LogStock";
		public const string LOGSTOCK_CN = "LogStock_CN";
		public const string TREE_CN = "Tree_CN";
		public const string LOGNUMBER = "LogNumber";
		public const string GRADE = "Grade";
		public const string SEENDEFECT = "SeenDefect";
		public const string PERCENTRECOVERABLE = "PercentRecoverable";
		public const string LENGTH = "Length";
		public const string EXPORTGRADE = "ExportGrade";
		public const string SMALLENDDIAMETER = "SmallEndDiameter";
		public const string LARGEENDDIAMETER = "LargeEndDiameter";
		public const string GROSSBOARDFOOT = "GrossBoardFoot";
		public const string NETBOARDFOOT = "NetBoardFoot";
		public const string GROSSCUBICFOOT = "GrossCubicFoot";
		public const string NETCUBICFOOT = "NetCubicFoot";
		public const string BOARDFOOTREMOVED = "BoardFootRemoved";
		public const string CUBICFOOTREMOVED = "CubicFootRemoved";
		public const string DIBCLASS = "DIBClass";
		public const string BARKTHICKNESS = "BarkThickness";
		public const string BOARDUTIL = "BoardUtil";
		public const string CUBICUTIL = "CubicUtil";
		public static string[] _ALL = { LOGSTOCK_CN, TREE_CN, LOGNUMBER, GRADE, SEENDEFECT, PERCENTRECOVERABLE, LENGTH, EXPORTGRADE, SMALLENDDIAMETER, LARGEENDDIAMETER, GROSSBOARDFOOT, NETBOARDFOOT, GROSSCUBICFOOT, NETCUBICFOOT, BOARDFOOTREMOVED, CUBICFOOTREMOVED, DIBCLASS, BARKTHICKNESS, BOARDUTIL, CUBICUTIL };
		public enum LOGSTOCK_FIELDS { LogStock_CN, Tree_CN, LogNumber, Grade, SeenDefect, PercentRecoverable, Length, ExportGrade, SmallEndDiameter, LargeEndDiameter, GrossBoardFoot, NetBoardFoot, GrossCubicFoot, NetCubicFoot, BoardFootRemoved, CubicFootRemoved, DIBClass, BarkThickness, BoardUtil, CubicUtil };
	}

	public static class SAMPLEGROUPSTATS
	{
		public const string _NAME = "SampleGroupStats";
		public const string SAMPLEGROUPSTATS_CN = "SampleGroupStats_CN";
		public const string STRATUMSTATS_CN = "StratumStats_CN";
		public const string CODE = "Code";
		public const string SGSET = "SgSet";
		public const string DESCRIPTION = "Description";
		public const string CUTLEAVE = "CutLeave";
		public const string UOM = "UOM";
		public const string PRIMARYPRODUCT = "PrimaryProduct";
		public const string SECONDARYPRODUCT = "SecondaryProduct";
		public const string DEFAULTLIVEDEAD = "DefaultLiveDead";
		public const string SGERROR = "SgError";
		public const string SAMPLESIZE1 = "SampleSize1";
		public const string SAMPLESIZE2 = "SampleSize2";
		public const string CV1 = "CV1";
		public const string CV2 = "CV2";
		public const string TREESPERACRE = "TreesPerAcre";
		public const string VOLUMEPERACRE = "VolumePerAcre";
		public const string TREESPERPLOT = "TreesPerPlot";
		public const string AVERAGEHEIGHT = "AverageHeight";
		public const string SAMPLINGFREQUENCY = "SamplingFrequency";
		public const string INSURANCEFREQUENCY = "InsuranceFrequency";
		public const string KZ = "KZ";
		public const string BIGBAF = "BigBAF";
		public const string BIGFIX = "BigFIX";
		public const string MINDBH = "MinDbh";
		public const string MAXDBH = "MaxDbh";
		public const string CV_DEF = "CV_Def";
		public const string CV2_DEF = "CV2_Def";
		public const string TPA_DEF = "TPA_Def";
		public const string VPA_DEF = "VPA_Def";
		public const string RECONPLOTS = "ReconPlots";
		public const string RECONTREES = "ReconTrees";
		public static string[] _ALL = { SAMPLEGROUPSTATS_CN, STRATUMSTATS_CN, CODE, SGSET, DESCRIPTION, CUTLEAVE, UOM, PRIMARYPRODUCT, SECONDARYPRODUCT, DEFAULTLIVEDEAD, SGERROR, SAMPLESIZE1, SAMPLESIZE2, CV1, CV2, TREESPERACRE, VOLUMEPERACRE, TREESPERPLOT, AVERAGEHEIGHT, SAMPLINGFREQUENCY, INSURANCEFREQUENCY, KZ, BIGBAF, BIGFIX, MINDBH, MAXDBH, CV_DEF, CV2_DEF, TPA_DEF, VPA_DEF, RECONPLOTS, RECONTREES };
		public enum SAMPLEGROUPSTATS_FIELDS { SampleGroupStats_CN, StratumStats_CN, Code, SgSet, Description, CutLeave, UOM, PrimaryProduct, SecondaryProduct, DefaultLiveDead, SgError, SampleSize1, SampleSize2, CV1, CV2, TreesPerAcre, VolumePerAcre, TreesPerPlot, AverageHeight, SamplingFrequency, InsuranceFrequency, KZ, BigBAF, BigFIX, MinDbh, MaxDbh, CV_Def, CV2_Def, TPA_Def, VPA_Def, ReconPlots, ReconTrees };
	}

	public static class SAMPLEGROUPSTATSTREEDEFAULTVALUE
	{
		public const string _NAME = "SampleGroupStatsTreeDefaultValue";
		public const string TREEDEFAULTVALUE_CN = "TreeDefaultValue_CN";
		public const string SAMPLEGROUPSTATS_CN = "SampleGroupStats_CN";
		public static string[] _ALL = { TREEDEFAULTVALUE_CN, SAMPLEGROUPSTATS_CN };
		public enum SAMPLEGROUPSTATSTREEDEFAULTVALUE_FIELDS { TreeDefaultValue_CN, SampleGroupStats_CN };
	}

	public static class STRATUMSTATS
	{
		public const string _NAME = "StratumStats";
		public const string STRATUMSTATS_CN = "StratumStats_CN";
		public const string STRATUM_CN = "Stratum_CN";
		public const string CODE = "Code";
		public const string DESCRIPTION = "Description";
		public const string METHOD = "Method";
		public const string SGSET = "SgSet";
		public const string SGSETDESCRIPTION = "SgSetDescription";
		public const string BASALAREAFACTOR = "BasalAreaFactor";
		public const string FIXEDPLOTSIZE = "FixedPlotSize";
		public const string STRERROR = "StrError";
		public const string SAMPLESIZE1 = "SampleSize1";
		public const string SAMPLESIZE2 = "SampleSize2";
		public const string WEIGHTEDCV1 = "WeightedCV1";
		public const string WEIGHTEDCV2 = "WeightedCV2";
		public const string TREESPERACRE = "TreesPerAcre";
		public const string VOLUMEPERACRE = "VolumePerAcre";
		public const string TOTALVOLUME = "TotalVolume";
		public const string TOTALACRES = "TotalAcres";
		public const string PLOTSPACING = "PlotSpacing";
		public const string USED = "Used";
		public static string[] _ALL = { STRATUMSTATS_CN, STRATUM_CN, CODE, DESCRIPTION, METHOD, SGSET, SGSETDESCRIPTION, BASALAREAFACTOR, FIXEDPLOTSIZE, STRERROR, SAMPLESIZE1, SAMPLESIZE2, WEIGHTEDCV1, WEIGHTEDCV2, TREESPERACRE, VOLUMEPERACRE, TOTALVOLUME, TOTALACRES, PLOTSPACING, USED };
		public enum STRATUMSTATS_FIELDS { StratumStats_CN, Stratum_CN, Code, Description, Method, SgSet, SgSetDescription, BasalAreaFactor, FixedPlotSize, StrError, SampleSize1, SampleSize2, WeightedCV1, WeightedCV2, TreesPerAcre, VolumePerAcre, TotalVolume, TotalAcres, PlotSpacing, Used };
	}

	public static class REGRESSION
	{
		public const string _NAME = "Regression";
		public const string REGRESSION_CN = "Regression_CN";
		public const string RVOLUME = "rVolume";
		public const string RVOLTYPE = "rVolType";
		public const string RSPEICES = "rSpeices";
		public const string RPRODUCT = "rProduct";
		public const string RLIVEDEAD = "rLiveDead";
		public const string COEFFICIENTA = "CoefficientA";
		public const string COEFFICIENTB = "CoefficientB";
		public const string COEFFICIENTC = "CoefficientC";
		public const string TOTALTREES = "TotalTrees";
		public const string MEANSE = "MeanSE";
		public const string RSQUARED = "Rsquared";
		public const string REGRESSMODEL = "RegressModel";
		public const string RMINDBH = "rMinDbh";
		public const string RMAXDBH = "rMaxDbh";
		public static string[] _ALL = { REGRESSION_CN, RVOLUME, RVOLTYPE, RSPEICES, RPRODUCT, RLIVEDEAD, COEFFICIENTA, COEFFICIENTB, COEFFICIENTC, TOTALTREES, MEANSE, RSQUARED, REGRESSMODEL, RMINDBH, RMAXDBH };
		public enum REGRESSION_FIELDS { Regression_CN, rVolume, rVolType, rSpeices, rProduct, rLiveDead, CoefficientA, CoefficientB, CoefficientC, TotalTrees, MeanSE, Rsquared, RegressModel, rMinDbh, rMaxDbh };
	}

	public static class LOGMATRIX
	{
		public const string _NAME = "LogMatrix";
		public const string REPORTNUMBER = "ReportNumber";
		public const string GRADEDESCRIPTION = "GradeDescription";
		public const string LOGSORTDESCRIPTION = "LogSortDescription";
		public const string SPECIES = "Species";
		public const string LOGGRADE1 = "LogGrade1";
		public const string LOGGRADE2 = "LogGrade2";
		public const string LOGGRADE3 = "LogGrade3";
		public const string LOGGRADE4 = "LogGrade4";
		public const string LOGGRADE5 = "LogGrade5";
		public const string LOGGRADE6 = "LogGrade6";
		public const string SEDLIMIT = "SEDlimit";
		public const string SEDMINIMUM = "SEDminimum";
		public const string SEDMAXIMUM = "SEDmaximum";
		public static string[] _ALL = { REPORTNUMBER, GRADEDESCRIPTION, LOGSORTDESCRIPTION, SPECIES, LOGGRADE1, LOGGRADE2, LOGGRADE3, LOGGRADE4, LOGGRADE5, LOGGRADE6, SEDLIMIT, SEDMINIMUM, SEDMAXIMUM };
		public enum LOGMATRIX_FIELDS { ReportNumber, GradeDescription, LogSortDescription, Species, LogGrade1, LogGrade2, LogGrade3, LogGrade4, LogGrade5, LogGrade6, SEDlimit, SEDminimum, SEDmaximum };
	}

	#endregion
	#region Settings Tables
	public static class TREEDEFAULTVALUETREEAUDITVALUE
	{
		public const string _NAME = "TreeDefaultValueTreeAuditValue";
		public const string TREEAUDITVALUE_CN = "TreeAuditValue_CN";
		public const string TREEDEFAULTVALUE_CN = "TreeDefaultValue_CN";
		public static string[] _ALL = { TREEAUDITVALUE_CN, TREEDEFAULTVALUE_CN };
		public enum TREEDEFAULTVALUETREEAUDITVALUE_FIELDS { TreeAuditValue_CN, TreeDefaultValue_CN };
	}

	public static class TREEAUDITVALUE
	{
		public const string _NAME = "TreeAuditValue";
		public const string TREEAUDITVALUE_CN = "TreeAuditValue_CN";
		public const string FIELD = "Field";
		public const string MIN = "Min";
		public const string MAX = "Max";
		public const string VALUESET = "ValueSet";
		public const string REQUIRED = "Required";
		public const string ERRORMESSAGE = "ErrorMessage";
		public static string[] _ALL = { TREEAUDITVALUE_CN, FIELD, MIN, MAX, VALUESET, REQUIRED, ERRORMESSAGE };
		public enum TREEAUDITVALUE_FIELDS { TreeAuditValue_CN, Field, Min, Max, ValueSet, Required, ErrorMessage };
	}

	public static class LOGAUDITRULE
	{
		public const string _NAME = "LogAuditRule";
		public const string SPECIES = "Species";
		public const string DEFECTMAX = "DefectMax";
		public const string FIELDNAME = "FieldName";
		public const string MIN = "Min";
		public const string MAX = "Max";
		public const string VALUES = "Values";
		public static string[] _ALL = { SPECIES, DEFECTMAX, FIELDNAME, MIN, MAX, VALUES };
		public enum LOGAUDITRULE_FIELDS { Species, DefectMax, FieldName, Min, Max, Values };
	}

	public static class LOGFIELDSETUP
	{
		public const string _NAME = "LogFieldSetup";
		public const string STRATUM_CN = "Stratum_CN";
		public const string FIELD = "Field";
		public const string FIELDORDER = "FieldOrder";
		public const string COLUMNTYPE = "ColumnType";
		public const string HEADING = "Heading";
		public const string WIDTH = "Width";
		public const string FORMAT = "Format";
		public const string BEHAVIOR = "Behavior";
		public static string[] _ALL = { STRATUM_CN, FIELD, FIELDORDER, COLUMNTYPE, HEADING, WIDTH, FORMAT, BEHAVIOR };
		public enum LOGFIELDSETUP_FIELDS { Stratum_CN, Field, FieldOrder, ColumnType, Heading, Width, Format, Behavior };
	}

	public static class TREEFIELDSETUP
	{
		public const string _NAME = "TreeFieldSetup";
		public const string STRATUM_CN = "Stratum_CN";
		public const string FIELD = "Field";
		public const string FIELDORDER = "FieldOrder";
		public const string COLUMNTYPE = "ColumnType";
		public const string HEADING = "Heading";
		public const string WIDTH = "Width";
		public const string FORMAT = "Format";
		public const string BEHAVIOR = "Behavior";
		public static string[] _ALL = { STRATUM_CN, FIELD, FIELDORDER, COLUMNTYPE, HEADING, WIDTH, FORMAT, BEHAVIOR };
		public enum TREEFIELDSETUP_FIELDS { Stratum_CN, Field, FieldOrder, ColumnType, Heading, Width, Format, Behavior };
	}

	public static class LOGFIELDSETUPDEFAULT
	{
		public const string _NAME = "LogFieldSetupDefault";
		public const string LOGFIELDSETUPDEFAULT_CN = "LogFieldSetupDefault_CN";
		public const string FIELD = "Field";
		public const string FIELDNAME = "FieldName";
		public const string FIELDORDER = "FieldOrder";
		public const string COLUMNTYPE = "ColumnType";
		public const string HEADING = "Heading";
		public const string WIDTH = "Width";
		public const string FORMAT = "Format";
		public const string BEHAVIOR = "Behavior";
		public static string[] _ALL = { LOGFIELDSETUPDEFAULT_CN, FIELD, FIELDNAME, FIELDORDER, COLUMNTYPE, HEADING, WIDTH, FORMAT, BEHAVIOR };
		public enum LOGFIELDSETUPDEFAULT_FIELDS { LogFieldSetupDefault_CN, Field, FieldName, FieldOrder, ColumnType, Heading, Width, Format, Behavior };
	}

	public static class TREEFIELDSETUPDEFAULT
	{
		public const string _NAME = "TreeFieldSetupDefault";
		public const string TREEFIELDSETUPDEFAULT_CN = "TreeFieldSetupDefault_CN";
		public const string METHOD = "Method";
		public const string FIELD = "Field";
		public const string FIELDNAME = "FieldName";
		public const string FIELDORDER = "FieldOrder";
		public const string COLUMNTYPE = "ColumnType";
		public const string HEADING = "Heading";
		public const string WIDTH = "Width";
		public const string FORMAT = "Format";
		public const string BEHAVIOR = "Behavior";
		public static string[] _ALL = { TREEFIELDSETUPDEFAULT_CN, METHOD, FIELD, FIELDNAME, FIELDORDER, COLUMNTYPE, HEADING, WIDTH, FORMAT, BEHAVIOR };
		public enum TREEFIELDSETUPDEFAULT_FIELDS { TreeFieldSetupDefault_CN, Method, Field, FieldName, FieldOrder, ColumnType, Heading, Width, Format, Behavior };
	}

	#endregion
	#region Lookup Tables
	public static class CRUISEMETHODS
	{
		public const string _NAME = "CruiseMethods";
		public const string CRUISEMETHODS_CN = "CruiseMethods_CN";
		public const string CODE = "Code";
		public const string FRIENDLYVALUE = "FriendlyValue";
		public static string[] _ALL = { CRUISEMETHODS_CN, CODE, FRIENDLYVALUE };
		public enum CRUISEMETHODS_FIELDS { CruiseMethods_CN, Code, FriendlyValue };
	}

	public static class LOGGINGMETHODS
	{
		public const string _NAME = "LoggingMethods";
		public const string LOGGINGMETHODS_CN = "LoggingMethods_CN";
		public const string CODE = "Code";
		public const string FRIENDLYVALUE = "FriendlyValue";
		public static string[] _ALL = { LOGGINGMETHODS_CN, CODE, FRIENDLYVALUE };
		public enum LOGGINGMETHODS_FIELDS { LoggingMethods_CN, Code, FriendlyValue };
	}

	public static class PRODUCTCODES
	{
		public const string _NAME = "ProductCodes";
		public const string PRODUCTCODES_CN = "ProductCodes_CN";
		public const string CODE = "Code";
		public const string FRIENDLYVALUE = "FriendlyValue";
		public static string[] _ALL = { PRODUCTCODES_CN, CODE, FRIENDLYVALUE };
		public enum PRODUCTCODES_FIELDS { ProductCodes_CN, Code, FriendlyValue };
	}

	public static class UOMCODES
	{
		public const string _NAME = "UOMCodes";
		public const string UOMCODES_CN = "UOMCodes_CN";
		public const string CODE = "Code";
		public const string FRIENDLYVALUE = "FriendlyValue";
		public static string[] _ALL = { UOMCODES_CN, CODE, FRIENDLYVALUE };
		public enum UOMCODES_FIELDS { UOMCodes_CN, Code, FriendlyValue };
	}

	public static class REGIONS
	{
		public const string _NAME = "Regions";
		public const string REGION_CN = "Region_CN";
		public const string NUMBER = "Number";
		public const string NAME = "Name";
		public static string[] _ALL = { REGION_CN, NUMBER, NAME };
		public enum REGIONS_FIELDS { Region_CN, Number, Name };
	}

	public static class FORESTS
	{
		public const string _NAME = "Forests";
		public const string FOREST_CN = "Forest_CN";
		public const string REGION_CN = "Region_CN";
		public const string STATE = "State";
		public const string NAME = "Name";
		public const string NUMBER = "Number";
		public static string[] _ALL = { FOREST_CN, REGION_CN, STATE, NAME, NUMBER };
		public enum FORESTS_FIELDS { Forest_CN, Region_CN, State, Name, Number };
	}

	#endregion
	#region Utility Tables
	public static class ERRORLOG
	{
		public const string _NAME = "ErrorLog";
		public const string TABLENAME = "TableName";
		public const string CN_NUMBER = "CN_Number";
		public const string COLUMNNAME = "ColumnName";
		public const string LEVEL = "Level";
		public const string MESSAGE = "Message";
		public const string PROGRAM = "Program";
		public const string SUPPRESS = "Suppress";
		public static string[] _ALL = { TABLENAME, CN_NUMBER, COLUMNNAME, LEVEL, MESSAGE, PROGRAM, SUPPRESS };
		public enum ERRORLOG_FIELDS { TableName, CN_Number, ColumnName, Level, Message, Program, Suppress };
	}

	public static class MESSAGELOG
	{
		public const string _NAME = "MessageLog";
		public const string MESSAGE_CN = "Message_CN";
		public const string PROGRAM = "Program";
		public const string MESSAGE = "Message";
		public const string DATE = "Date";
		public const string TIME = "Time";
		public const string LEVEL = "Level";
		public static string[] _ALL = { MESSAGE_CN, PROGRAM, MESSAGE, DATE, TIME, LEVEL };
		public enum MESSAGELOG_FIELDS { Message_CN, Program, Message, Date, Time, Level };
	}

	public static class GLOBALS
	{
		public const string _NAME = "Globals";
		public const string BLOCK = "Block";
		public const string KEY = "Key";
		public const string VALUE = "Value";
		public static string[] _ALL = { BLOCK, KEY, VALUE };
		public enum GLOBALS_FIELDS { Block, Key, Value };
	}

	public static class COMPONENT
	{
		public const string _NAME = "Component";
		public const string COMPONENT_CN = "Component_CN";
		public const string GUID = "GUID";
		public const string LASTMERGE = "LastMerge";
		public const string FILENAME = "FileName";
		public static string[] _ALL = { COMPONENT_CN, GUID, LASTMERGE, FILENAME };
		public enum COMPONENT_FIELDS { Component_CN, GUID, LastMerge, FileName };
	}

	#endregion

	public static class Schema
	{
		public static String[] TABLE_NAMES = { "Sale","CuttingUnit","Stratum","CuttingUnitStratum","SampleGroup","TreeDefaultValue","SampleGroupTreeDefaultValue","Plot","Tree","Log","Stem","CountTree","Tally","TreeEstimate","FixCNTTallyClass","FixCNTTallyPopulation","VolumeEquation","BiomassEquation","ValueEquation","QualityAdjEquation","Reports","TreeCalculatedValues","LCD","POP","PRO","LogStock","SampleGroupStats","SampleGroupStatsTreeDefaultValue","StratumStats","Regression","LogMatrix","TreeDefaultValueTreeAuditValue","TreeAuditValue","LogAuditRule","LogFieldSetup","TreeFieldSetup","LogFieldSetupDefault","TreeFieldSetupDefault","CruiseMethods","LoggingMethods","ProductCodes","UOMCodes","Regions","Forests","ErrorLog","MessageLog","Globals","Component" };
	}
}

