using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CruiseDAL.Schema
{
    public partial class DDL
    {
        public static string CREATE_TABLE_TREEAUDITRESOLUTION =
@"CREATE TABLE TreeAuditResolution (
TreeID TEXT NOTNULL,
TreeAuditRuleID TEXT NOTNULL,
Resolution TEXT,
Initials TEXT,
FOREIGN KEY (TreeID) REFERENCES Tree_V3 (TreeID) ON DELETE CASCADE,
FOREIGN KEY (TreeAuditRuleID) REFERENCES TreeAuditRule (TreeAuditRuleID) ON DELETE CASCADE
);";
    }
}
