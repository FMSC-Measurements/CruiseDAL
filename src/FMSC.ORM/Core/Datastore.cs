using Backpack.SqlBuilder;
using FMSC.ORM.Core.SQL.QueryBuilder;
using FMSC.ORM.EntityModel;
using FMSC.ORM.EntityModel.Support;
using FMSC.ORM.Logging;
using FMSC.ORM.Sql;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Common;
using System.Diagnostics;
using System.Linq;

#pragma warning disable RECS0122 // Initializing field with default value is redundant

namespace FMSC.ORM.Core
{
    public abstract partial class Datastore : IDisposable, IDatastore
    {
        protected ILogger Logger { get; set; } = LoggerProvider.Get();

        protected int _holdConnection = 0;

        public int ConnectionDepth
        {
            get => _holdConnection;
            protected set => _holdConnection = value;
        }

        protected bool _transactionCanceled = false;
        public DbTransaction CurrentTransaction { get; protected set; }
        public int TransactionDepth { get; protected set; }
        public object TransactionSyncLock { get; } = new object();

        public DbConnection PersistentConnection { get; protected set; }
        protected object PersistentConnectionSyncLock { get; } = new object();

        protected Dictionary<Type, EntityCache> _entityCache;

        protected IEntityDescriptionLookup EntityDescriptionLookup { get; set; } = GlobalEntityDescriptionLookup.Instance;

        // TODO should dialect be in the commandBuilder
        protected ISqlDialect SqlDialect { get; set; }
        public IExceptionProcessor ExceptionProcessor { get; }
        public IDatastoreBuilder DatabaseBuilder { get; }
        public ICommandBuilder CommandBuilder { get; }

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

        protected Datastore(ISqlDialect dialect,
                            IExceptionProcessor exceptionProcessor)
        {
            SqlDialect = dialect;
            ExceptionProcessor = exceptionProcessor;
            CommandBuilder = new CommandBuilder();
        }

        protected Datastore(ISqlDialect dialect,
                            IExceptionProcessor exceptionProcessor,
                            ICommandBuilder commandBuilder)
        {
            SqlDialect = dialect;
            ExceptionProcessor = exceptionProcessor;
            CommandBuilder = commandBuilder;
        }

        public virtual void CreateDatastore(IDatastoreBuilder builder)
        {
            var conn = OpenConnection();
            var transaction = conn.BeginTransaction();
            try
            {
                builder.BuildDatabase(conn, transaction, ExceptionProcessor);

                transaction.Commit();
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
            finally
            {
                ReleaseConnection();
            }
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

        #endregion Abstract Members

        #region fluent interface

        public QueryBuilder From(Type dataType, TableOrSubQuery source = null)
        {
            if (dataType is null) { throw new ArgumentNullException(nameof(dataType)); }

            var ed = EntityDescriptionLookup.LookUpEntityByType(dataType);
            SqlSelectBuilder builder = CommandBuilder.BuildSelect(source ?? ed.Source, ed.Fields);

            return new QueryBuilder(dataType, this, builder);
        }

        public QueryBuilder From(Type dataType, SqlSelectBuilder selectCMD)
        {
            var source = new TableOrSubQuery(selectCMD, null);
            return From(dataType, source);
        }

        public QueryBuilder<T> From<T>(SqlSelectBuilder selectCMD) where T : class, new()
        {
            var source = new TableOrSubQuery(selectCMD, null);
            return From<T>(source);
        }

        public QueryBuilder<T> From<T>(TableOrSubQuery source = null) where T : class, new()
        {
            var ed = EntityDescriptionLookup.LookUpEntityByType(typeof(T));
            SqlSelectBuilder builder = CommandBuilder.BuildSelect(source ?? ed.Source, ed.Fields);

            return new QueryBuilder<T>(this, builder);
        }

        public QueryBuilder<T> From<T>() where T : class, new()
        {
            return From<T>((TableOrSubQuery)null);
        }

        #endregion fluent interface

        #region CRUD

        public void Delete(object data, string tableName = null)
        {
            if (data is null) { throw new ArgumentNullException(nameof(data)); }

            var ed = EntityDescriptionLookup.LookUpEntityByType(data.GetType());

            lock (PersistentConnectionSyncLock)
            {
                var connection = OpenConnection();
                try
                {
                    connection.Delete(data,
                        tableName: tableName,
                        entityDescription: ed,
                        transaction: CurrentTransaction,
                        commandBuilder: CommandBuilder,
                        exceptionProcessor: ExceptionProcessor);
                }
                finally
                {
                    ReleaseConnection();
                }
            }
        }

        public object Insert(object data, string tableName = null, OnConflictOption option = OnConflictOption.Default, object keyValue = null, bool persistKeyvalue = true)
        {
            if (data is null) { throw new ArgumentNullException(nameof(data)); }

            var ed = EntityDescriptionLookup.LookUpEntityByType(data.GetType());

            lock (PersistentConnectionSyncLock)
            {
                var connection = OpenConnection();
                try
                {
                    return connection.Insert(data,
                        tableName: tableName,
                        entityDescription: ed,
                        transaction: CurrentTransaction,
                        option: option,
                        commandBuilder: CommandBuilder,
                        exceptionProcessor: ExceptionProcessor,
                        keyValue: keyValue, 
                        persistKeyvalue: persistKeyvalue);
                }
                finally
                {
                    ReleaseConnection();
                }
            }
        }

        public void Update(object data, string tableName = null, OnConflictOption option = OnConflictOption.Default, object keyValue = null)
        {
            var ed = EntityDescriptionLookup.LookUpEntityByType(data.GetType());

            lock (PersistentConnectionSyncLock)
            {
                var connection = OpenConnection();
                try
                {
                    connection.Update(data,
                        tableName: tableName,
                        entityDescription: ed,
                        option: option,
                        transaction: CurrentTransaction,
                        exceptionProcessor: ExceptionProcessor,
                        keyValue: keyValue);
                }
                finally
                {
                    ReleaseConnection();
                }
            }
        }

        #region read methods

        public IEnumerable<TResult> Read<TResult>(string commandText, object[] paramaters) where TResult : class, new()
        {
            lock (PersistentConnectionSyncLock)
            {
                var connection = OpenConnection();
                try
                {
                    var discription = GlobalEntityDescriptionLookup.Instance.LookUpEntityByType(typeof(TResult));
                    EntityCache cache = GetEntityCache(typeof(TResult));//TODO delegate access of cach to data context type
                    using (var command = CreateCommand())
                    {
                        command.CommandText = commandText;
                        command.SetParams(paramaters);

                        var exceptionProcessor = ExceptionProcessor;
                        using (var reader = connection.ExecuteReader(command, CurrentTransaction, exceptionProcessor: exceptionProcessor))
                        {
                            var inflator = InflatorLookup.Instance.GetEntityInflator(reader);
                            while (reader.Read())
                            {
                                TResult entity = null;
                                try
                                {
                                    object key = inflator.ReadPrimaryKey(reader, discription);
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
                                        if (entity is ISupportInitializeFromDatastore)
                                        {
                                            ((ISupportInitializeFromDatastore)entity).Initialize(this);
                                        }
                                        if (entity is ISupportInitialize)
                                        {
                                            ((ISupportInitialize)entity).BeginInit();// allow dataobject to suspend property changed notifications or whatever
                                        }
                                        try
                                        {
                                            inflator.ReadData(reader, entity, discription);
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
                                    if (exceptionProcessor != null)
                                    { throw exceptionProcessor.ProcessException(e, connection, commandText, CurrentTransaction); }
                                    else { throw; }
                                }

                                yield return entity;
                            }
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

            return From<T>().Where(keyField.Name + " = @p1").Read(primaryKeyValue).FirstOrDefault();
            //return ReadSingleRow<T>(null, "WHERE rowID = ?", primaryKeyValue);
        }

        #endregion read methods

        #region query methods

        public IEnumerable<GenericEntity> QueryGeneric(string commandText)
        {
            return QueryGeneric2(commandText, (object)null);
        }

        public IEnumerable<GenericEntity> QueryGeneric2(string commandText, object paramaters)
        {
            lock (PersistentConnectionSyncLock)
            {
                var connection = OpenConnection();
                try
                {
                    return connection.QueryGeneric2(commandText, paramaters, CurrentTransaction, ExceptionProcessor).ToArray();
                }
                finally
                {
                    ReleaseConnection();
                }
            }
        }

        public IEnumerable<TResult> QueryScalar<TResult>(string commandText, params object[] paramaters)
        {
            lock (PersistentConnectionSyncLock)
            {
                var connection = OpenConnection();
                try
                {
                    return connection.QueryScalar<TResult>(commandText, paramaters, CurrentTransaction, ExceptionProcessor)
                        .ToArray(); // need to force execution of the IEnumerable otherwise connection will be closed before it is executed
                }
                finally
                {
                    ReleaseConnection();
                }
            }
        }

        public IEnumerable<TResult> QueryScalar2<TResult>(string commandText, object paramaters = null)
        {
            lock (PersistentConnectionSyncLock)
            {
                var connection = OpenConnection();
                try
                {
                    return connection.QueryScalar2<TResult>(commandText, paramaters, CurrentTransaction, ExceptionProcessor)
                        .ToArray(); // need to force execution of the IEnumerable otherwise connection will be closed before it is executed
                }
                finally
                {
                    ReleaseConnection();
                }
            }
        }

        public IEnumerable<TResult> Query<TResult>(string commandText, params object[] paramaters) where TResult : new()
        {
            lock (PersistentConnectionSyncLock)
            {
                var connection = OpenConnection();
                try
                {
                    var discription = GlobalEntityDescriptionLookup.Instance.LookUpEntityByType(typeof(TResult));

                    using (var command = CreateCommand())
                    {
                        command.CommandText = commandText;
                        command.SetParams(paramaters);

                        var exceptionProcessor = ExceptionProcessor;
                        using (var reader = connection.ExecuteReader(command, CurrentTransaction, exceptionProcessor: exceptionProcessor))
                        {
                            var inflator = InflatorLookup.Instance.GetEntityInflator(reader);

                            while (reader.Read())
                            {
                                TResult newDO = new TResult();
                                Inflate(commandText, connection, discription, exceptionProcessor, reader, inflator, newDO);

                                yield return newDO;
                            }
                        }
                    }
                }
                finally
                {
                    ReleaseConnection();
                }
            }
        }

        public IEnumerable<object> Query(Type type, string commandText, params object[] paramaters)
        {
            lock (PersistentConnectionSyncLock)
            {
                var connection = OpenConnection();
                try
                {
                    var discription = GlobalEntityDescriptionLookup.Instance.LookUpEntityByType(type);

                    using (var command = CreateCommand())
                    {
                        command.CommandText = commandText;
                        command.SetParams(paramaters);

                        var exceptionProcessor = ExceptionProcessor;
                        using (var reader = connection.ExecuteReader(command, CurrentTransaction, exceptionProcessor: exceptionProcessor))
                        {
                            var inflator = InflatorLookup.Instance.GetEntityInflator(reader);

                            while (reader.Read())
                            {
                                object newDO = Activator.CreateInstance(type);
                                Inflate(commandText, connection, discription, exceptionProcessor, reader, inflator, newDO);

                                yield return newDO;
                            }
                        }
                    }
                }
                finally
                {
                    ReleaseConnection();
                }
            }
        }

        public IEnumerable<TResult> Query2<TResult>(string commandText, object paramaters) where TResult : new()
        {
            lock (PersistentConnectionSyncLock)
            {
                var connection = OpenConnection();
                try
                {
                    var discription = GlobalEntityDescriptionLookup.Instance.LookUpEntityByType(typeof(TResult));

                    using (var command = CreateCommand())
                    {
                        command.CommandText = commandText;
                        command.AddParams(paramaters);

                        var exceptionProcessor = ExceptionProcessor;
                        using (var reader = connection.ExecuteReader(command, CurrentTransaction, exceptionProcessor: exceptionProcessor))
                        {
                            var inflator = InflatorLookup.Instance.GetEntityInflator(reader);

                            while (reader.Read())
                            {
                                TResult newDO = new TResult();
                                Inflate(commandText, connection, discription, exceptionProcessor, reader, inflator, newDO);

                                yield return newDO;
                            }
                        }
                    }
                }
                finally
                {
                    ReleaseConnection();
                }
            }
        }

        public IEnumerable<object> Query2(Type type, string commandText, object paramaters)
        {
            lock (PersistentConnectionSyncLock)
            {
                var connection = OpenConnection();
                try
                {
                    var discription = GlobalEntityDescriptionLookup.Instance.LookUpEntityByType(type);

                    using (var command = CreateCommand())
                    {
                        command.CommandText = commandText;
                        command.AddParams(paramaters);

                        var exceptionProcessor = ExceptionProcessor;
                        using (var reader = connection.ExecuteReader(command, CurrentTransaction, exceptionProcessor: exceptionProcessor))
                        {
                            var inflator = InflatorLookup.Instance.GetEntityInflator(reader);

                            while (reader.Read())
                            {
                                object newDO = Activator.CreateInstance(type);
                                Inflate(commandText, connection, discription, exceptionProcessor, reader, inflator, newDO);

                                yield return newDO;
                            }
                        }
                    }
                }
                finally
                {
                    ReleaseConnection();
                }
            }
        }

        private void Inflate(string commandText, DbConnection connection, EntityDescription discription, IExceptionProcessor exceptionProcessor, DbDataReader reader, EntityInflator inflator, object newDO)
        {
            if (newDO is ISupportInitializeFromDatastore)
            {
                ((ISupportInitializeFromDatastore)newDO).Initialize(this);
            }
            if (newDO is ISupportInitialize)
            {
                ((ISupportInitialize)newDO).BeginInit();// allow dataobject to suspend property changed notifications or whatever
            }
            try
            {
                inflator.ReadData(reader, newDO, discription);
            }
            catch (Exception e)
            {
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
        }

        public IEnumerable<TResult> Query<TResult>(SqlSelectBuilder selectBuilder, Object[] selectionArgs) where TResult : new()
        {
            return Query<TResult>(selectBuilder.ToString() + ";", selectionArgs);
        }

        #endregion query methods

        #endregion CRUD

        #region general purpose command execution

        /// <summary>
        /// Executes SQL command returning number of rows affected
        /// </summary>
        /// <param name="commandText"></param>
        /// <returns></returns>
        public int Execute(string commandText, params object[] parameters)
        {
            if (string.IsNullOrEmpty(commandText)) { throw new ArgumentNullException("command"); }

            lock (PersistentConnectionSyncLock)
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

        public int Execute2(string commandText, object parameters)
        {
            if (string.IsNullOrEmpty(commandText)) { throw new ArgumentNullException("command"); }

            lock (PersistentConnectionSyncLock)
            {
                var connection = OpenConnection();
                try
                {
                    return connection.ExecuteNonQuery2(commandText, parameters, CurrentTransaction);
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
            lock (PersistentConnectionSyncLock)
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
            lock (PersistentConnectionSyncLock)
            {
                var conn = OpenConnection();
                try
                {
                    return conn.ExecuteScalar<T>(commandText, parameters, CurrentTransaction);
                }
                catch (Exception ex)
                {
                    throw ExceptionProcessor.ProcessException(ex, conn, commandText, CurrentTransaction);
                }
                finally
                {
                    ReleaseConnection();
                }
            }
        }

        public T ExecuteScalar2<T>(string command, object parameters)
        {
            lock (PersistentConnectionSyncLock)
            {
                var conn = OpenConnection();
                try
                {
                    return conn.ExecuteScalar2<T>(command, parameters, CurrentTransaction);
                }
                catch (Exception ex)
                {
                    throw ExceptionProcessor.ProcessException(ex, conn, command, CurrentTransaction);
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

        protected abstract DbConnection CreateConnection();

        protected abstract DbCommand CreateCommand();

        /// <summary>
        /// if _holdConnection > 0 returns PersistentConnection
        /// if _holdConnection creates new connection and return it
        /// increments _holdConnection if connection successfully opened
        /// </summary>
        /// <exception cref="ConnectionException"></exception>
        /// <returns></returns>
        public virtual DbConnection OpenConnection()
        {
            CheckIsDisposed();
            lock (PersistentConnectionSyncLock)
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
                        OnConnectionOpened(conn);
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
            lock (PersistentConnectionSyncLock)
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

        protected virtual void OnConnectionOpened(DbConnection connection)
        {
            Logger.Log("Connection Oppened", LogCategory.Connection, LogLevel.Verbose);
        }

        protected virtual void OnTransactionStarted()
        {
            Logger.Log("Transaction Started", LogCategory.Connection, LogLevel.Verbose);
        }

        protected virtual void OnTransactionEnding()
        {
            Logger.Log("Transaction Ending", LogCategory.Connection, LogLevel.Verbose);
        }

        protected virtual void OnTransactionCanceling()
        {
            Logger.Log("Transaction Canceling", LogCategory.Connection, LogLevel.Verbose);
        }

        protected virtual void OnTransactionReleasing()
        {
            Logger.Log("Transaction Releasing", LogCategory.Connection, LogLevel.Verbose);
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

        public void CreateTable(string tableName, IEnumerable<ColumnInfo> cols, bool temp = false)
        {
            lock (PersistentConnectionSyncLock)
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

        public void CreateTable(DbConnection connection, string tableName, IEnumerable<ColumnInfo> cols, bool temp = false, DbTransaction transaction = null)
        {
            var createTableCommand = new CreateTable(SqlDialect)
            {
                TableName = tableName,
                Temp = temp,
            }.WithColumns(cols);

            connection.ExecuteNonQuery(createTableCommand.ToString() + ";", (object[])null, transaction);
        }

        #region IDisposable Support

        protected void CheckIsDisposed()
        {
            if(isDisposed) { throw new ObjectDisposedException(this.GetType().Name); }
        }

        protected bool isDisposed = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (isDisposed) { return; }

            var logger = Logger;
            if (disposing)
            {
                logger.Log("Datastore Disposing", LogCategory.Datastore, LogLevel.Info);
                Logger = null;
            }
            else
            {
                logger?.Log("Datastore Finalized ", LogCategory.Datastore, LogLevel.Info);
            }

            //if(this._cache != null)
            //{
            //    _cache.Dispose();
            //}

            ReleaseConnection(true);

            isDisposed = true;
        }

        ~Datastore()
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