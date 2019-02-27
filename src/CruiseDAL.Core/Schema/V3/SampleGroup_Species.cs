using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CruiseDAL.Schema.V3
{
    public partial class DDL
    {
        public const string CREATE_TABLE_SAMPLEGROUP_SPECIES =
            "CREATE TABLE SampleGroup_FiaCode (" +
            "SampleGroup_FiaCode_CN INTEGER PRIMARY KEY AUTOINCREMENT, " +
            "StratumCode TEXT NOT NULL COLLATE NOCASE, " +
            "SampleGroupCode TEXT NOT NULL COLLATE NOCASE, " +
            "Species TEXT NOT NULL, " +
            "LiveDead TEXT DEFAULT ('default') COLLATE NOCASE, " +
            "UNIQUE (StratumCode, SampleGroupCode), " +
            "UNIQUE (StratumCode, SampleGroupCode, Species, LiveDead)," +
            "FOREIGN KEY (StratumCode, SampleGroupCode) REFERENCES SampleGroup (StratumCode, SampleGroupCode) ON DELETE CASCADE ON UPDATE CASCADE," +
            "FOREIGN KEY (Species) REFERENCES Species (Species) " +
            ");";
    }
}
