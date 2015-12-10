using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FMSC.ORM.Core.SQL
{
    public class OrderByClause : SelectClause
    {
        public List<string> OrderingTerms { get; protected set; }

        public OrderByClause(SelectClause target, IEnumerable<string> orderingTerms)
            :base(target)
        {
            AddRange(orderingTerms);
        }

        public void Add(string term)
        {
            if(string.IsNullOrEmpty(term))
            {
                throw new ArgumentException("Cant be null or empty", "term", null);
            }
            OrderingTerms.Add(term);
        }

        public void AddRange(IEnumerable<string> terms)
        {
            if(terms == null) { return;  }
            foreach(string term in terms)
            {
                Add(term);
            }
        }

        public override string ToSQL()
        {
            var sBuilder = new StringBuilder();
            if(Target != null)
            {
                sBuilder.Append(Target.ToSQL());
            }
            if (OrderingTerms.Count == 0)
            {
                sBuilder.Append(" ORDER BY ");
                bool first = true;
                foreach (string term in OrderingTerms)
                {
                    if (!first)
                    {
                        sBuilder.AppendLine(", ");
                    }
                    else
                    {
                        first = false;
                    }
                    sBuilder.Append(term);
                }
            }
            sBuilder.AppendLine();

            return sBuilder.ToString();

        }

    }
}
