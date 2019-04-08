using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CruiseDAL.Schema.V3
{
    public partial class DDL
    {
        public const string CREATE_VIEW_TREEERROR =
@"CTEATE VIEW TreeError AS ( 
SELECT 
	tae.TreeID, 
	tae.TreeAuditRuleID, 
	null AS ErrorCode, 
	tae.Message, 
	tae.Field, 
	tar.Resolution, 
	tar.Initials AS ResolutionInitials 
FROM TreeAuditError AS tae 
JOIN TreeAuditResolution AS tar USING (TreeAuditRuleID, Field);";
        // union view for creating errors 
    }
}