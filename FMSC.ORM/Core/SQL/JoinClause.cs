using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FMSC.ORM.Core.SQL
{
    public class JoinClause : SelectSource
    {
        //public SelectSource Target { get; protected set; }

        public TableOrSubQuery Source { get; set; }
        public string JoinConstraint { get; set; }

        public JoinClause(TableOrSubQuery source, string constraint)
        {
            Source = source;
            JoinConstraint = constraint;
        }

        public JoinClause(string table, string constraint, string alias)
            : this(new TableOrSubQuery(table, alias), constraint)
        {
        }

        public override string ToSQL()
        {
            return ParentElement.ToSQL() + " JOIN " 
                + Source.ToSQL()
                + " " + JoinConstraint;
        }
    }
}
