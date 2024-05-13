
namespace CruiseDAL.DownMigrators
{
    public class TreeDefaultValueDownMigrate : IDownMigrator
    {
        public string CreateCommand(string toDbName, string fromDbName, string cruiseID, string createdBy)
        {
            return
$@"
-- setup: before converting TDVs we need to create a unique index on the TDV table 
-- that properly implements the unique Species, Product, LiveDead unique constraint
CREATE UNIQUE INDEX IF NOT EXISTS {toDbName}.UIX_TreeDefaultValue_Species_PrimaryProduct_LiveDead 
ON TreeDefaultValue (Species, PrimaryProduct, LiveDead);




WITH 

tdvExpandSpecies AS (
    SELECT 
        tdv.CruiseID,
        tdv.TreeDefaultValue_CN,
        tdv.PrimaryProduct,
        sp.SpeciesCode,
        (SELECT ContractSpecies FROM {fromDbName}.Species_Product 
            WHERE CruiseID = tdv.CruiseID 
                AND SpeciesCode = sp.SpeciesCode 
                AND (PrimaryProduct IS NULL OR PrimaryProduct = tdv.PrimaryProduct)
                ORDER BY PrimaryProduct DESC -- nulls and empty strings last
                LIMIT 1 ) AS ContractSpecies,

        sp.FIACode,
        tdv.CullPrimary,
        tdv.CullPrimaryDead,
        tdv.HiddenPrimary,
        tdv.HiddenPrimaryDead,

        tdv.TreeGrade,
        tdv.TreeGradeDead,
        tdv.CullSecondary,
        tdv.HiddenSecondary,
        tdv.Recoverable,

        tdv.MerchHeightLogLength,
        tdv.MerchHeightType,
        tdv.FormClass,
        tdv.BarkThicknessRatio,
        tdv.AverageZ,

        tdv.ReferenceHeightPercent
    FROM {fromDbName}.TreeDefaultValue AS tdv
    JOIN {fromDbName}.Species AS sp USING (CruiseID)
    WHERE tdv.SpeciesCode IS NULL AND tdv.PrimaryProduct NOTNULL
),

tdvExpandProduct AS (
    SELECT 
        tdv.CruiseID,
        tdv.TreeDefaultValue_CN,
        prod.Product AS PrimaryProduct,
        tdv.SpeciesCode,
        (SELECT ContractSpecies FROM {fromDbName}.Species_Product 
            WHERE CruiseID = tdv.CruiseID 
                AND SpeciesCode = tdv.SpeciesCode 
                AND (PrimaryProduct IS NULL OR PrimaryProduct = prod.Product)
                ORDER BY PrimaryProduct DESC -- nulls and empty strings last
                LIMIT 1 ) AS ContractSpecies,

        sp.FIACode,
        tdv.CullPrimary,
        tdv.CullPrimaryDead,
        tdv.HiddenPrimary,
        tdv.HiddenPrimaryDead,

        tdv.TreeGrade,
        tdv.TreeGradeDead,
        tdv.CullSecondary,
        tdv.HiddenSecondary,
        tdv.Recoverable,

        tdv.MerchHeightLogLength,
        tdv.MerchHeightType,
        tdv.FormClass,
        tdv.BarkThicknessRatio,
        tdv.AverageZ,

        tdv.ReferenceHeightPercent
    FROM {fromDbName}.TreeDefaultValue AS tdv
    JOIN {fromDbName}.Species AS sp USING (CruiseID, SpeciesCode)
    JOIN {fromDbName}.LK_Product AS prod
    WHERE tdv.SpeciesCode NOTNULL AND tdv.PrimaryProduct IS NULL
),

tdvExpandProdAndSp AS (
    SELECT 
        tdv.CruiseID,
        tdv.TreeDefaultValue_CN,
        prod.Product AS PrimaryProduct,
        sp.SpeciesCode,
        (SELECT ContractSpecies FROM {fromDbName}.Species_Product 
            WHERE CruiseID = tdv.CruiseID 
                AND SpeciesCode = sp.SpeciesCode 
                AND (PrimaryProduct IS NULL OR PrimaryProduct = prod.Product)
                ORDER BY PrimaryProduct DESC -- nulls and empty strings last
                LIMIT 1 ) AS ContractSpecies,

        sp.FIACode,
        tdv.CullPrimary,
        tdv.CullPrimaryDead,
        tdv.HiddenPrimary,
        tdv.HiddenPrimaryDead,

        tdv.TreeGrade,
        tdv.TreeGradeDead,
        tdv.CullSecondary,
        tdv.HiddenSecondary,
        tdv.Recoverable,

        tdv.MerchHeightLogLength,
        tdv.MerchHeightType,
        tdv.FormClass,
        tdv.BarkThicknessRatio,
        tdv.AverageZ,

        tdv.ReferenceHeightPercent
    FROM {fromDbName}.TreeDefaultValue AS tdv
    JOIN {fromDbName}.LK_Product AS prod
    JOIN {fromDbName}.Species AS sp USING (CruiseID)
    WHERE tdv.SpeciesCode IS NULL AND tdv.PrimaryProduct IS NULL
),

tdvProdAndSpDefined AS (
    SELECT 
        tdv.CruiseID,
        tdv.TreeDefaultValue_CN,
        tdv.PrimaryProduct,
        tdv.SpeciesCode,
        (SELECT ContractSpecies FROM {fromDbName}.Species_Product 
            WHERE CruiseID = tdv.CruiseID 
                AND SpeciesCode = tdv.SpeciesCode 
                AND (PrimaryProduct IS NULL OR PrimaryProduct = tdv.PrimaryProduct)
                ORDER BY PrimaryProduct DESC -- nulls and empty strings last
                LIMIT 1 ) AS ContractSpecies,

        sp.FIACode,
        tdv.CullPrimary,
        tdv.CullPrimaryDead,
        tdv.HiddenPrimary,
        tdv.HiddenPrimaryDead,

        tdv.TreeGrade,
        tdv.TreeGradeDead,
        tdv.CullSecondary,
        tdv.HiddenSecondary,
        tdv.Recoverable,

        tdv.MerchHeightLogLength,
        tdv.MerchHeightType,
        tdv.FormClass,
        tdv.BarkThicknessRatio,
        tdv.AverageZ,

        tdv.ReferenceHeightPercent
    FROM {fromDbName}.TreeDefaultValue AS tdv
    JOIN {fromDbName}.Species AS sp USING (CruiseID, SpeciesCode)
    WHERE tdv.SpeciesCode NOTNULL AND tdv.PrimaryProduct NOTNULL
),

tdvExpanded AS (
    SELECT * FROM tdvProdAndSpDefined
    UNION ALL 
    SELECT * FROM tdvExpandProduct
    UNION ALL 
    SELECT * FROM tdvExpandSpecies
    UNION ALL
    SELECT * FROM tdvExpandProdAndSp
     
),

tdvLiveDeadExpanded AS (
    SELECT 
        CruiseID,
        TreeDefaultValue_CN,
        PrimaryProduct,
        SpeciesCode,
        ContractSpecies,

        FIACode,
        CullPrimary,
        HiddenPrimary,
        TreeGrade,
        CullSecondary,

        HiddenSecondary,
        Recoverable,
        MerchHeightLogLength,
        MerchHeightType,
        FormClass,

        BarkThicknessRatio,
        AverageZ,
        ReferenceHeightPercent,
        'L' AS LiveDead
    FROM tdvExpanded
    UNION ALL
    SELECT 
        CruiseID,
        TreeDefaultValue_CN,
        PrimaryProduct,
        SpeciesCode,
        ContractSpecies,

        FIACode,
        CullPrimaryDead AS CullPrimary,
        HiddenPrimaryDead AS HiddenPrimary,
        TreeGradeDead AS TreeGrade,
        CullSecondary,

        HiddenSecondary,
        Recoverable,
        MerchHeightLogLength,
        MerchHeightType,
        FormClass,

        BarkThicknessRatio,
        AverageZ,
        ReferenceHeightPercent,
        'D' AS LiveDead
    FROM tdvExpanded
)
        
INSERT INTO {toDbName}.TreeDefaultValue (
	PrimaryProduct,
	Species,
	LiveDead,
	FIAcode,
	CullPrimary,

	HiddenPrimary,
	CullSecondary,
	HiddenSecondary,
	Recoverable,
	ContractSpecies,

	TreeGrade,
	MerchHeightLogLength,
	MerchHeightType,
	FormClass,
	BarkThicknessRatio,

	AverageZ,
	ReferenceHeightPercent,
	CreatedBy
)
SELECT 
    PrimaryProduct,
	SpeciesCode,
	LiveDead,
	FIAcode,
	ifnull(CullPrimary, 0.0) AS CullPrimary,

	ifnull(HiddenPrimary, 0.0) AS HiddenPrimary,
	ifnull(CullSecondary, 0.0) AS CullSecondary,
	ifnull(HiddenSecondary, 0.0) AS HiddenSecondary,
	ifnull(Recoverable, 0.0) AS Recoverable,
	ContractSpecies,

	ifnull(TreeGrade, '0') AS TreeGrade,
	ifnull(MerchHeightLogLength, 0) AS MerchHeightLogLength,
	ifnull(MerchHeightType, 'F') AS MerchHeightType,
	ifnull(FormClass, 0.0) AS FormClass,
	ifnull(BarkThicknessRatio, 0.0) AS BarkThicknessRatio,

	ifnull(AverageZ, 0.0) AS AverageZ,
	ifnull(ReferenceHeightPercent, 0.0) AS ReferenceHeightPercent,
	'{createdBy}' AS CreatedBy
FROM tdvLiveDeadExpanded
WHERE CruiseID = '{cruiseID}'
ON CONFLICT (Species, PrimaryProduct, LiveDead)
DO NOTHING;
";
        }
    }
}

