using System.Collections.Generic;
using System.Linq;

namespace CruiseDAL.Schema
{
    public class SpeciesTableDefinition : ITableDefinition
    {
        public string TableName => "Species";

        public string CreateTable =>
@"CREATE TABLE Species (
    Species_cn INTEGER PRIMARY KEY AUTOINCREMENT,
    SpeciesCode TEXT COLLATE NOCASE, 
    CruiseID TEXT NOT NULL COLLATE NOCASE,
    ContractSpecies TEXT, 
    CHECK (length(SpeciesCode) > 0),
    
    UNIQUE (SpeciesCode, CruiseID)
); ";

        public string InitializeTable => null;

        public string CreateTombstoneTable => null;

        public string CreateIndexes => null;

        public IEnumerable<string> CreateTriggers => Enumerable.Empty<string>();
    }

    public partial class Migrations
    {
        public const string MIGRATE_SPECIESCODE_FROM_TREEDEFAULTVALUE =
@"INSERT INTO {0}.Species ( 
    SpeciesCode,
    CruiseID,
    ContractSpecies
) 
SELECT Species,  '{3}', ContractSpecies FROM {1}.TreeDefaultValue GROUP BY Species;";
    }
}