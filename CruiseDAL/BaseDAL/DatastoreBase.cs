using System;
using System.Collections.Generic;
using System.Text;
using System.Data.Common;
using System.Collections;
using Logger;

#if ANDROID
using Mono.Data.Sqlite;
using SQLiteCommand = Mono.Data.Sqlite.SqliteCommand;
using SQLiteParameter = Mono.Data.Sqlite.SqliteParameter;
using SQLiteConnection = Mono.Data.Sqlite.SqliteConnection;
#else
using System.Data.SQLite;
using System.Diagnostics;
#endif

namespace CruiseDAL
{
    public abstract class DatastoreBase
    {

        protected DbConnection _Connection;

        //protected Dictionary<int, DbConnection> _connectionTracker = new Dictionary<int, DbConnection>();
        protected int _holdConnection = 0;
        //protected int _openConnectionCount = 0;
        protected Object _connectionSyncLock = new object();
        protected Stack<DbTransaction> _transactionStack = new Stack<DbTransaction>(); 
        protected Action _buildSchemaCallerHandle;

        internal ObjectCache _IDTable;
        private static Dictionary<Type, EntityDescription> DataObjectDescriptionLookup = new Dictionary<Type, EntityDescription>();

        protected string _ConnectionString
        {
            get;
            set;
        }

        /// <summary>
        /// Get the schema version
        /// </summary>
        private string _databaseVersion;
        public string DatabaseVersion
        {
            get
            {
                return _databaseVersion;
            }
            internal set
            {
                this._databaseVersion = value;
            }

        }

        private DataObjectFactory _doFactory;
        public DataObjectFactory DOFactory 
        {
            get { return this._doFactory; }
        }

        private string _userInfo;
        /// <summary>
        /// Gets the string used to identify the user, for the purpose of CreatedBy and ModifiedBy values
        /// </summary>
        public string User
        {
            get
            {
                if (_userInfo == null)
                {
                    _userInfo = this.GetUserInformation();
                }
                return _userInfo;
            }
        }


        protected void InitializeBase()
        {
            this._doFactory = new DataObjectFactory(this);
            this._IDTable = new ObjectCache(this._doFactory);
        }

        #region Schema Utility Methods
        /// <summary>
        /// Build the schema of the Database
        /// </summary>
        public void Create()
        {
            BuildDBFile();
        }

        /// <summary>
        /// Asyncronisly Starts Creating a file at <see cref="Path"/> 
        /// User should call <see cref="EndCreate"/> at some point after calling 
        /// </summary>
        /// <example><code>
        /// void DoSomething()
        /// {
        ///     ...
        ///     var result = BeginCreate(new AsyncCallback(OnCreateDone));
        ///     ...
        /// }
        /// 
        /// void OnCreateDone(IAsyncCallback result)
        /// {
        ///     ...
        ///     EndCreate(result);
        ///     ...
        /// }
        /// 
        /// or 
        /// 
        /// void DoSomething()
        /// {
        ///     ...
        ///     var result = BeginCreate(null);
        ///     ...
        ///     EndCreate(result);
        ///     ...
        /// }
        /// </code></example>
        /// <param name="callbackFunct">optional</param>
        /// <returns></returns>
        //public IAsyncResult BeginCreate(AsyncCallback callbackFunct)
        //{
        //    _buildSchemaCallerHandle = new AsyncBuildSchemaCaller(this.BuildDBFile);
        //    return _buildSchemaCallerHandle.BeginInvoke(callbackFunct, null);
        //}

        /// <summary>
        /// Waits for BeginCreate to end 
        /// </summary>
        /// <param name="result"></param>
        public void EndCreate(IAsyncResult result)
        {
            if (result == null)
            {
                throw new ArgumentNullException("result");
            }
            if (_buildSchemaCallerHandle == null)
            {
                throw new ArgumentException("You must call BeginCreate first");
            }

            _buildSchemaCallerHandle.EndInvoke(result);
        }

        public void DumpTable(System.IO.TextWriter writer, String tableName, OnConflictOption cOpt, String projection, String selection, params object[] selectionArgs)
        {
            this.DumpTable(writer, tableName, tableName, cOpt, projection, selection, selectionArgs);
        }

        public void DumpTable(System.IO.TextWriter writer, String tableName, String destinationTable, OnConflictOption cOpt, String projection, String selection, params object[] selectionArgs)
        {
            if (String.IsNullOrEmpty(projection))
            {
                projection = "*";
            }

            String query = String.Format("SELECT {0} FROM {1} {2};", projection, tableName, selection);
            DbCommand command = this.CreateCommand(query);
            if (selectionArgs != null)
            {
                foreach (object obj in selectionArgs)
                {
                    command.Parameters.Add(obj);
                }
            }

            lock (this._connectionSyncLock)
            {
                DbConnection conn = this.OpenConnection();
                try
                {
                    command.Connection = conn;

                    using (DbDataReader reader = command.ExecuteReader())
                    {
                        bool first = true; 
                        while(reader.Read())
                        {
                            if(first) 
                            { 
                                first = false; 
                                WriteTableDumpStart(destinationTable, cOpt, writer, reader);
                            }
                            else 
                            {
                                writer.Write(",");
                            }

                            WriteTableDumpRowValues(writer, reader);
                        }
                    }

                    writer.WriteLine(";--");
                }
                catch (Exception e)
                {
                    throw this.ThrowDatastoreExceptionHelper(conn, command, e);
                }
                finally
                {
                    this.ReleaseConnection();
                }
            }            
        }

        protected virtual void WriteTableDumpStart(String tableName, OnConflictOption cOpt, System.IO.TextWriter writer, DbDataReader reader)
        {
            String[] colNames = new String[reader.VisibleFieldCount];
                for(int i = 0; i < reader.VisibleFieldCount; i++)
                {
                    colNames[i] = reader.GetName(i);
                }

                writer.WriteLine("INSERT OR {2} INTO {0} ({1}) VALUES ", tableName, string.Join(",", colNames), cOpt.ToString());
        }

        protected abstract void WriteTableDumpRowValues(System.IO.TextWriter writer, DbDataReader reader);
 


        public List<ColumnInfo> GetTableInfo(string tableName)
        {
            List<ColumnInfo> colList = new List<ColumnInfo>();
            
            DbDataReader reader = null;
            lock (this._connectionSyncLock)
            {
                DbConnection conn = OpenConnection();
                DbCommand command = this.CreateCommand(conn, String.Format("PRAGMA table_info({0});", tableName));

                try
                {
                    reader = command.ExecuteReader();
                    int nameOrd = reader.GetOrdinal("name");
                    int dbTypeOrd = reader.GetOrdinal("type");
                    int pkOrd = reader.GetOrdinal("pk");
                    int notNullOrd = reader.GetOrdinal("notnull");
                    int defaultValOrd = reader.GetOrdinal("dflt_value");
                    while (reader.Read())
                    {
                        ColumnInfo colInfo = new ColumnInfo();
                        colInfo.Name = reader.GetString(nameOrd);
                        colInfo.DBType = reader.GetString(dbTypeOrd);
                        colInfo.IsPK = reader.GetBoolean(pkOrd);
                        colInfo.IsRequired = reader.GetBoolean(notNullOrd);
                        if (!reader.IsDBNull(defaultValOrd))
                        {
                            colInfo.Default = reader.GetString(defaultValOrd);
                        }
                        colList.Add(colInfo);
                    }
                }
                catch (Exception e)
                {
                    throw this.ThrowDatastoreExceptionHelper(conn, command, e);
                }
                finally
                {
                    if (reader != null) { reader.Dispose(); }
                    if (command != null) { command.Dispose(); }
                    ReleaseConnection();
                }
                return colList;
            }
        }

        public void FlushCache()
        {
            this._IDTable.Flush();
        }

        public void AddField(string tableName, string fieldDef)
        {
            try
            {
                string command = string.Format("ALTER TABLE {0} ADD COLUMN {1};", tableName, fieldDef);
                this.Execute(command);
            }
            catch (Exception e)
            {
                Logger.Log.E(e);
            }
        }

        public bool CheckTableExists(string tableName)
        {
            return GetRowCount("sqlite_master", "WHERE type = 'table' AND name = ?", tableName) > 0;
        }

        public bool CheckFieldExists(string tableName, string field)
        {
            try
            {
                this.Execute(String.Format("SELECT {0} FROM {1};", field, tableName));
                return true;
            }
            catch
            {
                return false;
            }
        }
        #endregion

        


        #region Persistence Methods

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <param name="option"></param>
        /// <exception cref="DatabaseExecutionException"></exception>
        public void Save(DataObject data, OnConflictOption option)
        {
            lock (data)
            {
                if (data.HasChanges == false) { return; }

                Logger.Log.V(string.Format("Saving DO | {0:20} | {1:15} | IsPersisted = {2}\r\n", data.GetType().Name, data.GetID(), data.IsPersisted));

                if (data.IsPersisted)
                {
                    this.Update(data, option); 
                }
                else
                {
                    this.Insert(data, false, option);
                    this._IDTable.Add(data, ObjectCache.AddBehavior.OVERWRITE);
                }
            }
        }

        public void Insert(DataObject data, bool usePresetRowID, OnConflictOption option)
        {
            using (DbCommand command = DatastoreBase.GetObjectDiscription(data.GetType()).CreateSQLInsert(data, User, usePresetRowID, option))
            {
                object result = ExecuteScalar(command);
                data.rowID = Convert.ToInt64(result);
            }
           
            data.IsPersisted = true;
            data.HasChanges = false;
        }

        public void Update(DataObject data, OnConflictOption option)
        {
            this.Update(data, data.rowID, option);
        }

        public void Update(DataObject data, object key, OnConflictOption option)
        {
            Debug.Assert(key != null);
            using (DbCommand command = DatastoreBase.GetObjectDiscription(data.GetType()).CreateSQLUpdate(data, key, User, option))
            {
                ExecuteSQL(command);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="list"></param>
        /// <exception cref="DatabaseExecutionException"></exception>
        public void Save(IEnumerable list)
        {
            this.Save(list, OnConflictOption.Fail);
        }

        public void Save(IEnumerable list, OnConflictOption opt)
        {
            lock (list)
            {
                List<KeyValuePair<DataObject, RecordState>> preState = new List<KeyValuePair<DataObject, RecordState>>(); //temporary store, of record states incase save fails and we need to restore the states                
                try
                {
                    BeginTransaction();
                    foreach (DataObject obj in list)
                    {
                        preState.Add(new KeyValuePair<DataObject, RecordState>(obj, obj.RecordState));
                        Save(obj, opt);
                    }                   
                    EndTransaction();
                }
                catch (Exception e)
                {
                    CancelTransaction();
                    foreach (KeyValuePair<DataObject, RecordState> s in preState)
                    {
                        s.Key.RecordState = s.Value;
                    }
                    throw this.ThrowDatastoreExceptionHelper(null, null, e);
                }
            }
        }

        public void Delete(DataObject data)
        {
            lock (data)
            {
                if (data.IsPersisted)
                {
                    Logger.Log.V(string.Format("Deleting DO | {0:20} | {1:15} | IsPersisted = {2}\r\n", data.GetType().Name, data.GetID(), data.IsPersisted));
                    using (DbCommand command = DatastoreBase.GetObjectDiscription(data.GetType()).CreateSQLDelete(data))
                    {
                        ExecuteSQL(command);
                    }
                    _IDTable.Remove(data);
                    data.IsDeleted = true;
                }
            }

        }

        public void ChangeRowID(DataObject data, long newRowID, OnConflictOption option)
        {
            lock (data)
            {
                if (data.IsPersisted == false)
                {
                    throw new InvalidOperationException("Can't change row id on unsaved data");
                }
                EntityDescription doi = DatastoreBase.GetObjectDiscription(data.GetType());
                string command = string.Format("UPDATE OR {3} {0} SET rowid = {2} WHERE rowID = {1};", doi.ReadSource, data.rowID, newRowID, option.ToString());
                this.Execute(command);
                data.rowID = newRowID;
            }
        }
        #endregion



        //internal virtual DataObjectInfo GetObjectDiscription(Type T)
        //{
        //    return DatastoreBase.GetObjectDiscription(T);
        //}

        internal static EntityDescription GetObjectDiscription(Type T)
        {
            lock (DataObjectDescriptionLookup)
            {
                if (!DataObjectDescriptionLookup.ContainsKey(T))
                {
                    EntityDescription des = new EntityDescription(T);
                    DataObjectDescriptionLookup.Add(T, des);
                    return des;
                }
                return DataObjectDescriptionLookup[T];
            }
        }


        internal virtual DbParameter CreateParameter(string name, object value)
        {
            return new SQLiteParameter(name, value);
        }

        internal virtual DbCommand CreateCommand(string commandText)
        {
            return new SQLiteCommand(commandText);
        }

        internal virtual DbCommand CreateCommand(DbConnection connection, String commandText)
        {
            DbCommand cmd = connection.CreateCommand();
            cmd.CommandText = commandText;
            return cmd;
        }

        protected virtual DbConnection CreateConnection(string connectionString)
        {
            return new SQLiteConnection(connectionString);
        }

        protected abstract Exception ThrowDatastoreExceptionHelper(string message, Exception innerException, bool throwException);
        protected abstract Exception ThrowDatastoreExceptionHelper(DbConnection conn, DbCommand comm, Exception innerException, bool throwException);
                
        protected Exception ThrowDatastoreExceptionHelper(DbConnection conn, DbCommand comm, Exception innerException)
        {
            return ThrowDatastoreExceptionHelper(conn, comm, innerException, false);
        }

        protected string GetDataStoreVitals()
        {
            string connectionString = String.Empty; 
            string connectionState = String.Empty;
            try
            {
                connectionString = (this._Connection != null) ? this._Connection.ConnectionString : "n/a";
                connectionState = (this._Connection != null) ? this._Connection.State.ToString() : "n/a";
            }
            catch (ObjectDisposedException)
            {
                connectionState = "Disposed";
            }

            return String.Format("ConnStr:{0} ConnState:{1} HoldConn:{2} TransactionStackSize:{3}",
                connectionString,
                connectionState,
                this._holdConnection,
                this._transactionStack.Count);

        }

        //protected virtual void ThrowDatabaseExecutionHelper(DbConnection conn, DbCommand comm, Exception innerException)
        //{
        //    string message = String.Format("Read/Write Error Command:{0} ConnStr:{1} ConnState:{2} HoldConn:{3} OpenConnCount:{4}",
        //        (comm != null) ? comm.CommandText : "n/a",
        //        (conn != null) ? conn.ConnectionString : "n/a",
        //        (conn != null) ? conn.State.ToString() : "n/a",
        //        this._holdConnection,
        //        this._openConnectionCount);
        //    DatabaseExecutionException ex = new DatabaseExecutionException(message, innerException);
        //    Logger.Log.E(ex);
        //    throw ex;
        //}

        //internal virtual void ThrowDatabaseExecutionHelper(String message, Exception innerException)
        //{
        //    message += String.Format("Read/Write Error  HoldConn:{0} OpenConnCount:{1}",
        //        this._holdConnection,
        //        this._openConnectionCount);
        //    DatabaseExecutionException ex = new DatabaseExecutionException(message, innerException);
        //    Logger.Log.E(ex);
        //    throw ex;
        //}

        #region Connection management 
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

        protected void ReleaseConnectionHold()
        {
            this._holdConnection = 0;
        }


        void _Connection_StateChange(object sender, System.Data.StateChangeEventArgs e)
        {
            Debug.WriteLine("Connection state changed From " + e.OriginalState.ToString() + " to " + e.CurrentState.ToString(), LOG_LEV_DS_EVENT);
        }

        protected virtual DbConnection OpenConnection()
        {
            this.EnterConnectionHold();
            try
            {
                lock (this._connectionSyncLock)
                {

                    Debug.WriteLine("Connection In Use", LOG_LEV_DB_CONTROL_VERBOSE);
                    if (this._Connection != null)
                    {
                        return this._Connection;
                    }
                    else
                    {
                        System.Diagnostics.Debug.WriteLine("Connection created, threadID: " + System.Threading.Thread.CurrentThread.ManagedThreadId.ToString(), LOG_LEV_DS_EVENT);
                        this._Connection = CreateConnection(this._ConnectionString);
                        this._Connection.StateChange += new System.Data.StateChangeEventHandler(_Connection_StateChange);
                        this._Connection.Open();
                        
                        return this._Connection;
                    }
                }
            }
            catch (Exception e)
            {
                throw this.ThrowDatastoreExceptionHelper(this._Connection, null, e);
            }            
        }

        
        internal void ReleaseConnection()
        {            
            lock (this._connectionSyncLock)
            {
                Debug.WriteLine("Connection Released", LOG_LEV_DB_CONTROL_VERBOSE); 
                if (this._holdConnection == 1)
                {
                    Debug.Assert(_Connection != null);
                    _Connection.Dispose();
                    _Connection = null; 
                    Debug.WriteLine("Conection Closed", LOG_LEV_DS_EVENT);                         
                }
                this.ExitConnectionHold();
            }
        }
                    
#endregion

        #region Read/Write Methods
        /// <summary>
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="selection">ex "Where Column = ?", "Group By Column"</param>
        /// <param name="selectionArgs">Parameters for <see cref="selection"/> (? == parameter)</param>
        /// <returns>Number of rows matching selection</returns>
        public Int64 GetRowCount(string tableName, string selection, params Object[] selectionArgs)
        {
            string query = string.Format("SELECT Count(1) FROM {0} {1};", tableName, selection);
            lock (this._connectionSyncLock)
            {
                DbConnection conn = OpenConnection();
                DbCommand command = this.CreateCommand(conn, query);

                //add parameters if there are any
                if (selectionArgs != null)
                {
                    foreach (object obj in selectionArgs)
                    {
                        command.Parameters.Add(this.CreateParameter(null, obj));
                    }
                }
                Int64 result = 0L;

                try
                {
                    result = (Int64)command.ExecuteScalar();

                }
                catch (Exception e)
                {
                    throw this.ThrowDatastoreExceptionHelper(conn, command, e);
                }
                finally
                {
                    if (command != null) { command.Dispose(); }
                    ReleaseConnection();
                }
                return result;
            }

        }

        internal List<T> Read<T>(DbCommand command, bool cache, EntityDescription des) where T : DataObject, new()
        {
            List<T> doList = new List<T>();
            DbDataReader reader = null;            
            lock (this._connectionSyncLock)
            {
                DbConnection conn = OpenConnection();
                try
                {

                    command.Connection = conn;
                    reader = command.ExecuteReader();
                    //for each row in the data reader we are going to use
                    //the parser to get the id value for each row and use the id to get 
                    //a data object from the IDTable, with the IDTable 
                    //returning a new data object if one doesn't already 
                    //exist. then we will pass a message to our parser
                    //to write the data from the data reader into our data object
                    des.StartRead(reader);
                    while (reader.Read())
                    {
                        DataObject currentDO = null;
                        bool skipRead = false;
                        if (cache )
                        {
                            currentDO = _IDTable.GetByID<T>(des.ReadID(reader));
                            skipRead = ((DataObject)currentDO).IsPersisted; //&& !((DataObject)currentDO).HasChanges;
                        }
                        else
                        {
                            currentDO = _doFactory.GetNew<T>();
                        }
                        if (!cache || !skipRead)
                        {
                            currentDO.SuspendEvents();//disable notify property change on object
                            des.ReadData(reader, currentDO);
                            currentDO.ResumeEvents();
                        }
                        doList.Add((T)currentDO);
                    }
                }

                catch (Exception e)
                {
                    throw this.ThrowDatastoreExceptionHelper(conn, command, e);
                }
                finally
                {
                    if (reader != null) { reader.Dispose(); }
                    ReleaseConnection();
                }
                return doList;
            }
        }




        ///// <summary>
        ///// Reads a collection of rows from the database and returns them as a list of dataobjects
        ///// </summary>
        ///// <typeparam name="T"></typeparam>
        ///// <param name="tableName"></param>
        ///// <param name="selection"></param>
        ///// <param name="selectionArgs"></param>
        ///// <exception cref="DatabaseExecutionException"></exception>
        ///// <returns></returns>
        //public List<T> Read<T>(string tableName, String selection, params Object[] selectionArgs)
        //    where T : ReadDataObject, new()
        //{
        //    return this.Read<T>(tableName, true, selection, selectionArgs);
        //}

        /// <summary>
        /// Reads a collection of rows from the database and returns them as a list of dataobjects
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="tableName"></param>
        /// <param name="cache">if true, data objects will be cached and may be accessed by other calls to read</param>
        /// <param name="selection"></param>
        /// <param name="selectionArgs"></param>
        /// <returns></returns>
        public List<T> Read<T>(string tableName, String selection, params Object[] selectionArgs)
            where T : DataObject, new()
        {
            EntityDescription des = DatastoreBase.GetObjectDiscription(typeof(T));
            bool cache = des.IsCached;
            string query = String.Format(des.GetSelectCommandFormat(), selection);
            using (DbCommand command = this.CreateCommand(query))
            {

                //Add selection Arugments to command parameter list
                if (selectionArgs != null)
                {
                    foreach (object obj in selectionArgs)
                    {
                        command.Parameters.Add(this.CreateParameter(null, obj));
                    }
                }

                return Read<T>(command, cache, des);
            }
        }

        public T QuerySingleRecord<T>(String selectCommand, params Object[] selectionArgs) where T : new()
        {
            EntityDescription doi = DatastoreBase.GetObjectDiscription(typeof(T));
            DbCommand command = this.CreateCommand(selectCommand);

            //Add selection Arugments to command parameter list
            if (selectionArgs != null)
            {
                foreach (object obj in selectionArgs)
                {
                    command.Parameters.Add(this.CreateParameter(null, obj));
                }
            }

            DbDataReader reader = null;
            lock (this._connectionSyncLock)
            {
                T newDO = default(T);
                DbConnection conn = OpenConnection();
                try
                {
                    command.Connection = conn;
                    reader = command.ExecuteReader();

                    doi.StartRead(reader, true);
                    if (reader.Read())
                    {
                        newDO = new T();
                        doi.ReadData(reader, newDO);
                    }
                }
                catch (Exception e)
                {
                    throw this.ThrowDatastoreExceptionHelper(conn, command, e);
                }
                finally
                {
                    if (reader != null) { reader.Dispose(); }
                    if (command != null) { command.Dispose(); }
                    ReleaseConnection();
                }
                return newDO;
            }

        }

        public List<T> Query<T>(String selectCommand, params Object[] selectionArgs) where T : new()
        {
            EntityDescription doi = DatastoreBase.GetObjectDiscription(typeof(T));
            DbCommand command = this.CreateCommand(selectCommand);

            //Add selection Arugments to command parameter list
            if (selectionArgs != null)
            {
                foreach (object obj in selectionArgs)
                {
                    command.Parameters.Add(this.CreateParameter(null, obj));
                }
            }

            List<T> dataList = new List<T>();
            DbDataReader reader = null;
            lock (this._connectionSyncLock)
            {
                DbConnection conn = OpenConnection();
                try
                {
                    command.Connection = conn;
                    reader = command.ExecuteReader();

                    doi.StartRead(reader, true);
                    while (reader.Read())
                    {
                        Object newDO = new T();
                        doi.ReadData(reader, newDO);
                        dataList.Add((T)newDO);
                    }
                }

                catch (Exception e)
                {
                    throw this.ThrowDatastoreExceptionHelper(conn, command, e);
                }
                finally
                {
                    if (reader != null) { reader.Dispose(); }
                    if (command != null) { command.Dispose(); }
                    ReleaseConnection();
                }
                return dataList;
            }

        }



        internal T ReadSingleRow<T>(DbCommand command, bool cache, EntityDescription des) where T : DataObject, new()
        {
            DataObject row = null;
            DbDataReader reader = null;
            lock (this._connectionSyncLock)
            {
                DbConnection conn = OpenConnection();
                try
                {
                    command.Connection = conn;
                    reader = command.ExecuteReader();
                    des.StartRead(reader);

                    if (reader.Read())
                    {
                        bool skipRead = false;
                        if (cache)
                        {
                            row = _IDTable.GetByID<T>(des.ReadID(reader));
                            skipRead = ((DataObject)row).IsPersisted; //&& ((DataObject)row).HasChanges;
                        }
                        else
                        {
                            row = _doFactory.GetNew<T>();
                        }
                        if (!cache || !skipRead)
                        {
                            row.SuspendEvents();//disable notify property change on object
                            des.ReadData(reader, row);
                            row.ResumeEvents();
                        }
                    }
                }
                catch (Exception e)
                {
                    throw this.ThrowDatastoreExceptionHelper(conn, command, e);
                }
                finally
                {
                    if (reader != null) { reader.Dispose(); }
                    ReleaseConnection();
                }
                return (T)row;
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
        public T ReadSingleRow<T>(String tableName, long? rowID) where T : DataObject, new()
        {
            if (rowID == null)
            { return null; }
            EntityDescription des = DatastoreBase.GetObjectDiscription(typeof(T));
            string query = String.Format(des.GetSelectCommandFormat(), "WHERE TableRowID = @RowID");
            using (DbCommand command = this.CreateCommand(query))
            {
                command.Parameters.Add(this.CreateParameter("@RowID", rowID.Value));

                return ReadSingleRow<T>(command, true, des);
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

        public T ReadSingleRow<T>(String tableName, String selection, params Object[] selectionArgs) where T : DataObject, new()
        {
            if (tableName == null)
            {
                return null;
            }
            EntityDescription des = DatastoreBase.GetObjectDiscription(typeof(T));
            bool cache = des.IsCached;
            string query = String.Format(des.GetSelectCommandFormat(), selection);
            using (DbCommand command = this.CreateCommand(query))
            {
                if (selectionArgs != null)
                {
                    foreach (Object obj in selectionArgs)
                    {
                        command.Parameters.Add(this.CreateParameter(null, obj));
                    }
                }

                return ReadSingleRow<T>(command, cache, des);
            }
        }

        #endregion

        /// <summary>
        /// Executes sql command returning number of rows affected
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

        public int? Execute(DbCommand command, params object[] parameters)
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

        
        //public object ExecuteScalar(string command)
        //{
        //    object value = ExecuteScalar(this.CreateCommand(command));
        //    return (value is DBNull) ? null : value;
        //}
        /// <summary>
        /// Executes sql command returning single value
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

        internal object ExecuteScalar(DbCommand command)
        {
            lock (this._connectionSyncLock)
            {
                DbConnection conn = OpenConnection();
                try
                {
                    command.Connection = conn;
                    return command.ExecuteScalar();
                }
                //catch (ThreadAbortException)
                //{
                //    return null;
                //}
                catch (Exception e)
                {
                    throw this.ThrowDatastoreExceptionHelper(conn, command, e);
                }
                finally
                {
                    ReleaseConnection();
                }
            }

        }

        internal int? ExecuteSQL(DbCommand command)
        {
            lock (this._connectionSyncLock)
            {
                DbConnection conn = OpenConnection();
                try
                {
                    return this.ExecuteSQL(command, conn);
                }
                finally
                {
                    ReleaseConnection();
                    
                }
            }
        }

        protected int? ExecuteSQL(DbCommand command, DbConnection connection)
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

        #region Abstract methods
        public abstract void LogMessage(string message, string level);

        protected abstract void BuildDBFile();

        protected abstract string GetCreateSQL();

        protected abstract string BuildConnectionString(bool isNew);

        protected abstract string BuildConnectionString(bool isNew, string path);

        protected abstract  string GetUserInformation();
        #endregion

        #region IDisposable Members
        bool _disposed = false;

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool isDisposing)
        {
            
            if (_disposed)
            {
                return;
            }

            if (isDisposing)
            {
                Debug.WriteLine("Datastore Disposing: " + GetDataStoreVitals(), LOG_LEV_DS_EVENT);
                this.FlushCache();
            }
            else
            {
                Debug.WriteLine("Datastore Finalized: " + GetDataStoreVitals(), LOG_LEV_DS_EVENT);
            }
            if (this._Connection != null)
            {
                this._holdConnection = 1;
                this.ReleaseConnection();
            }

            _disposed = true;
        }

        #endregion




    }
}
