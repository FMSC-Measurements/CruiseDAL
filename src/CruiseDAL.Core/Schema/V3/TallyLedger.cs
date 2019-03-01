using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CruiseDAL.Schema.V3
{
    public partial class DDL
    {
        public const string CREATE_TABLE_TALLY_LEDGER_COMMAND =
        "CREATE TABLE TallyLedger ( " +
            "TallyLedgerID TEXT PRIMARY KEY, " +
            "TreeID TEXT, " +
            "CuttingUnitCode TEXT NOT NULL COLLATE NOCASE, " +
            "StratumCode TEXT NOT NULL COLLATE NOCASE, " +
            "SampleGroupCode TEXT NOT NULL COLLATE NOCASE, " +
            "PlotNumber INTEGER, " +
            "Species TEXT DEFAULT '' COLLATE NOCASE, " +
            "LiveDead TEXT DEFAULT '' COLLATE NOCASE, " +
            "TreeCount INTEGER NOT NULL, " +
            "KPI INTEGER Default 0, " +
            "STM TEXT DEFAULT 'N' COLLATE NOCASE, " +
            "ThreePRandomValue INTEGER Default 0, " +
            "Signature TEXT COLLATE NOCASE, " +
            "Reason TEXT, " +
            "Remarks TEXT, " +
            "EntryType TEXT COLLATE NOCASE, " +

            "CreatedBy TEXT DEFAULT 'none', " +
            "CreatedDate DateTime DEFAULT (datetime(current_timestamp, 'localtime')) ," +
            "IsDeleted BOOLEAN DEFAULT 0," +

            "FOREIGN KEY (CuttingUnitCode) REFERENCES CuttingUnit (Code) " +
            "FOREIGN KEY (CuttingUnitCode, StratumCode) REFERENCES CuttingUnit_Stratum (CuttingUnitCode, StratumCode), " +
            "FOREIGN KEY (StratumCode) REFERENCES Stratum (Code) , " +
            "FOREIGN KEY (SampleGroupCode, StratumCode) REFERENCES SampleGroup_V3 (SampleGroupCode, StratumCode), " +
            "FOREIGN KEY (StratumCode, SampleGroupCode, Species, LiveDead) REFERENCES TallyPopulation, " +
            "FOREIGN KEY (StratumCode, PlotNumber) REFERENCES Plot_Stratum (StratumCode, PlotNumber), " +
            "FOREIGN KEY (TreeID) REFERENCES Tree_V3 (TreeID) ON DELETE CASCADE" +
        ");";
    }

    public partial class Updater
    {
        public const string INITIALIZE_TALLYLEDGER_FROM_COUNTTREE =
            "INSERT INTO TallyLedger " +
            "(TallyLedgerID, CuttingUnitCode, StratumCode, SampleGroupCode, Species, LiveDead, TreeCount, KPI, EntryType) " +
            "SELECT " +
            "'initFromCountTree' | '-' | ifnull(Component_CN, 'master'), " +
            "CuttingUnit.Code AS CuttingUnitCode, " +
            "Stratum.Code AS StratumCode, " +
            "SampleGroup.Code AS SampleGroupCode, " +
            "TDV.Species AS Species, " +
            "TDV.LiveDead AS LiveDead, " +
            "Sum(TreeCount) AS TreeCount, " +
            "Sum(SumKPI) AS SumKPI, " +
            "'utility' AS EntryType " +
            "FROM CountTree AS ct " +
            "JOIN CuttingUnit AS cu USING (CuttingUnit_CN) " +
            "JOIN SampleGroup AS sg USING (SampleGroup_CN) " +
            "JOIN Stratum AS st USING (Stratum_CN) " +
            "LEFT JOIN TreeDefaultValue AS tdv USING (TreeDefaultValue_CN) " +
            "GROUP BY cu.Code, st.Code, sg.Code, ifnull(tdv.Species, ''), ifnull(tdv.LiveDead, ''), ifnull(ct.Component_CN, 0);";

        public const string INITIALIZE_TALLYLEDGER_FROM_TREE =
            "WITH measureTrees AS (" +
            "SELECT tv3.TreeID, tv3.CuttingUnitCode, tp.StratumCode, tp.SampleGroupCode, tp.Species, tp.LiveDead, " +
            "t.TreeCount, t.KPI, t.STM,  * FROM Tree as t " +
            "JOIN Tree_V3 as tv3 USING (Tree_CN) " +
            "JOIN TallyPopulation AS tp ON tp.StratumCode = tv3.StratumCode AND tp.SampleGroupCode = tv3.SampleGroupCode AND (tp.Species = tv3.Species OR tp.Species = '') AND (tp.LiveDead = tv3.LiveDead OR tp.LiveDead = 'default') " +
            "WHERE t.CountOrMeasure = 'M' OR t.CountOrMeasure = 'm') " +
            "INSERT INTO TallyLedger " +
            "(TallyLedgerID, TreeID, CuttingUnitCode, StratumCode, SampleGroupCode, Species, LiveDead, TreeCount, KPI, STM) " +
            "SELECT 'initFromTree' | TreeID AS TallyLedgerID, * FROM measureTrees;";
    }
}
