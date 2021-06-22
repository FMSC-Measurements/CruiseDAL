using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CruiseDAL.Schema
{
    public class StratumTemplateTreeFieldSetupTableDefinition : ITableDefinition
    {
        public string TableName => "StratumTemplateTreeFieldSetup";

        public string CreateTable =>
@"CREATE TABLE StratumTemplateTreeFieldSetup (
    StratumTemplateTreeFieldSetup_CN INTEGER PRIMARY KEY AUTOINCREMENT,
    StratumTemplateName TEXT NOT NULL COLLATE NOCASE,
    CruiseID TEXT NOT NULL COLLATE NOCASE,
    Field TEXT NOT NULL COLLATE NOCASE,
    FieldOrder INTEGER Default 0,
    IsHidden BOOLEAN Default 0,
    IsLocked BOOLEAN Default 0,
    -- value type determined by TreeField.DbType
    DefaultValueInt INTEGER, 
    DefaultValueReal REAL,
    DefaultValueBool BOOLEAN,
    DefaultValueText TEXT, 

    UNIQUE (Field, StratumTemplateName, CruiseID),
    
    FOREIGN KEY (StratumTemplateName, CruiseID) REFERENCES StratumTemplate (StratumTemplateName, CruiseID) ON DELETE CASCADE ON UPDATE CASCADE,
    FOREIGN KEY (Field) REFERENCES TreeField (Field)
);";

        public string InitializeTable => null;

        public string CreateTombstoneTable => null;

        public string CreateIndexes => 
@"CREATE INDEX NIX_StratumTemplateTreeFieldSetup_StratumTemplateName_CruiseID ON StratumTemplateTreeFieldSetup (StratumTemplateName, CruiseID);";

        public IEnumerable<string> CreateTriggers => null;
    }
}
