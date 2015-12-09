using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FMSC.ORM.Core.SQL
{
    public class TableOrSubQuery : SelectSource
    {
        public string Table { get; set; }
        public string SubQuery { get; set; }

        public TableOrSubQuery(SQLSelectBuilder builder) : base(builder)
        { }

        public override string ToSQL()
        {
            return Table ?? SubQuery;
        }
    }
}
