using System.Collections.Generic;

namespace CruiseDAL.Schema
{
    public class LK_Region : ITableDefinition
    {
        public string TableName => "LK_Region";

        public string CreateTable => GetCreateTable(TableName);


        public string GetCreateTable(string tableName)
        {
return $@"CREATE TABLE {tableName} (
    LK_Region_CN INTEGER PRIMARY KEY AUTOINCREMENT,
    Region TEXT NOT NULL COLLATE NOCASE,
    FriendlyName TEXT NOT NULL COLLATE NOCASE,
    UNIQUE (Region)
);";
        }

        public string InitializeTable =>
@"INSERT INTO LK_Region (Region, FriendlyName)
VALUES
    ('01', 'Northern'),
    ('02', 'Rocky Mountain'),
    ('03', 'Southwestern'),
    ('04', 'Intermountain'),
    ('05', 'Pacific Southwest'),
    ('06', 'Pacific Northwest'),
    ('08', 'Southern'),
    ('09', 'Eastern'),
    ('10', 'Alaska'),
    ('07', 'O&C BLM'),
    ('11', 'DOD');";

        public string CreateTombstoneTable => null;

        public string CreateIndexes => null;

        public IEnumerable<string> CreateTriggers => null;
    }
}