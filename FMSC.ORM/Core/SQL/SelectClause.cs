using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FMSC.ORM.Core.SQL
{
    public abstract class SelectClause : SelectComponent
    {

        public SelectClause Target { get; protected set; }

        public SelectClause(SelectClause target, SQLSelectBuilder builder) : base(builder)
        {
            Target = target;
        }

        public LimitClause Limit(int limitSize, int offset)
        {
            return new LimitClause(this, base.SelectExpression, limitSize, offset);
        }

        protected override void OnBuilderChanged(SQLSelectBuilder builder)
        {
            builder.Clause = this;
        }



    }
}
