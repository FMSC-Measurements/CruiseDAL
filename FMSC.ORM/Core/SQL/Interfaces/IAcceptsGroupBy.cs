using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FMSC.ORM.Core.SQL.Interfaces
{
    public interface IAcceptsGroupBy : IAcceptsLimit
    {
        void Accept(GroupByClause groupByClause);
        IAcceptsLimit GroupBy(IEnumerable<string> terms);
        IAcceptsLimit GroupBy(params string[] termArgs);
    }
}
