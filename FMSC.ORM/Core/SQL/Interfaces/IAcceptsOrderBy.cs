using System.Collections.Generic;

namespace FMSC.ORM.Core.SQL.Interfaces
{
    public interface IAcceptsOrderBy : IAcceptsLimit
    {
        void Accept(OrderByClause orderByClause);

        IAcceptsLimit OrderBy(IEnumerable<string> terms);

        IAcceptsLimit OrderBy(params string[] termArgs);
    }
}