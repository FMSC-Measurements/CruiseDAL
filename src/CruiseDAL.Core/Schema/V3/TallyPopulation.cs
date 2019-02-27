using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CruiseDAL.Schema.V3
{
    public partial class DDL
    {
        //internal const string CREATE_VIEW_TALLY_POPULATION =
        //    "CREATE VIEW TallyPopulation " +
        //    "( StratumCode, SampleGroupCode, Species, LiveDead, Description, HotKey) " +
        //    "AS " +
        //    "SELECT Stratum.Code, SampleGroup.Code, TDV.Species, TDV.LiveDead, Tally.Description, Tally.HotKey " +
        //    "FROM CountTree " +
        //    "JOIN SampleGroup USING (SampleGroup_CN) " +
        //    "JOIN Stratum USING (Stratum_CN) " +
        //    "LEFT JOIN TreeDefaultValue AS TDV USING (TreeDefaultValue_CN) " +
        //    "JOIN Tally USING (Tally_CN) " +
        //    "GROUP BY SampleGroup_CN, ifnull(TreeDefaultValue_CN, '');";

        public const string CREATE_TABLE_TALLYPOPULATION =
            "CREATE TABLE TallyPopulation ( " +
            "TallyPopulation_CN INTEGER PRIMARY KEY AUTOINCREMENT, " +
            "StratumCode TEXT NOT NULL COLLATE NOCASE, " +
            "SampleGroupCode TEXT NOT NULL COLLATE NOCASE, " +
            "Species TEXT DEFAULT '' COLLATE NOCASE, " +
            "LiveDead TEXT DEFAULT 'default' COLLATE NOCASE, " +
            "Description TEXT, " +
            "HotKey COLLATE NOCASE, " +
            "FOREIGN KEY (StratumCode) REFERENCES Stratum (Code), " +
            "FOREIGN KEY (SampleGroupCode, StratumCode) REFERENCES SampleGroup_V3 (SampleGroupCode, StratumCode)  ON DELETE CASCADE ON UPDATE CASCADE, " +
            "FOREIGN KEY (Species) REFERENCES Species (Species), " +
            "UNIQUE (StratumCode, SampleGroupCode, Species, LiveDead) " +
            ");";
    }
}
