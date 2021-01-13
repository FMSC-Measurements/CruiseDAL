namespace CruiseDAL.Schema.V2Backports
{
    public class TreeDefaultValue_V2_ViewDefinition : IViewDefinition
    {
        public string ViewName => "TreeDefaultValue_V2";

        public string CreateView =>
@"CREATE VIEW TreeDefaultValue_V2 AS 

WITH 
tdvExpandSpecies AS (
    SELECT tdv.TreeDefaultValue_CN,
        tdv.PrimaryProduct,
        sp.SpeciesCode,
        tdv.FIACode,
        tdv.CullPrimary,
        tdv.CullPrimaryDead,
        tdv.HiddenPrimary,
        tdv.HiddenPrimaryDead,
        tdv.CullSecondary,
        tdv.HiddenSecondary,
        tdv.Recoverable,
        tdv.MerchHeightLogLength,
        tdv.MerchHeightType,
        tdv.FormClass,
        tdv.BarkThicknessRatio,
        tdv.AverageZ,
        tdv.ReferenceHeightPercent
    FROM TreeDefaultValue AS tdv
    JOIN Species AS sp USING (CruiseID)
    WHERE tdv.SpeciesCode IS NULL AND tdv.PrimaryProduct NOTNULL
),

tdvExpandProduct AS (
    SELECT tdv.TreeDefaultValue_CN,
        prod.Product AS PrimaryProduct,
        tdv.SpeciesCode,
        tdv.FIACode,
        tdv.CullPrimary,
        tdv.CullPrimaryDead,
        tdv.HiddenPrimary,
        tdv.HiddenPrimaryDead,
        tdv.CullSecondary,
        tdv.HiddenSecondary,
        tdv.Recoverable,
        tdv.MerchHeightLogLength,
        tdv.MerchHeightType,
        tdv.FormClass,
        tdv.BarkThicknessRatio,
        tdv.AverageZ,
        tdv.ReferenceHeightPercent
    FROM TreeDefaultValue AS tdv
    JOIN Product AS prod
    WHERE tdv.SpeciesCode NOTNULL AND tdv.PrimaryProduct IS NULL
),

tdvExpandProdAndSp AS (
    SELECT tdv.TreeDefaultValue_CN,
        prod.Product AS PrimaryProduct,
        sp.SpeciesCode,
        tdv.FIACode,
        tdv.CullPrimary,
        tdv.CullPrimaryDead,
        tdv.HiddenPrimary,
        tdv.HiddenPrimaryDead,
        tdv.CullSecondary,
        tdv.HiddenSecondary,
        tdv.Recoverable,
        tdv.MerchHeightLogLength,
        tdv.MerchHeightType,
        tdv.FormClass,
        tdv.BarkThicknessRatio,
        tdv.AverageZ,
        tdv.ReferenceHeightPercent
    FROM TreeDefaultValue AS tdv
    JOIN Product AS prod
    JOIN Species AS sp USING (CruiseID)
    WHERE tdv.SpeciesCode IS NULL AND tdv.PrimaryProduct IS NULL
),

tdvProdAndSpDefined AS (
    SELECT tdv.TreeDefaultValue_CN,
        tdv.PrimaryProduct,
        tdv.SpeciesCode,
        tdv.FIACode,
        tdv.CullPrimary,
        tdv.CullPrimaryDead,
        tdv.HiddenPrimary,
        tdv.HiddenPrimaryDead,
        tdv.CullSecondary,
        tdv.HiddenSecondary,
        tdv.Recoverable,
        tdv.MerchHeightLogLength,
        tdv.MerchHeightType,
        tdv.FormClass,
        tdv.BarkThicknessRatio,
        tdv.AverageZ,
        tdv.ReferenceHeightPercent
    FROM TreeDefaultValue AS tdv
    WHERE tdv.SpeciesCode NOTNULL AND tdv.PrimaryProduct NOTNULL
)

SELECT * FROM tdvExpandSpecies
UNION ALL
SELECT * FROM tdvExpandProduct
UNION ALL 
SELECT * FROM tdvExpandProdAndSp
UNION ALL 
SELECT * FROM tdvProdAndSpDefined 
;";
    }
}