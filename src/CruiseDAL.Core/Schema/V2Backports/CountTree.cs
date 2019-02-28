using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CruiseDAL.Schema.V2Backports
{
    public partial class DDL
    {
        public const string CREATE_VIEW_COUNTTREE =
            "CREATE VIEW CountTree AS " +
            "WITH tallyLedgerGrouped AS (" +
                "SELECT CuttingUnitCode, StratumCode, SampleGroupCode, ifnull(Species, '') AS Species, ifnull(LiveDead, '') AS LiveDead, " +
                "sum(TreeCount), sum(KPI) AS SumKPI " +
                "FROM TallyLedger " +
                "GROUP BY CuttingUnitCode, StratumCode, SampleGroupCode, Species, LiveDead " +
            ") " +

            ", tallyPopulationTallyLedger AS (" +
                "SELECT cust.CuttingUnitCode, " +
                "tp.StratumCode, " +
                "tp.SampleGroupCode, " +
                "tp.Species, tp.LiveDead, " +
                "ifnull(tl.TreeCount, 0) AS TreeCount, ifnull(tl.SumKPI, 0) AS SumKPI " +
                "FROM TallyPopulation AS tp " +
                "JOIN CuttingUnit_Stratum AS cust USING (StratumCode) " +
                "LEFT JOIN tallyLedgerGrouped AS tl ON cust.CuttingUnitCode = tl.CuttingUnitCode " +
                    "AND tl.StratumCode = tp.StratumCode " +
                    "AND tl.SampleGroupCode = tp.SampleGroupCode " +
                    "AND tl.Species = tp.Species " +
                    "AND tl.LiveDead = tp.LiveDead " +
            ") " +

            "SELECT " +
                "row_number() AS CountTree_CN, " +
                "cu.CuttingUnit_CN, " +
                "sg.SampleGroup_CN, " +
                "tdv.TreeDefaultValue_CN, " +
                "0 AS Tally_CN, null AS Component_CN, " +
                "tptl.TreeCount, " +
                "tptl.SumKPI, " +
                "'' AS CreatedBy, " +
                "'' AS CreatedDate, " +
                "null AS ModifiedBy, " +
                "null AS ModifiedDate, " +
                "0 AS RowVersion " +
            "FROM tallyPopulationTallyLedger AS tptl " +
            "JOIN CuttingUnit AS cu ON tptl.CuttingUnitCode = cu.Code " +
            "JOIN SampleGroup AS sg USING (SampleGroupCode, StratumCode) " +
            "LEFT JOIN TreeDefaultValue AS tdv ON tptl.Species = tdv.Species AND tptl.LiveDead = tdv.LiveDead AND tdv.PrimaryProduct = sg.PrimaryProduct " +
            "; ";
    }
}
