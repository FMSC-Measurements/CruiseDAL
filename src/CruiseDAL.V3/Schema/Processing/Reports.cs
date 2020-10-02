using System.Collections.Generic;
using System.Linq;

namespace CruiseDAL.Schema
{
    public class ReportsTableDefinition : ITableDefinition
    {
        public string TableName => "Reports";

        public string CreateTable =>
@"CREATE TABLE Reports( 
    ReportID TEXT NOT NULL COLLATE NOCASE,
    CruiseID TEXT NOT NULL COLLATE NOCASE,
    Selected BOOLEAN Default 0,
    Title TEXT,

    FOREIGN KEY (CruiseID) REFERENCES Cruise (CruiseID) ON DELETE CASCADE,

    UNIQUE (ReportID)
);";

        public string InitializeTable => null;

        public string CreateTombstoneTable => null;

        public string CreateIndexes => null;

        public IEnumerable<string> CreateTriggers => Enumerable.Empty<string>();
    }

}