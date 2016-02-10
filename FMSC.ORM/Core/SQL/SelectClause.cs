using FMSC.ORM.Core.SQL.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace FMSC.ORM.Core.SQL
{
    public abstract class SelectClause : SelectElement
    {

        public SelectElement ParentElement { get; set; }


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

    }
}
