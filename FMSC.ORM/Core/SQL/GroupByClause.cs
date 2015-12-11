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

        public GroupByClause(IEnumerable<string> groupByExprs) 
            :this()
        {
            if(groupByExprs != null)
            {
                AddRange(groupByExprs);
            }
        }

        public GroupByClause()
        {
            this.Expressions = new List<string>();
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


        public override string ToSQL()
        {
            var sBuilder = new StringBuilder();
            if (ParentElement != null)
            {
                sBuilder.Append(ParentElement.ToSQL());
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
