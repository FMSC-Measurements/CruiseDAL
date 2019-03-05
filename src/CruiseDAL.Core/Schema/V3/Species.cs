using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CruiseDAL.Schema.V3
{
    public partial class DDL
    {
        public const string CREATE_TABLE_SPECIES =
            "CREATE TABLE Species (" +
                "Species PRIMARY KEY" +
            ");";
    }

    public partial class Updater
    {
        public const string INITIALIZE_SPECIES_FROM_TREEDEFAULTVALUE =
            "INSERT INTO Species " +
            "SELECT DISTINCT Species  FROM TreeDefaultValue;";
    }
}
