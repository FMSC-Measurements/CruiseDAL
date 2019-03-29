namespace CruiseDAL.Schema
{
    public partial class DDL
    {

        public const string CREATE_TALBE_SAMPLERSTATE =
            "CREATE TABLE SamplerState ( " +
                "SamplerState_CN INTEGER PRIMARY KEY AUTOINCREMENT, " +
                "StratumCode TEXT NOT NULL COLLATE NOCASE, " +
                "SampleGroupCode TEXT NOT NULL COLLATE NOCASE, " +
                "SampleSelectorType TEXT COLLATE NOCASE, " +
                "SampleSelectorState TEXT, " +

                "UNIQUE (StratumCode, SampleGroupCode), " +

                "FOREIGN KEY (StratumCode, SampleGroupCode) REFERENCES SampleGroup_V3 (StratumCode, SampleGroupCode) ON DELETE CASCADE " +
            ");";
    }

    public partial class Migrations
    {
        public const string MIGRATE_SAMPLERSTATE_FROM_SAMPLEGROUP_FORMAT_STR =
            "INSERT INTO {0}.SamplerState " +
            "SELECT " +
                "sg.SampleGroup_CN AS SamplerState_CN, " +
                "st.Code AS StratumCode, " +
                "sg.Code AS SampleGroupCode, " +
                "sg.SampleSelectorType, " +
                "sg.SampleSelectorState " +
            "FROM {1}.SampleGroup AS sg " +
            "JOIN {1}.Stratum AS st USING (Stratum_CN);";
    }
}