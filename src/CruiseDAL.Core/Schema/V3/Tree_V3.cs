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
                "CountOrMeasure TEXT DEFAULT 'M' COLLATE NOCASE," + // field is for compatibility with older schema. because plot cruising still requires a tree record to record non measure trees
                "Initials TEXT, " +                                 // initials of the cruiser taking measurments
                "XCoordinate REAL, " +
                "YCoordinate REAL, " +
                "ZCoordinate REAL, " +

                "CreatedBy TEXT DEFAULT '', " +
                "CreatedDate DateTime DEFAULT (datetime('now', 'localtime')), " +
                "ModifiedBy TEXT, " +
                "ModifiedDate DateTime , " +
                "RowVersion INTEGER DEFAULT 0, " +
                "UNIQUE (TreeID), " +
                "FOREIGN KEY (CuttingUnitCode) REFERENCES CuttingUnit (Code) ON DELETE CASCADE ON UPDATE CASCADE, " +
                "FOREIGN KEY (StratumCode) REFERENCES Stratum (Code) " +
                "FOREIGN KEY (SampleGroupCode, StratumCode) REFERENCES SampleGroup_V3 (SampleGroupCode, StratumCode) ON DELETE CASCADE ON UPDATE CASCADE, " +
                "FOREIGN KEY (PlotNumber) REFERENCES Plot_V3 (PlotNumber) ON DELETE CASCADE, " +
                //"FOREIGN KEY (Species, LiveDead, SampleGroupCode, StratumCode) REFERENCES SubPopulation (Species, LiveDead, SampleGroupCode, StratumCode), " +
                "FOREIGN KEY (Species) REFERENCES Species (Species) " +
            ")";

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
                "CountOrMeasure, " +
                "Initials " +
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
                    "Initials, " +
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
                    "t.Initials, " +
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