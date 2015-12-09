using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FMSC.ORM.Core.SQL
{
    public class ResultColumnCollection : List<ColumnInfo>
    {
        public bool Distinct { get; set; }

        public void Add(String colName)
        {
            this.Add(new ColumnInfo()
            {
                Name = colName
            });
        }

        public void AddRange(IEnumerable<String> colNames)
        {
            foreach(string name in colNames)
            {
                this.Add(name);
            }
        }

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
                foreach(ColumnInfo ci in this)
                {
                    if (!first)
                    {
                        sBuilder.AppendLine(", ");
                    }
                    else
                    {
                        first = false;
                    }
                    sBuilder.Append(ci.Name);
                }
            }
            return sBuilder.ToString();

        }

    }
}
