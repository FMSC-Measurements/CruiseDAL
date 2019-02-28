using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CruiseDAL.Schema.V2Backports
{
    public partial class DDL
    {
        public const string CREATE_VIEW_TREEDEFAULTVALUETREEAUDITVALUE =
            "CREATE VIEW TreeDefaultValueTreeAuditValue AS " +
            "SELECT tdv.TreeDefaultValue_CN, " +
            "tav.TreeAuditValue_CN " +
            "FROM TreeDefaultValue_TreeAuditValue suptav " +
            "JOIN TreeDefaultValue AS tdv USING (Species, LiveDead, PrimaryProduct) " +
            "JOIN TreeAuditValue AS tav USING (TreeAuditValueID)";
    }
}
