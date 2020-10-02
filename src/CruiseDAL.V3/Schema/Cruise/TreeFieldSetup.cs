using System.Collections.Generic;

namespace CruiseDAL.Schema
{
    public class TreeFieldSetupTableDefinition : ITableDefinition
    {
        public string TableName => "TreeFieldSetup";

        public string CreateTable =>
@"CREATE TABLE TreeFieldSetup (
    CruiseID TEXT NOT NULL COLLATE NOCASE,
    StratumCode TEXT NOT NULL COLLATE NOCASE,
    Field TEXT NOT NULL COLLATE NOCASE,
    FieldOrder INTEGER Default 0,
    Heading TEXT,
    Width REAL Default 0.0,
    UNIQUE(StratumCode, Field, CruiseID),
    FOREIGN KEY (StratumCode, CruiseID) REFERENCES Stratum (StratumCode, CruiseID) ON DELETE CASCADE ON UPDATE CASCADE,
    FOREIGN KEY (Field) REFERENCES TreeField (Field)
);";

        public string InitializeTable => null;

        public string CreateTombstoneTable =>
@"CREATE TABLE TreeFieldSetup_Tombstone (
    CruiseID TEXT NOT NULL COLLATE NOCASE,
    StratumCode TEXT NOT NULL COLLATE NOCASE,
    Field TEXT NOT NULL COLLATE NOCASE,
    FieldOrder INTEGER,
    Heading TEXT,
    Width REAL
);";

        public string CreateIndexes =>
@"CREATE INDEX TreeFieldSetup_Field ON TreeFieldSetup (Field);

CREATE INDEX TreeFieldSetup_StratumCode_CruiseID ON TreeFieldSetup (StratumCode, CruiseID);";

        public IEnumerable<string> CreateTriggers => new[] { CREATE_TRIGGER_TreeFieldSetup_OnDelete };

        public const string CREATE_TRIGGER_TreeFieldSetup_OnDelete =
@"CREATE TRIGGER TreeFieldSetup_OnDelete
BEFORE DELETE ON TreeFieldSetup
BEGIN
    INSERT OR REPLACE INTO TreeFieldSetup (
        CruiseID,
        StratumCode,
        Field,
        FieldOrder,
        Heading,
        Width
    ) VALUES (
        OLD.CruiseID,
        OLD.StratumCode,
        OLD.Field,
        OLD.FieldOrder,
        OLD.Heading,
        OLD.Width
    );
END;";

    }

    public partial class Migrations
    {
        public const string MIGRATE_TREEFIELDSETUP_FROM_TREEFIELDSETUP_FORMAT_STR =
            "INSERT INTO {0}.TreeFieldSetup ( " +
                    "CruiseID," +
                    "StratumCode, " +
                    "Field, " +
                    "FieldOrder, " +
                    "Heading, " +
                    "Width " +
                ") " +
                "SELECT " +
                    "'{3}', " +
                    "st.Code AS StratumCode, " +
                    "tfs.Field, " +
                    "tfs.FieldOrder, " +
                    "tfs.Heading, " +
                    "tfs.Width " +
                "FROM {1}.TreeFieldSetup AS tfs " +
                "JOIN {1}.Stratum AS st USING (Stratum_CN)" +
                "JOIN {0}.TreeField AS tf USING (Field);";
    }
}