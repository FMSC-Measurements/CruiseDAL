namespace CruiseDAL.Schema
{
    public partial class DDL
    {
        public const string CREATE_TABLE_Species =
@"CREATE TABLE Species (
    Species_cn INTEGER PRIMARY KEY AUTOINCREMENT,
    SpeciesCode TEXT COLLATE NOCASE, 
    CruiseID TEXT NOT NULL COLLATE NOCASE,
    ContractSpecies TEXT, 
    CHECK (length(SpeciesCode) > 0),
    
    UNIQUE (SpeciesCode, CruiseID)
); ";
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