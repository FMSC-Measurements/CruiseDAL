using System.Collections.Generic;

namespace CruiseDAL.Schema
{
    public class UOMCodesTableDefinition : ITableDefinition
    {
        public string TableName => "UOMCodes";

        public string CreateTable =>
@"CREATE TABLE UOMCodes (
    UOMCodes_CN INTEGER PRIMARY KEY AUTOINCREMENT,
    UOM TEXT NOT NULL COLLATE NOCASE,
    FriendlyName TEXT NOT NULL COLLATE NOCASE,
    UNIQUE (UOM)
);";

        public string InitializeTable =>
@"INSERT INTO UOMCodes (UOM, FriendlyName)
VALUES
    ('01', 'Board feet'),
    ('02', 'Cords'),
    ('03', 'Cubic feet'),
    ('04', 'Piece count'),
    ('05', 'Weight');";

        public string CreateTombstoneTable => null;

        public string CreateIndexes => null;

        public IEnumerable<string> CreateTriggers => null;
    }
}