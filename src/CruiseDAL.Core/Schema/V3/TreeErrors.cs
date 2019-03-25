using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CruiseDAL.Schema
{
    public partial class DDL
    {
        public const string CREATE_VIEW_TREEERROR =
@"CREATE VIEW Tree_Error AS
SELECT 
TreeID, 
TreeAuditRuleID
FROM Tree_V3 AS t
JOIN SampleGroup_V3 USING (SampleGroupCode, StratumCode)
JOIN TreeDefaultValue_TreeAuditRule tdvtar 
        ON (tdvtar.Species IS NULL OR tdvtar.Species = t.Species 
        AND (tdvtar.LiveDead IS NULL OR tdvtar.LiveDead = t.LiveDead) 
        AND (tdvtar.PrimaryProduct OR sg.PrimaryProduct = t.PrimaryProduct
JOIN TreeAuditRule AS tar USING (TreeAuditRuleID)
JOIN TreeFieldValue_All AS tfv ON (TreeID, Field)
WHERE  
tar.ValueReal IS NOT NULL
(tar.Min IS NOT NULL AND tfv.ValueReal < tar.Min) 
OR (tar.Max IS NOT NULL AND tfv.ValueReal > tar.Max)
;";
    }
}
