using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CruiseDAL.Schema.V3
{
    public partial class DDL
    {
        public const string CREATE_TABLE_TREEDEFAULTVALUE_TREEAUDITVALUE =
            "CREATE TABLE TreeDefaultValue_TreeAuditValue (" +
            "TreeDefaultValue_TreeAuditValue_CN INTEGER PRIMARY KEY AUTOINCREMENT, " +
            "Species TEXT NOT NULL COLLATE NOCASE, " +
            "LiveDead TEXT NOT NULL COLLATE NOCASE, " +
            "PrimaryProduct TEXT NOT NULL COLLATE NOCASE, " +
            "TreeAuditValueID TEXT NOT NULL, " +
            "FOREIGN KEY (Species, LiveDead, PrimaryProduct) REFERENCES TreeDefaultValue (Species, LiveDead, PrimaryProduct) ON DELETE CASCADE, " +
            "FOREIGN KEY (TreeAuditValueID) REFERENCES TreeAuditValue (TreeAuditValueID) ON DELETE CASCADE, " +
            "FOREIGN KEY (Species) REFERENCES Species (Species) ON UPDATE CASCADE" +
            ");";
    }

    public partial class Updater
    {
        public const string INITIALIZE_TREEDEFAULTVALUE_TREEAUDITVALUE_FROM_TREEDEFAULTVALUETREEAUDITVALUE =
            "INSERT INTO TreeDefaultValue_TreeAuditValue " +
            "SELECT " +
            "tdv.Species, " +
            "tdv.LiveDead, " +
            "tdv.PrimaryProduct, " +
            "tav.TreeAuditValueID " +
            "FROM TreeDefaultValueTreeAuditValue AS tdvtav " +
            "JOIN TreeDefaultValue AS tdv USING (TreeDefaultValue_CN) " +
            "JOIN TreeAuditValue AS tav USING (TreeAuditValue_CN)";
    }
}
