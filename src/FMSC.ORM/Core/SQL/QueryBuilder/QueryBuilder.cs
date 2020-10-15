using Backpack.SqlBuilder;
using System;
using System.Collections.Generic;
using System.Data.Common;

namespace FMSC.ORM.Core.SQL.QueryBuilder
{
    public class QueryBuilder<T> : IQuerryAcceptsJoin<T> where T : class, new()
    {
        protected DbConnection Connection { get; }
        protected Datastore Datastore { get; }
        protected SqlSelectBuilder Builder { get; }

        public QueryBuilder(Datastore datastore, SqlSelectBuilder builder)
        {
            Datastore = datastore;
            Builder = builder;
        }

        public QueryBuilder(DbConnection dbConnection, SqlSelectBuilder builder)
        {
            Connection = dbConnection;
            Builder = builder;
        }

        public IEnumerable<T> Query(params Object[] selectionArgs)
        {
            var connection = Connection;
            if(connection != null)
            { 
                return connection.Query<T>(Builder.ToString() + ";", selectionArgs); 
            }
            else
            {
                return Datastore.Query<T>(Builder, selectionArgs);
            }
        }

        //public IEnumerable<T> Query(Object[] selectionArgs,  DbTransaction transaction, IExceptionProcessor exceptionProcessor = null)
        //{
        //    var connection = Connection;
        //    if (connection != null)
        //    {
        //        return connection.Query<T>(Builder.ToString() + ";", selectionArgs, transaction, exceptionProcessor);
        //    }
        //    else
        //    {
        //        if (transaction != null)
        //        { throw new NotSupportedException(""); }
        //        else
        //        { return Datastore.Query<T>(Builder, selectionArgs); }
        //    }
        //}

        public IEnumerable<T> Read(params Object[] selectionArgs)
        {
            var connection = Connection;
            if (connection != null)
            {
                throw new NotSupportedException(); // connection extentions doesn't have support for Read (cached query)
            }
            else
            {
                return Datastore.Read<T>(Builder, selectionArgs);
            }
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

        public IQuerryAcceptsJoin<T> LeftJoin(string table, string joinContraint)
        {
            Builder.LeftJoin(table, joinContraint);
            return this;
        }

        public IQuerryAcceptsJoin<T> LeftJoin(string table, string joinContraint, string alias)
        {
            Builder.LeftJoin(table, joinContraint, alias);
            return this;
        }
    }
}