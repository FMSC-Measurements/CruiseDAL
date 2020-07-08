namespace CruiseDAL.Schema
{
    public partial class DDL
    {
        public const string CREATE_TABLE_SAMPLEGROUPSTATSTREEDEFAULTVALUE =
            "CREATE TABLE SampleGroupStatsTreeDefaultValue( " +
                "TreeDefaultValue_CN INTEGER NOT NULL, " +
                "SampleGroupStats_CN INTEGER NOT NULL, " +
                "UNIQUE (TreeDefaultValue_CN, SampleGroupStats_CN)," +
                "FOREIGN KEY (TreeDefaultValue_CN) REFERENCES TreeDefaultValue (TreeDefaultValue_CN) ON DELETE CASCADE," +
                "FOREIGN KEY (SampleGroupStats_CN) REFERENCES SampleGroupStats (SampleGroupStats_CN) ON DELETE CASCADE" +
            ");";

        public const string CREATE_INDEX_SampleGroupStatsTreeDefaultValue_SampleGroupStats_CN =
            @"CREATE INDEX 'SampleGroupStatsTreeDefaultValue_SampleGroupStats_CN' ON 'SampleGroupStatsTreeDefaultValue'('SampleGroupStats_CN');";
    }
}