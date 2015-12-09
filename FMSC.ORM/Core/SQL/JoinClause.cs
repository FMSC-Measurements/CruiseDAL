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

        public string JoinTable { get; set; }
        public string JoinSubQuery { get; set; }
        public string JoinConstraint { get; set; }

        public JoinClause(SQLSelectBuilder builder, SelectSource target): base(builder)
        {
            Target = target;
        }

        public override string ToSQL()
        {
            return Target + " JOIN " + (JoinTable ?? JoinSubQuery) + " " + JoinConstraint;
        }
    }
}
