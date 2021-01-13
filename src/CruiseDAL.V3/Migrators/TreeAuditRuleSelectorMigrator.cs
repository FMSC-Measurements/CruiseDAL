using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CruiseDAL.Migrators
{
    public class TreeAuditRuleSelectorMigrator : IMigrator
    {
        public string MigrateToV3(string toDbName, string fromDbName, string cruiseID, string saleID)
        {
            return
$@"INSERT INTO {toDbName}.TreeAuditRuleSelector (
        CruiseID,
        SpeciesCode,
        LiveDead,
        PrimaryProduct,
        TreeAuditRuleID
    )
    SELECT
        '{cruiseID}',
        tdv.Species,
        tdv.LiveDead,
        tdv.PrimaryProduct,
        tar.TreeAuditRuleID
    FROM {fromDbName}.TreeDefaultValueTreeAuditValue AS tdvtav
    JOIN {fromDbName}.TreeDefaultValue AS tdv USING (TreeDefaultValue_CN)
    JOIN {toDbName}.TreeAuditRule AS tar ON tdvtav.TreeAuditValue_CN = tar.TreeAuditRule_CN;";
        }
    }
}
