﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CruiseDAL.Schema
{ 
    public partial class DDL
    {
        public const string CREATE_TABLE_TALLYHOTKEY =
            "CREATE TABLE TallyHotKey ( " +
                "StratumCode TEXT NOT NULL COLLATE NOCASE, " +
                "SampleGroupCode TEXT NOT NULL COLLATE NOCASE, " +
                "Species TEXT DEFAULT '' COLLATE NOCASE, " +
                "LiveDead TEXT DEFAULT '' COLLATE NOCASE, " +
                "HotKey TEXT COLLATE NOCASE," +

                "UNIQUE (StratumCode, HotKey) ON CONFLICT REPLACE, " +
                "UNIQUE (StratumCode, SampleGroupCode, Species, LiveDead) ON CONFLICT REPLACE, " +

                "FOREIGN KEY (StratumCode, SampleGroupCode) REFERENCES SampleGroup_V3 (StratumCode, SampleGroupCode) ON DELETE CASCADE, " +
                "FOREIGN KEY (Species) REFERENCES Species (Species) ON UPDATE CASCADE " +
            ");";
    }

    public partial class Migrations
    {
        public const string MIGRATE_TALLYHOTKEY_FROM_COUNTTREE_FORMAT_STR =
            "WITH ctFlattened AS ( " +
            "SELECT SampleGroup_CN, TreeDefaultValue_CN, max(Tally_CN) AS Tally_CN " +
            "FROM {1}.CountTree " +
            "GROUP BY SampleGroup_CN, ifnull(TreeDefaultValue_CN, '')) " +

            "INSERT OR REPLACE INTO {0}.TallyHotKey ( " +
                "StratumCode, " +
                "SampleGroupCode, " +
                "Species, " +
                "LiveDead, " +
                "HotKey " +
            ")" +
            "SELECT " +
                "st.Code AS StratumCode, " +
                "sg.Code AS SampleGroupCode, " +
                "tdv.Species, " +
                "tdv.LiveDead, " +
                "t.HotKey " +
            "FROM ctFlattened " +
            "JOIN {1}.SampleGroup AS sg USING (SampleGroup_CN) " +
            "JOIN {1}.Stratum AS st USING (Stratum_CN) " +
            "JOIN {1}.Tally AS t USING (Tally_CN) " +
            "LEFT JOIN {1}.TreeDefaultValue AS tdv USING (TreeDefaultValue_CN)" +
            "; ";

    }

}