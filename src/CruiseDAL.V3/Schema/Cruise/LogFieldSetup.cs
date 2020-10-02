using System.Collections.Generic;
using System.Linq;

namespace CruiseDAL.Schema
{
    public class LogFieldSetupTableDefinition : ITableDefinition
    {
        public string TableName => "LogFieldSetup";

        public string CreateTable =>
@"CREATE TABLE LogFieldSetup (
    StratumCode TEXT NOT NULL,
    CruiseID TEXT NOT NULL COLLATE NOCASE,
    Field TEXT NOT NULL,
    FieldOrder INTEGER Default 0,
    Heading TEXT,
    Width REAL Default 0.0,

    UNIQUE (CruiseID, StratumCode, Field),

    FOREIGN KEY (StratumCode, CruiseID) REFERENCES Stratum (StratumCode, CruiseID) ON DELETE CASCADE ON UPDATE CASCADE,
    FOREIGN KEY (Field) REFERENCES LogField (Field)
);";

        public string InitializeTable => null;

        public string CreateTombstoneTable => null;// TODO

        public string CreateIndexes =>
@"CREATE INDEX 'LogFieldSetup_Field' ON 'LogFieldSetup'('Field' COLLATE NOCASE);

CREATE INDEX 'LogFieldSetup_StratumCode_CruiseID' ON 'LogFieldSetup'('StratumCode', 'CruiseID');";

        public IEnumerable<string> CreateTriggers => Enumerable.Empty<string>();
    }

    public partial class Migrations
    {
        public const string MIGRATE_LOGFIELDSETUP_FROM_LOGFIELDSETUP =
            "INSERT INTO {0}.LogFieldSetup ( " +
                    "StratumCode, " +
                    "CruiseID, " +
                    "Field, " +
                    "FieldOrder, " +
                    "Heading, " +
                    "Width " +
                ") " +
                "SELECT " +
                    "st.Code AS StratumCode, " +
                    "'{3}'," +
                    "lfs.Field, " +
                    "lfs.FieldOrder, " +
                    "lfs.Heading, " +
                    "lfs.Width " +
                "FROM {1}.LogFieldSetup AS lfs " +
                "JOIN {1}.Stratum AS st USING (Stratum_CN)" +
                "JOIN {0}.LogField USING (Field);"; // join with LogField so we only get valid fields
    }

    //public partial class Updater
    //{
    //    public const string INITIALIZE_LOGFIELDSETUP_V3_FROM_LOGFIELDSETUP =
    //        "INSERT INTO LogFieldSetup_V3 " +
    //        "SELECT " +
    //        "st.Code AS StratumCode, " +
    //        "Field, " +
    //        "FieldOrder, " +
    //        "Heading, " +
    //        "Width " +
    //        "FROM LogFieldSetup " +
    //        "JOIN Stratum AS st USING (Stratum_CN);";
    //}
}