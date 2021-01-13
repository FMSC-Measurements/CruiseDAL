using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CruiseDAL.Migrators
{
    public class TreeDefaultValueMigrator : IMigrator
    {
        public string MigrateToV3(string toDbName, string fromDbName, string cruiseID, string saleID)
        {
            return
$@"INSERT INTO {toDbName}.TreeDefaultValue (
        TreeDefaultValue_CN,
        CruiseID,
        PrimaryProduct,
        SpeciesCode,
        CullPrimary,
        CullPrimaryDead,
        HiddenPrimary,
        HiddenPrimaryDead,
        TreeGrade,
        TreeGradeDead,
        CullSecondary,
        HiddenSecondary,
        Recoverable,
        MerchHeightLogLength,
        MerchHeightType,
        FormClass,
        BarkThicknessRatio,        
        AverageZ, 
        ReferenceHeightPercent,
        CreatedBy,
        Created_TS,
        ModifiedBy,
        Modified_TS
    )
    SELECT
        tdvl.TreeDefaultValue_CN,
        '{cruiseID}',
        tdvl.PrimaryProduct,
        tdvl.Species,
        tdvl.CullPrimary,
        tdvd.CullPrimary,
        tdvl.HiddenPrimary,
        tdvd.HiddenPrimary,
        tdvl.TreeGrade,
        tdvd.TreeGrade,
        tdvl.CullSecondary,
        tdvl.HiddenSecondary,
        tdvl.Recoverable,
        tdvl.MerchHeightLogLength,
        tdvl.MerchHeightType,
        tdvl.FormClass,
        tdvl.BarkThicknessRatio,
        tdvl.AverageZ,
        tdvl.ReferenceHeightPercent,
        tdvl.CreatedBy,
        tdvl.CreatedDate,
        tdvl.ModifiedBy,
        tdvl.ModifiedDate
    FROM (SELECT * FROM {fromDbName}.TreeDefaultValue WHERE LiveDead = 'L') AS tdvl 
    LEFT JOIN (SELECT * FROM {fromDbName}.TreeDefaultValue WHERE LiveDead = 'D') AS tdvd USING (Species, PrimaryProduct);
";
        }
    }
}
