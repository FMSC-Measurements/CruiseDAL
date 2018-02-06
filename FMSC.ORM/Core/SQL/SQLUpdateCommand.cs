using System;
using System.Collections.Generic;
using System.Text;

namespace FMSC.ORM.Core.SQL
{
    public class SQLUpdateCommand
    {
        public string TableName { get; set; }
        public OnConflictOption ConflictOption { get; set; }
        public IList<String> ColumnNames { get; set; }

        public IList<String> ValueExpressions { get; set; }

        public WhereClause Where { get; set; }

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.AppendLine("UPDATE");
            if (ConflictOption != OnConflictOption.Default)
            { sb.AppendLine("OR " + ConflictOption.ToString()); }//enum.GetName not avalible in NetCF
            sb.AppendLine(TableName);

            for (int i = 0; i < ColumnNames.Count; i++)
            {
                if (i == 0) { sb.AppendLine("SET"); }
                else { sb.Append(", "); }
                sb.AppendLine(ColumnNames[i] + " = " + ValueExpressions[i]);
            }
            if (Where != null) { sb.AppendLine(Where.ToString()); }
            sb.Append(";");
            return sb.ToString();
        }
    }
}