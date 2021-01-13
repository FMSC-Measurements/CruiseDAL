using System;
using System.Collections.Generic;

namespace CruiseDAL.Schema
{
    public class TreeFieldHeading : ITableDefinition
    {
        public string TableName => "TreeFieldHeading";

        public string CreateTable =>
@"CREATE TABLE TreeFieldHeading (
    TreeFieldHeading_CN INTEGER PRIMARY KEY AUTOINCREMENT,
    CruiseID TEXT NOT NULL COLLATE NOCASE,
    Field TEXT NOT NULL COLLATE NOCASE,
    Heading TEXT NOT NULL, 

    UNIQUE (CruiseID, Field),

    FOREIGN KEY (CruiseID) REFERENCES Cruise (CruiseID) ON DELETE CASCADE, 
    FOREIGN KEY (Field) REFERENCES TreeField (Field) ON DELETE CASCADE
);";

        public string InitializeTable => null;

        public string CreateTombstoneTable => null;

        public string CreateIndexes => null;

        public IEnumerable<string> CreateTriggers => null;
    }
}
