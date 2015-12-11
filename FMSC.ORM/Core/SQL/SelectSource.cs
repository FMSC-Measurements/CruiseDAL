using FMSC.ORM.Core.SQL.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FMSC.ORM.Core.SQL
{
    public abstract class SelectSource : SelectElement
    {
        

        public SelectElement ParentElement { get; set; }

        public void Accept(SelectElement parent)
        {
            this.ParentElement = parent;
        }

        public abstract String ToSQL();

        public override string ToString()
        {
            return ToSQL();
        }

    }
}
