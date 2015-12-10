using System;
using System.Collections.Generic;

namespace FMSC.ORM.Core.SQL
{
    public class WhereClause : SelectClause
    {

        public WhereClause(string expression)
            : this((SQLSelectBuilder)null, expression)
        { }        

        public WhereClause(SQLSelectBuilder builder, string expression) 
            : base((SelectClause)null, builder)
        {
            Expression = expression;
        }

        public string Expression { get; set; }

        public GroupByClause GroupBy(IEnumerable<string> groupByExprs)
        {
            return new GroupByClause(this, this.SelectExpression, groupByExprs);
        }

        public OrderByClause OrderBy(IEnumerable<string> orderingTerms)
        {
            return new OrderByClause(this, this.SelectExpression, orderingTerms);
        }


        public override string ToSQL()
        {
            return " WHERE " + Expression + Environment.NewLine;
        }
    }
}
