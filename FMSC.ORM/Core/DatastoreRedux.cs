using FMSC.ORM.Core.SQL.QueryBuilder;
using FMSC.ORM.EntityModel;
using FMSC.ORM.EntityModel.Attributes;
using FMSC.ORM.EntityModel.Support;
using Backpack.SqlBuilder;
using Backpack.SqlBuilder.Dialects;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Common;
using System.Diagnostics;
using System.Linq;

#pragma warning disable RECS0122 // Initializing field with default value is redundant

namespace FMSC.ORM.Core
{
    public abstract partial class DatastoreRedux : IDisposable
    {
        protected static Logger _logger = new Logger();

        protected int _holdConnection = 0;

        protected readonly object _transactionSyncLock = new object();
        protected readonly object _persistentConnectionSyncLock = new object();

        protected bool _transactionCanceled = false;
        public DbTransaction CurrentTransaction { get; protected set; }
        public int TransactionDepth { get; protected set; }

        public DbConnection PersistentConnection { get; protected set; }

        protected Dictionary<Type, EntityCache> _entityCache;

        protected ISqlDialect SqlDialect { get; set; }
        protected IExceptionProcessor ExceptionProcessor { get; set; }
        protected DbProviderFactory ProviderFactory { get; set; }

        public DatabaseBuilder DatabaseBuilder { get; set; }

        private string _path;

        public string Path
        {
            get { return _path; }
            protected set
            {
                if (value == null || value == "") { throw new ArgumentException("Path"); }
                _path = value;
            }
        }

        public object TransactionSyncLock { get { return _transactionSyncLock; } }

        protected DatastoreRedux(ISqlDialect dialect, IExceptionProcessor exceptionProcessor, DbProviderFactory providerFactory)
        {
            SqlDialect = dialect;
            ExceptionProcessor = exceptionProcessor;
            ProviderFactory = providerFactory;
        }

        #region Entity Info

        public void ClearCache(Type type)
        {
            if (_entityCache != null && _entityCache.ContainsKey(type))
            {
                var cache = _entityCache[type];
                cache.Clear();
            }
        }

        protected EntityCache GetEntityCache(Type type)
        {
            if (_entityCache == null) { _entityCache = new Dictionary<Type, EntityCache>(); }
            if (_entityCache.ContainsKey(type) == false)
            {
                var newCache = new EntityCache();
                _entityCache.Add(type, newCache);
                return newCache;
            }
            else
            {
                return _entityCache[type];
            }
        }

        #endregion Entity Info

        #region Abstract Members

        public abstract IEnumerable<ColumnInfo> GetTableInfo(string tableName);

        public abstract Int64 GetRowCount(string tableName, string selection, params Object[] selectionArgs);

        public abstract bool HasForeignKeyErrors(string table_name);

        public abstract long GetLastInsertRowID(DbConnection connection, DbTransaction transaction);

        public abstract object GetLastInsertKeyValue(DbConnection connection, String tableName, String fieldName, DbTransaction transaction);

        #endregion Abstract Members

        #region fluent interface

        public QueryBuilder<T> From<T>() where T : class, new()
        {
            return From<T>((TableOrSubQuery)null);
        }

        public QueryBuilder<T> From<T>(SqlSelectBuilder selectCMD) where T : class, new()
        {
            var source = new TableOrSubQuery(selectCMD, null);
            return From<T>(source);
        }

        public QueryBuilder<T> From<T>(TableOrSubQuery source) where T : class, new()
        {
            EntityDescription entityDescription = GlobalEntityDescriptionLookup.Instance.LookUpEntityByType(typeof(T));
            SqlSelectBuilder builder = entityDescription.CommandBuilder.MakeSelectCommand(source);

            return new QueryBuilder<T>(this, builder);
        }

        #endregion fluent interface

        #region CRUD

        public void Delete(object data)
        {
            lock (_persistentConnectionSyncLock)
            {
                var connection = OpenConnection();
                try
                {
                    connection.Delete(data, CurrentTransaction);
                }
                catch(Exception ex)
                {
                    throw ExceptionProcessor.ProcessException(ex, connection, (string)null, CurrentTransaction);
                }
                finally
                {
                    ReleaseConnection();
                }
            }
        }

        //protected void Delete(IDbConnection connection, object data, IDbTransaction transaction)
        //{
        //    if (data == null) { throw new ArgumentNullException("data"); }

        //    EntityDescription entityDescription = LookUpEntityByType(data.GetType());
        //    PrimaryKeyFieldAttribute keyFieldInfo = entityDescription.Fields.PrimaryKeyField;

        //    if (keyFieldInfo == null) { throw new InvalidOperationException("type doesn't have primary key field"); }

        //    EntityCommandBuilder builder = entityDescription.CommandBuilder;

        //    lock (data)
        //    {
        //        OnDeletingData(data);

        //        using (var command = connection.CreateCommand())
        //        {
        //            builder.BuildSQLDeleteCommand(command, data);

        //            ExecuteNonQuery(connection, command, transaction);
        //        }

        //        if (data is IPersistanceTracking)
        //        {
        //            ((IPersistanceTracking)data).OnDeleted();
        //        }
        //    }
        //}

        public object Insert(object data, OnConflictOption option)
        {
            lock (_persistentConnectionSyncLock)
            {
                var connection = OpenConnection();
                try
                {
                    return Insert(connection, data, CurrentTransaction, option);
                }
                finally
                {
                    ReleaseConnection();
                }
            }
        }

        public object Insert(object data, object keyData, OnConflictOption option)
        {
            lock (_persistentConnectionSyncLock)
            {
                var connection = OpenConnection();
                try
                {
                    return Insert(connection, data, keyData, CurrentTransaction, option);
                }
                finally
                {
                    ReleaseConnection();
                }
            }
        }

        public object Insert(DbConnection connection, object data, DbTransaction transaction, OnConflictOption option)
        {
            if (data == null) { throw new ArgumentNullException("data"); }

            EntityDescription entityDescription = GlobalEntityDescriptionLookup.Instance.LookUpEntityByType(data.GetType());
            var keyField = entityDescription.Fields.PrimaryKeyField;
            object keyData = (keyField != null) ? keyField.GetFieldValue(data) : null;
            return Insert(connection, data, keyData, transaction, option);
        }

        public object Insert(DbConnection connection, object data, object keyData, DbTransaction transaction, OnConflictOption option)
        {
            if (data == null) { throw new ArgumentNullException("data"); }

            EntityDescription entityDescription = GlobalEntityDescriptionLookup.Instance.LookUpEntityByType(data.GetType());
            EntityCommandBuilder builder = entityDescription.CommandBuilder;

            if (data is IPersistanceTracking)
            {
                ((IPersistanceTracking)data).OnInserting();
            }

            using (var command = connection.CreateCommand())
            {
                builder.BuildInsertCommand(command, data, keyData, option);

                try
                {
                    var changes = connection.ExecuteNonQuery(command, transaction);
                    if (changes == 0)   //command did not result in any changes to the database
                    {
                        return null;    //so do not try to get the rowid or call OnInsertedData
                    }
                    else
                    {
                        var primaryKeyField = entityDescription.Fields.PrimaryKeyField;
                        if (primaryKeyField != null)
                        {
                            if (primaryKeyField.KeyType == KeyType.RowID)
                            {
                                keyData = GetLastInsertRowID(connection, transaction);
                            }
                            else
                            {
                                keyData = GetLastInsertKeyValue(connection, entityDescription.SourceName, primaryKeyField.Name, transaction);
                            }

                            primaryKeyField.SetFieldValue(data, keyData);
                        }

                        if (data is IPersistanceTracking)
                        {
                            ((IPersistanceTracking)data).OnInserted();
                        }

                        return keyData;
                    }
                }
                catch (Exception ex)
                {
                    throw ExceptionProcessor.ProcessException(ex, connection, (string)null, CurrentTransaction);
                }
            }
        }

        public void Update(object data, OnConflictOption option)
        {
            lock (_persistentConnectionSyncLock)
            {
                var connection = OpenConnection();
                try
                {
                    connection.Update(data, CurrentTransaction, option);
                }
                catch (Exception ex)
                {
                    throw ExceptionProcessor.ProcessException(ex, connection, (string)null, CurrentTransaction);
                }
                finally
                {
                    ReleaseConnection();
                }
            }
        }

        public void Update(object data, object keyData, OnConflictOption option)
        {
            lock (_persistentConnectionSyncLock)
            {
                var connection = OpenConnection();
                try
                {
                    connection.Update(data, keyData, CurrentTransaction, option);
                }
                catch (Exception ex)
                {
                    throw ExceptionProcessor.ProcessException(ex, connection, (string)null, CurrentTransaction);
                }
                finally
                {
                    ReleaseConnection();
                }
            }
        }

        //public void Update(IDbConnection connection, object data, IDbTransaction transaction, OnConflictOption option)
        //{
        //    if (data == null) { throw new ArgumentNullException("data"); }

        //    EntityDescription entityDescription = LookUpEntityByType(data.GetType());
        //    EntityCommandBuilder builder = entityDescription.CommandBuilder;
        //    var keyField = entityDescription.Fields.PrimaryKeyField;
        //    object keyData = keyField.GetFieldValue(data);

        //    Update(connection, data, keyData, transaction, option);
        //}

        //public void Update(IDbConnection connection, object data, object keyData, IDbTransaction transaction, OnConflictOption option)
        //{
        //    if (data == null) { throw new ArgumentNullException("data"); }

        //    EntityDescription entityDescription = LookUpEntityByType(data.GetType());
        //    EntityCommandBuilder builder = entityDescription.CommandBuilder;

        //    OnUpdatingData(data);

        //    using (var command = connection.CreateCommand())
        //    {
        //        builder.BuildUpdateCommand(command, data, keyData, option);

        //        var changes = ExecuteNonQuery(connection, command, transaction);
        //        if (option != OnConflictOption.Ignore)
        //        {
        //            Debug.Assert(changes > 0, "update command resulted in no changes");
        //        }
        //    }

        //    if (data is IPersistanceTracking)
        //    {
        //        ((IPersistanceTracking)data).OnUpdated();
        //    }
        //}

        public void Save(IPersistanceTracking data, OnConflictOption option, bool cache)
        {
            if (data == null) { throw new ArgumentNullException("data"); }

            if (data is System.ComponentModel.IChangeTracking
                && ((System.ComponentModel.IChangeTracking)data).IsChanged == false)
            {
                _logger.WriteLine("save skipped because data has no changes", Logger.DS_DATA);
                return;
            }

            if (!data.IsPersisted)
            {
                object primaryKey = Insert(data, option);
                if (cache && primaryKey != null)
                {
                    EntityCache cacheStore = GetEntityCache(data.GetType());

                    Debug.Assert(cacheStore.ContainsKey(primaryKey) == false, "Cache already contains entity, existing entity will be replaced");
                    if (cacheStore.ContainsKey(primaryKey))
                    {
                        cacheStore[primaryKey] = data;
                    }
                    else
                    {
                        cacheStore.Add(primaryKey, data);
                    }
                }
            }
            else
            {
                Update(data, option);
            }
        }

        public void Save(DbConnection connection, IPersistanceTracking data, DbTransaction transaction, OnConflictOption option, bool cache)
        {
            if (data == null) { throw new ArgumentNullException("data"); }

            if (data is System.ComponentModel.IChangeTracking
                && ((System.ComponentModel.IChangeTracking)data).IsChanged == false)
            {
                _logger.WriteLine("save skipped because data has no changes", Logger.DS_DATA);
                return;
            }

            if (!data.IsPersisted)
            {
                object primaryKey = Insert(connection, data, transaction, option);
                if (cache && primaryKey != null)
                {
                    EntityCache cacheStore = GetEntityCache(data.GetType());

                    Debug.Assert(cacheStore.ContainsKey(primaryKey) == false, "Cache already contains entity, existing entity will be replaced");
                    if (cacheStore.ContainsKey(primaryKey))
                    {
                        cacheStore[primaryKey] = data;
                    }
                    else
                    {
                        cacheStore.Add(primaryKey, data);
                    }
                }
            }
            else
            {
                try
                {
                    connection.Update(data, transaction, option);
                }
                catch (Exception ex)
                {
                    throw ExceptionProcessor.ProcessException(ex, connection, (string)null, transaction);
                }
            }
        }

        #region read methods

        public IEnumerable<TResult> Read<TResult>(string commandText, object[] paramaters) where TResult : class, new()
        {
            lock (_persistentConnectionSyncLock)
            {
                var connection = OpenConnection();
                try
                {
                    EntityInflator inflator = GlobalEntityDescriptionLookup.Instance.GetEntityInflator(typeof(TResult));
                    EntityCache cache = GetEntityCache(typeof(TResult));//TODO delegate access of cach to data context type

                    using (var reader = ExecuteReader(connection, commandText, paramaters, CurrentTransaction))
                    {
                        inflator.CheckOrdinals(reader);
                        while (reader.Read())
                        {
                            TResult entity = null;
                            try
                            {
                                object key = inflator.ReadPrimaryKey(reader);
                                if (key != null && cache.ContainsKey(key))
                                {
                                    entity = cache[key] as TResult;
                                }
                                else
                                {
                                    entity = new TResult();
                                    if (key != null)
                                    {
                                        cache.Add(key, entity);
                                    }
                                    if (entity is IDataObject)
                                    {
                                        ((IDataObject)entity).DAL = this;
                                    }
                                    if (entity is ISupportInitialize)
                                    {
                                        ((ISupportInitialize)entity).BeginInit();// allow dataobject to suspend property changed notifications or whatever
                                    }
                                    try
                                    {
                                        inflator.ReadData(reader, entity);
                                    }
                                    finally
                                    {
                                        if (entity is ISupportInitialize)
                                        {
                                            ((ISupportInitialize)entity).EndInit();
                                        }
                                    }
                                }
                            }
                            catch (Exception e)
                            {
                                var exceptionProcessor = ExceptionProcessor;
                                if (exceptionProcessor != null)
                                { throw exceptionProcessor.ProcessException(e, connection, commandText, CurrentTransaction); }
                                else { throw; }
                            }

                            yield return entity;
                        }
                    }
                }
                finally
                {
                    ReleaseConnection();
                }
            }
        }

        public IEnumerable<TResult> Read<TResult>(SqlSelectBuilder selectBuilder, object[] selectionArgs) where TResult : class, new()
        {
            return Read<TResult>(selectBuilder.ToString() + ";", selectionArgs);
        }

        //protected IEnumerable<TResult> Read<TResult>(IDbConnection connection, IDbTransaction transaction, string commandText, object[] paramaters) where TResult : class, new()
        //{
        //    using (IDbCommand command = connection.CreateCommand())
        //    {
        //        command.CommandText = commandText;
        //        command.SetParams(paramaters);

        //        EntityDescription entityDescription = LookUpEntityByType(typeof(TResult));
        //        EntityInflator inflator = entityDescription.Inflator;
        //        EntityCache cache = GetEntityCache(typeof(TResult));//TODO delegate access of cach to data context type

        //        using (var reader = ExecuteReader(connection, command, CurrentTransaction))
        //        {
        //            inflator.CheckOrdinals(reader);
        //            while (reader.Read())
        //            {
        //                TResult entity = null;
        //                try
        //                {
        //                    object key = inflator.ReadPrimaryKey(reader);
        //                    if (key != null && cache.ContainsKey(key))
        //                    {
        //                        entity = cache[key] as TResult;
        //                    }
        //                    else
        //                    {
        //                        entity = new TResult();
        //                        if (key != null)
        //                        {
        //                            cache.Add(key, entity);
        //                        }
        //                        if (entity is IDataObject)
        //                        {
        //                            ((IDataObject)entity).DAL = this;
        //                        }

        //                        inflator.ReadData(reader, entity);
        //                    }
        //                }
        //                catch (Exception e)
        //                {
        //                    var exceptionProcessor = ExceptionProcessor;
        //                    if (exceptionProcessor != null)
        //                    { throw exceptionProcessor.ProcessException(e, connection, command.CommandText, transaction); }
        //                    else { throw; }
        //                }

        //                yield return entity;
        //            }
        //        }
        //    }
        //}

        /// <summary>
        /// Retrieves a single row from the database
        /// </summary>
        /// <typeparam name="T">Type of data object to return</typeparam>
        /// <exception cref="SQLException"></exception>
        /// <returns>a single data object</returns>
        public T ReadSingleRow<T>(object primaryKeyValue) where T : class, new()
        {
            var entityDiscription = GlobalEntityDescriptionLookup.Instance.LookUpEntityByType(typeof(T));
            var keyField = entityDiscription.Fields.PrimaryKeyField;

            return From<T>().Where(keyField.Name + " = ?").Read(primaryKeyValue).FirstOrDefault();
            //return ReadSingleRow<T>(null, "WHERE rowID = ?", primaryKeyValue);
        }

        #endregion read methods

        #region query methods

        public IEnumerable<TResult> Query<TResult>(String commandText, Object[] paramaters) where TResult : new()
        {
            lock (_persistentConnectionSyncLock)
            {
                var connection = OpenConnection();
                try
                {
                    FMSC.ORM.EntityModel.Support.EntityInflator inflator = GlobalEntityDescriptionLookup.Instance.GetEntityInflator(typeof(TResult));

                    using (var reader = ExecuteReader(connection, commandText, paramaters, CurrentTransaction))
                    {
                        //HACK with microsoft.data.sqlite calling GetOrdinal throws exception if reader is empty
                        //if(reader is DbDataReader && ((DbDataReader)reader).HasRows == false) { yield break; }
                        inflator.CheckOrdinals(reader);

                        while (reader.Read())
                        {
                            TResult newDO = new TResult();
                            if (newDO is IDataObject)
                            {
                                ((IDataObject)newDO).DAL = this;
                            }
                            if (newDO is ISupportInitialize)
                            {
                                ((ISupportInitialize)newDO).BeginInit();// allow dataobject to suspend property changed notifications or whatever
                            }
                            try
                            {
                                inflator.ReadData(reader, newDO);
                            }
                            catch (Exception e)
                            {
                                var exceptionProcessor = ExceptionProcessor;
                                if (exceptionProcessor != null)
                                {
                                    throw exceptionProcessor.ProcessException(e, connection, commandText, CurrentTransaction);
                                }
                                else
                                {
                                    throw;
                                }
                            }
                            finally
                            {
                                if (newDO is ISupportInitialize)
                                {
                                    ((ISupportInitialize)newDO).EndInit();
                                }
                            }

                            yield return newDO;
                        }
                    }
                }
                finally
                {
                    ReleaseConnection();
                }
            }
        }

        public IEnumerable<TResult> Query<TResult>(SqlSelectBuilder selectBuilder, Object[] selectionArgs) where TResult : new()
        {
            return Query<TResult>(selectBuilder.ToString() + ";", selectionArgs);
        }

        //protected IEnumerable<TResult> Query<TResult>(IDbConnection connection, IDbTransaction transaction, String commandText, Object[] paramaters) where TResult : new()
        //{
        //    using (IDbCommand command = connection.CreateCommand())
        //    {
        //        command.CommandText = commandText;
        //        command.SetParams(paramaters);

        //        FMSC.ORM.EntityModel.Support.EntityDescription entityDescription = LookUpEntityByType(typeof(TResult));
        //        FMSC.ORM.EntityModel.Support.EntityInflator inflator = entityDescription.Inflator;

        //        using (var reader = ExecuteReader(connection, command, CurrentTransaction))
        //        {
        //            inflator.CheckOrdinals(reader);

        //            while (reader.Read())
        //            {
        //                TResult newDO = new TResult();
        //                if (newDO is IDataObject)
        //                {
        //                    ((IDataObject)newDO).DAL = this;
        //                }
        //                try
        //                {
        //                    inflator.ReadData(reader, newDO);
        //                }
        //                catch (Exception e)
        //                {
        //                    var exceptionProcessor = ExceptionProcessor;
        //                    if (exceptionProcessor != null)
        //                    {
        //                        throw exceptionProcessor.ProcessException(e, connection, commandText, transaction);
        //                    }
        //                    else
        //                    {
        //                        throw;
        //                    }
        //                }
        //                yield return newDO;
        //            }
        //        }
        //    }
        //}

        #endregion query methods

        #endregion CRUD

        #region general purpose command execution

        //public IDataReader ExecuteReader(IDbConnection connection, string commandText, object[] paramaters, IDbTransaction transaction)
        //{
        //    using (var command = connection.CreateCommand())
        //    {
        //        command.CommandText = commandText;
        //        command.SetParams(paramaters);

        //        command.Transaction = transaction;

        //        _logger.LogCommand(command);

        //        try
        //        {
        //            return command.ExecuteReader();
        //        }
        //        catch (Exception e)
        //        {
        //            var exceptionProcessor = ExceptionProcessor;
        //            if (exceptionProcessor != null)
        //            { throw exceptionProcessor.ProcessException(e, connection, command.CommandText, transaction); }
        //            else { throw; }
        //        }
        //    }
        //}

        //public IDataReader ExecuteReader(IDbConnection connection, IDbCommand command, IDbTransaction transaction)
        //{
        //    if (connection == null) { throw new ArgumentNullException("connection"); }
        //    if (command == null) { throw new ArgumentNullException("command"); }

        //    command.Connection = connection;
        //    command.Transaction = transaction;

        //    _logger.LogCommand(command);
        //    try
        //    {
        //        return command.ExecuteReader();
        //    }
        //    catch (Exception e)
        //    {
        //        var exceptionProcessor = ExceptionProcessor;
        //        if (exceptionProcessor != null)
        //        { throw exceptionProcessor.ProcessException(e, connection, command.CommandText, transaction); }
        //        else { throw; }
        //    }
        //}

        //public int ExecuteNonQuery(IDbConnection connection, string commandText, object[] parameters, IDbTransaction transaction)
        //{
        //    if (string.IsNullOrEmpty(commandText)) { throw new ArgumentException("command can't be null or empty", "command"); }

        //    using (IDbCommand command = connection.CreateCommand())
        //    {
        //        command.CommandText = commandText;
        //        command.SetParams(parameters);

        //        return ExecuteNonQuery(connection, command, transaction);
        //    }
        //}

        //public int ExecuteNonQuery(IDbConnection connection, IDbCommand command, IDbTransaction transaction)
        //{
        //    if (connection == null) { throw new ArgumentNullException("connection"); }
        //    if (command == null) { throw new ArgumentNullException("command"); }

        //    command.Connection = connection;
        //    command.Transaction = transaction;

        //    _logger.LogCommand(command);
        //    try
        //    {
        //        return command.ExecuteNonQuery();
        //    }
        //    catch (Exception e)
        //    {
        //        var exceptionProcessor = ExceptionProcessor;
        //        if (exceptionProcessor != null)
        //        { throw exceptionProcessor.ProcessException(e, connection, command.CommandText, transaction); }
        //        else { throw; }
        //    }
        //}

        //public object ExecuteScalar(IDbConnection connection, string commandText, object[] parameters, IDbTransaction transaction)
        //{
        //    using (var command = connection.CreateCommand())
        //    {
        //        command.CommandText = commandText;
        //        command.SetParams(parameters);

        //        command.Transaction = transaction;

        //        try
        //        {
        //            return command.ExecuteScalar();
        //        }
        //        catch (Exception e)
        //        {
        //            var exceptionProcessor = ExceptionProcessor;
        //            if (exceptionProcessor != null)
        //            { throw exceptionProcessor.ProcessException(e, connection, commandText, transaction); }
        //            else { throw; }
        //        }
        //    }
        //}

        //public T ExecuteScalar<T>(IDbConnection connection, string commandText, object[] parameters, IDbTransaction transaction)
        //{
        //    var result = ExecuteScalar(connection, commandText, parameters, transaction);

        //    if (result is DBNull)
        //    {
        //        return default(T);
        //    }
        //    else if (result is T)
        //    {
        //        return (T)result;
        //    }
        //    else
        //    {
        //        Type targetType = typeof(T);

        //        if (targetType.IsGenericType && targetType.GetGenericTypeDefinition() == typeof(Nullable<>))
        //        {
        //            targetType = Nullable.GetUnderlyingType(targetType);
        //        }

        //        if (result is IConvertible)
        //        {
        //            return (T)Convert.ChangeType(result, targetType
        //                , System.Globalization.CultureInfo.CurrentCulture);
        //        }
        //        else
        //        {
        //            try
        //            {
        //                return (T)result;
        //            }
        //            catch(InvalidCastException)
        //            {
        //                return (T)Activator.CreateInstance(targetType, result);
        //            }
        //        }
        //    }
        //}

        protected DbDataReader ExecuteReader(DbConnection connection, string commandText, object[] paramaters, DbTransaction transaction)
        {
            try
            {
                return connection.ExecuteReader(commandText, paramaters, transaction);
            }
            catch (Exception ex)
            {
                throw ExceptionProcessor.ProcessException(ex, connection, commandText, CurrentTransaction);
            }
        }

        protected DbDataReader ExecuteReader2(DbConnection connection, string commandText, object paramaterData, DbTransaction transaction)
        {
            try
            {
                return connection.ExecuteReader2(commandText, paramaterData, transaction);
            }
            catch (Exception ex)
            {
                throw ExceptionProcessor.ProcessException(ex, connection, commandText, CurrentTransaction);
            }
        }

        //protected int ExecuteNonQuery(DbConnection connection, string commandText, object[] parameters, DbTransaction transaction)
        //{
        //    try
        //    {
        //        return connection.ExecuteNonQuery(commandText, parameters, transaction);
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ExceptionProcessor.ProcessException(ex, connection, commandText, transaction);
        //    }
        //}

        //protected int ExecuteNonQuery(DbConnection connection, DbCommand command, DbTransaction transaction)
        //{
        //    try
        //    {
        //        return connection.ExecuteNonQuery(command, transaction);
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ExceptionProcessor.ProcessException(ex, connection, command.CommandText, CurrentTransaction);
        //    }
        //}

        /// <summary>
        /// Executes SQL command returning number of rows affected
        /// </summary>
        /// <param name="commandText"></param>
        /// <returns></returns>
        public int Execute(String commandText, params object[] parameters)
        {
            if (string.IsNullOrEmpty(commandText)) { throw new ArgumentNullException("command"); }

            lock (_persistentConnectionSyncLock)
            {
                var connection = OpenConnection();
                try
                {
                    return connection.ExecuteNonQuery(commandText, parameters, CurrentTransaction);
                }
                catch (Exception ex)
                {
                    throw ExceptionProcessor.ProcessException(ex, connection, commandText, CurrentTransaction);
                }
                finally
                {
                    ReleaseConnection();
                }
            }
        }

        /// <summary>
        /// Executes SQL command returning single value
        /// </summary>
        /// <param name="command"></param>
        /// <returns>value or null</returns>
        public object ExecuteScalar(string commandText, params object[] parameters)
        {
            lock (_persistentConnectionSyncLock)
            {
                var conn = OpenConnection();
                try
                {
                    return conn.ExecuteScalar(commandText, parameters, CurrentTransaction);
                }
                catch (Exception e)
                {
                    throw ExceptionProcessor.ProcessException(e, conn, commandText, (DbTransaction)null);
                }
                finally
                {
                    ReleaseConnection();
                }
            }
        }

        public T ExecuteScalar<T>(String query)
        {
            return ExecuteScalar<T>(query, (object[])null);
        }

        public T ExecuteScalar<T>(String commandText, params object[] parameters)
        {
            lock (_persistentConnectionSyncLock)
            {
                var conn = OpenConnection();
                try
                {
                    return conn.ExecuteScalar<T>(commandText, parameters, CurrentTransaction);
                }
                catch(Exception ex)
                {
                    throw ExceptionProcessor.ProcessException(ex, conn, commandText, CurrentTransaction);
                }
                finally
                {
                    ReleaseConnection();
                }
            }
        }

        #endregion general purpose command execution

        #region transaction management

        public void BeginTransaction()
        {
            lock (TransactionSyncLock)
            {
                if (TransactionDepth == 0)
                {
                    Debug.Assert(CurrentTransaction == null);

                    var connection = OpenConnection();

                    try
                    {
                        var newTransaction = connection.BeginTransaction();
                        CurrentTransaction = newTransaction;
                    }
                    catch (Exception ex)
                    {
                        ReleaseConnection();
                        throw ExceptionProcessor.ProcessException(ex, connection, (string)null, CurrentTransaction);
                    }

                    _transactionCanceled = false;

                    this.EnterConnectionHold();
                    OnTransactionStarted();
                }
                TransactionDepth++;
            }
        }

        public void CommitTransaction()
        {
            lock (TransactionSyncLock)
            {
                var transactionDepth = TransactionDepth;

                OnTransactionEnding();

                if (transactionDepth > 0)
                {
                    transactionDepth--;
                    if (transactionDepth == 0)//transaction depth was 1
                    {
                        ReleaseTransaction();
                    }
                    TransactionDepth = transactionDepth;
                }
                //else// transactionDepth <= 0
                //{
                //    Debug.Fail("Transaction depth is " + transactionDepth.ToString());
                //}
            }
        }

        public void RollbackTransaction()
        {
            lock (TransactionSyncLock)
            {
                var transactionDepth = TransactionDepth;

                OnTransactionCanceling();
                _transactionCanceled = true;

                if (transactionDepth > 0)
                {
                    transactionDepth--;
                    if (transactionDepth == 0)
                    {
                        ReleaseTransaction();
                    }
                    TransactionDepth = transactionDepth;
                }
                //else
                //{
                //    Debug.Fail("Transaction depth is " + transactionDepth.ToString());
                //}
            }
        }

        private void ReleaseTransaction()
        {
            OnTransactionReleasing();

            try
            {
                if (CurrentTransaction != null)
                {
                    if (_transactionCanceled)
                    {
                        CurrentTransaction.Rollback();
                    }
                    else
                    {
                        CurrentTransaction.Commit();
                    }
                }
                else
                { Debug.Fail("_currentTransaction is null"); }
            }
            catch (Exception ex)
            {
                throw ExceptionProcessor.ProcessException(ex, PersistentConnection, (string)null, CurrentTransaction);
            }
            finally
            {
                if (CurrentTransaction != null)
                { CurrentTransaction.Dispose(); }
                CurrentTransaction = null;
                ExitConnectionHold();
                ReleaseConnection();
            }
        }

        #endregion transaction management

        #region Connection Management

        protected void EnterConnectionHold()
        {
            System.Threading.Interlocked.Increment(ref this._holdConnection);
        }

        protected void ExitConnectionHold()
        {
            Debug.Assert(_holdConnection > 0);
            System.Threading.Interlocked.Decrement(ref this._holdConnection);
        }

        public DbConnection CreateConnection()
        {
            var conn = ProviderFactory.CreateConnection();
            conn.ConnectionString = BuildConnectionString();

            return conn;
        }

        protected abstract string BuildConnectionString();

        /// <summary>
        /// if _holdConnection > 0 returns PersistentConnection
        /// if _holdConnection creates new connection and return it
        /// increments _holdConnection if connection successfully opened
        /// </summary>
        /// <exception cref="ConnectionException"></exception>
        /// <returns></returns>
        public virtual DbConnection OpenConnection()
        {
            lock (_persistentConnectionSyncLock)
            {
                DbConnection conn;
                if (_holdConnection == 0)
                {
                    conn = CreateConnection();
                }
                else
                {
                    Debug.Assert(PersistentConnection != null);
                    conn = PersistentConnection;
                }

                try
                {
                    if (conn.State == System.Data.ConnectionState.Broken)
                    {
                        conn.Close();
                    }

                    if (conn.State == System.Data.ConnectionState.Closed)
                    {
                        conn.Open();
                        PersistentConnection = conn;
                        OnConnectionOpened();
                    }

                    EnterConnectionHold();

                    return conn;
                }
                catch (Exception e)
                {
                    throw new ConnectionException("failed to open connection", e);
                }
            }
        }

        public void ReleaseConnection()
        {
            ReleaseConnection(false);
        }

        public virtual void ReleaseConnection(bool force)
        {
            lock (_persistentConnectionSyncLock)
            {
                if (_holdConnection > 0)
                {
                    if (force)
                    {
                        _holdConnection = 0;
                    }
                    else
                    {
                        ExitConnectionHold();
                    }
                    if (_holdConnection == 0)
                    {
                        Debug.Assert(PersistentConnection != null);
                        PersistentConnection.Dispose();
                        PersistentConnection = null;
                    }
                }
            }
        }

        #endregion Connection Management

        #region events and logging

        protected virtual void OnDeletingData(object data)
        {
            if (data is IPersistanceTracking)
            {
                ((IPersistanceTracking)data).OnDeleting();
            }
        }

        protected virtual void OnInsertingData(object data, OnConflictOption option)
        {
            if (data is IPersistanceTracking)
            {
                ((IPersistanceTracking)data).OnInserting();
            }
        }

        protected virtual void OnInsertedData(object data)
        {
            if (data is IPersistanceTracking)
            {
                ((IPersistanceTracking)data).OnInserted();
            }
            if (data is System.ComponentModel.IChangeTracking)
            {
                ((System.ComponentModel.IChangeTracking)data).AcceptChanges();
            }
        }

        protected virtual void OnUpdatingData(object data)
        {
            if (data is IPersistanceTracking)
            {
                ((IPersistanceTracking)data).OnUpdating();
            }
        }

        /// <summary>
        /// called when connection is in use
        /// </summary>

        protected virtual void OnConnectionOpened()
        {
            _logger.WriteLine("Connection opened", Logger.DB_CONTROL);
        }

        protected virtual void OnTransactionStarted()
        {
            _logger.WriteLine("Transaction Started", Logger.DB_CONTROL);
        }

        protected virtual void OnTransactionEnding()
        {
            _logger.WriteLine("Transaction Ending", Logger.DB_CONTROL);
        }

        protected virtual void OnTransactionCanceling()
        {
            _logger.WriteLine("Transaction Canceling", Logger.DB_CONTROL);
        }

        protected virtual void OnTransactionReleasing()
        {
            _logger.WriteLine("Transaction Releasing", Logger.DB_CONTROL);
        }

        #endregion events and logging

        /// <summary>
        /// Adds field to table.
        /// </summary>
        /// <param name="tableName">Name of the table.</param>
        /// <param name="fieldDef">The field definition.</param>
        public void AddField(string tableName, ColumnInfo fieldDef)
        {
            var command = string.Format("ALTER TABLE {0} ADD COLUMN {1};", tableName, SqlDialect.GetColumnDefinition(fieldDef));
            Execute(command);
        }

        public void CreateTable(string tableName, IEnumerable<ColumnInfo> cols, bool temp)
        {
            lock (_persistentConnectionSyncLock)
            {
                var connection = OpenConnection();
                try
                {
                    CreateTable(connection, tableName, cols, temp, CurrentTransaction);
                }
                finally
                {
                    ReleaseConnection();
                }
            }
        }

        public void CreateTable(DbConnection connection, string tableName, IEnumerable<ColumnInfo> cols, bool temp, DbTransaction transaction)
        {
            var createTableCommand = new CreateTable(SqlDialect)
            {
                TableName = tableName,
                Columns = cols,
                Temp = temp
            };

            connection.ExecuteNonQuery(createTableCommand.ToString() + ";", (object[])null, transaction);
        }

        #region IDisposable Support

        private bool isDisposed = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!isDisposed)
            {
                if (disposing)
                {
                    _logger.WriteLine("Datastore Disposing ", Logger.DS_EVENT);
                }
                else
                {
                    _logger.WriteLine("Datastore Finalized ", Logger.DS_EVENT);
                }

                //if(this._cache != null)
                //{
                //    _cache.Dispose();
                //}

                ReleaseConnection(true);

                isDisposed = true;
            }
        }

        ~DatastoreRedux()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion IDisposable Support
    }
}