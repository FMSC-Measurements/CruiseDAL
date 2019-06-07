namespace CruiseDAL.Schema
{
    public partial class DDL
    {
        public const string CREATE_TABLE_TREEAUDITRULE =
@"CREATE TABLE TreeAuditRule (
    TreeAuditRule_CN INTEGER PRIMARY KEY AUTOINCREMENT,
    TreeAuditRuleID TEXT NOT NULL,
    Field TEXT NOT NULL COLLATE NOCASE,
    Min REAL,
    Max REAL,
    UNIQUE (TreeAuditRuleID),
    CHECK ((Min IS NULL OR Max IS NULL) OR (Min < Max)),
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
    Field,
    Min,
    Max
)
SELECT
    TreeAuditValue_CN,
    'migrateTreeAuditValue-' || TreeAuditValue_CN,
    Field,
    nullif(Min,0) AS Min,
    nullif(Max,0) AS Max
FROM {1}.TreeAuditValue;";
    }
}