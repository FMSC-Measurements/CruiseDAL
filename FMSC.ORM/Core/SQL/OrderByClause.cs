using System;
using System.Collections.Generic;
using System.Text;

namespace FMSC.ORM.Core.SQL
{
    public class OrderByClause : SelectClause
    {
        public List<string> OrderingTerms { get; protected set; }

        public OrderByClause(IEnumerable<string> orderingTerms)
        {
            AddRange(orderingTerms);
        }

        public void Add(string term)
        {
            if(string.IsNullOrEmpty(term))
            {
                throw new ArgumentException("Cant be null or empty", "term");
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
            if(ParentElement != null)
            {
                sBuilder.Append(ParentElement.ToSQL());
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
