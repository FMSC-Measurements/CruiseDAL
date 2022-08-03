using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CruiseDAL.Schema
{
    public class ValueEquationTableDefinition : ITableDefinition
    {
        public string TableName => "ValueEquation";

        public string CreateTable => GetCreateTable(TableName);

        public string GetCreateTable(string tableName)
        {
return $@"CREATE TABLE {tableName} (
    CruiseID TEXT NOT NULL COLLATE NOCASE,
    Species TEXT NOT NULL,
    PrimaryProduct TEXT NOT NULL,
    ValueEquationNumber TEXT,
    Grade TEXT,
    Coefficient1 REAL Default 0.0,
    Coefficient2 REAL Default 0.0,
    Coefficient3 REAL Default 0.0,
    Coefficient4 REAL Default 0.0,
    Coefficient5 REAL Default 0.0,
    Coefficient6 REAL Default 0.0,

    CreatedBy TEXT DEFAULT 'none',
    Created_TS DATETIME DEFAULT (CURRENT_TIMESTAMP),
    ModifiedBy TEXT,
    Modified_TS DATETIME,

    UNIQUE (CruiseID, Species, PrimaryProduct, ValueEquationNumber),

    FOREIGN KEY (CruiseID) REFERENCES Cruise (CruiseID) ON DELETE CASCADE
);";
        }

        public string InitializeTable => null;

        public string CreateTombstoneTable => null;

        public string CreateIndexes => null;

        public IEnumerable<string> CreateTriggers => null;
    }
}
