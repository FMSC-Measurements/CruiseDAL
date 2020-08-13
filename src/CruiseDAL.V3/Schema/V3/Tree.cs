namespace CruiseDAL.Schema
{
    public partial class DDL
    {
        // values stored in the tree table are value we don't expect to change after the initial insert of the record
        // for changable values use the treeMeasurments table or treeFieldValue table
        public const string CREATE_TABLE_TREE =
            "CREATE TABLE Tree ( " +
                "Tree_CN INTEGER PRIMARY KEY AUTOINCREMENT, " +
                "CruiseID TEXT NOT NULL COLLATE NOCASE, " +
                "TreeID TEXT NOT NULL , " +
                "CuttingUnitCode TEXT NOT NULL COLLATE NOCASE, " +
                "StratumCode TEXT NOT NULL COLLATE NOCASE, " +
                "SampleGroupCode TEXT NOT NULL COLLATE NOCASE, " +
                "Species TEXT COLLATE NOCASE, " +
                "LiveDead TEXT COLLATE NOCASE, " +
                "PlotNumber INTEGER, " +
                "TreeNumber INTEGER NOT NULL, " +
                "CountOrMeasure TEXT DEFAULT 'M' COLLATE NOCASE," + // field is for compatibility with older schema. because plot cruising still requires a tree record to record non measure trees                               // initials of the cruiser taking measurments
                "XCoordinate REAL, " +
                "YCoordinate REAL, " +
                "ZCoordinate REAL, " +

                "CreatedBy TEXT DEFAULT '', " +
                "CreatedDate DateTime DEFAULT (datetime('now', 'localtime')), " +
                "ModifiedBy TEXT, " +
                "ModifiedDate DateTime , " +
                "RowVersion INTEGER DEFAULT 0, " +

                "UNIQUE (TreeID), " +

                "CHECK (TreeID LIKE '________-____-____-____-____________'), " +
                "CHECK (CountOrMeasure IN ('C', 'M', 'I')), " +
                "CHECK (LiveDead IN ('L', 'D') OR LiveDead IS NULL)," +

                "FOREIGN KEY (CuttingUnitCode, CruiseID) REFERENCES CuttingUnit (Code, CruiseID) ON DELETE CASCADE ON UPDATE CASCADE, " +
                "FOREIGN KEY (SampleGroupCode, StratumCode, CruiseID) REFERENCES SampleGroup (SampleGroupCode, StratumCode, CruiseID) ON DELETE CASCADE ON UPDATE CASCADE, " +
                "FOREIGN KEY (PlotNumber, CuttingUnitCode, CruiseID) REFERENCES Plot (PlotNumber, CuttingUnitCode, CruiseID) ON DELETE CASCADE ON UPDATE CASCADE, " +
                //"FOREIGN KEY (Species, LiveDead, SampleGroupCode, StratumCode) REFERENCES SubPopulation (Species, LiveDead, SampleGroupCode, StratumCode), " +
                "FOREIGN KEY (Species) REFERENCES SpeciesCode (Species) " +
            ")";

        public const string CREATE_INDEX_Tree_TreeNumber_CruiseID =
            "CREATE INDEX Tree_TreeNumber_CruiseID ON Tree (TreeNumber, CruiseID);";

        public const string CREATE_INDEX_Tree_Species =
            @"CREATE INDEX 'Tree_Species' ON 'Tree'('Species');";

        public const string CREATE_INDEX_Tree_PlotNumber_CuttingUnitCode_CruiseID =
            @"CREATE INDEX 'Tree_PlotNumber_CuttingUnitCode_CruiseID' ON 'Tree'('PlotNumber', 'CuttingUnitCode', 'CruiseID');";

        public const string CREATE_INDEX_Tree_SampleGroupCode_StratumCode_CruiseID =
            @"CREATE INDEX 'Tree_SampleGroupCode_StratumCode_CruiseID' ON 'Tree'('SampleGroupCode', 'StratumCode', 'CruiseID');";

        public const string CREATE_INDEX_Tree_StratumCode_CruiseID =
            @"CREATE INDEX 'Tree_StratumCode_CruiseID' ON 'Tree'('StratumCode', 'CruiseID');";

        public const string CREATE_INDEX_Tree_CuttingUnitCode_CruiseID =
            @"CREATE INDEX 'Tree_CuttingUnitCode_CruiseID' ON 'Tree'('CuttingUnitCode', 'CruiseID');";

        public const string CREATE_INDEX_Tree_TreeID_CuttingUnitCode_SampleGroupCode_StratumCode =
            @"CREATE UNIQUE INDEX Tree_TreeID_CuttingUnitCode_SampleGroupCode_StratumCode ON Tree (TreeID, CuttingUnitCode, SampleGroupCode, StratumCode);";

        public const string CREATE_INDEX_Tree_TreeID_Species =
            @"CREATE UNIQUE INDEX Tree_TreeID_Species ON Tree (TreeID, Species);";

        public const string CREATE_INDEX_Tree_TreeID_LiveDead =
            @"CREATE UNIQUE INDEX Tree_TreeID_LiveDead ON Tree (TreeID, LiveDead);";

        public const string CREATE_INDEX_Tree_TreeID_PlotNumber =
            @"CREATE UNIQUE INDEX Tree_TreeID_PlotNumber ON Tree (TreeID, PlotNumber);";

        public const string CREATE_TRIGGER_TREE_ONUPDATE =
            "CREATE TRIGGER Tree_OnUpdate " +
            "AFTER UPDATE OF " +
                "TreeID, " +
                "CuttingUnitCode, " +
                "StratumCode, " +
                "SampleGroupCode, " +
                "Species, " +
                "LiveDead, " +
                "PlotNumber, " +
                "TreeNumber, " +
                "CountOrMeasure " +
            "ON Tree_V3 " +
            "FOR EACH ROW " +
            "BEGIN " +
                "UPDATE Tree SET ModifiedDate = datetime('now', 'localtime') WHERE Tree_CN = old.Tree_CN; " +
                "UPDATE Tree SET RowVersion = old.RowVersion + 1 WHERE Tree_CN = old.Tree_CN; " +
            "END; ";
    }

    public partial class Migrations
    {
        public const string MIGRATE_TREE_FROM_TREE =
            //@"WITH generate_guid AS ( SELECT hex( randomblob(4)) || '-' || hex( randomblob(2)) 
            // || '-' || '4' || substr(hex(randomblob(2)), 2) || '-'
            // || substr('AB89', 1 + (abs(random()) % 4), 1) ||
            // substr(hex(randomblob(2)), 2) || '-' || hex(randomblob(6)) AS guid ) " +

            "INSERT INTO {0}.Tree ( " +
                    "Tree_CN, " +
                    "CruiseID, " +
                    "TreeID, " +
                    "CuttingUnitCode, " +
                    "StratumCode, " +
                    "SampleGroupCode, " +
                    "Species, " +
                    "LiveDead, " +
                    "PlotNumber, " +
                    "TreeNumber, " +
                    "CountOrMeasure, " +
                    "CreatedBy, " +
                    "CreatedDate, " +
                    "ModifiedBy, " +
                    "ModifiedDate, " +
                    "RowVersion " +
                ") " +
                "SELECT " +
                    "t.Tree_CN, " +
                    "'{4}'," +
                    "ifnull( " +
                        "(CASE typeof(Tree_GUID) COLLATE NOCASE " + // ckeck the type of Tree_GUID
                            "WHEN 'TEXT' THEN " + // if text
                                "(CASE WHEN Tree_GUID LIKE '________-____-____-____-____________' " + // check to see if it is a properly formated guid
                                    "THEN nullif(Tree_GUID, '00000000-0000-0000-0000-000000000000') " + // if not a empty guid return that value otherwise return null for now
                                    "ELSE NULL END) " + // if it is not a properly formatted guid return Tree_GUID
                            "ELSE NULL END)" + // if value is not a string return null
                        ", (hex( randomblob(4)) || '-' || hex( randomblob(2)) " +
                             "|| '-' || '4' || substr(hex(randomblob(2)), 2) || '-' " +
                             "|| substr('AB89', 1 + (abs(random()) % 4), 1) || " +
                             "substr(hex(randomblob(2)), 2) || '-' || hex(randomblob(6)))) AS TreeID, " + // if value is null sofar generate guid
                    "cu.Code AS CuttingUnitCode, " +
                    "st.Code AS StratumCode, " +
                    "sg.Code AS SampleGroupCode, " +
                    "tdv.Species, " +
                    "t.LiveDead, " + // use livedead from tree instead of tdv, because that is the value used by cruise processing
                    "p.PlotNumber, " +
                    "t.TreeNumber, " +
                    "t.CountOrMeasure, " +
                    "t.CreatedBy, " +
                    "t.CreatedDate, " +
                    "t.ModifiedBy, " +
                    "t.ModifiedDate, " +
                    "t.RowVersion " +
                "FROM {1}.Tree as t " +
                "JOIN {1}.CuttingUnit AS cu USING (CuttingUnit_CN) " +
                "JOIN {1}.Stratum AS st USING (Stratum_CN) " +
                "JOIN {1}.SampleGroup AS sg USING (SampleGroup_CN) " +
                "LEFT JOIN {1}.TreeDefaultValue AS tdv USING (TreeDefaultValue_CN) " +
                "LEFT JOIN {1}.Plot AS p USING (Plot_CN);";
    }

    //public partial class Updater
    //{
    //    public const string INITIALIZE_TREE_V3_FROM_TREE =
    //        "INSERT INTO Tree_V3 " +
    //        "SELECT " +
    //            "t.Tree_CN, " +
    //            "t.Tree_GUID AS TreeID, " +
    //            "cu.Code AS CuttingUnitCode, " +
    //            "st.Code AS StratumCode, " +
    //            "sg.Code AS SampleGroupCode, " +
    //            "tdv.Species, " +
    //            "tdv.LiveDead, " +
    //            "p.PlotNumber, " +
    //            "t.TreeNumber, " +
    //            "t.CountOrMeasure, " +
    //            "t.Initials, " +
    //            "t.CreatedBy, " +
    //            "t.CreatedDate, " +
    //            "t.ModifiedBy, " +
    //            "t.ModifiedDate, " +
    //            "t.RowVersion " +
    //        "FROM Tree as t " +
    //        "JOIN CuttingUnit AS cu USING (CuttingUnit_CN) " +
    //        "JOIN Stratum AS st USING (Stratum_CN) " +
    //        "JOIN SampleGroup AS sg USING (SampleGroup_CN) " +
    //        "LEFT JOIN TreeDefaultValue AS tdv USING (TreeDefaultValue_CN) " +
    //        "LEFT JOIN Plot AS p USING (Plot_CN);";
    //}
}