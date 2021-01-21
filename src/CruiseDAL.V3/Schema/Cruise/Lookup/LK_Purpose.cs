using System;
using System.Collections.Generic;

namespace CruiseDAL.Schema.Cruise.Lookup
{
    public class LK_Purpose : ITableDefinition
    {
        public string TableName => "LK_Purpose";

        public string CreateTable =>
@"CREATE TABLE LK_Purpose (
    LK_Purpose_CN INTEGER PRIMARY KEY AUTOINCREMENT,
    Purpose TEXT NOT NULL COLLATE NOCASE,
    ShortCode TEXT NOT NULL COLLATE NOCASE, -- currently short code is only used in V2 file names
    UNIQUE (Purpose)
);";

        public string InitializeTable =>
@"INSERT INTO LK_Purpose (
    Purpose,
    ShortCode
) VALUES 
    ('Timber Sale', 'TS'),
    ('Check Cruise', 'Check'), 
    ('Right of Way', 'RoW'),
    ('Recon', 'Recon'),
    ('Other', 'Other');";

        public string CreateTombstoneTable => null;

        public string CreateIndexes => null;

        public IEnumerable<string> CreateTriggers => null;
    }
}