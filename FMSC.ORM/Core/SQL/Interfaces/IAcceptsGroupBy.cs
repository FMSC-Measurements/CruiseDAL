using System;
using System.Collections.Generic;
using System.Text;

namespace FMSC.ORM.Core.SQL.Interfaces
{
    public interface IAcceptsGroupBy : IAcceptsOrderBy
    {
        void Accept(GroupByClause groupByClause);
        IAcceptsOrderBy GroupBy(IEnumerable<string> terms);
        IAcceptsOrderBy GroupBy(params string[] termArgs);
    }
}
