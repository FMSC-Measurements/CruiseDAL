using Backpack.SqlBuilder;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;

namespace FMSC.ORM.Core.SQL.QueryBuilder
{
    public abstract class QueryBuilderBase<T> : IQuerryAcceptsJoin<T>
    {
        protected DbConnection Connection { get; }
        protected Datastore Datastore { get; }
        protected SqlSelectBuilder Builder { get; }

        protected QueryBuilderBase(Datastore datastore, SqlSelectBuilder builder)
        {
            Datastore = datastore;
            Builder = builder;
        }

        protected QueryBuilderBase(DbConnection dbConnection, SqlSelectBuilder builder)
        {
            Connection = dbConnection;
            Builder = builder;
        }

        public abstract IEnumerable<T> Query(params Object[] selectionArgs);

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

        public abstract IEnumerable<T> Query2(object parameters);

        public abstract IEnumerable<T> Read(params Object[] selectionArgs);

        public long Count(params object[] selectionArgs)
        {
            var builder = Builder;
            var countBuilder = new SqlSelectBuilder
            {
                Source = builder.Source,
                Dialect = builder.Dialect,
                WhereClause = builder.WhereClause,
                GroupByClause = builder.GroupByClause,
                OrderByClause = builder.OrderByClause,
                LimitClause = builder.LimitClause,
                Clauses = builder.Clauses,
            };
            foreach (var jc in builder.JoinClauses) { countBuilder.JoinClauses.Add(jc); }
            countBuilder.ResultColumns.Add("count(1)");

            var connection = Connection;
            if (connection != null)
            {
                return connection.ExecuteScalar<long>(countBuilder.ToString() + ";", parameters: selectionArgs);
            }
            else
            {
                return Datastore.ExecuteScalar<long>(countBuilder.ToString() + ";", parameters: selectionArgs);
            }
        }

        public long Count2(object parameters)
        {
            var builder = Builder;
            var countBuilder = new SqlSelectBuilder
            {
                Source = builder.Source,
                Dialect = builder.Dialect,
                WhereClause = builder.WhereClause,
                GroupByClause = builder.GroupByClause,
                OrderByClause = builder.OrderByClause,
                LimitClause = builder.LimitClause,
                Clauses = builder.Clauses,
            };
            foreach (var jc in builder.JoinClauses) { countBuilder.JoinClauses.Add(jc); }
            countBuilder.ResultColumns.Add("count(1)");

            var connection = Connection;
            if (connection != null)
            {
                return connection.ExecuteScalar2<long>(countBuilder.ToString() + ";", parameters: parameters);
            }
            else
            {
                return Datastore.ExecuteScalar2<long>(countBuilder.ToString() + ";", parameters: parameters);
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