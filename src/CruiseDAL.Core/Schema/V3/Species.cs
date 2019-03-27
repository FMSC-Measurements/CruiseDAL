namespace CruiseDAL.Schema
{
    public partial class DDL
    {
        public static readonly string[] SPECIES = new string[]
        {
            CREATE_TABLE_SPECIES,
            INITIALIZE_TABLE_SPECIES,
        };

        public const string CREATE_TABLE_SPECIES =
            "CREATE TABLE Species (" +
                "Species PRIMARY KEY COLLATE NOCASE CHECK (length(Species) > 0)" +
            "); ";

        public const string INITIALIZE_TABLE_SPECIES =
            ";";//"INSERT INTO Species (Species) VALUES ('');";
    }

    public partial class Migrations
    {
        public const string MIGRATE_SPECIES_FROM_TREEDEFAULTVALUE =
            "INSERT INTO {0}.Species ( " +
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