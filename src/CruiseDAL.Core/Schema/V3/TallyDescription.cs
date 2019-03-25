using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CruiseDAL.Schema
{
    public partial class DDL
    {
        public const string CREATE_TABLE_TALLYDESCRIPTION =
            "CREATE TABLE TallyDescription ( " +
                "StratumCode TEXT NOT NULL COLLATE NOCASE, " +
                "SampleGroupCode TEXT NOT NULL COLLATE NOCASE, " +
                "Species TEXT COLLATE NOCASE, " +
                "LiveDead TEXT COLLATE NOCASE, " +
                "Description TEXT, " +

                //"UNIQUE (StratumCode, SampleGroupCode, Species, LiveDead)  ON CONFLICT REPLACE, " +
                //"UNIQUE (StratumCode, Description), " +

                "FOREIGN KEY (StratumCode, SampleGroupCode) REFERENCES SampleGroup_V3 (StratumCode, SampleGroupCode) ON DELETE CASCADE, " +
                "FOREIGN KEY (Species) REFERENCES Species (Species) ON DELETE CASCADE ON UPDATE CASCADE " +
            ");" +
            "CREATE UNIQUE INDEX TallyDescription_StratumCode_SampleGroupCode_Species_LiveDead " +
            "(StratumCode, SampleGroupCode, ifnull(Species, ''), ifnull(LiveDead, ''));";
    }

    public partial class Migrations
    {
        public const string MIGRATE_TALLYDESCRIPTION_FROM_COUNTTREE_FORMAT_STR =
            "WITH ctFlattened AS ( " +
            "SELECT SampleGroup_CN, TreeDefaultValue_CN, max(Tally_CN) AS Tally_CN " +
            "FROM {1}.CountTree " +
            "GROUP BY SampleGroup_CN, ifnull(TreeDefaultValue_CN, '')) " +

            "INSERT OR REPLACE INTO {0}.TallyDescription ( " +
                "StratumCode, " +
                "SampleGroupCode, " +
                "Species, " +
                "LiveDead, " +
                "Description " +
            ")" +
            "SELECT " +
                "st.Code AS StratumCode, " +
                "sg.Code AS SampleGroupCode, " +
                "tdv.Species, " +
                "tdv.LiveDead, " +
                "t.Description " +
            "FROM ctFlattened " +
            "JOIN {1}.SampleGroup AS sg USING (SampleGroup_CN) " +
            "JOIN {1}.Stratum AS st USING (Stratum_CN) " +
            "JOIN {1}.Tally AS t USING (Tally_CN) " +
            "LEFT JOIN {1}.TreeDefaultValue AS tdv USING (TreeDefaultValue_CN)" +
            "; ";

    }

}
