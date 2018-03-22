using System;
using System.Collections.Generic;
using System.Text;

namespace SqlBuilder
{
    public class SqlInsertCommand
    {
        public string TableName { get; set; }
        public OnConflictOption ConflictOption { get; set; }

        public IEnumerable<KeyValuePair<string, object>> Values { get; set; }
        public IEnumerable<String> ColumnNames { get; set; }
        public IEnumerable<String> ValueExpressions { get; set; }
        public SqlSelectBuilder SelectExpression { get; set; }

        public override string ToString()
        {
            var sb = new StringBuilder();

            sb.Append("INSERT");
            if (ConflictOption != OnConflictOption.Default)
            { sb.Append(" OR " + ConflictOption.ToString()); }
            sb.Append(" INTO " + TableName).Append(" ");

            if (Values != null)
            {
                sb.AppendLine("(");
                var count = 0;
                foreach (var pair in Values)
                {
                    if (count++ > 0) { sb.Append(","); }
                    if (count > 0 && count % 4 == 0) { sb.AppendLine(); }
                    else { sb.Append(" "); }
                    sb.Append(pair.Key);
                }
                sb.Append(") VALUES (");

                count = 0;
                foreach (var pair in Values)
                {
                    if (count++ > 0) { sb.Append(","); }
                    if (count > 0 && count % 4 == 0) { sb.AppendLine(); }
                    else { sb.Append(" "); }
                    sb.Append(pair.Value);
                }
                sb.Append(")");
            }
            else
            {
                if (ColumnNames != null)
                {
                    sb.AppendLine("(");
                    bool first = true;
                    foreach (string colName in ColumnNames)
                    {
                        if (first)
                        {
                            sb.Append(" " + colName);
                            first = false;
                        }
                        else
                        {
                            sb.Append(", " + colName);
                        }
                    }
                    sb.Append(")");
                }

                if (ValueExpressions != null)
                {
                    sb.AppendLine("VALUES");
                    sb.Append("(");

                    var first = true;
                    foreach (string valExpr in ValueExpressions)
                    {
                        if (first)
                        {
                            sb.Append(valExpr);
                            first = false;
                        }
                        else
                        {
                            sb.Append("," + valExpr);
                        }
                    }
                    sb.Append(")");
                }
                else if (SelectExpression != null)
                {
                    sb.AppendLine().Append(SelectExpression.ToString());
                }
                else
                {
                    sb.AppendLine().Append("DEFAULT VALUES");
                }
            }

            return sb.ToString();
        }
    }
}