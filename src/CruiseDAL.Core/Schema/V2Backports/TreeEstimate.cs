using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CruiseDAL.Schema.V2Backports
{
    public partial class DDL
    {
        public const string CREATE_VIEW_TREEESTIMATE =
            "CREATE VIEW TreeEstimate AS " +
                "SELECT row_number() AS TreeEstimate_CN, " +
                "ct.CountTree_CN, " +
                "TallyLedgerID AS TreeEstimate_GUID, " +
                "tl.KPI, " +
                "tl.CreatedBy, " +
                "tl.CreatedDate, " +
                "null AS ModifiedBy, " +
                "null AS ModifiedDate;";
    }
}
