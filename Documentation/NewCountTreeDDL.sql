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
				TreeGrade TEXT Default "0",
				MerchHeightLogLength INTEGER Default 0,
				MerchHeightType TEXT Default "F",
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

CREATE TABLE Component (
				Component_CN INTEGER PRIMARY KEY AUTOINCREMENT,
				GUID TEXT,
				LastMerge DATETIME,
				FileName TEXT);

CREATE TABLE TallyPopulation (
TallyPopulation_CN INTEGER PRIMARY KEY AUTOINCREMENT,
SampleGroup_CN INTEGER REFERENCES SampleGroup NOT NULL,
TreeDefaultValue_CN INTEGER REFERENCES TreeDefaultValue,
HotKey TEXT,
Description TEXT );

CREATE TABLE TallyLedger (
TallyLedger_GUID TEXT Default (hex(randomblob(16))), 
TallyPopulation_CN INTEGER REFERENCES TallyPopulation NOT NULL,
CuttingUnit_CN INTEGER REFERENCES CuttingUnit NOT NULL,
Component_CN INTEGER REFERENCES Component, 
TreeCount Integer Default 0,
SumKPI Integer Default 0);

/*CREATE VIEW CountTree as
SELECT 
CAST ((TallyPopulation_CN || ifnull(TP_NK,0)) AS INTEGER) AS CountTree_CN,
TallyPopulation_CN,
SampleGroup_CN, TreeDefaultValue_CN, HotKey, Description,
s1.CuttingUnit_CN, s1.Component_CN,
s1.TreeCount, 
s1.SumKPI, 
Tally.Tally_CN
FROM TallyPopulation
LEFT JOIN Tally USING (HotKey, Description)
LEFT JOIN 
( 
	SELECT  TallyPopulation_CN, CuttingUnit_CN, Component_CN, 
	(CuttingUnit_CN || ifnull(Component_CN, 0)) AS TL_NK,
	sum(TallyLedger.TreeCount) AS TreeCount, 
	sum(TallyLedger.SumKPI) AS SumKPI  
	FROM TallyLedger 
	GROUP BY TallyLedger.TallyPopulation_CN, TallyLedger.CuttingUnit_CN, ifnull(TallyLedger.Component_CN, 0)
) AS s1
USING (TallyPopulation_CN);*/

CREATE VIEW CountTree as
SELECT 
CAST ((tp.TallyPopulation_CN || s1.TL_NK) AS INTEGER) AS CountTree_CN,
tp.TallyPopulation_CN,
tp.SampleGroup_CN, tp.TreeDefaultValue_CN, HotKey, Description,
s1.CuttingUnit_CN, s1.Component_CN,
s1.TreeCount, 
s1.SumKPI, 
Tally.Tally_CN
FROM 
(
	SELECT  TallyPopulation_CN, CuttingUnit_CN, Component_CN, 
	(CuttingUnit_CN || ifnull(Component_CN, 0)) AS TL_NK,
	sum(TallyLedger.TreeCount) AS TreeCount, 
	sum(TallyLedger.SumKPI) AS SumKPI  
	FROM TallyLedger 
	GROUP BY TallyLedger.TallyPopulation_CN, TallyLedger.CuttingUnit_CN, ifnull(TallyLedger.Component_CN, 0)
) AS s1
JOIN TallyPopulation AS tp USING (TallyPopulation_CN)
LEFT JOIN Tally AS Tally USING (HotKey, Description);




CREATE TRIGGER InsertCountTree 
INSTEAD OF INSERT 
ON CountTree 
BEGIN
	--inset new tally population if one doesn't already exist
	INSERT INTO TallyPopulation (SampleGroup_CN, TreeDefaultValue_CN, HotKey, Description)
	SELECT new.SampleGroup_CN, new.TreeDefaultValue_CN, new.HotKey, new.Description 
	WHERE NOT EXISTS (SELECT 1 FROM TallyPopulation AS tp WHERE tp.SampleGroup_CN = new.SampleGroup_CN AND ifnull(tp.TreeDefaultValue_CN,0) = ifnull(new.TreeDefaultValue_CN,0));
	
	--create tally ledger entry as long as cutting unit is provided
	INSERT INTO TallyLedger (TallyPopulation_CN, CuttingUnit_CN, Component_CN, TreeCount, SumKPI)
	SELECT 
	(SELECT TallyPopulation_CN FROM TallyPopulation AS tp WHERE tp.SampleGroup_CN = new.SampleGroup_CN AND ifnull(tp.TreeDefaultValue_CN,0) = ifnull(new.TreeDefaultValue_CN,0)), --select tally population we just created or existing
	new.CuttingUnit_CN, new.Component_CN, ifnull(new.TreeCount,0), ifnull(new.SumKPI,0)
	WHERE new.CuttingUnit_CN IS NOT NULL;
END;

--trigger handles setting of tally_CN, by coping the hot key and discription of the target tally entry to our tally population
CREATE TRIGGER UpdateCountTree_Tally_CN
INSTEAD OF UPDATE OF Tally_CN 
ON CountTree
BEGIN
	UPDATE TallyPopulation 
	Set HotKey = (SELECT HotKey FROM Tally Where Tally_CN = new.Tally_CN),
	Description = (SELECT Description FROM Tally Where Tally_CN = new.Tally_CN)
	WHERE TallyPopulation_CN = new.TallyPopulation_CN;
END;

CREATE TRIGGER UpdateCountTree_TallyLedger
INSTEAD OF UPDATE OF TreeCount, SumKPI 
ON CountTree 
WHEN (new.TreeCount - old.TreeCount) != 0 OR (new.SumKPI - old.SumKPI) != 0
BEGIN
	INSERT INTO TallyLedger (TallyPopulation_CN, CuttingUnit_CN, Component_CN, TreeCount, SumKPI)
	VALUES
	((SELECT TallyPopulation_CN
	FROM TallyPopulation as tp WHERE tp.SampleGroup_CN = new.SampleGroup_CN AND ifnull(tp.TreeDefaultValue_CN, 0) = ifnull(new.TreeDefaultValue_CN, 0)),
	new.CuttingUnit_CN, new.Component_CN, (new.TreeCount - old.TreeCount), (new.SumKPI - old.SumKPI));
END;



CREATE TRIGGER IgnoreConflictsOnCountTree
BEFORE INSERT 
ON CountTree 
WHEN Exists 
(
	SELECT 1 FROM CountTree WHERE CountTree.CuttingUnit_CN = new.CuttingUnit_CN 
	AND CountTree.SampleGroup_CN = new.SampleGroup_CN 
	AND ifnull(CountTree.TreeDefaultValue_CN,0) = ifnull(new.TreeDefaultValue_CN,0) 
	AND ifnull(CountTree.Component_CN,0) = ifnull(new.Component_CN,0)
)
BEGIN
SELECT RAISE (IGNORE);
END;




	
	