using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CruiseDAL.Schema
{
    public partial class DDL
    {
        public const string CREATE_VIEW_TallyLedger_Totals =
@"CREATE VIEW TallyLedger_Totals AS
SELECT 
    tl.StratumCode,
    tl.SampleGroupCode,
    tl.Species,
    tl.LiveDead,
    sum(tl.TreeCount) AS TreeCount,
    sum(tl.KPI) AS KPI
FROM TallyLedger AS tl
GROUP BY 
    CuttingUnitCode,
    StratumCode,
    SampleGroupCode,
    ifnull(Species, ''),
    ifnull(LiveDead, '');";
    }
}
