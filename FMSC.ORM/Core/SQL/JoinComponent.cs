using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FMSC.ORM.Core.SQL
{
    public class JoinComponent
    {
        public TableOrSubQuery Source { get; set; }
        public string JoinConstraint { get; set; }

        public JoinComponent(TableOrSubQuery source, string constraint)
        {
            Source = source;
            JoinConstraint = constraint;
        }

        public JoinComponent(string table, string constraint, string alias)
            : this(new TableOrSubQuery(table, alias), constraint)
        {
        }

        public string ToSQL()
        {
            return " JOIN "
                + Source.ToSQL()
                + " " + JoinConstraint;
        }
    }
}
