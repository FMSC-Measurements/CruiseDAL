namespace CruiseDAL.Schema
{
    public partial class DDL
    {
        public const string CREATE_TALBE_SAMPLERSTATE =
            "CREATE TABLE SamplerState ( " +
                "SamplerState_CN INTEGER PRIMARY KEY AUTOINCREMENT, " +
                "DeviceID TEXT NOT NULL COLLATE NOCASE, " +
                "CruiseID TEXT NOT NULL COLLATE NOCASE, " +
                "StratumCode TEXT NOT NULL COLLATE NOCASE, " +
                "SampleGroupCode TEXT NOT NULL COLLATE NOCASE, " +
                "SampleSelectorType TEXT COLLATE NOCASE, " +
                "BlockState TEXT, " +
                "SystematicIndex INTEGER DEFAULT 0, " +
                "Counter INTEGER DEFAULT 0, " +
                "InsuranceIndex DEFAULT -1," +
                "InsuranceCounter DEFAULT -1, " +
                "ModifiedDate DateTime, " +

                "UNIQUE (CruiseID, DeviceID, StratumCode, SampleGroupCode), " +

                "FOREIGN KEY (DeviceID) REFERENCES Device (DeviceID) ON DELETE CASCADE, " +
                "FOREIGN KEY (StratumCode, SampleGroupCode, CruiseID) REFERENCES SampleGroup (StratumCode, SampleGroupCode, CruiseID) ON DELETE CASCADE ON UPDATE CASCADE " +
            ");";

        public const string CREATE_TRIGGER_SAMPLERSTATE_ONUPDATE =
            "CREATE TRIGGER SamplerState_OnUpdate " +
            "AFTER UPDATE OF " +
                "BlockState, " +
                "Counter, " +
                "InsuranceCounter " +
            "ON SamplerState " +
            "FOR EACH ROW " +
            "BEGIN " +
                "UPDATE SamplerState SET ModifiedDate = datetime('now', 'localtime') WHERE SamplerState_CN = old.SamplerState_CN; " +
            "END; ";
    }

    public partial class Migrations
    {
        //public const string MIGRATE_SAMPLERSTATE_FROM_SAMPLEGROUP_FORMAT_STR =
        //    "INSERT INTO {0}.SamplerState " +
        //    "SELECT " +
        //        "sg.SampleGroup_CN AS SamplerState_CN, " +
        //        "st.Code AS StratumCode, " +
        //        "sg.Code AS SampleGroupCode, " +
        //        "sg.SampleSelectorType, " +
        //        "sg.SampleSelectorState " +
        //    "FROM {1}.SampleGroup AS sg " +
        //    "JOIN {1}.Stratum AS st USING (Stratum_CN);";
    }
}