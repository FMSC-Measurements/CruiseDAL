using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FMSC.ORM.Core.SQL
{
    public class LimitClause : SelectClause
    {
        public int LimitSize { get; set; }
        public int Offset { get; set; }

        public LimitClause(SelectClause target, SQLSelectBuilder builder, int limitSize, int offset)
            :base(target, builder)
        {
            LimitSize = limitSize;
            Offset = offset;
        }

        public override string ToSQL()
        {
            var sBuilder = new StringBuilder();
            if (Target != null)
            {
                sBuilder.Append(Target.ToSQL());
            }

            if (LimitSize > 0)
            {
                sBuilder.AppendLine(" LIMIT " + LimitSize.ToString());
                if (Offset > 0)
                {
                    sBuilder.AppendLine(" OFFSET " + Offset.ToString());
                }
            }

            return sBuilder.ToString();
        }
    }
}
