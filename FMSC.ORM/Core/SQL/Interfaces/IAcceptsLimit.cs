using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FMSC.ORM.Core.SQL.Interfaces
{
    public interface IAcceptsLimit : SelectElement
    {
        void Accept(LimitClause limitClause);
        SelectElement Limit(int limit, int offset);
    }
}
