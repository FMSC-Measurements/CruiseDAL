using System;
using System.Collections.Generic;
using System.Text;

#if NetCF
using FMSC.ORM.NetCF;
#endif

namespace FMSC.ORM.Core.SQL
{
    public class SQLInsertCommand
    {
        public string TableName { get; set; }
        public OnConflictOption ConflictOption { get; set; }
        public ICollection<String> ColumnNames { get; set; }

        public ICollection<String> ValueExpressions { get; set; }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            
            sb.AppendLine("INSERT");
            if (ConflictOption != OnConflictOption.Default)
            { sb.AppendLine("OR " + ConflictOption.ToString()); }
            sb.AppendLine("INTO " + TableName);

            bool first = true;
            foreach(string colName in ColumnNames)
            {
                if(first)
                {
                    sb.AppendLine(colName);
                    first = false;
                }
                else
                {
                    sb.AppendLine(colName + ",");
                }
            }

            sb.AppendLine("VALUES");
            sb.AppendLine("(");

            first = true;
            foreach(string valExpr in ValueExpressions)
            {
                if(first)
                {
                    sb.AppendLine(valExpr);
                    first = false;
                }
                else
                {
                    sb.AppendLine(valExpr);
                }
            }
            sb.AppendLine(");");
            return sb.ToString();
        }
    }
            
}

