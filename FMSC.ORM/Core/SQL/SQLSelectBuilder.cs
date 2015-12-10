using FMSC.ORM.Core.SQL.Interfaces;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

#if NetCF
using FMSC.ORM.NetCF;
#endif

namespace FMSC.ORM.Core.SQL
{

    public class SQLSelectBuilder : IAcceptsJoin
    {
        public SelectSource Source { get; set; }
        public ResultColumnCollection ResultColumns { get; set; }
        public SelectClause Clause { get; set; }


        public string ToSQL()
        {
            StringBuilder builder = new StringBuilder();
            builder.Append("SELECT ");
            builder.AppendLine(ResultColumns.ToSQL());

            builder.AppendLine("FROM " + Source.ToSQL());

            if(Clause != null)
            {
                builder.Append(Clause.ToSQL());
            }

            builder.AppendLine(";");

            return builder.ToString();
        }

        public override string ToString()
        {
            return ToSQL();
        }

        public IAcceptsJoin Join(TableOrSubQuery source, string constraint)
        {
            this.Source = new JoinClause(this.Source, source, constraint);
            return this;
        }

        public IAcceptsJoin Join(string table, string constraint)
        {
            this.Source = new JoinClause(this.Source, table, constraint);
            return this;
        }

        public IAcceptsGroupBy Where(string expression)
        {
            this.Clause = new WhereClause(expression);
            return this;
        }

        public IAcceptsLimit GroupBy(IEnumerable<string> terms)
        {
            this.Clause = new GroupByClause(this.Clause, terms);
            return this;
        }

        public SelectElement Limit(int limit, int offset)
        {
            this.Clause = new LimitClause(this.Clause, limit, offset);
            return this;
        }
    }
}
