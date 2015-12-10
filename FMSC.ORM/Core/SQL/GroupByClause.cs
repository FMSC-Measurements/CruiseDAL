using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FMSC.ORM.Core.SQL
{
    public class GroupByClause : SelectClause
    {
        public List<String> Expressions { get; protected set; }

        public GroupByClause(SelectClause target, IEnumerable<string> groupByExprs) 
            :base(target)
        {
            if(groupByExprs != null)
            {
                AddRange(groupByExprs);
            }
        }

        public void Add(String expression)
        {
            Expressions.Add(expression);
        }

        public void AddRange(IEnumerable<string> expressions)
        {
            foreach(string expr in expressions)
            {
                Add(expr);
            }
        }

        public OrderByClause OrderBy(IEnumerable<string> orderingTerms)
        {
            return new OrderByClause(this, orderingTerms);
        }


        public override string ToSQL()
        {
            var sBuilder = new StringBuilder();
            if (Target != null)
            {
                sBuilder.Append(Target.ToSQL());
            }

            if(Expressions.Count != 0)
            {
                sBuilder.AppendLine(" Group BY ");
                bool first = true;
                foreach(string expr in Expressions)
                {
                    if(!first)
                    {
                        sBuilder.AppendLine(", ");
                    }
                    else
                    {
                        first = false;
                    }
                    sBuilder.Append(expr);
                }
            }

            return sBuilder.ToString();
        }
    }
}
