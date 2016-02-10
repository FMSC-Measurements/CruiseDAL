using System;
using System.Collections.Generic;
using System.Text;

namespace FMSC.ORM.Core.SQL
{
    public class TableOrSubQuery : SelectSource
    {
        public string Table { get; set; }
        public SQLSelectBuilder SubQuery { get; set; }
        public string Alias { get; set; }

        public override string SourceName
        {
            get
            {
                return Table ?? Alias;
            }
        }

        public TableOrSubQuery(String tableName, string alias)
            : this()
        {
            this.Table = tableName;
            this.Alias = alias;
        }

        public TableOrSubQuery(SQLSelectBuilder subQuery, string alias)
            : this()
        {
            this.SubQuery = subQuery;
            this.Alias = alias;
        }

        public TableOrSubQuery()
        { }

        

        public override JoinClause Join(string table, string constraint, string alias)
        {
            var joinClause = new JoinClause(this);
            joinClause.Join(table, constraint, alias);
            return joinClause;
        }

        public override JoinClause Join(TableOrSubQuery source, string constraint)
        {
            var joinClause = new JoinClause(this);
            joinClause.Join(source, constraint);
            return joinClause;
        }

        public override string ToSQL()
        {
            var source = Table ?? "( " + SubQuery.ToSQL() + " )";
            var alias = (String.IsNullOrEmpty(Alias)) ? string.Empty : " AS " + Alias;
            return source + alias;
        }
    }
}
