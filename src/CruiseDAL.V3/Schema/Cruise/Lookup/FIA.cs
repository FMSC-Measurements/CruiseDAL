using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CruiseDAL.Schema
{
    public class FIATableDefinition : ITableDefinition
    {
        public string TableName => "FIA";

        public string CreateTable =>
@"CREATE TABLE FIA (
    FIA_cn INTEGER PRIMARY KEY AUTOINCREMENT,
    FIACode INTEGER NOT NULL,
    CommonName TEXT NOT NULL,

    UNIQUE (FIACode)
);";

        public string InitializeTable => null;

        public string CreateTombstoneTable => null;

        public string CreateIndexes => null;

        public IEnumerable<string> CreateTriggers => Enumerable.Empty<string>();
    }
}
