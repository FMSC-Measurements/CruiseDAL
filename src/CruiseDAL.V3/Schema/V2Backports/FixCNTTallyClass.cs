using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CruiseDAL.Schema.V2Backports
{
    public class FixCNTTallyClass  : IViewDefinition
    {
        public string ViewName => "FixCNTTallyClass";

        public string CreateView =>
@"CREATE VIEW FixCNTTallyClass AS 
    SELECT Stratum_CN, FixCNTField AS Field 
    FROM Stratum;";
    }
}
