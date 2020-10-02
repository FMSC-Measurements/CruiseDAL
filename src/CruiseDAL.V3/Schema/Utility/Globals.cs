using System.Collections.Generic;
using System.Linq;

namespace CruiseDAL.Schema
{
    public class GlobalsTableDefinition : ITableDefinition
    {
        public string TableName => "Globals";

        public string CreateTable =>
@"CREATE TABLE Globals (
    Block TEXT DEFAULT '' COLLATE NOCASE,
    Key TEXT COLLATE NOCASE,
    Value TEXT,
    UNIQUE (Block, Key)
);";

        // db version is set in the db builder instead of in the table initializer
        public string InitializeTable => null;

        public string CreateTombstoneTable => null;

        public string CreateIndexes => null;

        public IEnumerable<string> CreateTriggers => Enumerable.Empty<string>();
    }
}