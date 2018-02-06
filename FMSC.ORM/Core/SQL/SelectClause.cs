using FMSC.ORM.Core.SQL.Interfaces;
using System;

namespace FMSC.ORM.Core.SQL
{
    public abstract class SelectClause : ISelectElement
    {
        public ISelectElement ParentElement { get; set; }

        public void Accept(ISelectElement node)
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