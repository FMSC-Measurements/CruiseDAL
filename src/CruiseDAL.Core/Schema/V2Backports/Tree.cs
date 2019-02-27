using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CruiseDAL.Schema.V2Backports
{
    public partial class DDL
    {
        public const string CREATE_VIEW_TREE =
            "CREATE VIEW Tree AS " +
            "SELECT t.Tree_CN, t.TreeID, t.CuttingUnitCode, t.StratumCode, t.SampleGroupCode, t.Species, tdv.FiaCode, t.LiveDead, t.PlotNumber, t.TreeNumber, " +
            "st.Stratum_CN, sg.SampleGroup_CN, tdv.TreeDefaultValue_CN,  plt.Plot_Stratum_CN AS Plot_CN, " +
            "tl.TreeCount, tl.KPI, tl.STM, tl.Signature, " +
            "tcv.ExpansionFactor, tcv.TreeFactor, tcv.PointFactor, " +
            "tm.* " +
            "FROM Tree_V3 AS t " +
            "JOIN Stratum AS st ON StratumCode = st.Code " +
            "JOIN SampleGroup AS sg USING (StratumCode, SampleGroupCode) " +
            "JOIN TallyLedger AS tl USING (TreeID) " +
            "LEFT JOIN TreeMeasurments AS tm USING (TreeID) " +
            "LEFT JOIN TreeCalculatedValues AS tcv USING (Tree_CN) " +
            "LEFT JOIN TreeDefaultValue AS tdv ON tdv.Species = t.Species AND tdv.LiveDead = t.LiveDead AND tdv.PrimaryProduct = sg.PrimaryProduct " +
            "LEFT JOIN Plot_Stratum AS plt USING (StratumCode, PlotNumber);";

        public const string CTEATE_TRIGGER_TREE_ONUPDATE =
            "CREATE TRIGGER TREE_ONUPDATE_PROCESSING " +
            "INSTEAD OF UPDATE OF ExpansionFactor, TreeFactor, PointFactor ON Tree " +
            "FOR EACH ROW " +
            "BEGIN " +
            "UPDATE TreeCalculatedValues SET ExpansionFactor = new.ExpansionFactor, TreeFactor = new.TreeFactor, PointFactor = new.PointFactor WHERE Tree_CN = new.Tree_CN; " +
            "END;";
    }
}
