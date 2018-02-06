using System.Collections.Generic;

namespace FMSC.ORM.Core.SQL.Interfaces
{
    public interface IAcceptsGroupBy : IAcceptsOrderBy
    {
        void Accept(GroupByClause groupByClause);

        IAcceptsOrderBy GroupBy(IEnumerable<string> terms);

        IAcceptsOrderBy GroupBy(params string[] termArgs);
    }
}