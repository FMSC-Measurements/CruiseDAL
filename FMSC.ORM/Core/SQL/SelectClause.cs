using FMSC.ORM.Core.SQL.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FMSC.ORM.Core.SQL
{
    public abstract class SelectClause : SelectElement
    {

        public SelectElement ParentElement { get; set; }

        //public SelectClause(SelectClause target)
        //{
        //    ParentElement = target;
        //}

        public void Accept(SelectElement node)
        {
            if (this.ParentElement != null)
            {
                this.ParentElement.Accept(node);
            }
            else
            {
                this.ParentElement = node;
            }
        }

        public abstract String ToSQL();

        public override string ToString()
        {
            return ToSQL();
        }

        //public LimitClause Limit(int limitSize, int offset)
        //{
        //    return new LimitClause(this, base.SelectExpression, limitSize, offset);
        //}

        //protected override void OnBuilderChanged(SQLSelectBuilder builder)
        //{
        //    builder.Clause = this;
        //}
    }
}
