namespace CruiseDAL.Schema
{
    public static partial class Schema
    {
        public static readonly string CREATE_TRIGGERS =
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
AFTER UPDATE OF CuttingUnit_CN, Code, Area, Description, LoggingMethod, PaymentUnit, TallyHistory, Rx
ON CuttingUnit
BEGIN
	UPDATE CuttingUnit
	SET ModifiedDate = datetime(current_timestamp, 'localtime')
	WHERE rowID = new.rowID;
END;

CREATE TRIGGER OnUpdateCuttingUnitRowVersion AFTER UPDATE
OF CuttingUnit_CN, Code, Area, Description, LoggingMethod, PaymentUnit, TallyHistory, Rx
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
AFTER UPDATE OF Stratum_CN, Code, Description, Method, BasalAreaFactor, FixedPlotSize, KZ3PPNT, SamplingFrequency, Hotkey, FBSCode, YieldComponent, VolumeFactor, Month, Year
ON Stratum
BEGIN
	UPDATE Stratum
	SET ModifiedDate = datetime(current_timestamp, 'localtime')
	WHERE rowID = new.rowID;
END;

CREATE TRIGGER OnUpdateStratumRowVersion AFTER UPDATE
OF Stratum_CN, Code, Description, Method, BasalAreaFactor, FixedPlotSize, KZ3PPNT, SamplingFrequency, Hotkey, FBSCode, YieldComponent, VolumeFactor, Month, Year
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

--SamplerState--
CREATE TRIGGER SamplerState_OnUpdate 
    AFTER UPDATE OF 
        BlockState, 
        Counter, 
        InsuranceCounter 
    ON SamplerState 
    FOR EACH ROW 
    BEGIN 
        UPDATE SamplerState SET ModifiedDate = datetime('now', 'localtime') WHERE SamplerState_CN = old.SamplerState_CN;
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
AFTER UPDATE OF Plot_CN, Plot_GUID, Stratum_CN, CuttingUnit_CN, PlotNumber, IsEmpty, Slope, KPI, Aspect, Remarks, XCoordinate, YCoordinate, ZCoordinate, MetaData, Blob, ThreePRandomValue
ON Plot
BEGIN
	UPDATE Plot
	SET ModifiedDate = datetime(current_timestamp, 'localtime')
	WHERE rowID = new.rowID;
END;

CREATE TRIGGER OnUpdatePlotRowVersion AFTER UPDATE
OF Plot_CN, Plot_GUID, Stratum_CN, CuttingUnit_CN, PlotNumber, IsEmpty, Slope, KPI, Aspect, Remarks, XCoordinate, YCoordinate, ZCoordinate, MetaData, Blob, ThreePRandomValue
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
--LogGradeAuditRule--
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
    }//END CLASS
}//END NAMESPACE