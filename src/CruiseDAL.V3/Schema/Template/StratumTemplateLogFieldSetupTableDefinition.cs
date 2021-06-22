using System.Collections.Generic;

namespace CruiseDAL.Schema
{
    public class StratumTemplateLogFieldSetupTableDefinition : ITableDefinition
    {
        public string TableName => "StratumTemplateLogFieldSetup";

        public string CreateTable =>
@"CREATE TABLE StratumTemplateLogFieldSetup (
    StratumTemplateLogFieldSetup_CN INTEGER PRIMARY KEY AUTOINCREMENT,
    StratumTemplateName TEXT NOT NULL COLLATE NOCASE,
    CruiseID TEXT NOT NULL COLLATE NOCASE,
    Field TEXT NOT NULL COLLATE NOCASE,
    FieldOrder INTEGER Default 0,

    UNIQUE (StratumTemplateName, CruiseID, Field),    

    FOREIGN KEY (StratumTemplateName, CruiseID) REFERENCES StratumTemplate (StratumTemplateName, CruiseID) ON DELETE CASCADE ON UPDATE CASCADE,
    FOREIGN KEY (Field) REFERENCES LogField (Field)
);";

        public string InitializeTable => null;

        public string CreateTombstoneTable => null;

        public string CreateIndexes => 
@"CREATE INDEX NIX_StratumTemplateLogFileSetup_StratumTemplateName_CruiseID ON StratumTemplateLogFieldSetup
(StratumTemplateName, CruiseID);";

        public IEnumerable<string> CreateTriggers => null;
    }
}