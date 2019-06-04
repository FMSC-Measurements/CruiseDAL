namespace CruiseDAL.Schema
{
    public partial class DDL
    {
        // values stored in the tree table are value we don't expect to change after the initial insert of the record
        // for changable values use the treeMeasurments table or treeFieldValue table
        public const string CREATE_TABLE_TREE_V3 =
            "CREATE TABLE Tree_V3 ( " +
                "Tree_CN INTEGER PRIMARY KEY AUTOINCREMENT, " +
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

                "CHECK (CountOrMeasure IN ('C', 'M', 'I')), " +
                "CHECK (LiveDead IN ('L', 'D') OR LiveDead IS NULL)" +

                "FOREIGN KEY (CuttingUnitCode) REFERENCES CuttingUnit (Code) ON DELETE CASCADE ON UPDATE CASCADE, " +
                "FOREIGN KEY (StratumCode) REFERENCES Stratum (Code) " +
                "FOREIGN KEY (SampleGroupCode, StratumCode) REFERENCES SampleGroup_V3 (SampleGroupCode, StratumCode) ON DELETE CASCADE ON UPDATE CASCADE, " +
                "FOREIGN KEY (PlotNumber, CuttingUnitCode) REFERENCES Plot_V3 (PlotNumber, CuttingUnitCode) ON DELETE CASCADE, " +
                //"FOREIGN KEY (Species, LiveDead, SampleGroupCode, StratumCode) REFERENCES SubPopulation (Species, LiveDead, SampleGroupCode, StratumCode), " +
                "FOREIGN KEY (Species) REFERENCES Species (Species) " +
            ")";

        public const string CREATE_INDEX_Tree_V3_Species =
            @"CREATE INDEX 'Tree_V3_Species' ON 'Tree_V3'('Species');";

        public const string CREATE_INDEX_Tree_V3_PlotNumber_CuttingUnitCode =
            @"CREATE INDEX 'Tree_V3_PlotNumber_CuttingUnitCode' ON 'Tree_V3'('PlotNumber', 'CuttingUnitCode');";

        public const string CREATE_INDEX_Tree_V3_SampleGroupCode_StratumCode =
            @"CREATE INDEX 'Tree_V3_SampleGroupCode_StratumCode' ON 'Tree_V3'('SampleGroupCode', 'StratumCode');";

        public const string CREATE_INDEX_Tree_V3_StratumCode =
            @"CREATE INDEX 'Tree_V3_StratumCode' ON 'Tree_V3'('StratumCode');";

        public const string CREATE_INDEX_Tree_V3_CuttingUnitCode =
            @"CREATE INDEX 'Tree_V3_CuttingUnitCode' ON 'Tree_V3'('CuttingUnitCode');";

        public const string CREATE_INDEX_Tree_V3_TreeID_CuttingUnitCode_SampleGroupCode_StratumCode =
            @"CREATE UNIQUE INDEX Tree_V3_TreeID_CuttingUnitCode_SampleGroupCode_StratumCode ON Tree_V3 (TreeID, CuttingUnitCode, SampleGroupCode, StratumCode);";

        public const string CREATE_INDEX_Tree_V3_TreeID_Species =
            @"CREATE UNIQUE INDEX Tree_V3_TreeID_Species ON Tree_V3 (TreeID, Species);";

        public const string CREATE_INDEX_Tree_V3_TreeID_LiveDead =
            @"CREATE UNIQUE INDEX Tree_V3_TreeID_LiveDead ON Tree_V3 (TreeID, LiveDead);";

        public const string CREATE_INDEX_Tree_V3_TreeID_PlotNumber =
            @"CREATE UNIQUE INDEX Tree_V3_TreeID_PlotNumber ON Tree_V3 (TreeID, PlotNumber);";

        public const string CREATE_TRIGGER_TREE_V3_ONUPDATE =
            "CREATE TRIGGER Tree_V3_OnUpdate " +
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
                "UPDATE Tree_V3 SET ModifiedDate = datetime('now', 'localtime') WHERE Tree_CN = old.Tree_CN; " +
                "UPDATE Tree_V3 SET RowVersion = old.RowVersion + 1 WHERE Tree_CN = old.Tree_CN; " +
            "END; ";
    }

    public partial class Migrations
    {
        public const string MIGRATE_TREE_V3_FROM_TREE =
            "INSERT INTO {0}.Tree_V3 ( " +
                    "Tree_CN, " +
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
                    "ifnull(t.Tree_GUID, 'migrateTree-' || t.Tree_CN) AS TreeID, " +
                    "cu.Code AS CuttingUnitCode, " +
                    "st.Code AS StratumCode, " +
                    "sg.Code AS SampleGroupCode, " +
                    "tdv.Species, " +
                    "tdv.LiveDead, " +
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