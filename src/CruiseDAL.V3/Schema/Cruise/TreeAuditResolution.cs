namespace CruiseDAL.Schema
{
    public partial class DDL
    {
        public const string CREATE_TABLE_TREEAUDITRESOLUTION =
@"CREATE TABLE TreeAuditResolution (
    TreeAuditResolution_CN INTEGER PRIMARY KEY AUTOINCREMENT, 
    CruiseID TEXT NOT NULL COLLATE NOCASE,
    TreeID TEXT NOT NULL,
    TreeAuditRuleID TEXT NOT NULL,
    Resolution TEXT,    -- description indicating the reason of the resolution. optional
    Initials TEXT NOT NULL, -- initials of the cruiser that resolved the error. 

    UNIQUE (TreeID, TreeAuditRuleID),

    FOREIGN KEY (CruiseID) REFERENCES Cruise (CruiseID) ON DELETE CASCADE,
    FOREIGN KEY (TreeID) REFERENCES Tree (TreeID) ON DELETE CASCADE,
    FOREIGN KEY (TreeAuditRuleID) REFERENCES TreeAuditRule (TreeAuditRuleID) ON DELETE CASCADE
);";

        public const string CREATE_INDEX_TreeAuditResolution_TreeAuditRuleID =
            @"CREATE INDEX 'TreeAuditResolution_TreeAuditRuleID' ON 'TreeAuditResolution'('TreeAuditRuleID');";

        public const string CREATE_INDEX_TreeAuditResolution_TreeID =
            @"CREATE INDEX 'TreeAuditResolution_TreeID' ON 'TreeAuditResolution'('TreeID');";

        public const string CREATE_TRIGGER_TreeAuditResolution_OnDelete =
@"CREATE TRIGGER TreeAuditResolution_OnDelete 
BEFORE DELETE ON TreeAuditResolution
FOR EACH ROW 
BEGIN 
    INSERT OR REPLACE INTO TreeAuditResolution_Tombstone
        CruiseID,
        TreeID,
        TreeAuditRuleID,
        Resolution,
        Initials
    ) VALUES (
        OLD.CruiseID,
        OLD.TreeID,
        OLD.TreeAuditRuleID,
        OLD.Resolution,
        OLD.Initials
    );
END;";

        public const string CREATE_TOMBSTONE_TABLE_TreeAuditResolution_Tombstone =
@"CREATE TABLE TreeAuditResolution_Tombstone (
    CruiseID TEXT NOT NULL COLLATE NOCASE,
    TreeID TEXT NOT NULL,
    TreeAuditRuleID TEXT NOT NULL,
    Resolution TEXT,
    Initials TEXT NOT NULL
);";
    }
}