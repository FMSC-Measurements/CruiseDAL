using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CruiseDAL.Schema.V3
{
    public partial class DLL
    {
        public const string CREATE_TABLE_TREE_V3 =
            "CREATE TABLE Tree_V3 ( " +
                "Tree_CN INTEGER PRIMARY KEY AUTOINCREMENT, " +
                "TreeID TEXT NOT NULL , " +
                "CuttingUnitCode TEXT NOT NULL COLLATE NOCASE, " +
                "StratumCode TEXT NOT NULL COLLATE NOCASE, " +
                "SampleGroupCode TEXT NOT NULL COLLATE NOCASE, " +
                "Species TEXT COLLATE NOCASE, " +
                "LiveDead TEXT COLLATE NOCASE, " +
                "PlotNumber INTEGER, " +
                "TreeNumber INTEGER NOT NULL, " +
                "CountOrMeasure TEXT DEFAULT 'M' COLLATE NOCASE," +
                "Initials TEXT, " +
                
                "CreatedBy TEXT DEFAULT 'none', " +
                "CreatedDate DateTime DEFAULT (datetime('now', 'localtime')) , " +
                "ModifiedBy TEXT, " +
                "ModifiedDate DateTime , " +
                "RowVersion INTEGER DEFAULT 0, " +
                "UNIQUE (TreeID), " +
                "FOREIGN KEY (CuttingUnitCode) REFERENCES CuttingUnit (Code) ON DELETE CASCADE ON UPDATE CASCADE, " +
                "FOREIGN KEY (StratumCode) REFERENCES Stratum (Code) " +
                "FOREIGN KEY (SampleGroupCode, StratumCode) REFERENCES SampleGroup_V3 ON DELETE CASCADE ON UPDATE CASCADE," +
                "FOREIGN KEY (Species, LiveDead, SampleGroupCode, StratumCode) REFERENCES SampleGroup_Species " +
                "FOREIGN KEY (Species) REFERENCES Species (Species) " +
            ")";
    }

    public partial class Updater
    {
        public const string INITIALIZE_TREE_V3_FROM_TREE =
            "INSERT INTO Tree_V3 " +
            "SELECT " +
            "Tree_CN, " +
            "Tree_GUID AS TreeID, " +
            "cu.Code AS CuttingUnitCode, " +
            "st.Code AS StratumCode, " +
            "sg.Code AS SampleGroupCode, " +
            "tdv.Species, " +
            "tdv.LiveDead, " +
            "p.PlotNumber, " +
            "TreeNumber, " +
            "CountOrMeasure, " +
            "Initials, " +
            "CreatedBy, " +
            "CreatedDate, " +
            "ModifiedDate, " +
            "RowVersion " +
            "FROM Tree as t " +
            "JOIN CuttingUnit AS cu USING (CuttingUnit_CN) " +
            "JOIN Stratum AS st USING (Stratum_CN) " +
            "JOIN SampleGroup AS sg USING (SampleGroup_CN) " +
            "LEFT JOIN TreeDefaultValue AS tdv USING (TreeDefaultValue_CN) " +
            "LEFT JOIN Plot AS p USING (Plot_CN);";
    }
}
