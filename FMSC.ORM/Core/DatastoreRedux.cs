using FMSC.ORM.Core.SQL;
using FMSC.ORM.Core.SQL.QueryBuilder;
using FMSC.ORM.EntityModel;
using FMSC.ORM.EntityModel.Attributes;
using FMSC.ORM.EntityModel.Support;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Diagnostics;
using System.Linq;

#pragma warning disable RECS0122 // Initializing field with default value is redundant

namespace FMSC.ORM.Core
{
    public abstract partial class DatastoreRedux : IDisposable
    {
        protected static Dictionary<string, EntityDescription> _globalEntityDescriptionLookup = new Dictionary<string, EntityDescription>();

        protected int _holdConnection = 0;
        protected int _transactionDepth = 0;
        protected bool _transactionCanceled = false;
        protected readonly object _transactionSyncLock = new object();
        protected readonly object _persistentConnectionSyncLock = new object();

        protected IDbTransaction _CurrentTransaction;
        protected Dictionary<Type, EntityCache> _entityCache;

        protected IDbConnection PersistentConnection { get; set; }
        protected DbProviderFactoryAdapter Provider { get; set; }

        public DatabaseBuilder DatabaseBuilder { get; set; }
        public string Path { get; protected set; }
        public object TransactionSyncLock { get { return _transactionSyncLock; } }

        protected DatastoreRedux(DbProviderFactoryAdapter provider)
        {
            this.Provider = provider;
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

        private EntityInflator GetEntityInflator(Type type)
        {
            return LookUpEntityByType(type).Inflator;
        }

        public static EntityDescription LookUpEntityByType(Type t)
        {
            string name = t.Name;
            if (!_globalEntityDescriptionLookup.ContainsKey(name))
            {
                _globalEntityDescriptionLookup.Add(name, new EntityDescription(t));
            }

            return _globalEntityDescriptionLookup[t.Name];
        }

        #endregion Entity Info

        #region Abstract Members

        protected abstract string BuildConnectionString();

        public abstract IEnumerable<ColumnInfo> GetTableInfo(string tableName);

        public abstract Int64 GetRowCount(string tableName, string selection, params Object[] selectionArgs);

        public abstract bool HasForeignKeyErrors(string table_name);

        protected abstract Exception ThrowExceptionHelper(IDbConnection conn, IDbCommand comm, Exception innerException);

        #endregion Abstract Members

        #region fluent interface

        public QueryBuilder<T> From<T>() where T : class, new()
        {
            return From<T>((SelectSource)null);
        }

        public QueryBuilder<T> From<T>(SQLSelectBuilder selectCMD) where T : class, new()
        {
            var source = new TableOrSubQuery(selectCMD, null);
            return From<T>(source);
        }

        public QueryBuilder<T> From<T>(SelectSource source) where T : class, new()
        {
            EntityDescription entityDescription = LookUpEntityByType(typeof(T));
            SQLSelectBuilder builder = entityDescription.CommandBuilder.MakeSelectCommand(source);

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
                    Delete(connection, data, _CurrentTransaction);
                }
                finally
                {
                    ReleaseConnection();
                }
            }
        }

        protected void Delete(IDbConnection connection, object data, IDbTransaction transaction)
        {
            if (data == null) { throw new ArgumentNullException("data"); }

            EntityDescription entityDescription = LookUpEntityByType(data.GetType());
            PrimaryKeyFieldAttribute keyFieldInfo = entityDescription.Fields.PrimaryKeyField;

            if (keyFieldInfo == null) { throw new InvalidOperationException("type doesn't have primary key field"); }

            EntityCommandBuilder builder = entityDescription.CommandBuilder;

            lock (data)
            {
                OnDeletingData(data);

                using (var command = connection.CreateCommand())
                {
                    builder.BuildSQLDeleteCommand(command, data);

                    ExecuteNonQuery(connection, command, transaction);
                }

                if (data is IPersistanceTracking)
                {
                    ((IPersistanceTracking)data).OnDeleted();
                }
            }
        }

#if NetCF
        public object Insert(object data, SQL.OnConflictOption option)
#else

        public object Insert(object data, SQL.OnConflictOption option = OnConflictOption.Default)
#endif
        {
            lock (_persistentConnectionSyncLock)
            {
                var connection = OpenConnection();
                try
                {
                    return Insert(connection, data, _CurrentTransaction, option);
                }
                finally
                {
                    ReleaseConnection();
                }
            }
        }

#if NetCF
        public object Insert(object data, object keyData, SQL.OnConflictOption option)
#else

        public object Insert(object data, object keyData, SQL.OnConflictOption option = OnConflictOption.Default)
#endif
        {
            lock (_persistentConnectionSyncLock)
            {
                var connection = OpenConnection();
                try
                {
                    return Insert(connection, data, keyData, _CurrentTransaction, option);
                }
                finally
                {
                    ReleaseConnection();
                }
            }
        }

        public object Insert(IDbConnection connection, object data, IDbTransaction transaction, SQL.OnConflictOption option)
        {
            if (data == null) { throw new ArgumentNullException("data"); }

            EntityDescription entityDescription = LookUpEntityByType(data.GetType());
            var keyField = entityDescription.Fields.PrimaryKeyField;
            object keyData = (keyField != null) ? keyField.GetFieldValue(data) : null;
            return Insert(connection, data, keyData, transaction, option);
        }

        public object Insert(IDbConnection connection, object data, object keyData, IDbTransaction transaction, SQL.OnConflictOption option)
        {
            if (data == null) { throw new ArgumentNullException("data"); }

            EntityDescription entityDescription = LookUpEntityByType(data.GetType());
            EntityCommandBuilder builder = entityDescription.CommandBuilder;

            OnInsertingData(data, option);

            using (var command = connection.CreateCommand())
            {
                builder.BuildInsertCommand(command, data, keyData, option);

                var changes = ExecuteNonQuery(connection, command, transaction);
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
                            keyData = GetLastInsertKeyValue(connection, entityDescription.SourceName
, primaryKeyField.Name, transaction);
                        }

                        primaryKeyField.SetFieldValue(data, keyData);
                    }

                    OnInsertedData(data);

                    return keyData;
                }
            }
        }

        protected long GetLastInsertRowID(IDbConnection conn, IDbTransaction transaction)
        {
            using (var command = conn.CreateCommand())
            {
                command.CommandText = "SELECT last_insert_rowid()";
                return this.ExecuteScalar<long>(conn, command, transaction);
            }
        }

        protected object GetLastInsertKeyValue(IDbConnection conn, String tableName, String fieldName, IDbTransaction transaction)
        {
            var ident = GetLastInsertRowID(conn, transaction);

            var query = "SELECT " + fieldName + " FROM " + tableName + " WHERE rowid = ?;";

            return ExecuteScalar(conn, query, new object[] { ident }, transaction);

            //String query = "Select " + fieldName + " FROM " + tableName + " WHERE rowid = last_insert_rowid();";
        }

#if NetCF
        public void Update(object data, SQL.OnConflictOption option)
#else

        public void Update(object data, SQL.OnConflictOption option = OnConflictOption.Default)
#endif
        {
            lock (_persistentConnectionSyncLock)
            {
                var connection = OpenConnection();
                try
                {
                    Update(connection, data, _CurrentTransaction, option);
                }
                finally
                {
                    ReleaseConnection();
                }
            }
        }

        public void Update(object data, object keyData, SQL.OnConflictOption option)
        {
            lock (_persistentConnectionSyncLock)
            {
                var connection = OpenConnection();
                try
                {
                    Update(connection, data, keyData, _CurrentTransaction, option);
                }
                finally
                {
                    ReleaseConnection();
                }
            }
        }

        public void Update(IDbConnection connection, object data, IDbTransaction transaction, SQL.OnConflictOption option)
        {
            if (data == null) { throw new ArgumentNullException("data"); }

            EntityDescription entityDescription = LookUpEntityByType(data.GetType());
            EntityCommandBuilder builder = entityDescription.CommandBuilder;
            var keyField = entityDescription.Fields.PrimaryKeyField;
            object keyData = keyField.GetFieldValue(data);

            Update(connection, data, keyData, transaction, option);
        }

        public void Update(IDbConnection connection, object data, object keyData, IDbTransaction transaction, SQL.OnConflictOption option)
        {
            if (data == null) { throw new ArgumentNullException("data"); }

            EntityDescription entityDescription = LookUpEntityByType(data.GetType());
            EntityCommandBuilder builder = entityDescription.CommandBuilder;

            OnUpdatingData(data);

            using (var command = connection.CreateCommand())
            {
                builder.BuildUpdateCommand(command, data, keyData, option);

                var changes = ExecuteNonQuery(connection, command, transaction);
                if (option != OnConflictOption.Ignore)
                {
                    Debug.Assert(changes > 0, "update command resulted in no changes");
                }
            }

            if (data is IPersistanceTracking)
            {
                ((IPersistanceTracking)data).OnUpdated();
            }
        }

#if NetCF
        public void Save(IPersistanceTracking data)
        {
            Save(data, OnConflictOption.Default, true);
        }

        public void Save(IPersistanceTracking data, SQL.OnConflictOption option)
        {
            Save(data, option, true);
        }

        public void Save(IPersistanceTracking data, SQL.OnConflictOption option, bool cache)
#else

        public void Save(IPersistanceTracking data, SQL.OnConflictOption option = OnConflictOption.Default, bool cache = true)
#endif
        {
            if (data == null) { throw new ArgumentNullException("data"); }

            if (data is System.ComponentModel.IChangeTracking
                && ((System.ComponentModel.IChangeTracking)data).IsChanged == false)
            {
                Debug.Write("save skipped because data has no changes");
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

        public void Save(IDbConnection connection, IPersistanceTracking data, IDbTransaction transaction, SQL.OnConflictOption option = OnConflictOption.Default, bool cache = true)
        {
            if (data == null) { throw new ArgumentNullException("data"); }

            if (data is System.ComponentModel.IChangeTracking
                && ((System.ComponentModel.IChangeTracking)data).IsChanged == false)
            {
                Debug.Write("save skipped because data has no changes");
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
                Update(connection, data, transaction, option);
            }
        }

        #region read methods

        public IEnumerable<TResult> Read<TResult>(String commandText, Object[] selectionArgs) where TResult : class, new()
        {
            lock (_persistentConnectionSyncLock)
            {
                var connection = OpenConnection();
                try
                {
                    return Read<TResult>(connection, _CurrentTransaction, commandText, selectionArgs);
                }
                finally
                {
                    ReleaseConnection();
                }
            }
        }

        public IEnumerable<TResult> Read<TResult>(SQLSelectBuilder selectBuilder, Object[] selectionArgs) where TResult : class, new()
        {
            return Read<TResult>(selectBuilder.ToSQL() + ";", selectionArgs);
        }

        protected IEnumerable<TResult> Read<TResult>(IDbConnection connection, IDbTransaction transaction, string commandText, Object[] paramaters) where TResult : class, new()
        {
            using (IDbCommand command = connection.CreateCommand())
            {
                command.CommandText = commandText;
                command.SetParams(paramaters);

                EntityDescription entityDescription = LookUpEntityByType(typeof(TResult));
                EntityInflator inflator = entityDescription.Inflator;
                EntityCache cache = GetEntityCache(typeof(TResult));//TODO delegate access of cach to data context type

                using (var reader = ExecuteReader(connection, command, _CurrentTransaction))
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

                                inflator.ReadData(reader, entity);
                            }
                        }
                        catch (Exception e)
                        {
                            throw this.ThrowExceptionHelper(connection, command, e);
                        }

                        yield return entity;
                    }
                }
            }
        }

        /// <summary>
        /// Retrieves a single row from the database
        /// </summary>
        /// <typeparam name="T">Type of data object to return</typeparam>
        /// <exception cref="SQLException"></exception>
        /// <returns>a single data object</returns>
        public T ReadSingleRow<T>(object primaryKeyValue) where T : class, new()
        {
            var entityDiscription = LookUpEntityByType(typeof(T));
            var keyField = entityDiscription.Fields.PrimaryKeyField;

            return From<T>().Where(keyField.Name + " = ?").Read(primaryKeyValue).FirstOrDefault();
            //return ReadSingleRow<T>(null, "WHERE rowID = ?", primaryKeyValue);
        }

        internal T ReadSingleRow<T>(IDbCommand command, EntityDescription entityDescription)
            where T : new()
        {
            object entity = null;
            EntityCache cache = GetEntityCache(typeof(T));
            EntityInflator inflator = entityDescription.Inflator;

            lock (_persistentConnectionSyncLock)
            {
                var conn = OpenConnection();
                try
                {
                    using (var reader = ExecuteReader(conn, command, _CurrentTransaction))
                    {
                        inflator.CheckOrdinals(reader);

                        if (reader.Read())
                        {
                            object key = inflator.ReadPrimaryKey(reader);
                            if (cache.ContainsKey(key))
                            {
                                entity = cache[key];
                            }
                            else
                            {
                                entity = inflator.CreateInstanceOfEntity();
                                if (entity is IDataObject)
                                {
                                    ((IDataObject)entity).DAL = this;
                                }
                                inflator.ReadData(reader, entity);
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    throw this.ThrowExceptionHelper(conn, command, e);
                }
                finally
                {
                    ReleaseConnection();
                }
                return (T)entity;
            }
        }

        #endregion read methods

        #region query methods

        public IEnumerable<TResult> Query<TResult>(String commandText, Object[] selectionArgs) where TResult : new()
        {
            lock (_persistentConnectionSyncLock)
            {
                var connection = OpenConnection();
                try
                {
                    return Query<TResult>(connection, _CurrentTransaction, commandText, selectionArgs);
                }
                finally
                {
                    ReleaseConnection();
                }
            }
        }

        public IEnumerable<TResult> Query<TResult>(SQLSelectBuilder selectBuilder, Object[] selectionArgs) where TResult : new()
        {
            return Query<TResult>(selectBuilder.ToSQL() + ";", selectionArgs);
        }

        protected IEnumerable<TResult> Query<TResult>(IDbConnection connection, IDbTransaction transaction, String commandText, Object[] paramaters) where TResult : new()
        {
            using (IDbCommand command = connection.CreateCommand())
            {
                command.CommandText = commandText;
                command.SetParams(paramaters);


                EntityDescription entityDescription = LookUpEntityByType(typeof(TResult));
                EntityInflator inflator = entityDescription.Inflator;

                using (var reader = ExecuteReader(connection, command, _CurrentTransaction))
                {
                    inflator.CheckOrdinals(reader);

                    while (reader.Read())
                    {
                        TResult newDO = new TResult();
                        if (newDO is IDataObject)
                        {
                            ((IDataObject)newDO).DAL = this;
                        }
                        try
                        {
                            inflator.ReadData(reader, newDO);
                        }
                        catch (Exception e)
                        {
                            throw this.ThrowExceptionHelper(connection, command, e);
                        }
                        yield return newDO;
                    }
                }
            }
        }

        #endregion query methods

        #endregion CRUD

        #region general purpose command execution

        /// <summary>
        /// Executes SQL command returning number of rows affected
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        public int Execute(String command, params object[] parameters)
        {
            if (string.IsNullOrEmpty(command)) { throw new ArgumentNullException("command"); }

            lock (_persistentConnectionSyncLock)
            {
                var connection = OpenConnection();
                try
                {
                    return ExecuteNonQuery(connection,command, parameters, _CurrentTransaction);
                }
                finally
                {
                    ReleaseConnection();
                }
            }
        }

        public IDataReader ExecuteReader(IDbConnection connection, IDbCommand command, IDbTransaction transaction)
        {
            if (connection == null) { throw new ArgumentNullException("connection"); }
            if (command == null) { throw new ArgumentNullException("command"); }

            command.Connection = connection;
            command.Transaction = transaction;

            LogCommand(command);
            try
            {

                return command.ExecuteReader();
            }
            catch (Exception e)
            {
                throw ThrowExceptionHelper(command.Connection, command, e);
            }
        }

        protected int ExecuteNonQuery(IDbConnection conn, string commandText, object[] parameters, IDbTransaction transaction)
        {
            if (string.IsNullOrEmpty(commandText)) { throw new ArgumentException("command can't be null or empty", "command"); }

            using (IDbCommand command = conn.CreateCommand())
            {
                command.CommandText = commandText;
                command.SetParams(parameters);

                return ExecuteNonQuery(conn, command, transaction);
            }
        }

        protected int ExecuteNonQuery(IDbConnection connection, IDbCommand command, IDbTransaction transaction)
        {
            if (connection == null) { throw new ArgumentNullException("connection"); }
            if (command == null) { throw new ArgumentNullException("command"); }

            command.Connection = connection;
            command.Transaction = transaction;

            LogCommand(command);
            try
            {
                return command.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                throw this.ThrowExceptionHelper(connection, command, e);
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
                    return ExecuteScalar(conn, commandText, parameters, _CurrentTransaction);
                }
                finally
                {
                    ReleaseConnection();
                }
            }
        }

        protected object ExecuteScalar(IDbConnection connection, string commandText, object[] parameters, IDbTransaction transaction)
        {
            using (var command = connection.CreateCommand())
            {
                command.CommandText = commandText;
                command.SetParams(parameters);

                return ExecuteScalar(connection, command, transaction);
            }
        }

        //TODO make static
        protected object ExecuteScalar(IDbConnection connection, IDbCommand command, IDbTransaction transaction)
        {
            if (connection == null) { throw new ArgumentNullException("connection"); }
            if (command == null) { throw new ArgumentNullException("command"); }

            command.Connection = connection;
            command.Transaction = transaction;

            LogCommand(command);
            try
            {
                return command.ExecuteScalar();
            }
            catch (Exception e)
            {
                throw this.ThrowExceptionHelper(connection, command, e);
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
                    return ExecuteScalar<T>(conn, commandText, parameters, _CurrentTransaction);
                }
                finally
                {
                    ReleaseConnection();
                }
            }
        }

        protected T ExecuteScalar<T>(IDbConnection connection, string commandText, object[] parameters, IDbTransaction transaction)
        {
            using (var command = connection.CreateCommand())
            {
                command.CommandText = commandText;
                command.SetParams(parameters);

                return ExecuteScalar<T>(connection, command, transaction);
            }
        }

        //TODO make static and test heavely
        protected T ExecuteScalar<T>(IDbConnection connection, IDbCommand command, IDbTransaction transaction)
        {
            object result = ExecuteScalar(connection, command, transaction);
            if (result is DBNull)
            {
                return default(T);
            }
            else if (result is T)
            {
                return (T)result;
            }
            else
            {
                Type t = typeof(T);
                if (t.IsGenericType && t.GetGenericTypeDefinition() == typeof(Nullable<>))
                {
                    t = Nullable.GetUnderlyingType(t);
                }

                return (T)Convert.ChangeType(result, t
                    , System.Globalization.CultureInfo.CurrentCulture);
                //return (T)Convert.ChangeType(result, typeof(T));
            }
        }

        #endregion general purpose command execution

        #region transaction management

        public void BeginTransaction()
        {
            lock (TransactionSyncLock)
            {
                if (_transactionDepth == 0)
                {
                    Debug.Assert(_CurrentTransaction == null);

                    var connection = OpenConnection();

                    try
                    {
                        _CurrentTransaction = connection.BeginTransaction();
                        _transactionCanceled = false;

                        this.EnterConnectionHold();
                        OnTransactionStarted();
                    }
                    catch (Exception ex)
                    {
                        ReleaseConnection();
                        throw ThrowExceptionHelper(connection, null, ex);
                    }
                }
                _transactionDepth++;
            }
        }

        public void CommitTransaction()
        {
            lock (TransactionSyncLock)
            {
                var transactionDepth = _transactionDepth;

                OnTransactionEnding();

                if (transactionDepth > 0)
                {
                    transactionDepth--;
                    if (transactionDepth == 0)//transaction depth was 1
                    {
                        ReleaseTransaction();
                    }
                    _transactionDepth = transactionDepth;
                }
                else// transactionDepth <= 0
                {
                    Debug.Fail("Transaction depth is " + transactionDepth.ToString());
                }
            }
        }

        public void RollbackTransaction()
        {
            lock (TransactionSyncLock)
            {
                var transactionDepth = _transactionDepth;

                OnTransactionCanceling();
                _transactionCanceled = true;

                if (transactionDepth > 0)
                {
                    transactionDepth--;
                    if (transactionDepth == 0)
                    {
                        ReleaseTransaction();
                    }
                    _transactionDepth = transactionDepth;
                }
                else
                {
                    Debug.Fail("Transaction depth is " + transactionDepth.ToString());
                }
            }
        }

        private void ReleaseTransaction()
        {
            OnTransactionReleasing();

            try
            {
                if (_CurrentTransaction != null)
                {
                    if (_transactionCanceled)
                    {
                        _CurrentTransaction.Rollback();
                    }
                    else
                    {
                        _CurrentTransaction.Commit();
                    }
                }
                else
                { Debug.Fail("_currentTransaction is null"); }
            }
            catch (Exception ex)
            {
                throw ThrowExceptionHelper(PersistentConnection, null, ex);
            }
            finally
            {
                if (_CurrentTransaction != null)
                { _CurrentTransaction.Dispose(); }
                _CurrentTransaction = null;
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

        public IDbConnection CreateConnection()
        {
            DbConnection conn = Provider.CreateConnection();
            conn.ConnectionString = BuildConnectionString();
            conn.StateChange += _Connection_StateChange;
            return conn;
        }

        /// <summary>
        /// if _holdConnection > 0 returns PersistentConnection
        /// if _holdConnection creates new connection and return it
        /// increments _holdConnection if connection successfully opened
        /// </summary>
        /// <exception cref="ConnectionException"></exception>
        /// <returns></returns>
        protected virtual IDbConnection OpenConnection()
        {
            lock (_persistentConnectionSyncLock)
            {
                IDbConnection conn;
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

        protected void ReleaseConnection()
        {
            ReleaseConnection(false);
        }

        protected virtual void ReleaseConnection(bool force)
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

        [Conditional("Debug")]
        protected void LogCommand(IDbCommand command)
        {
            Debug.WriteLine("Executing Command:" + command.CommandText);
        }

        protected virtual void OnDeletingData(object data)
        {
            if (data is IPersistanceTracking)
            {
                ((IPersistanceTracking)data).OnDeleting();
            }
        }

        protected virtual void OnInsertingData(object data, SQL.OnConflictOption option)
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
            Debug.WriteLine("Connection opened", Constants.Logging.DB_CONTROL);
        }

        //for logging connection state changes
        private void _Connection_StateChange(object sender, System.Data.StateChangeEventArgs e)
        {
            if (e.CurrentState == System.Data.ConnectionState.Closed)
            {
                Debug.Assert(_CurrentTransaction == null);
            }
            Debug.WriteLine("Connection state changed From " + e.OriginalState.ToString() + " to " + e.CurrentState.ToString(), Constants.Logging.DS_EVENT);
        }

        protected virtual void OnTransactionStarted()
        {
            Debug.WriteLine("Transaction Started", Constants.Logging.DB_CONTROL);
        }

        protected virtual void OnTransactionEnding()
        {
            Debug.WriteLine("Transaction Ending", Constants.Logging.DB_CONTROL);
        }

        protected virtual void OnTransactionCanceling()
        {
            Debug.WriteLine("Transaction Canceling", Constants.Logging.DB_CONTROL);
        }

        protected virtual void OnTransactionReleasing()
        {
            Debug.WriteLine("Transaction Releasing", Constants.Logging.DB_CONTROL);
        }

        #endregion events and logging

        #region IDisposable Support

        private bool isDisposed = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!isDisposed)
            {
                if (disposing)
                {
                    Debug.WriteLine("Datastore Disposing ", Constants.Logging.DS_EVENT);
                }
                else
                {
                    Debug.WriteLine("Datastore Finalized ", Constants.Logging.DS_EVENT);
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