using FMSC.ORM.Core.SQL.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace FMSC.ORM.Core.SQL.QueryBuilder
{
    public class QueryBuilder<T> : IQuerryAcceptsJoin<T>
    {
        protected DatastoreRedux Datastore;
        protected SQLSelectBuilder Builder;

        public QueryBuilder(DatastoreRedux datastore, SQLSelectBuilder builder)
        {
            Datastore = datastore;
            Builder = builder;   
        }

        public IEnumerable<T> Query(params Object[] selectionArgs)
        {
            return Datastore.Query<T>(Builder, selectionArgs);
        }
        //public IEnumerable<TResult> Query<TSource, TResult>(Func<TSource, TResult> selector)
        //{

        //}

        public IEnumerable<T> Read(params Object[] selectionArgs)
        {
            return Datastore.Read<T>(Builder, selectionArgs);
        }

        public IQueryBuilder<T> Limit(int limit, int offset)
        {
            Builder.Limit(limit, offset);
            return this;
        }

        public IQuerryAcceptsLimit<T> GroupBy(IEnumerable<string> terms)
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
    }
}
