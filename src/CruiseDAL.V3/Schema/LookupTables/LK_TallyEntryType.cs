using System.Collections.Generic;

namespace CruiseDAL.Schema
{
    public class LK_TallyEntryType : ITableDefinition
    {
        public string TableName => "LK_TallyEntryType";

        public string CreateTable => GetCreateTable(TableName);

        public string GetCreateTable(string tableName)
        {
return $@"CREATE TABLE {tableName} (
    LK_TallyEntryType_CN  INTEGER PRIMARY KEY AUTOINCREMENT,
    EntryType TEXT NOT NULL COLLATE NOCASE,

    UNIQUE (EntryType)
);
";
        }

        public string InitializeTable =>
@"INSERT INTO LK_TallyEntryType (
    EntryType
) VALUES
    ('tally'),
    ('utility'),
    ('treecount_edit'),
    ('clicker'),
    ('manual_tree')
;
";

        public string CreateTombstoneTable => null;

        public string CreateIndexes => null;

        public IEnumerable<string> CreateTriggers => null;
    }
}