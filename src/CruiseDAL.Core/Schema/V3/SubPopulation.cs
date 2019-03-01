using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CruiseDAL.Schema.V3
{
    public partial class DDL
    {
        public const string CREATE_TABLE_SUBPOPULATION =
            "CREATE TABLE Subpopulation (" +
            "Subpopulation_CN INTEGER PRIMARY KEY AUTOINCREMENT, " +
            "StratumCode TEXT NOT NULL COLLATE NOCASE, " +
            "SampleGroupCode TEXT NOT NULL COLLATE NOCASE, " +
            "Species TEXT NOT NULL, " +
            "LiveDead TEXT COLLATE NOCASE, " +
            "UNIQUE (StratumCode, SampleGroupCode, Species, LiveDead), " +
            "FOREIGN KEY (StratumCode) REFERENCES Stratum (Code), " +
            "FOREIGN KEY (StratumCode, SampleGroupCode) REFERENCES SampleGroup (StratumCode, SampleGroupCode) ON DELETE CASCADE ON UPDATE CASCADE," +
            "FOREIGN KEY (Species) REFERENCES Species (Species) " +
            ");";
    }

    public partial class Updater
    {
        public const string INITIALIZE_SUBPOPULATION_FROM_SAMPLEGROUPTREEDEFAULTVALUE =
            "INSERT INTO SubPopulation " +
            "SELECT " +
            "sg.StratumCode, " +
            "sg.SampleGroupCode, " +
            "tdv.Species, " +
            "tdv.LiveDead " +
            "FROM SampleGroupTreeDefaultValue as sgtdv " +
            "JOIN SampleGroup_V3 AS sg USING (SampleGroup_CN) " +
            "JOIN TreeDefaultValue AS tdv USING (TreeDefaultValue_CN);";
    }
}
