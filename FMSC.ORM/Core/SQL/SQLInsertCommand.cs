﻿using System;
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
            sb.Append("(");
            bool first = true;
            foreach(string colName in ColumnNames)
            {
                if(first)
                {
                    sb.Append(" " + colName);
                    first = false;
                }
                else
                {
                    sb.Append(", " + colName );
                }
            }
            sb.AppendLine(")");

            sb.AppendLine("VALUES");
            sb.Append("(");

            first = true;
            foreach(string valExpr in ValueExpressions)
            {
                if(first)
                {
                    sb.Append(valExpr);
                    first = false;
                }
                else
                {
                    sb.Append("," + valExpr);
                }
            }
            sb.Append(");");
            return sb.ToString();
        }
    }
            
}
