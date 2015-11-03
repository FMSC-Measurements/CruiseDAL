using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Diagnostics;
using System.Threading;

using FMSC.ORM.Core.EntityModel;
using FMSC.ORM.Core.SQL;
using FMSC.ORM.Core.EntityAttributes;
using FMSC.ORM.Core.Constants;

namespace FMSC.ORM.Core
{
    public abstract class DataStoreContext : IDisposable
    {
        public DatastoreRedux DataStore { get; set; }

        //public DbConnectionStringBuilder ConnectionStringBuilder { get; protected set; }

        protected int _holdConnection = 0;

        protected Object _readOnlyConnectionSyncLock = new object();
        protected Object _readWriteConnectionSyncLock = new object();

        protected DbConnection _ReadWriteConnection;
        protected DbConnection _ReadOnlyConnection;

        protected object _transactionSyncLock = new object(); 
        protected DbTransaction _CurrentTransaction;

        protected Stack<DbTransaction> _transactionStack = new Stack<DbTransaction>();

        protected Dictionary<Type, EntityCache> _entityCache;


        #region abstract members
        //protected abstract DbConnection CreateConnection(string connectionString);
        protected abstract Exception ThrowExceptionHelper(DbConnection conn, DbCommand comm, Exception innerException);

        #endregion

        private EntityCache GetEntityCache(Type type)
        {
            if(_entityCache == null) { _entityCache = new Dictionary<Type, EntityCache>(); }
            if (_entityCache.ContainsKey(type) == false)
            {
                EntityCache newCache = new EntityCache();
                _entityCache.Add(type, newCache);
                return newCache;
            }
            else
            {
                return _entityCache[type];
            }
        }

        private EntityDescription GetEntityInfo(Type type)
        {
            throw new NotImplementedException();
        }

        private EntityInflator GetEntityInflator(Type type)
        {
            return GetEntityInfo(type).Inflator;
        }

        #region sugar
        DbCommand CreateCommand(string commandText)
        {
            return DbProviderFactoryAdapter.Instance.CreateCommand(commandText);
        }

        DbParameter CreateParameter(string name, object value)
        {
            return DbProviderFactoryAdapter.Instance.CreateParameter(name, value);
        }

        //DbConnectionStringBuilder GetConnectionStringBuilder()
        //{
        //    throw new NotImplementedException();
        //}
        #endregion

        #region Transaction Management
        public void BeginTransaction()
        {
            lock(_transactionSyncLock)
            {
                if(_CurrentTransaction != null)
                {
                    throw new InvalidOperationException("one transaction at a time");
                }

                this.EnterConnectionHold();
                DbConnection connection = OpenReadWriteConnection(true);
                _CurrentTransaction = connection.BeginTransaction();

                Debug.WriteLine("Transaction Started", Logging.DB_CONTROL);
            }
        }

        public void RollbackTransaction()
        {
            lock(_transactionSyncLock)
            {
                if(_CurrentTransaction == null)
                {
                    throw new InvalidOperationException("no active transaction");
                }

                _CurrentTransaction.Rollback();
                _CurrentTransaction.Dispose();
                _CurrentTransaction = null;
                ExitConnectionHold();
            }


            //lock (_transactionStack)
            //{
            //    if(_transactionStack.Count > 0)
            //    {
            //        DbTransaction top = _transactionStack.Peek();
            //        this.CancelTransaction(top);
            //    }
            //    else
            //    {
            //        Debug.Fail("transaction stack empty, are you missing a BeginTransaction?");
            //    }    
            //}
        }

        //public void CancelTransaction(DbTransaction transaction)
        //{
        //    Debug.Assert(transaction != null);



        //    lock (_transactionStack)
        //    {
        //        if (this._transactionStack.Contains(transaction))
        //        {
        //            DbTransaction top = null;
        //            int transactionDepth = 0;
        //            do
        //            {
        //                top = _transactionStack.Pop();
        //                transactionDepth++;


        //            } while (!Object.ReferenceEquals(top, transaction));

        //            try
        //            {
        //                this.ExitConnectionHold(transactionDepth);
        //                top.Rollback();
                        
        //                Debug.WriteLine("Transaction Rolled back", Logging.DB_CONTROL);
        //            }
        //            catch (Exception e)
        //            {
        //                throw this.ThrowExceptionHelper(top.Connection, null, e);
        //            }
        //        }
        //        else
        //        {
        //            Debug.Fail("transaction not on stack, are you missing a BeginTransaction?");
        //        }
        //    }
        //}

        public void CommitTransaction()
        {
            lock (_transactionSyncLock)
            {
                if (_CurrentTransaction == null)
                {
                    throw new InvalidOperationException("no active transaction");
                }

                _CurrentTransaction.Commit();
                _CurrentTransaction.Dispose();
                _CurrentTransaction = null;
                ExitConnectionHold();
            }

            //lock (_transactionStack)
            //{
            //    if(_transactionStack.Count > 0)
            //    { 
            //        DbTransaction top = _transactionStack.Peek();
            //        this.EndTransaction(top);
            //    }
            //    else
            //    { 
            //        Debug.Fail("transaction stack empty, are you missing a BeginTransaction?");
            //    }
            //}
        }

        //public void EndTransaction(DbTransaction transaction)
        //{
        //    lock (_transactionStack)
        //    {
        //        if (this._transactionStack.Contains(transaction))
        //        {
        //            DbTransaction top = null;
        //            do
        //            {
        //                top = _transactionStack.Pop();



        //            } while (!Object.ReferenceEquals(top, transaction));

        //            try
        //            {
        //                top.Commit();
        //                Debug.WriteLine("Transaction Committed", Logging.DB_CONTROL);
        //            }
        //            catch (Exception e)
        //            {
        //                throw this.ThrowExceptionHelper(top.Connection, null, e);
        //            }
        //        }
        //        else
        //        {
        //            Debug.Fail("transaction not on stack, are you missing a BeginTransaction?");
        //        }
        //    }
        //}

        #endregion

        #region Persistence Methods
        public void Save<T>(T data, SQL.OnConflictOption option) where T : IPersistanceTracking
        {
            if (data.HasChanges == false) { return; }
            if (!data.IsPersisted)
            {
                object primaryKey = Insert(data, option);
                if (primaryKey != null)
                {
                    EntityCache cache = GetEntityCache(typeof(T));

                    Debug.Assert(cache.ContainsKey(primaryKey) == false);
                    cache.Add(primaryKey, data);
                }
            }
            else
            {
                Update(data, option);
            }
        }

        public object Insert(object data, SQL.OnConflictOption option)
        {
            EntityDescription entityDescription = GetEntityInfo(data.GetType());
            PrimaryKeyFieldAttribute primaryKeyField = entityDescription.Fields.PrimaryKeyField;

            EntityCommandBuilder builder = entityDescription.CommandBuilder;

            object primaryKey = null;
            using (DbCommand command = builder.BuildInsertCommand(data, option))
            {
                ExecuteSQL(command);

                if(primaryKeyField != null)
                {
                    primaryKey = GetLastInsertKeyValue(entityDescription.SourceName
                        , primaryKeyField.FieldName);

                    primaryKeyField.SetFieldValue(data, primaryKey);
                }
            }

            if(data is IPersistanceTracking)
            {
                ((IPersistanceTracking)data).IsPersisted = true;
                ((IPersistanceTracking)data).HasChanges = false;
                ((IPersistanceTracking)data).OnInserted();
            }

            return primaryKey;
        }

        
        //public void Update(DataObject data, OnConflictOption option)
        //{
        //    this.Update(data, data.rowID, option);
        //}

        public void Update(object data, SQL.OnConflictOption option)
        {
            EntityDescription entityDescription = GetEntityInfo(data.GetType());
            EntityCommandBuilder builder = entityDescription.CommandBuilder;

            using (DbCommand command = builder.BuildUpdateCommand(data, option))
            {
                ExecuteSQL(command);
            }

            if (data is IPersistanceTracking)
            {
                ((IPersistanceTracking)data).IsPersisted = true;
                ((IPersistanceTracking)data).HasChanges = false;
                ((IPersistanceTracking)data).OnUpdated();
            }

        }

        public void Delete(object data)
        {
            EntityDescription entityDescription = GetEntityInfo(data.GetType());
            PrimaryKeyFieldAttribute keyFieldInfo = entityDescription.Fields.PrimaryKeyField;

            if(keyFieldInfo == null) { throw new InvalidOperationException("type doesn't have primary key field"); }

            EntityCommandBuilder builder = entityDescription.CommandBuilder;

            lock (data)
            {
                if (data is IPersistanceTracking)
                {
                    Debug.Assert(((IPersistanceTracking)data).IsPersisted == true);
                    ((IPersistanceTracking)data).IsDeleted = true;
                    ((IPersistanceTracking)data).OnDeleting();
                }

                object keyValue = keyFieldInfo.GetFieldValue(data);
                
                using (DbCommand command = builder.BuildSQLDeleteCommand(keyFieldInfo.FieldName, keyValue))
                {
                    ExecuteSQL(command);
                }

                if (data is IPersistanceTracking)
                {
                    ((IPersistanceTracking)data).IsDeleted = true;
                    ((IPersistanceTracking)data).OnDeleted();
                }
            }
        }


        #endregion

        #region command execution methods

        protected long GetLastInsertRowID()
        {
            using (DbCommand command = CreateCommand("SELECT last_insert_rowid()"))
            {
                return this.ExecuteScalar<long>(command);
            }
        }

        protected object GetLastInsertKeyValue(String tableName, String fieldName)
        {
            String query = "Select " + fieldName + " FROM " + tableName + " WHERE rowid = last_insert_rowid()";
            return ExecuteScalar(query);
        }


        public T ExecuteScalar<T>(String query)
        {
            return ExecuteScalar<T>(query, (object[])null);
        }

        public T ExecuteScalar<T>(String query, params object[] parameters)
        {
            using (DbCommand comm = this.CreateCommand(query))
            {
                if (parameters != null)
                {
                    foreach (object val in parameters)
                    {
                        comm.Parameters.Add(this.CreateParameter(null, val));
                    }
                }
                return ExecuteScalar<T>(comm);
            }
        }

        public T ExecuteScalar<T>(String query, IEnumerable<KeyValuePair<String, object>> parameters)
        {
            using (DbCommand comm = this.CreateCommand(query))
            {
                if (parameters != null)
                {
                    foreach (var pair in parameters)
                    {
                        var param = CreateParameter(pair.Key, pair.Value);
                        comm.Parameters.Add(param);
                    }
                }
                return ExecuteScalar<T>(comm);
            }
        }


        /// <summary>
        /// Executes SQL command returning single value
        /// </summary>
        /// <param name="command"></param>
        /// <returns>value or null</returns>
        public object ExecuteScalar(string query, params object[] parameters)
        {
            using (DbCommand comm = this.CreateCommand(query))
            {
                if (parameters != null)
                {
                    foreach (object val in parameters)
                    {
                        comm.Parameters.Add(this.CreateParameter(null, val));
                    }
                }
                object value = ExecuteScalar(comm);
                return (value is DBNull) ? null : value;
            }

        }

        internal T ExecuteScalar<T>(DbCommand command)
        {
            object result = ExecuteScalar(command);
            if(result is DBNull)
            {
                return default(T);
            }
            else if(result is T)
            {
                return (T)result;
            }
            else
            {
                return (T)Convert.ChangeType(result, typeof(T)
                    , System.Globalization.CultureInfo.CurrentCulture);
                //return (T)Convert.ChangeType(result, typeof(T));
            }
        }

        internal object ExecuteScalar(DbCommand command)
        {
            lock (_readWriteConnectionSyncLock)
            {
                DbConnection conn = OpenReadWriteConnection(false);
                try
                {
                    command.Connection = conn;
                    return command.ExecuteScalar();
                }
                catch (Exception e)
                {
                    throw this.ThrowExceptionHelper(conn, command, e);
                }
                finally
                {
                    ReleaseReadWriteConnection();
                }
            }

        }

        /// <summary>
        /// Executes SQL command returning number of rows affected
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        public int? Execute(String command, params object[] parameters)
        {
            using (DbCommand com = this.CreateCommand(command))
            {
                return this.Execute(com, parameters);
            }
        }

        public int? Execute(String command, IEnumerable<KeyValuePair<String, object>> parameters)
        {
            using (DbCommand comm = this.CreateCommand(command))
            {
                if (parameters != null)
                {
                    foreach (var pair in parameters)
                    {
                        var param = CreateParameter(pair.Key, pair.Value);
                        comm.Parameters.Add(param);
                    }
                }
                return ExecuteSQL(comm);
            }
        }

        private int? Execute(DbCommand command, params object[] parameters)
        {
            if (parameters != null)
            {
                foreach (object p in parameters)
                {
                    command.Parameters.Add(this.CreateParameter(null, p));
                }
            }
            return ExecuteSQL(command);
        }

        internal int ExecuteSQL(DbCommand command)
        {
            lock (_readWriteConnectionSyncLock)
            {
                DbConnection conn = OpenReadWriteConnection(false);
                try
                {
                    return this.ExecuteSQL(command, conn);
                }
                finally
                {
                    ReleaseReadWriteConnection();
                }
            }
        }

        protected int ExecuteSQL(DbCommand command, DbConnection connection)
        {
            try
            {
                command.Connection = connection;
                return command.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                throw this.ThrowExceptionHelper(connection, command, e);
            }
        }

        //protected DbDataReader ExecuteReader(DbCommand command)
        //{
        //    lock(_readOnlyConnectionSyncLock)
        //    {
        //        DbConnection conn = OpenReadOnlyConnection(true);
        //        command.Connection = conn;
        //        return command.ExecuteReader();

        //    }

        //}
        #endregion

        #region Connection Management
        void _Connection_StateChange(object sender, System.Data.StateChangeEventArgs e)
        {
            OnConnectionStateChanged(e);
        }

        protected virtual void OnConnectionStateChanged(System.Data.StateChangeEventArgs e)
        {
            Debug.WriteLine("Connection state changed From " + e.OriginalState.ToString() + " to " + e.CurrentState.ToString(), Logging.DS_EVENT);
        }

        protected void EnterConnectionHold()
        {
            System.Threading.Interlocked.Increment(ref this._holdConnection);
        }

        protected void ExitConnectionHold()
        {
            if (this._holdConnection > 0)
            {
                System.Threading.Interlocked.Decrement(ref this._holdConnection);
            }
        }

        protected void ExitConnectionHold(int depth)
        {
            for(int i = 0; i < depth; i++)
            {
                ExitConnectionHold();
            }
        }

        protected void ReleaseConnectionHold()
        {
            this._holdConnection = 0;
        }

        protected DbConnection CreateReadWriteConnection()
        {
            throw new NotImplementedException();
            //DbConnectionStringBuilder builder = GetConnectionStringBuilder();
            //builder.Add("Read Only", true);

            //DbConnection conn = DbProviderFactoryAdapter.Instance.CreateConnection();
            //conn.ConnectionString = builder.ToString();
            //conn.StateChange += _Connection_StateChange;
            //return conn;
        }

        protected DbConnection CreateReadOnlyConnection()
        {
            throw new NotImplementedException();
            //DbConnectionStringBuilder builder = GetConnectionStringBuilder();

            //DbConnection conn = DbProviderFactoryAdapter.Instance.CreateConnection();
            //conn.ConnectionString = builder.ToString();
            //conn.StateChange += _Connection_StateChange;
            //return conn;
        }

        protected virtual DbConnection OpenReadWriteConnection(bool retry)
        {
            lock(this._readWriteConnectionSyncLock)
            {
                DbConnection conn; 

                

                if(_ReadWriteConnection == null)
                {
                    _ReadWriteConnection = CreateReadWriteConnection();
                }
                conn = _ReadWriteConnection;

                if (conn.State != System.Data.ConnectionState.Open)
                {
                    try
                    {
                        conn.Open();
                        EnterConnectionHold();
                    }
                    catch (Exception e)
                    {
                        if(!retry)
                        {
                            throw new ConnectionException(null, e)
                            {
                                ConnectionString = conn.ConnectionString,
                                ConnectionState = conn.State
                            };
                        }
                        else
                        {
                            conn.Dispose();
                            _ReadWriteConnection = null;
                            Thread.Sleep(100);
                            conn = OpenReadWriteConnection(false);
                        }
                    }
                }

                Debug.WriteLine("Read Write Connection Opened", Logging.DB_CONTROL_VERBOSE);
                return conn;
            }
        }

        protected virtual DbConnection OpenReadOnlyConnection(bool retry)
        {

            lock (this._readOnlyConnectionSyncLock)
            {
                DbConnection conn;

                

                if (_ReadOnlyConnection == null)
                {
                    _ReadOnlyConnection = CreateReadOnlyConnection();

                }
                conn = _ReadOnlyConnection;

                if (conn.State != System.Data.ConnectionState.Open)
                {
                    try
                    {
                        conn.Open();
                    }
                    catch
                    {
                        if (!retry)
                        {
                            throw;
                        }
                        else
                        {
                            conn.Dispose();
                            _ReadOnlyConnection = null;
                            Thread.Sleep(100);
                            conn = OpenReadOnlyConnection(false);
                        }
                    }
                }

                Debug.WriteLine("Read Only Connection Opened", Logging.DB_CONTROL_VERBOSE);
                return conn;
            }
        }

        //protected virtual DbConnection OpenConnection()
        //{
        //    this.EnterConnectionHold();
        //    try
        //    {
        //        lock (this._connectionSyncLock)
        //        {

        //            Debug.WriteLine("Connection In Use", Logging.DB_CONTROL_VERBOSE);
        //            if (this._Connection != null)
        //            {
        //                return this._Connection;
        //            }
        //            else
        //            {
        //                System.Diagnostics.Debug.WriteLine("Connection created, threadID: " + System.Threading.Thread.CurrentThread.ManagedThreadId.ToString(), LOG_LEV_DS_EVENT);
        //                this._Connection = CreateConnection(this._ConnectionString);
        //                this._Connection.StateChange += new System.Data.StateChangeEventHandler(_Connection_StateChange);
        //                this._Connection.Open();

        //                return this._Connection;
        //            }
        //        }
        //    }
        //    catch (Exception e)
        //    {
        //        throw this.ThrowDatastoreExceptionHelper(this._Connection, null, e);
        //    }
        //}

        //internal void ReleaseConnection()
        //{
        //    lock (this._connectionSyncLock)
        //    {
        //        Debug.WriteLine("Connection Released", Logging.DB_CONTROL_VERBOSE);
        //        if (this._holdConnection == 1)
        //        {
        //            Debug.Assert(_Connection != null);
        //            _Connection.Dispose();
        //            _Connection = null;
        //            Debug.WriteLine("Connection Closed", Logging.DS_EVENT);
        //        }
        //        this.ExitConnectionHold();
        //    }
        //}

        public void ReleaseAllConnections(bool force)
        {
            ReleaseReadOnlyConnection();
            if(_holdConnection > 1 && force) { throw new NotImplementedException(); }
            ReleaseReadWriteConnection();
        }

        void ReleaseConnection(DbConnection conn)
        {
            conn.Close();
            conn.Dispose();
        }

        void ReleaseReadOnlyConnection()
        {
            lock(_readOnlyConnectionSyncLock)
            {
                Debug.Assert(_ReadOnlyConnection != null);
                if(_ReadOnlyConnection == null) { return; }
                ReleaseConnection(_ReadOnlyConnection);
                _ReadOnlyConnection = null;
                Debug.WriteLine("Read Only Connection Released", Logging.DB_CONTROL_VERBOSE);
            }
        }

        void ReleaseReadWriteConnection()
        {
            lock (_readWriteConnectionSyncLock)
            {
                Debug.Assert(_ReadWriteConnection != null);
                if (_ReadOnlyConnection == null) { return; }
                ExitConnectionHold();
                if (_holdConnection == 0)
                {
                    ReleaseConnection(_ReadWriteConnection);
                    _ReadWriteConnection = null;
                    Debug.WriteLine("ReadWrite Connection Released", Logging.DB_CONTROL_VERBOSE);
                }
            }
        }

        #endregion

        #region Read Methods

        public List<T> Read<T>(string selection, params object[] selectionArgs) 
            where T : new()
        {
            EntityDescription entityDescription = GetEntityInfo(typeof(T));
            EntityCommandBuilder commandBuilder = entityDescription.CommandBuilder;

            using (DbCommand command = commandBuilder.BuildSelectLegacy(selection))
            {
                //Add selection Arguments to command parameter list
                if (selectionArgs != null)
                {
                    foreach (object obj in selectionArgs)
                    {
                        command.Parameters.Add(this.CreateParameter(null, obj));
                    }
                }

                return Read<T>(command, entityDescription);
            }
        }

        public List<T> Read<T>(WhereClause where, params Object[] selectionArgs)
            where T : new()
        {
            EntityDescription entityDescription = GetEntityInfo(typeof(T));
            EntityCommandBuilder commandBuilder = entityDescription.CommandBuilder;

            using (DbCommand command = commandBuilder.BuildSelectCommand(where))
            {
                //Add selection Arguments to command parameter list
                if (selectionArgs != null)
                {
                    foreach (object obj in selectionArgs)
                    {
                        command.Parameters.Add(this.CreateParameter(null, obj));
                    }
                }

                return Read<T>(command, entityDescription);
            }
        }

        internal List<T> Read<T>(DbCommand command, EntityDescription entityDescription) 
            where T : new()
        {
            List<T> doList = new List<T>();
            EntityCache cache = GetEntityCache(typeof(T));
            EntityInflator inflator = entityDescription.Inflator;

            DbDataReader reader = null;
            lock (_readOnlyConnectionSyncLock)
            {
                DbConnection conn = OpenReadOnlyConnection(true);
                try
                {

                    command.Connection = conn;
                    reader = command.ExecuteReader();

                    inflator.CheckOrdinals(reader);
                    while (reader.Read())
                    {
                        Object entity = null;
                        object key = inflator.ReadPrimaryKey(reader);
                        if(cache.ContainsKey(key))
                        {
                            entity = cache[key];
                        }
                        else
                        {
                            entity = inflator.CreateInstanceOfEntity();
                            if(entity is IDataObject)
                            {
                                ((IDataObject)entity).DAL = this.DataStore;
                            }
                        }

                        inflator.ReadData(reader, entity);

                        doList.Add((T)entity);
                    }
                }

                catch (Exception e)
                {
                    throw this.ThrowExceptionHelper(conn, command, e);
                }
                finally
                {
                    if (reader != null) { reader.Dispose(); }
                    ReleaseReadOnlyConnection();
                }
                return doList;
            }
        }


        ///// <summary>
        ///// Retrieves a single row from the database Note: data object type must match the
        ///// table.
        ///// </summary>
        ///// <typeparam name="T">Type of data object to return</typeparam>
        ///// <param name="tableName">Name of table to read from</param>
        ///// <param name="selection">the where clause to define selection Note: only the first row from the resulting selection will be returned</param>
        ///// <param name="selectionArgs">array of parameters to use with selection string</param>
        ///// <exception cref="DatabaseExecutionException"></exception>
        ///// <returns>a single data object</returns>
        //public T ReadSingleRow<T>(String tableName, String selection, params Object[] selectionArgs) where T : ReadDataObject, new()
        //{
        //    return ReadSingleRow<T>(tableName, true, selection, selectionArgs);
        //}

        public T ReadSingleRow<T>(WhereClause where, params Object[] selectionArgs) 
            where T : new()
        {

            EntityDescription entityDescription = GetEntityInfo(typeof(T));
            EntityCommandBuilder commandBuilder = entityDescription.CommandBuilder;

            using (DbCommand command = commandBuilder.BuildSelectCommand(where))
            {
                //Add selection Arguments to command parameter list
                if (selectionArgs != null)
                {
                    foreach (object obj in selectionArgs)
                    {
                        command.Parameters.Add(this.CreateParameter(null, obj));
                    }
                }

                return ReadSingleRow<T>(command, entityDescription);
            }
        }

        /// <summary>
        /// Retrieves a single row from the database Note: data object type must match the 
        /// table. 
        /// </summary>
        /// <typeparam name="T">Type of data object to return</typeparam>
        /// <param name="tableName">Name of table to read from</param>
        /// <param name="rowID">row id of the row to read</param>
        /// <exception cref="DatabaseExecutionException"></exception>
        /// <returns>a single data object</returns>
        public T ReadSingleRow<T>(object primaryKeyValue) 
            where T : new()
        {
            return ReadSingleRow<T>(new WhereClause("rowID = ?"), primaryKeyValue);
        }

        internal T ReadSingleRow<T>(DbCommand command, EntityDescription entityDescription) 
            where T : new()
        {
            object entity = null;
            EntityCache cache = GetEntityCache(typeof(T));
            EntityInflator inflator = entityDescription.Inflator;

            DbDataReader reader = null;
            lock (_readOnlyConnectionSyncLock)
            {
                DbConnection conn = OpenReadOnlyConnection(true);
                try
                {

                    command.Connection = conn;
                    reader = command.ExecuteReader();

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
                                ((IDataObject)entity).DAL = this.DataStore;
                            }
                        }

                        inflator.ReadData(reader, entity);
                    }
                }
                catch (Exception e)
                {
                    throw this.ThrowExceptionHelper(conn, command, e);
                }
                finally
                {
                    if (reader != null) { reader.Dispose(); }
                    ReleaseReadOnlyConnection();
                }
                return (T)entity;
            }
        }

        #endregion

        #region Query Methods
        public List<T> Query<T>(DbCommand command, EntityDescription entityDescription) 
            where T : new()
        {
            EntityInflator inflator = entityDescription.Inflator;
            List<T> dataList = new List<T>();
            DbDataReader reader = null;
            lock (_readOnlyConnectionSyncLock)
            {
                DbConnection conn = OpenReadOnlyConnection(true);
                try
                {
                    command.Connection = conn;
                    reader = command.ExecuteReader();
                    inflator.CheckOrdinals(reader);

                    while (reader.Read())
                    {
                        Object newDO = inflator.CreateInstanceOfEntity();
                        inflator.ReadData(reader, newDO);
                        dataList.Add((T)newDO);
                    }
                }

                catch (Exception e)
                {
                    throw this.ThrowExceptionHelper(conn, command, e);
                }
                finally
                {
                    if (reader != null) { reader.Dispose(); }
                    ReleaseReadOnlyConnection();
                }
                return dataList;
            }
        }

        public List<T> Query<T>(WhereClause where, params Object[] selectionArgs) 
            where T : new()
        {
            EntityDescription entityDescription = GetEntityInfo(typeof(T));
            EntityCommandBuilder commandBuilder = entityDescription.CommandBuilder;

            using (DbCommand command = commandBuilder.BuildSelectCommand(where))
            {
                //Add selection Arguments to command parameter list
                if (selectionArgs != null)
                {
                    foreach (object obj in selectionArgs)
                    {
                        command.Parameters.Add(this.CreateParameter(null, obj));
                    }
                }

                return Query<T>(command, entityDescription);
            }

        }

        public T QuerySingleRecord<T>(WhereClause where, params Object[] selectionArgs) 
            where T : new()
        {
            EntityDescription entityDescription = GetEntityInfo(typeof(T));
            EntityCommandBuilder commandBuilder = entityDescription.CommandBuilder;

            using (DbCommand command = commandBuilder.BuildSelectCommand(where))
            {
                //Add selection Arguments to command parameter list
                if (selectionArgs != null)
                {
                    foreach (object obj in selectionArgs)
                    {
                        command.Parameters.Add(this.CreateParameter(null, obj));
                    }
                }

                return QuerySingleRecord<T>(command, entityDescription);
            }

        }

        public T QuerySingleRecord<T>(DbCommand command, EntityDescription entityDescription) 
            where T : new()
        {
            EntityInflator inflator = entityDescription.Inflator;

            DbDataReader reader = null;
            lock (_readOnlyConnectionSyncLock)
            {
                object newDO = default(T);
                DbConnection conn = OpenReadOnlyConnection(true);
                try
                {
                    command.Connection = conn;
                    reader = command.ExecuteReader();

                    inflator.CheckOrdinals(reader);
                    if (reader.Read())
                    {
                        newDO = inflator.CreateInstanceOfEntity();
                        inflator.ReadData(reader, newDO);
                    }
                }
                catch (Exception e)
                {
                    throw this.ThrowExceptionHelper(conn, command, e);
                }
                finally
                {
                    if (reader != null) { reader.Dispose(); }
                    ReleaseReadOnlyConnection();
                }
                return (T)newDO;
            }
        }


        #endregion

        #region IDisposable Support
        private bool isDisposed = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!isDisposed)
            {
                if (disposing)
                {
                    Debug.WriteLine("DatastoreContext Disposing ", Logging.DS_EVENT);
                }
                else
                {
                    Debug.WriteLine("DatastoreContext Finalized ", Logging.DS_EVENT);
                }

                //if(this._cache != null)
                //{
                //    _cache.Dispose();
                //}

                if (_ReadWriteConnection != null)
                {
                    ReleaseReadWriteConnection();
                }
                if (_ReadOnlyConnection != null)
                {
                    ReleaseReadOnlyConnection();
                }

                isDisposed = true;
            }
        }

        ~DataStoreContext()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        #endregion
    }
}
