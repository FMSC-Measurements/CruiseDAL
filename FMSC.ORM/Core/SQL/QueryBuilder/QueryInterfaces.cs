using System;
using System.Collections.Generic;
using System.Text;

namespace FMSC.ORM.Core.SQL.QueryBuilder
{
    public interface IQueryBuilder<T>
    {
        IEnumerable<T> Query(params Object[] selectionArgs);

        IEnumerable<T> Read(params Object[] selectionArgs);
    }

    public interface IQuerryAcceptsLimit<T> : IQueryBuilder<T>
    {
        IQueryBuilder<T> Limit(int limit, int offset);
    }

    public interface IQuerryAcceptsOrderBy<T> : IQuerryAcceptsLimit<T>
    {
        IQuerryAcceptsLimit<T> OrderBy(IEnumerable<string> terms);
        IQuerryAcceptsLimit<T> OrderBy(params String[] termsArgs);
    }

    public interface IQuerryAcceptsGroupBy<T> : IQuerryAcceptsOrderBy<T>
    {
        IQuerryAcceptsOrderBy<T> GroupBy(IEnumerable<string> terms);
        IQuerryAcceptsOrderBy<T> GroupBy(params String[] termsArgs);
    }

    public interface IQuerryAcceptsWhere<T> : IQuerryAcceptsGroupBy<T>
    {
        IQuerryAcceptsGroupBy<T> Where(string expression);
    }

    public interface IQuerryAcceptsJoin<T> : IQuerryAcceptsWhere<T>
    {
        IQuerryAcceptsJoin<T> Join(string table, string joinContraint);
        IQuerryAcceptsJoin<T> Join(string table, string joinContraint, string alias);
    }
}
