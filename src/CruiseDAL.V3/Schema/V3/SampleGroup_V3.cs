namespace CruiseDAL.Schema
{
    public partial class DDL
    {
        public const string CREATE_TABLE_SAMPLEGROUP_V3 =
            "CREATE TABLE SampleGroup_V3 (" +
                "SampleGroup_CN INTEGER PRIMARY KEY AUTOINCREMENT, " +
                "SampleGroupCode TEXT NOT NULL COLLATE NOCASE, " +
                "StratumCode TEXT NOT NULL COLLATE NOCASE, " +
                "CutLeave TEXT DEFAULT 'C' COLLATE NOCASE, " +
                "UOM TEXT DEFAULT '' COLLATE NOCASE, " +
                "PrimaryProduct TEXT DEFAULT '' COLLATE NOCASE, " +
                "SecondaryProduct TEXT COLLATE NOCASE, " +
                "BiomassProduct TEXT, " +
                "DefaultLiveDead TEXT DEFAULT 'L' COLLATE NOCASE, " +
                "SamplingFrequency INTEGER Default 0, " +
                "InsuranceFrequency INTEGER Default 0, " +
                "KZ INTEGER Default 0, " +
                "BigBAF INTEGER Default 0, " +
                "TallyBySubPop BOOLEAN DEFAULT 0, " +
                "TallyMethod TEXT COLLATE NOCASE, " +
                "Description TEXT, " +
                "MinKPI INTEGER Default 0, " +
                "MaxKPI INTEGER Default 0, " +
                "SmallFPS REAL DEFAULT 0.0," +
                "CreatedBy TEXT DEFAULT '', " +
                "CreatedDate DATETIME DEFAULT (datetime('now', 'localtime')), " +
                "ModifiedBy TEXT, " +
                "ModifiedDate DATETIME, " +
                "RowVersion INTEGER DEFAULT 0, " +

                "UNIQUE (StratumCode, SampleGroupCode), " +

                "FOREIGN KEY (StratumCode) REFERENCES Stratum (Code) ON DELETE CASCADE ON UPDATE CASCADE" +
            ");";

        public const string CREATE_TRIGGER_SAMPLEGROUP_V3_ONUPDATE =
            "CREATE TRIGGER SampleGroup_V3_OnUpdate " +
            "AFTER UPDATE OF " +
                "SampleGroupCode, " +
                "StratumCode, " +
                "CutLeave, " +
                "UOM, " +
                "PrimaryProduct, " +
                "SecondaryProduct, " +
                "BiomassProduct, " +
                "DefaultLiveDead, " +
                "SamplingFrequency, " +
                "InsuranceFrequency, " +
                "KZ, " +
                "BigBAF, " +
                "TallyBySubPop," +
                "TallyMethod, " +
                "Description, " +
                "MinKPI, " +
                "MaxKPI, " +
                "SmallFPS " +
            "ON SampleGroup_V3 " +
            "FOR EACH ROW " +
            "BEGIN " +
                "UPDATE SampleGroup_V3 SET ModifiedDate = datetime('now', 'localtime') WHERE SampleGroup_CN = old.SampleGroup_CN;" +
                "UPDATE SampleGroup_V3 SET RowVersion = old.RowVersion WHERE SampleGroup_CN = old.SampleGroup_CN; " +
            "END;";
    }

    public partial class Migrations
    {
        public const string MIGRATE_SAMPLEGROUP_V3_FROM_SAMPLEGROUP =
            "INSERT INTO {0}.SampleGroup_V3 ( " +
                    "SampleGroup_CN, " +
                    "SampleGroupCode, " +
                    "StratumCode, " +
                    "CutLeave, " +
                    "UOM, " +
                    "PrimaryProduct, " +
                    "SecondaryProduct, " +
                    "BiomassProduct, " +
                    "DefaultLiveDead, " +
                    "SamplingFrequency, " +
                    "InsuranceFrequency, " +
                    "KZ, " +
                    "BigBAF, " +
                    "TallyBySubPop," +
                    "TallyMethod, " +
                    "Description, " +
                    "MinKPI, " +
                    "MaxKPI, " +
                    "SmallFPS, " +
                    "CreatedBy, " +
                    "CreatedDate, " +
                    "ModifiedBy, " +
                    "ModifiedDate, " +
                    "RowVersion " +
                ") " +
                "SELECT " +
                    "sg.SampleGroup_CN, " +
                    "sg.Code AS SampleGroupCode, " +
                    "st.Code AS StratumCode, " +
                    "sg.CutLeave, " +
                    "sg.UOM, " +
                    "sg.PrimaryProduct, " +
                    "sg.SecondaryProduct, " +
                    "sg.BiomassProduct, " +
                    "sg.DefaultLiveDead, " +
                    "sg.SamplingFrequency, " +
                    "sg.InsuranceFrequency, " +
                    "sg.KZ, " +
                    "sg.BigBAF, " +
                    "(EXISTS (" +
                            "SELECT * FROM {1}.CountTree AS ct " +
                            "WHERE ct.SampleGroup_CN = sg.SampleGroup_CN AND ct.TreeDefaultValue_CN NOT NULL" +
                        ")) AS TallyBySubPop, " +
                    "sg.TallyMethod, " +
                    "sg.Description, " +
                    "sg.MinKPI, " +
                    "sg.MaxKPI, " +
                    "sg.SmallFPS, " +
                    "sg.CreatedBy, " +
                    "sg.CreatedDate, " +
                    "sg.ModifiedBy, " +
                    "sg.ModifiedDate, " +
                    "sg.RowVersion " +
                "FROM {1}.SampleGroup AS sg " +
                "JOIN {1}.Stratum AS st USING (Stratum_CN);";
    }

    //public partial class Updater
    //{
    //    public const string INITIALIZE_SAMPLEGROUP_V3_FROM_SAMPLEGROUP =
    //        "INSERT INTO SampleGroup_V3 " +
    //        "SELECT " +
    //        "sg.SampleGroup_CN, " +
    //        "sg.Code AS SampleGroupCode, " +
    //        "st.Code AS StratumCode, " +
    //        "sg.CutLeave, " +
    //        "sg.UOM, " +
    //        "sg.PrimaryProduct, " +
    //        "sg.SecondaryProduct, " +
    //        "sg.BiomassProduct, " +
    //        "sg.DefaultLiveDead, " +
    //        "sg.SamplingFrequency, " +
    //        "sg.InsuranceFrequency, " +
    //        "sg.KZ, " +
    //        "sg.BigBAF, " +
    //        "sg.TallyMethod, " +
    //        "sg.Description, " +
    //        "sg.SampleSelectorType, " +
    //        "sg.MinKPI, " +
    //        "sg.MaxKPI, " +
    //        "sg.SmallFPS, " +
    //        "sg.CreatedBy, " +
    //        "sg.CreatedDate, " +
    //        "sg.ModifiedBy, " +
    //        "sg.ModifiedDate, " +
    //        "sg.RowVersion " +
    //        "FROM SampleGroup AS sg " +
    //        "JOIN Stratum AS st USING (Stratum_CN);";
    //}
}