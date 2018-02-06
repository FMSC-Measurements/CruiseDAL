using System.Text;

namespace FMSC.ORM.Core.SQL
{
    public class LimitClause : SelectClause
    {
        public int LimitSize { get; set; }
        public int Offset { get; set; }

        public LimitClause(int limitSize, int offset)
        {
            LimitSize = limitSize;
            Offset = offset;
        }

        public override string ToSQL()
        {
            var sBuilder = new StringBuilder();
            if (ParentElement != null)
            {
                sBuilder.Append(ParentElement.ToSQL());
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