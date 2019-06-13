namespace CruiseDAL.Schema
{
    public partial class DDL
    {
        // HACK decided to call the table SpeciesCode so that we wouldn't have a table with the same name as a common collumn name
        // Idealy we would have a table called Species and a field called SpeciesCode but because the field came first
        // I had to make do. At one point I had both the field and the table named Species but that can cause issues because .net doesn't allow a Property with the same name as the class
        public const string CREATE_TABLE_SpeciesCode =
            "CREATE TABLE SpeciesCode (" +
                "Species TEXT PRIMARY KEY COLLATE NOCASE CHECK (length(Species) > 0)" +
            "); ";

        public const string INITIALIZE_TABLE_SpeciesCode =
            ";";//"INSERT INTO Species (Species) VALUES ('');";
    }

    public partial class Migrations
    {
        public const string MIGRATE_SPECIESCODE_FROM_TREEDEFAULTVALUE =
            "INSERT INTO {0}.SpeciesCode ( " +
                "Species" +
            ") " +
            "SELECT DISTINCT Species FROM {1}.TreeDefaultValue;";
    }

    //public partial class Updater
    //{
    //    public const string INITIALIZE_SPECIES_FROM_TREEDEFAULTVALUE =
    //        "INSERT INTO Species " +
    //        "SELECT DISTINCT Species  FROM TreeDefaultValue;";
    //}
}