using System.Collections.Generic;
using System.Text;

namespace SqlBuilder
{
    public class ResultColumnCollection : List<string>
    {
        public bool Distinct { get; set; }

        public override string ToString()
        {
            var sBuilder = new StringBuilder();
            if (Distinct)
            {
                sBuilder.Append("DISTINCT ");
            }
            if (Count == 0)
            {
                sBuilder.Append("*");
            }
            else
            {
                bool first = true;
                foreach (string colExpr in this)
                {
                    if (!first)
                    {
                        sBuilder.Append(", ");
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