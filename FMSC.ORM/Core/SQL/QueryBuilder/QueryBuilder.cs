using SqlBuilder;
using System;
using System.Collections.Generic;

namespace FMSC.ORM.Core.SQL.QueryBuilder
{
    public class QueryBuilder<T> : IQuerryAcceptsJoin<T> where T : class, new()
    {
        protected DatastoreRedux Datastore;
        protected SqlSelectBuilder Builder;

        public QueryBuilder(DatastoreRedux datastore, SqlSelectBuilder builder)
        {
            Datastore = datastore;
            Builder = builder;
        }

        public IEnumerable<T> Query(params Object[] selectionArgs)
        {
            return Datastore.Query<T>(Builder, selectionArgs);
        }

        public IEnumerable<T> Read(params Object[] selectionArgs)
        {
            return Datastore.Read<T>(Builder, selectionArgs);
        }

        public IQueryBuilder<T> Limit(int limit, int offset)
        {
            Builder.Limit(limit, offset);
            return this;
        }

        public IQuerryAcceptsLimit<T> OrderBy(IEnumerable<string> terms)
        {
            Builder.OrderBy(terms);
            return this;
        }

        public IQuerryAcceptsLimit<T> OrderBy(params String[] termsArgs)
        {
            Builder.OrderBy(termsArgs);
            return this;
        }

        public IQuerryAcceptsOrderBy<T> GroupBy(IEnumerable<string> terms)
        {
            Builder.GroupBy(terms);
            return this;
        }

        public IQuerryAcceptsOrderBy<T> GroupBy(params string[] terms)
        {
            Builder.GroupBy(terms);
            return this;
        }

        public IQuerryAcceptsGroupBy<T> Where(string expression)
        {
            Builder.Where(expression);
            return this;
        }

        public IQuerryAcceptsJoin<T> Join(string table, string joinContraint, string alias)
        {
            Builder.Join(table, joinContraint, alias);
            return this;
        }

        public IQuerryAcceptsJoin<T> Join(string table, string joinContraint)
        {
            Builder.Join(table, joinContraint, null);
            return this;
        }
    }
}