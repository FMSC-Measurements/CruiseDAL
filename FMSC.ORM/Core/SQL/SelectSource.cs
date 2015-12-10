using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FMSC.ORM.Core.SQL
{
    public abstract class SelectSource : SelectComponent
    {
        

        public SelectSource(SQLSelectBuilder builder) : base(builder)
        {

        }

        protected override void OnBuilderChanged(SQLSelectBuilder builder)
        {
            builder.Source = this;
        }

        public SelectSource Join(string table, string joinConstraint)
        {
            return new JoinClause(SelectExpression, this)
            {
                JoinTable = table,
                JoinConstraint = joinConstraint
            };
        }

        public WhereClause Where(string expr)
        {
            return new WhereClause(SelectExpression, expr);
        }
    }
}
