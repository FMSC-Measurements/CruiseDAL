using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CruiseDAL.Schema
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
            ");" +
        ");";
    }
}
