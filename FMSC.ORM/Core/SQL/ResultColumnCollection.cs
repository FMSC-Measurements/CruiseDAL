using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FMSC.ORM.Core.SQL
{
    public class ResultColumnCollection : List<string>
    {
        public bool Distinct { get; set; }

        public string ToSQL()
        {
            var sBuilder = new StringBuilder();
            if(Distinct)
            {
                sBuilder.AppendLine("DISTINCT ");
            }
            if(Count == 0)
            {
                sBuilder.AppendLine("*");
            }
            else
            {
                bool first = true;
                foreach(string colExpr in this)
                {
                    if (!first)
                    {
                        sBuilder.AppendLine(", ");
                    }
                    else
                    {
                        first = false;
                    }
                    sBuilder.Append(colExpr);
                }
            }
            return sBuilder.ToString();

        }

    }
}
