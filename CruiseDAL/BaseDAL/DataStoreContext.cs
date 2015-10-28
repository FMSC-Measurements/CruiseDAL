using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CruiseDAL.Core.Constants;
using System.Diagnostics;
using System.Threading;
using System.ComponentModel;
using CruiseDAL.Core.EntityModel;
using CruiseDAL.BaseDAL.SQL;

namespace CruiseDAL.BaseDAL
{
    public abstract class DataStoreContext : IDisposable
    {
        public DatastoreBase DataStore { get; set; }

        public DbConnectionStringBuilder ConnectionStringBuilder { get; protected set; }

        protected Object _readOnlyConnectionSyncLock = new object();
        protected Object _readWriteConnectionSyncLock = new object();

        protected DbConnection _ReadWriteConnection;
        protected DbConnection _ReadOnlyConnection;

        protected Stack<DbTransaction> _transactionStack = new Stack<DbTransaction>();

        protected EntityCache _cache;


        #region abstract members
        //protected abstract DbConnection CreateConnection(string connectionString);
        protected abstract Exception ThrowExceptionHelper(DbConnection conn, DbCommand comm, Exception innerException);

        #endregion

        #region sugar
        DbCommand CreateCommand(string commandText)
        {
            return DbProviderFactoryAdapter.Instance.CreateCommand(commandText);
        }

        DbParameter CreateParameter(string name, object value)
        {
            return DbProviderFactoryAdapter.Instance.CreateParameter(name, value);
        }

        DbConnectionStringBuilder GetConnectionStringBuilder()
        {
            throw new NotImplementedException();
        }
        #endregion

        #region Transaction Management
        public DbTransaction BeginTransaction()
        {
            lock (_transactionStack)
            {
                DbConnection conn = null;
                try
                {
                    Debug.WriteLine("Transaction Started", Logging.DB_CONTROL);
                    conn = this.OpenReadWriteConnection(true);
                    DbTransaction transaction = conn.BeginTransaction();
                    this._transactionStack.Push(transaction);
                    return transaction;
                }
                catch (Exception e)
                {
                    throw this.ThrowExceptionHelper(conn, null, e);
                }
            }
        }

        public void CancelTransaction()
        {
            lock (_transactionStack)
            {
                if(_transactionStack.Count > 0)
                {
                    DbTransaction top = _transactionStack.Peek();
                    this.CancelTransaction(top);
                }
                else
                {
                    Debug.Fail("transaction stack empty, are you missing a BeginTransaction?");
                }    
            }
        }

        public void CancelTransaction(DbTransaction transaction)
        {
            lock (_transactionStack)
            {
                if (this._transactionStack.Contains(transaction))
                {
                    DbTransaction top = null;
                    do
                    {
                        top = _transactionStack.Pop();


                    } while (!Object.ReferenceEquals(top, transaction));

                    try
                    {
                        top.Rollback();
                        Debug.WriteLine("Transaction Rolled back", Logging.DB_CONTROL);
                    }
                    catch (Exception e)
                    {
                        throw this.ThrowExceptionHelper(top.Connection, null, e);
                    }
                }
                else
                {
                    Debug.Fail("transaction not on stack, are you missing a BeginTransaction?");
                }
            }
        }

        public void EndTransaction()
        {
            lock (_transactionStack)
            {
                if(_transactionStack.Count > 0)
                { 
                    DbTransaction top = _transactionStack.Peek();
                    this.EndTransaction(top);
                }
                else
                { 
                    Debug.Fail("transaction stack empty, are you missing a BeginTransaction?");
                }
            }

        }

        public void EndTransaction(DbTransaction transaction)
        {
            lock (_transactionStack)
            {
                if (this._transactionStack.Contains(transaction))
                {
                    DbTransaction top = null;
                    do
                    {
                        top = _transactionStack.Pop();



                    } while (!Object.ReferenceEquals(top, transaction));

                    try
                    {
                        top.Commit();
                        Debug.WriteLine("Transaction Committed", Logging.DB_CONTROL);
                    }
                    catch (Exception e)
                    {
                        throw this.ThrowExceptionHelper(top.Connection, null, e);
                    }
                }
                else
                {
                    Debug.Fail("transaction not on stack, are you missing a BeginTransaction?");
                }
            }
        }

        #endregion

        #region Persistence Methods
        public void Insert(object data,  OnConflictOption option)
        {
            EntityCommandBuilder builder = GetEntityCommandBuilder(data.GetType());
            using (DbCommand command = builder.BuildInsertCommand(data, option))
            {
                object result = ExecuteScalar(command);
                throw new NotImplementedException("need to set key on data");

                data.IsPersisted = true;
                data.HasChanges = false;
            }
        }

        //public void Update(DataObject data, OnConflictOption option)
        //{
        //    this.Update(data, data.rowID, option);
        //}

        public void Update(object data, OnConflictOption option)
        {
            EntityCommandBuilder builder = GetEntityCommandBuilder(data.GetType());
            using (DbCommand command = builder.BuildUpdateCommand(data, DataStore.User, option))
            {
                ExecuteSQL(command);
            }

        }

        public void Delete(object data)
        {
            lock (data)
            {
                if (data.IsPersisted)
                {
                    EntityCommandBuilder builder = GetEntityCommandBuilder(data.GetType());
                    using (DbCommand command = builder.BuildSQLDeleteCommand(data))
                    {
                        ExecuteSQL(command);

                        data.IsDeleted = true;
                    }
                }
            }
        }


        #endregion

        #region command execution methods

        public T ExecuteScalar<T>(String query)
        {
            return ExecuteScalar<T>(query, null);
        }

        public T ExecuteScalar<T>(String query, IDictionary<String, object> parameters)
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
        /// Executes sql command returning single value
        /// </summary>
        /// <param name="command"></param>
        /// <returns>value or null</returns>
        /// 
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
                return (T)Convert.ChangeType(result, typeof(T));
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
                    throw this.ThrowDatastoreExceptionHelper(conn, command, e);
                }
                finally
                {
                    ReleaseReadWriteConnection();
                }
            }

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
                throw this.ThrowDatastoreExceptionHelper(connection, command, e, false);
            }
        }

        protected DbDataReader ExecuteReader(DbCommand command)
        {
            lock(_readOnlyConnectionSyncLock)
            {
                DbConnection conn = OpenReadOnlyConnection(true);
                command.Connection = conn;
                return command.ExecuteReader();

            }

        }
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

        protected DbConnection CreateReadWriteConnection()
        {
            DbConnectionStringBuilder builder = GetConnectionStringBuilder();
            builder.Add("Read Only", true);

            DbConnection conn = DbProviderFactoryAdapter.Instance.CreateConnection();
            conn.ConnectionString = builder.ToString();
            conn.StateChange += _Connection_StateChange;
            return conn;
        }

        protected DbConnection CreateReadOnlyConnection()
        {
            DbConnectionStringBuilder builder = GetConnectionStringBuilder();

            DbConnection conn = DbProviderFactoryAdapter.Instance.CreateConnection();
            conn.ConnectionString = builder.ToString();
            conn.StateChange += _Connection_StateChange;
            return conn;
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

        void ReleaseConnection(DbConnection conn)
        {
            conn.Close();
            conn.Dispose();
        }

        void ReleaseReadOnlyConnection()
        {
            lock(_readOnlyConnectionSyncLock)
            {
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
                if (_ReadOnlyConnection == null) { return; }
                ReleaseConnection(_ReadWriteConnection);
                _ReadWriteConnection = null;
                Debug.WriteLine("ReadWrite Connection Released", Logging.DB_CONTROL_VERBOSE);
            }
        }

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

                if(this._cache != null)
                {
                    _cache.Dispose();
                }

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

        #endregion

        #region Read Methods

        public IList<T> Read<T>(WhereClause where, params Object[] selectionArgs)
            where T : new()
        {
            EntityCommandBuilder commandBuilder = GetEntityCommandBuilder(typeof(T));
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

                return Read<T>(command);
            }
        }

        internal IList<T> Read<T>(DbCommand command) where T : new()
        {
            List<T> doList = new List<T>();
            EntityCache cache = GetEntityCache(typeof(T));
            EntityInflator inflator = GetEntityInflator(typeof(T));

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
        ///// Retrieves a single row from the database Note: dataobject type must match the
        ///// table.
        ///// </summary>
        ///// <typeparam name="T">Type of dataobject to return</typeparam>
        ///// <param name="tableName">Name of table to read from</param>
        ///// <param name="selection">the where clause to define selection Note: only the first row from the resulting selection will be returned</param>
        ///// <param name="selectionArgs">array of paramatures to use with selection string</param>
        ///// <exception cref="DatabaseExecutionException"></exception>
        ///// <returns>a single dataobject</returns>
        //public T ReadSingleRow<T>(String tableName, String selection, params Object[] selectionArgs) where T : ReadDataObject, new()
        //{
        //    return ReadSingleRow<T>(tableName, true, selection, selectionArgs);
        //}

        public T ReadSingleRow<T>(WhereClause where, params Object[] selectionArgs) where T : DataObject, new()
        {

            EntityCommandBuilder commandBuilder = GetEntityCommandBuilder(typeof(T));
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

                return ReadSingleRow<T>(command);
            }
        }

        /// <summary>
        /// Retrieves a single row from the database Note: dataobject type must match the 
        /// table. 
        /// </summary>
        /// <typeparam name="T">Type of dataobject to return</typeparam>
        /// <param name="tableName">Name of table to read from</param>
        /// <param name="rowID">row id of the row to read</param>
        /// <exception cref="DatabaseExecutionException"></exception>
        /// <returns>a single databoject</returns>
        public T ReadSingleRow<T>(object primaryKeyValue) where T : DataObject, new()
        {
            return ReadSingleRow<T>(new WhereClause("rowID = ?"), primaryKeyValue);
        }

        internal T ReadSingleRow<T>(DbCommand command) where T : new()
        {
            object entity = null;
            EntityCache cache = GetEntityCache(typeof(T));
            EntityInflator inflator = GetEntityInflator(typeof(T));

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
        public List<T> Query<T>(DbCommand command) where T : new()
        {
            EntityInflator inflator = GetEntityInflator(typeof(T));
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

        public List<T> Query<T>(WhereClause where, params Object[] selectionArgs) where T : new()
        {
            EntityCommandBuilder commandBuilder = GetEntityCommandBuilder(typeof(T));
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

                return Query<T>(command);
            }

        }

        public T QuerySingleRecord<T>(WhereClause where, params Object[] selectionArgs) where T : new()
        {
            EntityCommandBuilder commandBuilder = GetEntityCommandBuilder(typeof(T));
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

                return QuerySingleRecord<T>(command);
            }

        }

        public T QuerySingleRecord<T>(DbCommand command)
        {
            EntityInflator inflator = GetEntityInflator(typeof(T));

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
    }
}
