using FMSC.ORM.Core.SQL.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace FMSC.ORM.Core.SQL
{
    public class SQLSelectBuilder : IAcceptsJoin
    {
        public SelectSource Source { get; set; }
        public ResultColumnCollection ResultColumns { get; set; }
        public SelectClause Clause { get; set; }
        public ISelectElement ParentElement { get; set; }

        public SQLSelectBuilder()
        {
            this.ResultColumns = new ResultColumnCollection();
        }

        public string ToSQL()
        {
            var builder = new StringBuilder();
            builder.Append("SELECT ");
            builder.AppendLine(ResultColumns.ToSQL());

            builder.AppendLine("FROM " + Source.ToSQL());

            if (Clause != null)
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
            this.Accept(this.Source.Join(source, constraint));
            return this;
        }

        public IAcceptsJoin Join(string table, string constraint, string alias)
        {
            this.Accept(this.Source.Join(table, constraint, alias));
            return this;
        }

        public IAcceptsGroupBy Where(string expression)
        {
            this.Accept(new WhereClause(expression));
            return this;
        }

        public IAcceptsOrderBy GroupBy(params string[] termArgs)
        {
            return GroupBy((IEnumerable<string>)termArgs);
        }

        public IAcceptsOrderBy GroupBy(IEnumerable<string> terms)
        {
            this.Accept(new GroupByClause(terms));
            return this;
        }

        public IAcceptsLimit OrderBy(IEnumerable<string> terms)
        {
            this.Accept(new OrderByClause(terms));
            return this;
        }

        public IAcceptsLimit OrderBy(params string[] termArgs)
        {
            this.Accept(new OrderByClause(termArgs));
            return this;
        }

        public ISelectElement Limit(int limit, int offset)
        {
            this.Accept(new LimitClause(limit, offset));
            return this;
        }

        public void Accept(ISelectElement parent)
        {
            throw new NotSupportedException("select can't have parent");
        }

        public void Accept(JoinClause joinClause)
        {
            this.Source = joinClause;
        }

        public void Accept(WhereClause whereClause)
        {
            whereClause.Accept(null);
            this.Clause = whereClause;
        }

        public void Accept(GroupByClause groupByClause)
        {
            groupByClause.Accept((ISelectElement)this.Clause);
            this.Clause = groupByClause;
        }

        public void Accept(OrderByClause orderByClause)
        {
            orderByClause.Accept((ISelectElement)this.Clause);
            this.Clause = orderByClause;
        }

        public void Accept(LimitClause limitClause)
        {
            limitClause.Accept((ISelectElement)this.Clause);
            this.Clause = limitClause;
        }
    }
}