using System;
using System.Collections.Generic;
using System.Text;

#if NetCF
using FMSC.ORM.NetCF;
#endif

namespace FMSC.ORM.Core.SQL
{
    public class SQLSelectExpression
    {

        public String TableOrSubQuery { get; set; }
        public bool Distinct { get; set; }
        public List<String> ResultColumns { get; set; }
        public WhereClause Where { get; set; }
        public String GroupByExpression { get; set; }
        public String OrderBy { get; set; }
        public String Limit { get; set; }

        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();
            builder.Append("SELECT ");
            if(Distinct) { builder.Append("DISTINCT "); }
            builder.AppendLine();

            bool first = true;
            foreach(String s in ResultColumns)
            {
                if(!first)
                {
                    builder.AppendLine(", " + s);
                }
                else
                {
                    builder.AppendLine(s);
                    first = false;
                }
            }

            builder.AppendLine("FROM " + TableOrSubQuery);

            if (Where != null)
            { builder.AppendLine(Where.ToString()); }
            if (GroupByExpression != null)
            { builder.AppendLine("GROUP BY " + GroupByExpression); }
            if(Limit != null)
            { builder.AppendLine("LIMIT " + Limit); }
            builder.Append(";");

            return builder.ToString();
        }

    }
}
