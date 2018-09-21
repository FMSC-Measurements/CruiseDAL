using System;
using System.Collections.Generic;

namespace FMSC.ORM.Core.SQL.QueryBuilder
{
    public interface IQueryBuilder<T> where T : new()
    {
        IEnumerable<T> Query(params Object[] selectionArgs);

        IEnumerable<T> Read(params Object[] selectionArgs);
    }

    public interface IQuerryAcceptsLimit<T> : IQueryBuilder<T> where T : new()
    {
        IQueryBuilder<T> Limit(int limit, int offset);
    }

    public interface IQuerryAcceptsOrderBy<T> : IQuerryAcceptsLimit<T> where T : new()
    {
        IQuerryAcceptsLimit<T> OrderBy(IEnumerable<string> terms);

        IQuerryAcceptsLimit<T> OrderBy(params String[] termsArgs);
    }

    public interface IQuerryAcceptsGroupBy<T> : IQuerryAcceptsOrderBy<T> where T : new()
    {
        IQuerryAcceptsOrderBy<T> GroupBy(IEnumerable<string> terms);

        IQuerryAcceptsOrderBy<T> GroupBy(params String[] termsArgs);
    }

    public interface IQuerryAcceptsWhere<T> : IQuerryAcceptsGroupBy<T> where T : new()
    {
        IQuerryAcceptsGroupBy<T> Where(string expression);
    }

    public interface IQuerryAcceptsJoin<T> : IQuerryAcceptsWhere<T> where T : new()
    {
        IQuerryAcceptsJoin<T> Join(string table, string joinContraint);

        IQuerryAcceptsJoin<T> Join(string table, string joinContraint, string alias);

        IQuerryAcceptsJoin<T> LeftJoin(string table, string joinContraint);

        IQuerryAcceptsJoin<T> LeftJoin(string table, string joinContraint, string alias);
    }
}