using System;
using System.Text;
using SqlBuilder;

namespace FMSC.ORM.Core.SQL
{
    public class LegacyWhereClausePlaceholder : WhereClause
    {
        public LegacyWhereClausePlaceholder() 
        {
        }

        public uint Index { get; set; }

        public override void AppendTo(StringBuilder sb)
        {
            sb.Append("{").Append(Index).Append("}");
        }
    }
}