﻿using System;
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

    public interface IQuerryAcceptsGroupBy<T> : IQuerryAcceptsLimit<T>
    {
        IQuerryAcceptsLimit<T> GroupBy(IEnumerable<string> terms);
    }

    public interface IQuerryAcceptsWhere<T> : IQuerryAcceptsGroupBy<T>
    {
        IQuerryAcceptsGroupBy<T> Where(string expression);
    }

    public interface IQuerryAcceptsJoin<T> : IQuerryAcceptsWhere<T>
    {
        IQuerryAcceptsJoin<T> Join(string table, string joinContraint, string alias);
    }
}