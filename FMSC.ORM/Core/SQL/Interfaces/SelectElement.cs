using System;
using System.Collections.Generic;
using System.Text;

namespace FMSC.ORM.Core.SQL.Interfaces
{
    public interface SelectElement
    {
        SelectElement ParentElement { get; set; }

        void Accept(SelectElement parentElement);

        string ToSQL();
    }
}
