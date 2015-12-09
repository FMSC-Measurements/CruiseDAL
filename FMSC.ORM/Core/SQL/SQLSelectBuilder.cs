using System;
using System.Collections.Generic;
using System.Text;

#if NetCF
using FMSC.ORM.NetCF;
#endif

namespace FMSC.ORM.Core.SQL
{
    public class SQLSelectBuilder
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

    }
}
