using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FMSC.ORM.Core.SQL
{
    public class JoinClause : SelectSource
    {
        public SelectSource Target { get; protected set; }

        public TableOrSubQuery Source { get; set; }
        public string JoinConstraint { get; set; }

        public JoinClause(SelectSource target, TableOrSubQuery source, string constraint)
        {
            Target = target;
            Source = source;
            JoinConstraint = constraint;
        }

        public JoinClause(SelectSource target, string table, string constraint)
            : this(target, new TableOrSubQuery(table), constraint)
        {
        }

        public override string ToSQL()
        {
            return Target + " JOIN " + (Source.Table ?? ("( " + Source.SubQuery + " )")) + " " + JoinConstraint;
        }
    }
}
