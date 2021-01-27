
namespace CruiseDAL.DownMigrators
{
    public class TreeDefaultValueDownMigrate : IDownMigrator
    {
        public string CreateCommand(string toDbName, string fromDbName, string cruiseID, string createdBy)
        {
            return
$@"
WITH 
tdvExpandSpecies AS (
    SELECT 
        tdv.CruiseID,
        tdv.TreeDefaultValue_CN,
        tdv.PrimaryProduct,
        sp.SpeciesCode,
        sp.ContractSpecies,
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
        sp.ContractSpecies,
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
        sp.ContractSpecies,
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
        sp.ContractSpecies,
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
    SELECT * FROM tdvExpandSpecies
    UNION ALL
    SELECT * FROM tdvExpandProduct
    UNION ALL 
    SELECT * FROM tdvExpandProdAndSp
    UNION ALL 
    SELECT * FROM tdvProdAndSpDefined 
),

tdvLiveDeadExpanded AS (
    SELECT 
        CruiseID,
        TreeDefaultValue_CN,
        'L' AS LiveDead,
        PrimaryProduct,
        SpeciesCode,
        ContractSpecies,
        FIACode,
        CullPrimary,
        HiddenPrimary,
        CullSecondary,
        HiddenSecondary,
        TreeGrade,
        Recoverable,
        MerchHeightLogLength,
        MerchHeightType,
        FormClass,
        BarkThicknessRatio,
        AverageZ,
        ReferenceHeightPercent
    FROM tdvExpanded
    UNION ALL
    SELECT 
        CruiseID,
        'D' AS LiveDead,
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
        ReferenceHeightPercent
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
	Chargeable,
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
	CullPrimary,
	HiddenPrimary,
	CullSecondary,
	HiddenSecondary,
	Recoverable,
	'False',
	ContractSpecies,
	TreeGrade,
	MerchHeightLogLength,
	MerchHeightType,
	FormClass,
	BarkThicknessRatio,
	AverageZ,
	ReferenceHeightPercent,
	'{createdBy}' AS CreatedBy
FROM tdvLiveDeadExpanded
WHERE CruiseID = '{cruiseID}';
";
        }
    }
}

