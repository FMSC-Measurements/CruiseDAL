using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CruiseDAL.Migrators
{
    public interface IMigrator
    {
        string MigrateToV3(string toDbName, string fromDbName, string cruiseID, string saleID);
    }
}
