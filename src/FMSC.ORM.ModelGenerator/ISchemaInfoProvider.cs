using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FMSC.ORM.ModelGenerator
{
    public interface ISchemaInfoProvider
    {
        IEnumerable<TableInfo> Tables { get; }
    }
}
