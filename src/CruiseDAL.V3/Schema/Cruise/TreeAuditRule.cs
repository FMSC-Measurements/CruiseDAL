namespace CruiseDAL.Schema
{
    public partial class DDL
    {
        public const string CREATE_TABLE_TREEAUDITRULE =
@"CREATE TABLE TreeAuditRule (
    TreeAuditRule_CN INTEGER PRIMARY KEY AUTOINCREMENT,
    TreeAuditRuleID TEXT NOT NULL,
    CruiseID TEXT NOT NULL COLLATE NOCASE,
    Field TEXT NOT NULL COLLATE NOCASE,
    Min REAL,
    Max REAL,
    Desctiption TEXT,
    UNIQUE (TreeAuditRuleID),

    CHECK ((Min IS NULL OR Max IS NULL) OR (Min < Max)),
    CHECK (TreeAuditRuleID LIKE '________-____-____-____-____________'),

    FOREIGN KEY (CruiseID) REFERENCES Cruise (CruiseID) ON DELETE CASCADE,
    FOREIGN KEY (Field) REFERENCES TreeField (Field)
);";

        public const string CREATE_INDEX_TreeAuditRule_Field =
            @"CREATE INDEX 'TreeAuditRule_Field' ON 'TreeAuditRule'('Field');";
    }

    public partial class Migrations
    {
        public const string MIGRATE_TREEAUDITRULE_FROM_TREEAUDITVALUE_FORMAT_STR =
@"INSERT INTO {0}.TreeAuditRule (
    TreeAuditRule_CN,
    TreeAuditRuleID,
    CruiseID, 
    Field,
    Min,
    Max
)
SELECT
    TreeAuditValue_CN,
    (hex( randomblob(4)) || '-' || hex( randomblob(2)) 
        || '-' || '4' || substr(hex(randomblob(2)), 2) || '-' 
        || substr('AB89', 1 + (abs(random()) % 4), 1) || 
        substr(hex(randomblob(2)), 2) || '-' || hex(randomblob(6))),
    '{3}',
    Field,
    nullif(Min,0) AS Min,
    nullif(Max,0) AS Max
FROM {1}.TreeAuditValue;";
    }
}