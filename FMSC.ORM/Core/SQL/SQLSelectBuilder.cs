using FMSC.ORM.Core.SQL.Interfaces;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;


namespace FMSC.ORM.Core.SQL
{

    public class SQLSelectBuilder : IAcceptsJoin
    {
        public SelectSource Source { get; set; }
        public ResultColumnCollection ResultColumns { get; set; }
        public SelectClause Clause { get; set; }
        public SelectElement ParentElement { get; set; }

        public SQLSelectBuilder()
        {
            this.ResultColumns = new ResultColumnCollection();
        }

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

            //builder.AppendLine(";");

            return builder.ToString();
        }

        public override string ToString()
        {
            return ToSQL();
        }

        public IAcceptsJoin Join(TableOrSubQuery source, string constraint)
        {
            this.Accept(new JoinClause(source, constraint));
            return this;
        }

        public IAcceptsJoin Join(string table, string constraint, string alias)
        {
            this.Accept(new JoinClause(table, constraint, alias));
            return this;
        }

        public IAcceptsGroupBy Where(string expression)
        {
            this.Accept(new WhereClause(expression));
            return this;
        }

        public IAcceptsLimit GroupBy(params string[] termArgs)
        {
            return GroupBy((IEnumerable<string>)termArgs);
        }

        public IAcceptsLimit GroupBy(IEnumerable<string> terms)
        {
            this.Accept( new GroupByClause(terms));
            return this;
        }

        public SelectElement Limit(int limit, int offset)
        {
            this.Accept( new LimitClause(limit, offset));
            return this;
        }

        public void Accept(SelectElement parent)
        {
            throw new NotSupportedException("select can't have parent");
        }

        public void Accept(JoinClause joinClause)
        {
            joinClause.Accept(this.Source);
            this.Source = joinClause;
        }

        public void Accept(WhereClause whereClause)
        {
            whereClause.Accept(this);
            this.Clause = whereClause;
        }

        public void Accept(GroupByClause groupByClause)
        {
            groupByClause.Accept((SelectElement)this.Clause ?? this);
            this.Clause = groupByClause;
        }

        public void Accept(LimitClause limitClause)
        {
            limitClause.Accept((SelectElement)this.Clause ?? this);
            this.Clause = limitClause;
        }
    }
}
