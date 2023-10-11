using Backpack.SqlBuilder;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Threading;

namespace FMSC.ORM.Core.SQL.QueryBuilder
{
    public class QueryBuilder : QueryBuilderBase<object>
    {
        public Type DataType { get; }

        public QueryBuilder(Type dataType, Datastore datastore, SqlSelectBuilder builder) : base(datastore, builder)
        {
            DataType = dataType ?? throw new ArgumentNullException(nameof(dataType));
        }

        public QueryBuilder(Type dataType, DbConnection dbConnection, SqlSelectBuilder builder) : base(dbConnection, builder)
        { 
            DataType = dataType ?? throw new ArgumentNullException(nameof(dataType));
        }

        public override IEnumerable<object> Query(params object[] selectionArgs)
        {
            var connection = Connection;
            if (connection != null)
            {
                return connection.Query(DataType, Builder.ToString() + ";", selectionArgs);
            }
            else
            {
                return Datastore.Query(DataType, Builder.ToString() + ";", selectionArgs);
            }
        }

        public override IEnumerable<object> Query2(object parameters)
        {
            var connection = Connection;
            if (connection != null)
            {
                return connection.Query2(DataType, Builder.ToString() + ";", paramaters: parameters);
            }
            else
            {
                return Datastore.Query2(DataType, Builder.ToString() + ";", paramaters: parameters);
            }
        }

        public override IEnumerable<object> Read(params object[] selectionArgs)
        {
            // read methods being depreciated
            throw new NotSupportedException();
        }
    }

    public class QueryBuilder<T> : QueryBuilderBase<T> where T : class, new()
    {
        public QueryBuilder(Datastore datastore, SqlSelectBuilder builder) : base(datastore, builder)
        { }

        public QueryBuilder(DbConnection dbConnection, SqlSelectBuilder builder) : base(dbConnection, builder)
        { }

        public override IEnumerable<T> Query(params Object[] selectionArgs)
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

        public override IEnumerable<T> Query2(object parameters)
        {
            var connection = Connection;
            if (connection != null)
            {
                return connection.Query2<T>(Builder.ToString() + ";", paramaters: parameters);
            }
            else
            {
                return Datastore.Query2<T>(Builder.ToString() + ";", paramaters: parameters);
            }
        }



        public override IEnumerable<T> Read(params Object[] selectionArgs)
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

    }
}