using System.Collections.Generic;

namespace CruiseDAL.Schema
{
    public class TreeTableDefinition : ITableDefinition
    {
        public string TableName => "Tree";

        // values stored in the tree table are value we don't expect to change after the initial insert of the record
        // for changable values use the treeMeasurments table or treeFieldValue table
        public string CreateTable =>
@"CREATE TABLE Tree (
    Tree_CN INTEGER PRIMARY KEY AUTOINCREMENT,
    CruiseID TEXT NOT NULL COLLATE NOCASE,
    TreeID TEXT NOT NULL ,
    CuttingUnitCode TEXT NOT NULL COLLATE NOCASE,
    StratumCode TEXT NOT NULL COLLATE NOCASE,
    SampleGroupCode TEXT NOT NULL COLLATE NOCASE,
    SpeciesCode TEXT COLLATE NOCASE,
    LiveDead TEXT COLLATE NOCASE,
    PlotNumber INTEGER,
    TreeNumber INTEGER NOT NULL,
    CountOrMeasure TEXT DEFAULT 'M' COLLATE NOCASE, -- field is for compatibility with older schema. because plot cruising still requires a tree record to record non measure trees                               // initials of the cruiser taking measurments

    CreatedBy TEXT DEFAULT 'none',
    Created_TS DATETIME DEFAULT (CURRENT_TIMESTAMP),
    ModifiedBy TEXT,
    Modified_TS DATETIME,

    UNIQUE (TreeID),

    CHECK (TreeID LIKE '________-____-____-____-____________'),
    CHECK (CountOrMeasure IN ('C', 'M', 'I')),
    CHECK (LiveDead IN ('L', 'D') OR LiveDead IS NULL),

    FOREIGN KEY (CuttingUnitCode, CruiseID)
        REFERENCES CuttingUnit (CuttingUnitCode, CruiseID) ON DELETE CASCADE ON UPDATE CASCADE,
    FOREIGN KEY (SampleGroupCode, StratumCode, CruiseID)
        REFERENCES SampleGroup (SampleGroupCode, StratumCode, CruiseID) ON DELETE CASCADE ON UPDATE CASCADE,
    FOREIGN KEY (PlotNumber, CuttingUnitCode, CruiseID)
        REFERENCES Plot (PlotNumber, CuttingUnitCode, CruiseID) ON DELETE CASCADE ON UPDATE CASCADE,
    --FOREIGN KEY (Species, LiveDead, SampleGroupCode, StratumCode)
    --    REFERENCES SubPopulation (Species, LiveDead, SampleGroupCode, StratumCode),
    FOREIGN KEY (SpeciesCode, CruiseID)
        REFERENCES Species (SpeciesCode, CruiseID)
)";

        public string InitializeTable => null;

        public string CreateTombstoneTable =>
@"CREATE TABLE Tree_Tombstone (
    CruiseID TEXT NOT NULL COLLATE NOCASE,
    TreeID TEXT NOT NULL ,
    CuttingUnitCode TEXT NOT NULL COLLATE NOCASE,
    StratumCode TEXT NOT NULL COLLATE NOCASE,
    SampleGroupCode TEXT NOT NULL COLLATE NOCASE,
    SpeciesCode TEXT COLLATE NOCASE,
    LiveDead TEXT COLLATE NOCASE,
    PlotNumber INTEGER,
    TreeNumber INTEGER NOT NULL,
    CountOrMeasure TEXT COLLATE NOCASE,

    CreatedBy TEXT,
    Created_TS DATETIME,
    ModifiedBy TEXT,
    Modified_TS DATETIME,
    Deleted_TS DATETIME
);

CREATE INDEX Tree_Tombstone_TreeID ON Tree_Tombstone 
(TreeID);

CREATE INDEX Tree_Tombstone_CruiseID_CuttingUnitCode_PlotNumber_TreeNumber ON Tree_Tombstone 
(CruiseID, CuttingUnitCode, PlotNumber, TreeNumber);";

        public string CreateIndexes =>
@"CREATE INDEX Tree_TreeNumber_CruiseID ON Tree (TreeNumber, CruiseID);

CREATE INDEX 'Tree_SpeciesCode' ON 'Tree'('SpeciesCode');

CREATE INDEX 'Tree_PlotNumber_CuttingUnitCode_CruiseID' ON 'Tree'('PlotNumber', 'CuttingUnitCode', 'CruiseID');

CREATE INDEX 'Tree_SampleGroupCode_StratumCode_CruiseID' ON 'Tree'('SampleGroupCode', 'StratumCode', 'CruiseID');

CREATE INDEX 'Tree_StratumCode_CruiseID' ON 'Tree'('StratumCode', 'CruiseID');

CREATE INDEX 'Tree_CuttingUnitCode_CruiseID' ON 'Tree'('CuttingUnitCode', 'CruiseID');

CREATE UNIQUE INDEX Tree_TreeID_CuttingUnitCode_SampleGroupCode_StratumCode ON Tree (TreeID, CuttingUnitCode, SampleGroupCode, StratumCode);

CREATE UNIQUE INDEX Tree_TreeID_SpeciesCode ON Tree (TreeID, SpeciesCode);

CREATE UNIQUE INDEX Tree_TreeID_LiveDead ON Tree (TreeID, LiveDead);

CREATE UNIQUE INDEX Tree_TreeID_PlotNumber ON Tree (TreeID, PlotNumber);

CREATE UNIQUE INDEX Tree_TreeNumber_CuttingUnitCode_PlotNumber_StratumCode_CruiseID ON Tree
    (TreeNumber, CuttingUnitCode, PlotNumber, StratumCode, CruiseID) WHERE PlotNumber IS NOT NULL;

CREATE UNIQUE INDEX Tree_TreeNumber_CuttingUnitCode_CruiseID ON Tree
    (TreeNumber, CuttingUnitCode, CruiseID) WHERE PlotNumber IS NULL;";

        public IEnumerable<string> CreateTriggers => new[] { CREATE_TRIGGER_TREE_ONUPDATE, CREATE_TRIGGER_Tree_OnDelete };

        public const string CREATE_TRIGGER_TREE_ONUPDATE =
@"CREATE TRIGGER Tree_OnUpdate
AFTER UPDATE OF
    TreeID,
    CuttingUnitCode,
    StratumCode,
    SampleGroupCode,
    SpeciesCode,
    LiveDead,
    PlotNumber,
    TreeNumber,
    CountOrMeasure
ON Tree
FOR EACH ROW
BEGIN
    UPDATE Tree SET Modified_TS = CURRENT_TIMESTAMP WHERE Tree_CN = old.Tree_CN;
END;";

        public const string CREATE_TRIGGER_Tree_OnDelete =
@"CREATE TRIGGER Tree_OnDelete
BEFORE DELETE ON Tree
FOR EACH ROW
BEGIN
    INSERT OR REPLACE INTO Tree_Tombstone (
        CruiseID,
        TreeID,
        CuttingUnitCode,
        StratumCode,
        SampleGroupCode,
        SpeciesCode,
        LiveDead,
        PlotNumber,
        TreeNumber,
        CountOrMeasure,

        CreatedBy,
        Created_TS,
        ModifiedBy,
        Modified_TS,
        Deleted_TS
    ) VALUES (
        OLD.CruiseID,
        OLD.TreeID,
        OLD.CuttingUnitCode,
        OLD.StratumCode,
        OLD.SampleGroupCode,
        OLD.SpeciesCode,
        OLD.LiveDead,
        OLD.PlotNumber,
        OLD.TreeNumber,
        OLD.CountOrMeasure,

        OLD.CreatedBy,
        OLD.Created_TS,
        OLD.ModifiedBy,
        OLD.Modified_TS,
        CURRENT_TIMESTAMP
    );
END;";
    }
}