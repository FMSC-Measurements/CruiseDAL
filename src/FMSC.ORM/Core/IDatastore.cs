using Backpack.SqlBuilder;
using FMSC.ORM.Core.SQL.QueryBuilder;
using FMSC.ORM.EntityModel;
using FMSC.ORM.Sql;
using System.Collections.Generic;
using System.Data.Common;

namespace FMSC.ORM.Core
{
    public interface IDatastore
    {
        string Path { get; }
        IExceptionProcessor ExceptionProcessor { get; }
        IDatastoreBuilder DatabaseBuilder { get; }
        ICommandBuilder CommandBuilder { get; }

        void AddField(string tableName, ColumnInfo fieldDef);
        void CreateDatastore(IDatastoreBuilder builder);
        void BeginTransaction();
        void CommitTransaction();
        void Delete(object data, string tableName = null);
        int Execute(string commandText, params object[] parameters);
        int Execute2(string commandText, object parameters);
        object ExecuteScalar(string commandText, params object[] parameters);
        T ExecuteScalar<T>(string query);
        T ExecuteScalar<T>(string commandText, params object[] parameters);
        T ExecuteScalar2<T>(string command, object parameters);
        QueryBuilder<T> From<T>(TableOrSubQuery source = null) where T : class, new();
        QueryBuilder<T> From<T>(SqlSelectBuilder selectCMD) where T : class, new();
        long GetRowCount(string tableName, string selection, params object[] selectionArgs);
        IEnumerable<ColumnInfo> GetTableInfo(string tableName);
        object Insert(object data, string tableName = null, OnConflictOption option = OnConflictOption.Default, object keyValue = null);
        DbConnection OpenConnection();
        IEnumerable<TResult> Query<TResult>(SqlSelectBuilder selectBuilder, object[] selectionArgs) where TResult : new();
        IEnumerable<TResult> Query<TResult>(string commandText, params object[] paramaters) where TResult : new();
        IEnumerable<TResult> Query2<TResult>(string commandText, object paramaters) where TResult : new();
        IEnumerable<GenericEntity> QueryGeneric(string commandText);
        IEnumerable<GenericEntity> QueryGeneric2(string commandText, object paramaters);
        IEnumerable<TResult> QueryScalar<TResult>(string commandText, params object[] paramaters);
        IEnumerable<TResult> QueryScalar2<TResult>(string commandText, object paramaters = null);
        IEnumerable<TResult> Read<TResult>(SqlSelectBuilder selectBuilder, object[] selectionArgs) where TResult : class, new();
        IEnumerable<TResult> Read<TResult>(string commandText, object[] paramaters) where TResult : class, new();
        T ReadSingleRow<T>(object primaryKeyValue) where T : class, new();
        void ReleaseConnection();
        void ReleaseConnection(bool force);
        void RollbackTransaction();
        void Update(object data, string tableName = null, OnConflictOption option = OnConflictOption.Default, object keyValue = null);
    }
}