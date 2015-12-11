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
        public SQLSelectBuilder SubQuery { get; set; }
        public string Alias { get; set; }

        public TableOrSubQuery(String tableName, string alias)
            : this()
        {
            this.Table = tableName;
        }

        public TableOrSubQuery(SQLSelectBuilder subQuery, string alias)
            : this()
        {
            this.SubQuery = subQuery;
            this.Alias = alias;
        }

        public TableOrSubQuery()
        { }

        public override string ToSQL()
        {
            var source = Table ?? "( " + SubQuery.ToSQL() + " )";
            var alias = (String.IsNullOrEmpty(Alias)) ? string.Empty : " AS " + Alias;
            return source + alias;
        }
    }
}
