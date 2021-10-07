using System.Collections.Generic;
using System.Linq;

namespace CruiseDAL.Schema
{
    public class MessageLogTableDefinition : ITableDefinition
    {
        public string TableName => "MessageLog";

        // instead of following the convention used by other tables
        // the primary key for MessageLog is Message_CN instead of MessageLog_CN
        // this is for compatibility with the older cruise schema

        public string CreateTable => GetCreateTable(TableName);


        public string GetCreateTable(string tableName)
        {
return $@"CREATE TABLE {tableName} (
    Message_CN INTEGER PRIMARY KEY AUTOINCREMENT,
    Program TEXT COLLATE NOCASE,
    Message TEXT,
    Date TEXT DEFAULT (date('now', 'localtime')),
    Time TEXT DEFAULT (time('now', 'localtime')),
    Level TEXT COLLATE NOCASE DEFAULT 'N'
);";
        }

        public string InitializeTable => null;

        public string CreateTombstoneTable => null;

        public string CreateIndexes => null;

        public IEnumerable<string> CreateTriggers => Enumerable.Empty<string>();

    }

}