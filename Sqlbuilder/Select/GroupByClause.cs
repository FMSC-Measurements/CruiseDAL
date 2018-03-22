using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SqlBuilder
{
    public class GroupByClause : SelectClause
    {
        public static GroupByClause operator +(GroupByClause left, GroupByClause right)
        {
            var exprs = left.Expressions.Concat(right.Expressions).ToArray();
            return new GroupByClause(exprs);
        }

        public IEnumerable<String> Expressions { get; protected set; }

        public GroupByClause(IEnumerable<string> groupByExprs)
        {
            Expressions = groupByExprs;
        }

        public override void AppendTo(StringBuilder sb)
        {
            if (Expressions != null && Expressions.Count() != 0)
            {
                sb.Append("GROUP BY ");
                bool first = true;
                foreach (string expr in Expressions)
                {
                    if (!first) { sb.Append(", "); }
                    else { first = false; }
                    sb.Append(expr);
                }
            }
        }
    }
}