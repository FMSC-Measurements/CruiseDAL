using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CruiseDAL.Schema.V3
{
    public partial class DDL
    {
        public const string CREATE_TABLE_CUTTINGUNIT_STRATUM =
            "CREATE TABLE CuttingUnit_Stratum (" +
            "CuttingUnit_Stratum_CN INTEGER PRIMARY KEY AUTOINCREMENT, " +
            "CuttingUnitCode TEXT NOT NULL COLLATE NOCASE, " +
            "StratumCode TEXT NOT NULL COLLATE NOCASE, " +
            "StratumArea REAL, " +
            "FOREIGN KEY (CuttingUnitCode) REFERENCES CuttingUnit (Code) ON DELETE CASCADE ON UPDATE CASCADE, " +
            "FOREIGN KEY (StratumCode) REFERENCES Stratum (Code) ON DELETE CASCADE ON UPDATE CASCADE, " +
            "UNIQUE (CuttingUnitCode, StratumCode));";
    }

    public partial class Updater
    {
        public const string INITIALIZE_TABLE_CUTTINGUNIT_STRATUM_FROM_CUTTINGUNITSTRATUM =
            "INSERT INTO CuttingUnitStratum " +
            "(CuttingUnitCode, StratumCode, StratumArea)" +
            "SELECT cu.Code, st.Code, cust.StratumArea " +
            "FROM CuttingUnitStratum AS cust " +
            "JOIN CuttingUnit AS cu USING (CuttingUnit_CN) " +
            "JOIN Stratum AS st USING (Stratum_CN);";
    }

}
