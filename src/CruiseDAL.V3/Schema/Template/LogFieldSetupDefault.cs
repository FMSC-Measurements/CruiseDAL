using System.Collections.Generic;

namespace CruiseDAL.Schema
{
    public class LogFieldSetupDefaultTableDefinition : ITableDefinition
    {

        public string TableName => "LogFieldSetupDefault";

        public string CreateTable =>
@"CREATE TABLE LogFieldSetupDefault (
    LogFieldSetupDefault_CN INTEGER PRIMARY KEY AUTOINCREMENT,
    StratumDefaultID TEXT COLLATE NOCASE,
    Field TEXT NOT NULL COLLATE NOCASE,
    FieldOrder INTEGER Default 0,
    UNIQUE(Field),
    FOREIGN KEY (Field) REFERENCES LogField (Field)
);";

        public string InitializeTable => null;

        public string CreateTombstoneTable => null;

        public string CreateIndexes => null;

        public IEnumerable<string> CreateTriggers => null;
    }
}