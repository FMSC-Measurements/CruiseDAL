using System;
using System.Collections.Generic;
using System.Text;

using System.Collections;
using System.Data.Common;

using FMSC.ORM.Core.EntityModel;
using FMSC.ORM.Core.SQL;
using FMSC.ORM.Core.EntityAttributes;
using System.Diagnostics;
using System.Threading;
using FMSC.ORM.Core.SQL.QueryBuilder;

namespace FMSC.ORM.Core
{
    public abstract class DatastoreRedux : IDisposable
    {
        //const bool DEFAULT_RETRY_RO_CONNECTION_BEHAVIOR = false;
        //const bool DEFAULT_RETRY_RW_CONNECTION_BEHAVIOR = false;

        int _transactionHold = 0;
        protected int _holdConnection = 0;
        protected int _transactionDepth = 0;
        protected bool _transactionCanceled = false;

        //protected Object _readOnlyConnectionSyncLock = new object();
        //protected Object _readWriteConnectionSyncLock = new object();
        protected Object _persistentConnectionSyncLock = new object();

        protected DbConnection PersistentConnection { get; set; }

        //protected DbConnection _ReadWriteConnection;
        //protected DbConnection _ReadOnlyConnection;

        public object TransactionSyncLock = new object();
        protected DbTransaction _CurrentTransaction;

        protected Dictionary<Type, EntityCache> _entityCache;

        protected static Dictionary<string, EntityDescription> _globalEntityDescriptionLookup = new Dictionary<string, EntityDescription>();

        
        
        
        protected DbProviderFactoryAdapter Provider { get; set; }

        public DatabaseBuilder DatabaseBuilder { get; set; }
        public string Path { get; protected set; }

        protected DatastoreRedux(DbProviderFactoryAdapter provider)
        {
            this.Provider = provider;
        }


        #region sugar

        protected EntityCache GetEntityCache(Type type)
        {
            if (_entityCache == null) { _entityCache = new Dictionary<Type, EntityCache>(); }
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

        public static EntityDescription LookUpEntityByType(Type t)
        {
            string name = t.Name;
            if(!_globalEntityDescriptionLookup.ContainsKey(name))
            {
                
                _globalEntityDescriptionLookup.Add(name, new EntityDescription(t));
            }

            return _globalEntityDescriptionLookup[t.Name];
        }

        private EntityInflator GetEntityInflator(Type type)
        {
            return LookUpEntityByType(type).Inflator;
        }

        #endregion

        #region abstract members

        protected abstract string BuildConnectionString();
        protected abstract Exception ThrowExceptionHelper(DbConnection conn, DbCommand comm, Exception innerException);
        public abstract bool HasForeignKeyErrors(string table_name);
        public abstract List<ColumnInfo> GetTableInfo(string tableName);
        public abstract Int64 GetRowCount(string tableName, string selection, params Object[] selectionArgs);
        #endregion


        #region fluent interface
        public QueryBuilder<T> From<T>()
        {
            EntityDescription entityDescription = LookUpEntityByType(typeof(T));
            SQLSelectBuilder builder = entityDescription.CommandBuilder.MakeSelectCommand();

            return new QueryBuilder<T>(this, builder);
        }


        #endregion

        #region CRUD


        public object Insert(object data, SQL.OnConflictOption option)
        {
            OnInsertingData(data, option);
            EntityDescription entityDescription = LookUpEntityByType(data.GetType());
            PrimaryKeyFieldAttribute primaryKeyField = entityDescription.Fields.PrimaryKeyField;

            EntityCommandBuilder builder = entityDescription.CommandBuilder;

            object primaryKey = null;
            DbConnection conn = OpenConnection();
            try
            {
                
                using (DbCommand command = builder.BuildInsertCommand(Provider, data, option))
                {
                    ExecuteSQL(command);
                }

                if (primaryKeyField != null)
                {
                    if (primaryKeyField.KeyType == KeyType.RowID)
                    {
                        primaryKey = GetLastInsertRowID(conn);
                    }
                    else
                    {
                        primaryKey = GetLastInsertKeyValue(entityDescription.SourceName
                       , primaryKeyField.FieldName, conn);
                    }

                    primaryKeyField.SetFieldValue(data, primaryKey);
                }
            }
            finally
            {
                ReleaseConnection();
            }
            

            if (data is IPersistanceTracking)
            {
                ((IPersistanceTracking)data).IsPersisted = true;
                ((IPersistanceTracking)data).HasChanges = false;
                ((IPersistanceTracking)data).OnInserted();
            }

            return primaryKey;
        }

        public void Update(object data, SQL.OnConflictOption option)
        {
            OnUpdatingData(data);
            EntityDescription entityDescription = LookUpEntityByType(data.GetType());
            EntityCommandBuilder builder = entityDescription.CommandBuilder;

            using (DbCommand command = builder.BuildUpdateCommand(Provider, data, option))
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
            OnDeletingData(data);
            EntityDescription entityDescription = LookUpEntityByType(data.GetType());
            PrimaryKeyFieldAttribute keyFieldInfo = entityDescription.Fields.PrimaryKeyField;

            if (keyFieldInfo == null) { throw new InvalidOperationException("type doesn't have primary key field"); }

            EntityCommandBuilder builder = entityDescription.CommandBuilder;

            lock (data)
            {
                if (data is IPersistanceTracking)
                {
                    Debug.Assert(((IPersistanceTracking)data).IsPersisted == true);
                    ((IPersistanceTracking)data).OnDeleting();
                }

                using (DbCommand command = builder.BuildSQLDeleteCommand(Provider, data))
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

        public void Save(IPersistanceTracking data, SQL.OnConflictOption option)
        {
            Save(data, option, true);
        }

        public void Save(IPersistanceTracking data, SQL.OnConflictOption option, bool cache)
        {
            if (data.HasChanges == false) { return; }
            if (!data.IsPersisted)
            {
                object primaryKey = Insert(data, option);
                if (cache && primaryKey != null)
                {
                    EntityCache cacheStore = GetEntityCache(data.GetType());

                    Debug.Assert(cacheStore.ContainsKey(primaryKey) == false);
                    cacheStore.Add(primaryKey, data);
                }
            }
            else
            {
                Update(data, option);
            }
        }


        #region read methods
        [Obsolete("use Read<T>(string selection, params Object[] selectionArgs) instead")]
        public List<T> Read<T>(string tableName, string selection, params Object[] selectionArgs) where T : new()
        {
            return Read<T>(selection, selectionArgs);
        }

        public List<T> Read<T>(string selection, params object[] selectionArgs)
            where T : new()
        {
            EntityDescription entityDescription = LookUpEntityByType(typeof(T));
            EntityCommandBuilder commandBuilder = entityDescription.CommandBuilder;

            using (DbCommand command = commandBuilder.BuildSelectLegacy(Provider, selection))
            {
                //Add selection Arguments to command parameter list
                if (selectionArgs != null)
                {
                    foreach (object obj in selectionArgs)
                    {
                        command.Parameters.Add(Provider.CreateParameter(null, obj));
                    }
                }

                return Read<T>(command, entityDescription);
            }
        }

        public List<T> Read<T>(WhereClause where, params Object[] selectionArgs)
            where T : new()
        {
            EntityDescription entityDescription = LookUpEntityByType(typeof(T));
            EntityCommandBuilder commandBuilder = entityDescription.CommandBuilder;

            using (DbCommand command = commandBuilder.BuildSelectCommand(Provider, where))
            {
                //Add selection Arguments to command parameter list
                if (selectionArgs != null)
                {
                    foreach (object obj in selectionArgs)
                    {
                        command.Parameters.Add(Provider.CreateParameter(null, obj));
                    }
                }

                return Read<T>(command, entityDescription);
            }
        }

        internal IEnumerable<TResult> Read<TResult>(SQLSelectBuilder selectBuilder, params Object[] selectionArgs)
        {

            using (DbCommand command = Provider.CreateCommand())
            {
                command.CommandText = selectBuilder.ToSQL() + ";";

                //Add selection Arguments to command parameter list
                if (selectionArgs != null)
                {
                    foreach (object obj in selectionArgs)
                    {
                        command.Parameters.Add(Provider.CreateParameter(null, obj));
                    }
                }

                EntityDescription entityDescription = LookUpEntityByType(typeof(TResult));
                EntityCache cache = GetEntityCache(typeof(TResult));
                EntityInflator inflator = entityDescription.Inflator;

                lock (_persistentConnectionSyncLock)
                {
                    DbConnection conn = OpenConnection();
                    try
                    {
                        command.Connection = conn;
                        using (DbDataReader reader = command.ExecuteReader())
                        {

                            inflator.CheckOrdinals(reader);
                            while (reader.Read())
                            {
                                Object entity = null;
                                try
                                {
                                    object key = inflator.ReadPrimaryKey(reader);
                                    if (key != null && cache.ContainsKey(key))
                                    {
                                        entity = cache[key];
                                    }
                                    else
                                    {
                                        entity = inflator.CreateInstanceOfEntity();
                                        if (key != null)
                                        {
                                            cache.Add(key, entity);
                                        }
                                        if (entity is IDataObject)
                                        {
                                            ((IDataObject)entity).DAL = this;
                                        }
                                    }

                                    inflator.ReadData(reader, entity);
                                }
                                catch (Exception e)
                                {
                                    throw this.ThrowExceptionHelper(conn, command, e);
                                }

                                yield return (TResult)entity;
                            }
                        }
                    }
                    finally
                    {
                        ReleaseConnection();
                    }
                }
            }
        }

        //protected IEnumerable<TResult> Read<TResult>(DbCommand command)
        //{
        //    EntityDescription entityDescription = LookUpEntityByType(typeof(TResult));
        //    EntityCache cache = GetEntityCache(typeof(TResult));
        //    EntityInflator inflator = entityDescription.Inflator;

        //    lock (_persistentConnectionSyncLock)
        //    {
        //        DbConnection conn = OpenConnection();
        //        try
        //        {
        //            command.Connection = conn;
        //            using (DbDataReader reader = command.ExecuteReader())
        //            {

        //                inflator.CheckOrdinals(reader);
        //                while (reader.Read())
        //                {
        //                    Object entity = null;
        //                    try
        //                    {
        //                        object key = inflator.ReadPrimaryKey(reader);
        //                        if (key != null && cache.ContainsKey(key))
        //                        {
        //                            entity = cache[key];
        //                        }
        //                        else
        //                        {
        //                            entity = inflator.CreateInstanceOfEntity();
        //                            if (key != null)
        //                            {
        //                                cache.Add(key, entity);
        //                            }
        //                            if (entity is IDataObject)
        //                            {
        //                                ((IDataObject)entity).DAL = this;
        //                            }
        //                        }

        //                        inflator.ReadData(reader, entity);
        //                    }
        //                    catch (Exception e)
        //                    {
        //                        throw this.ThrowExceptionHelper(conn, command, e);
        //                    }

        //                    yield return (TResult)entity;
        //                }
        //            }
        //        }
        //        finally
        //        {
        //            ReleaseConnection();
        //        }
        //    }

        //}

        protected List<T> Read<T>(DbCommand command, EntityDescription entityDescription)
            where T : new()
        {
            List<T> doList = new List<T>();
            EntityCache cache = GetEntityCache(typeof(T));
            EntityInflator inflator = entityDescription.Inflator;

            lock (_persistentConnectionSyncLock)
            {
                DbConnection conn = OpenConnection();
                try
                {
                    command.Connection = conn;
                    using (DbDataReader reader = command.ExecuteReader())
                    {

                        inflator.CheckOrdinals(reader);
                        while (reader.Read())
                        {
                            Object entity = null;
                            object key = inflator.ReadPrimaryKey(reader);
                            if (key != null && cache.ContainsKey(key))
                            {
                                entity = cache[key];
                            }
                            else
                            {
                                entity = inflator.CreateInstanceOfEntity();
                                if (key != null)
                                {
                                    cache.Add(key, entity);
                                }
                                if (entity is IDataObject)
                                {
                                    ((IDataObject)entity).DAL = this;
                                }
                            }

                            inflator.ReadData(reader, entity);

                            doList.Add((T)entity);
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
                return doList;
            }
        }

        


        [Obsolete("use ReadSingleRow<T>(string selection, params Object[] selectionArgs)")]
        public T ReadSingleRow<T>(string tableName, string selection, params Object[] selectionArgs) where T : new()
        {
            return ReadSingleRow<T>(selection, selectionArgs);
            //return Context.ReadSingleRow<T>(selection, selectionArgs);
        }

        public T ReadSingleRow<T>(string selection, params Object[] selectionArgs) where T : new()
        {
            EntityDescription entityDescription = LookUpEntityByType(typeof(T));
            EntityCommandBuilder commandBuilder = entityDescription.CommandBuilder;

            using (DbCommand command = commandBuilder.BuildSelectLegacy(Provider, selection))
            {
                //Add selection Arguments to command parameter list
                if (selectionArgs != null)
                {
                    foreach (object obj in selectionArgs)
                    {
                        command.Parameters.Add(Provider.CreateParameter(null, obj));
                    }
                }

                return ReadSingleRow<T>(command, entityDescription);
            }
            //return Context.ReadSingleRow<T>(selection, selectionArgs);
        }

        public T ReadSingleRow<T>(WhereClause where, params Object[] selectionArgs)
            where T : new()
        {

            EntityDescription entityDescription = LookUpEntityByType(typeof(T));
            EntityCommandBuilder commandBuilder = entityDescription.CommandBuilder;

            using (DbCommand command = commandBuilder.BuildSelectCommand(Provider, where))
            {
                //Add selection Arguments to command parameter list
                if (selectionArgs != null)
                {
                    foreach (object obj in selectionArgs)
                    {
                        command.Parameters.Add(Provider.CreateParameter(null, obj));
                    }
                }

                return ReadSingleRow<T>(command, entityDescription);
            }
        }

        internal T ReadSingleRow<T>(DbCommand command, EntityDescription entityDescription)
            where T : new()
        {
            object entity = null;
            EntityCache cache = GetEntityCache(typeof(T));
            EntityInflator inflator = entityDescription.Inflator;

            DbDataReader reader = null;
            lock (_persistentConnectionSyncLock)
            {
                DbConnection conn = OpenConnection();
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
                                ((IDataObject)entity).DAL = this;
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
                    ReleaseConnection();
                }
                return (T)entity;
            }
        }

        [Obsolete("use ReadSingleRow<T>(object primaryKey) instead")]
        public T ReadSingleRow<T>(string tableName, long? rowID) where T : new()
        {
            return ReadSingleRow<T>(rowID);
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

        protected long GetLastInsertRowID()
        {
            DbConnection conn = OpenConnection();
            try
            {
                return GetLastInsertRowID(conn);
            }
            finally
            {
                ReleaseConnection();
            }
        }

        protected long GetLastInsertRowID(DbConnection conn)
        {
            using (DbCommand command = Provider.CreateCommand("SELECT last_insert_rowid()"))
            {
                return this.ExecuteScalar<long>(command, conn);
            }
        }

        protected object GetLastInsertKeyValue(String tableName, String fieldName, DbConnection conn)
        {
            var ident = GetLastInsertRowID(conn);

            //String query = "Select " + fieldName + " FROM " + tableName + " WHERE rowid = last_insert_rowid();";
            using (DbCommand command = Provider.CreateCommand("SELECT " + fieldName + " FROM " + tableName + " WHERE rowid = ?;"))
            {
                command.Parameters.Add(Provider.CreateParameter(null, ident));
                var value = ExecuteScalar(command);
                Debug.Assert(value != null);
                return value;
            }
        }

        #endregion

        #region query methods
        public List<T> Query<T>(string selectCommand, params Object[] selectionArgs) where T : new()
        {
            DbCommand command = Provider.CreateCommand(selectCommand);

            //Add selection Arguments to command parameter list
            if (selectionArgs != null)
            {
                foreach (object obj in selectionArgs)
                {
                    command.Parameters.Add(Provider.CreateParameter(null, obj));
                }
            }

            EntityDescription entityType = LookUpEntityByType(typeof(T));
            return Query<T>(command, entityType);
        }

        public List<T> Query<T>(WhereClause where, params Object[] selectionArgs)
            where T : new()
        {
            EntityDescription entityDescription = LookUpEntityByType(typeof(T));
            EntityCommandBuilder commandBuilder = entityDescription.CommandBuilder;

            using (DbCommand command = commandBuilder.BuildSelectCommand(Provider, where))
            {
                //Add selection Arguments to command parameter list
                if (selectionArgs != null)
                {
                    foreach (object obj in selectionArgs)
                    {
                        command.Parameters.Add(Provider.CreateParameter(null, obj));
                    }
                }

                return Query<T>(command, entityDescription);
            }

        }

        internal IEnumerable<TResult> Query<TResult>(SQLSelectBuilder selectBuilder, params Object[] selectionArgs)
        {

            using (DbCommand command = Provider.CreateCommand())
            {
                command.CommandText = selectBuilder.ToSQL() + ";";

                //Add selection Arguments to command parameter list
                if (selectionArgs != null)
                {
                    foreach (object obj in selectionArgs)
                    {
                        command.Parameters.Add(Provider.CreateParameter(null, obj));
                    }
                }

                EntityDescription entityDescription = LookUpEntityByType(typeof(TResult));
                EntityInflator inflator = entityDescription.Inflator;

                lock (_persistentConnectionSyncLock)
                {
                    DbConnection conn = OpenConnection();
                    try
                    {
                        command.Connection = conn;
                        using (DbDataReader reader = command.ExecuteReader())
                        {
                            inflator.CheckOrdinals(reader);

                            while (reader.Read())
                            {
                                Object newDO = inflator.CreateInstanceOfEntity();
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
                                    throw this.ThrowExceptionHelper(conn, command, e);
                                }
                                yield return (TResult)newDO;
                            }
                        }

                    }
                    finally
                    {
                        ReleaseConnection();
                    }
                }

            }
        }

        //protected IEnumerable<TResult> Query<TResult>(DbCommand command)
        //{
        //    EntityDescription entityDescription = LookUpEntityByType(typeof(TResult));
        //    EntityInflator inflator = entityDescription.Inflator;

        //    lock (_persistentConnectionSyncLock)
        //    {
        //        DbConnection conn = OpenConnection();
        //        try
        //        {
        //            command.Connection = conn;
        //            using (DbDataReader reader = command.ExecuteReader())
        //            {
        //                inflator.CheckOrdinals(reader);

        //                while (reader.Read())
        //                {
        //                    Object newDO = inflator.CreateInstanceOfEntity();
        //                    try
        //                    {
        //                        inflator.ReadData(reader, newDO);
        //                    }
        //                    catch (Exception e)
        //                    {
        //                        throw this.ThrowExceptionHelper(conn, command, e);
        //                    }
        //                    yield return (TResult)newDO;
        //                }
        //            }

        //        }
        //        finally
        //        {
        //            ReleaseConnection();
        //        }
        //    }
        //}

        protected List<T> Query<T>(DbCommand command, EntityDescription entityDescription)
            where T : new()
        {
            EntityInflator inflator = entityDescription.Inflator;
            List<T> dataList = new List<T>();
            DbDataReader reader = null;
            lock (_persistentConnectionSyncLock)
            {
                DbConnection conn = OpenConnection();
                try
                {
                    command.Connection = conn;
                    reader = command.ExecuteReader();
                    inflator.CheckOrdinals(reader);

                    while (reader.Read())
                    {
                        Object newDO = inflator.CreateInstanceOfEntity();
                        if (newDO is IDataObject)
                        {
                            ((IDataObject)newDO).DAL = this;
                        }
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
                    ReleaseConnection();
                }
                return dataList;
            }
        }

        public T QuerySingleRecord<T>(string selectCommand, params Object[] selectionArgs) where T : new()
        {
            DbCommand command = Provider.CreateCommand(selectCommand);

            //Add selection Arguments to command parameter list
            if (selectionArgs != null)
            {
                foreach (object obj in selectionArgs)
                {
                    command.Parameters.Add(Provider.CreateParameter(null, obj));
                }
            }

            EntityDescription entityType = LookUpEntityByType(typeof(T));
            return QuerySingleRecord<T>(command, entityType);
        }

        public T QuerySingleRecord<T>(WhereClause where, params Object[] selectionArgs)
            where T : new()
        {
            EntityDescription entityDescription = LookUpEntityByType(typeof(T));
            EntityCommandBuilder commandBuilder = entityDescription.CommandBuilder;

            using (DbCommand command = commandBuilder.BuildSelectCommand(Provider, where))
            {
                //Add selection Arguments to command parameter list
                if (selectionArgs != null)
                {
                    foreach (object obj in selectionArgs)
                    {
                        command.Parameters.Add(Provider.CreateParameter(null, obj));
                    }
                }

                return QuerySingleRecord<T>(command, entityDescription);
            }

        }

        protected T QuerySingleRecord<T>(DbCommand command, EntityDescription entityDescription)
            where T : new()
        {
            EntityInflator inflator = entityDescription.Inflator;

            DbDataReader reader = null;
            lock (_persistentConnectionSyncLock)
            {
                object newDO = null;
                DbConnection conn = OpenConnection();
                try
                {
                    command.Connection = conn;
                    reader = command.ExecuteReader();

                    inflator.CheckOrdinals(reader);
                    if (reader.Read())
                    {
                        newDO = inflator.CreateInstanceOfEntity();
                        if (newDO is IDataObject)
                        {
                            ((IDataObject)newDO).DAL = this;
                        }
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
                    ReleaseConnection();
                }
                return (T)newDO;
            }
        }

        #endregion

        #endregion

        #region general purpose command execution

        /// <summary>
        /// Executes SQL command returning number of rows affected
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        public int Execute(String command, params object[] parameters)
        {
            using (DbCommand com = Provider.CreateCommand(command))
            {
                return this.Execute(com, parameters);
            }
        }

        public int Execute(String command, IEnumerable<KeyValuePair<String, object>> parameters)
        {
            using (DbCommand comm = Provider.CreateCommand(command))
            {
                if (parameters != null)
                {
                    foreach (var pair in parameters)
                    {
                        var param = Provider.CreateParameter(pair.Key, pair.Value);
                        comm.Parameters.Add(param);
                    }
                }
                return ExecuteSQL(comm);
            }
        }

        protected int Execute(DbCommand command, params object[] parameters)
        {
            if (parameters != null)
            {
                foreach (object p in parameters)
                {
                    command.Parameters.Add(Provider.CreateParameter(null, p));
                }
            }
            return ExecuteSQL(command);
        }

        protected int ExecuteSQL(DbCommand command)
        {
            lock (_persistentConnectionSyncLock)
            {
                DbConnection conn = OpenConnection();
                try
                {
                    return ExecuteSQL(command, conn);
                }                
                finally
                {
                    ReleaseConnection();
                }
            }
        }

        protected int ExecuteSQL(DbCommand command, DbConnection conn)
        {
            try
            {
                command.Connection = conn;
                return command.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                throw this.ThrowExceptionHelper(conn, command, e);
            }
        }


        /// <summary>
        /// Executes SQL command returning single value
        /// </summary>
        /// <param name="command"></param>
        /// <returns>value or null</returns>
        public object ExecuteScalar(string query, params object[] parameters)
        {
            using (DbCommand comm = Provider.CreateCommand(query))
            {
                if (parameters != null)
                {
                    foreach (object val in parameters)
                    {
                        comm.Parameters.Add(Provider.CreateParameter(null, val));
                    }
                }
                object value = ExecuteScalar(comm);
                return (value is DBNull) ? null : value;
            }

        }

        protected object ExecuteScalar(DbCommand command)
        {
            lock (_persistentConnectionSyncLock)
            {
                DbConnection conn = OpenConnection();
                try
                {
                    return ExecuteScalar(command, conn);
                }
                finally
                {
                    ReleaseConnection();
                }
            }
        }

        protected object ExecuteScalar(DbCommand command, DbConnection conn)
        {
            try
            {
                command.Connection = conn;
                return command.ExecuteScalar();
            }
            catch (Exception e)
            {
                throw this.ThrowExceptionHelper(conn, command, e);
            }
        }

        public T ExecuteScalar<T>(String query)
        {
            return ExecuteScalar<T>(query, (object[])null);
        }

        public T ExecuteScalar<T>(String query, params object[] parameters)
        {
            using (DbCommand comm = Provider.CreateCommand(query))
            {
                if (parameters != null)
                {
                    foreach (object val in parameters)
                    {
                        comm.Parameters.Add(Provider.CreateParameter(null, val));
                    }
                }
                return ExecuteScalar<T>(comm);
            }
        }

        public T ExecuteScalar<T>(String query, IEnumerable<KeyValuePair<String, object>> parameters)
        {
            using (DbCommand comm = Provider.CreateCommand(query))
            {
                if (parameters != null)
                {
                    foreach (var pair in parameters)
                    {
                        var param = Provider.CreateParameter(pair.Key, pair.Value);
                        comm.Parameters.Add(param);
                    }
                }
                return ExecuteScalar<T>(comm);
            }
        }

        protected T ExecuteScalar<T>(DbCommand command)
        {
            DbConnection conn = OpenConnection();
            try
            {
                return this.ExecuteScalar<T>(command, conn);
            }
            finally
            {
                ReleaseConnection();
            }
        }

        protected T ExecuteScalar<T>(DbCommand command, DbConnection conn)
        {
            object result = ExecuteScalar(command, conn);
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
                if(t.IsGenericType && t.GetGenericTypeDefinition() == typeof(Nullable<>))
                {
                    t = Nullable.GetUnderlyingType(t);
                }

                return (T)Convert.ChangeType(result, t
                    , System.Globalization.CultureInfo.CurrentCulture);
                //return (T)Convert.ChangeType(result, typeof(T));
            }
        }

        #endregion

        #region transaction management 
        public void BeginTransaction()
        {
            lock (TransactionSyncLock)
            {
                _transactionDepth++;
                if (_transactionDepth == 1)
                {
                    Debug.Assert(_CurrentTransaction == null);

                    DbConnection connection = OpenConnection();
                    _CurrentTransaction = connection.BeginTransaction();

                    _transactionCanceled = false;

                    this.EnterConnectionHold();
                    OnTransactionStarted();
                }
            }
        }

        [Obsolete("use CommitTransaction")]
        public void EndTransaction()
        {
            this.CommitTransaction();
        }

        public void CommitTransaction()
        {
            lock (TransactionSyncLock)
            {
                OnTransactionEnding();

                _transactionDepth--;
                if (_transactionDepth == 0)
                {
                    ReleaseTransaction();
                }


                //if (_CurrentTransaction == null)
                //{
                //    throw new InvalidOperationException("no active transaction");
                //}

                //_CurrentTransaction.Commit();
                //_CurrentTransaction.Dispose();
                //_CurrentTransaction = null;
                //ExitConnectionHold();
                //ReleaseConnection();
            }
        }

        [Obsolete("use RollbackTransaction instead")]
        public void CancelTransaction()
        {
            this.RollbackTransaction();
        }

        public void RollbackTransaction()
        {
            lock (TransactionSyncLock)
            {
                OnTransactionCanceling();
                _transactionCanceled = true;
                _transactionDepth--;
                if (_transactionDepth == 0)
                {
                    ReleaseTransaction();
                }


                //if (_CurrentTransaction == null)
                //{
                //    throw new InvalidOperationException("no active transaction");
                //}

                //_CurrentTransaction.Rollback();
                //_CurrentTransaction.Dispose();
                //_CurrentTransaction = null;
                //ExitConnectionHold();
                //ReleaseConnection();
            }
        }

        private void ReleaseTransaction()
        {
            OnTransactionReleasing();

            if (_transactionCanceled)
            {
                _CurrentTransaction.Rollback();
            }
            else
            {
                _CurrentTransaction.Commit();
            }

            _CurrentTransaction.Dispose();
            _CurrentTransaction = null;
            ExitConnectionHold();
            ReleaseConnection();
        }
        #endregion

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


        protected DbConnection CreateConnection()
        {
            DbConnection conn = Provider.CreateConnection();
            conn.ConnectionString = BuildConnectionString();
            conn.StateChange += _Connection_StateChange;
            return conn;
        }

        //protected DbConnection CreateReadWriteConnection()
        //{
        //    DbConnection conn = Provider.CreateConnection();
        //    conn.ConnectionString = BuildConnectionString(false);
        //    conn.StateChange += _Connection_StateChange;
        //    return conn;
        //}

        //protected DbConnection CreateReadOnlyConnection()
        //{
        //    DbConnection conn = Provider.CreateConnection();
        //    conn.ConnectionString = BuildConnectionString(true);
        //    conn.StateChange += _Connection_StateChange;
        //    return conn;
        //}

        /// <summary>
        /// if _holdConnection > 0 returns PersistentConnection
        /// if _holdConnection creates new connection and return it
        /// increments _holdConnection if connection successfully opened   
        /// </summary>
        /// <returns></returns>
        protected DbConnection OpenConnection()
        {
            lock(_persistentConnectionSyncLock)
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
                    }
                    OnConnectionOpened();
                }
                catch(Exception e)
                {
                    throw new ConnectionException("failed to open connection", e);
                }

                PersistentConnection = conn;
                EnterConnectionHold();

                return conn;
            }
        }

       

        //protected DbConnection OpenReadWriteConnection()
        //{
        //    return OpenReadWriteConnection(DEFAULT_RETRY_RW_CONNECTION_BEHAVIOR);
        //}

        //protected virtual DbConnection OpenReadWriteConnection(bool retry)
        //{
        //    lock (this._readWriteConnectionSyncLock)
        //    {
        //        DbConnection conn;



        //        if (_ReadWriteConnection == null)
        //        {
        //            _ReadWriteConnection = CreateReadWriteConnection();
        //        }
        //        conn = _ReadWriteConnection;

        //        if (conn.State != System.Data.ConnectionState.Open)
        //        {
        //            try
        //            {
        //                conn.Open();
        //                //EnterConnectionHold();
        //            }
        //            catch (Exception e)
        //            {
        //                if (!retry)
        //                {
        //                    var newEx = new ConnectionException(null, e);
        //                    newEx.AddConnectionInfo(conn);
        //                    throw newEx;
        //                }
        //                else
        //                {
        //                    conn.Dispose();
        //                    _ReadWriteConnection = null;
        //                    Thread.Sleep(100);
        //                    conn = OpenReadWriteConnection(false);
        //                }
        //            }
        //        }

        //        Debug.WriteLine("Read Write Connection Opened", Constants.Logging.DB_CONTROL_VERBOSE);
        //        return conn;
        //    }
        //}

        //protected DbConnection OpenReadOnlyConnection()
        //{
        //    return OpenReadOnlyConnection(DEFAULT_RETRY_RO_CONNECTION_BEHAVIOR);
        //}

        //protected virtual DbConnection OpenReadOnlyConnection(bool retry)
        //{
        //    lock (this._readOnlyConnectionSyncLock)
        //    {
        //        DbConnection conn;

        //        if (_ReadOnlyConnection == null)
        //        {
        //            _ReadOnlyConnection = CreateReadOnlyConnection();

        //        }
        //        else
        //        {
        //            Debug.WriteLine("Existing Connection used");
        //        }
        //        conn = _ReadOnlyConnection;

        //        if (conn.State != System.Data.ConnectionState.Open)
        //        {
        //            try
        //            {
        //                conn.Open();
        //            }
        //            catch
        //            {
        //                if (!retry)
        //                {
        //                    throw;
        //                }
        //                else
        //                {
        //                    conn.Dispose();
        //                    _ReadOnlyConnection = null;
        //                    Thread.Sleep(100);
        //                    conn = OpenReadOnlyConnection(false);
        //                }
        //            }
        //        }

        //        Debug.WriteLine("Read Only Connection Opened", Constants.Logging.DB_CONTROL_VERBOSE);
        //        return conn;
        //    }
        //}

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


        //TODO make protected 
        //public virtual void ReleaseAllConnections(bool force)
        //{
        //    ReleaseReadOnlyConnection();
        //    ReleaseReadWriteConnection(force);
        //}

        protected void ReleaseConnection()
        {
            lock(_persistentConnectionSyncLock)
            {
                if(_holdConnection > 0)
                {
                    ExitConnectionHold();
                    if(_holdConnection == 0)
                    {
                        Debug.Assert(PersistentConnection != null);
                        PersistentConnection.Dispose();
                        PersistentConnection = null;
                    }
                }
            }

        }

        //protected void ReleaseReadOnlyConnection()
        //{
        //    lock (_readOnlyConnectionSyncLock)
        //    {
        //        Debug.Assert(_ReadOnlyConnection != null);
        //        if (_ReadOnlyConnection == null) { return; }
        //        ReleaseConnection(_ReadOnlyConnection);
        //        _ReadOnlyConnection = null;
        //        Debug.WriteLine("Read Only Connection Released", Constants.Logging.DB_CONTROL_VERBOSE);
        //    }
        //}

        //protected void ReleaseReadWriteConnection(bool force)
        //{
        //    lock (_readWriteConnectionSyncLock)
        //    {
        //        Debug.Assert(_ReadWriteConnection != null);
        //        if (_ReadWriteConnection == null) { return; }
        //        //ExitConnectionHold();
        //        if (_holdConnection == 0 || force)
        //        {
        //            ReleaseConnection(_ReadWriteConnection);
        //            _ReadWriteConnection = null;
        //            Debug.WriteLine("Read-Write Connection Released", Constants.Logging.DB_CONTROL_VERBOSE);
        //        }
        //        else
        //        {
        //            Debug.WriteLine("Read-Write Connection Survived", Constants.Logging.DB_CONTROL_VERBOSE);
        //        }
        //    }
        //}

        #endregion

        #region events and logging

        protected virtual void OnDeletingData(object data)
        {

        }

        protected virtual void OnInsertingData(object data, SQL.OnConflictOption option)
        {

        }

        protected virtual void OnUpdatingData(object data)
        {

        }

        /// <summary>
        /// called when connection is in use 
        /// </summary>
        protected virtual void OnConnectionOpened()
        {
            Debug.WriteLine("Connection opened", Constants.Logging.DB_CONTROL);
        }

        //for logging connection state changes
        void _Connection_StateChange(object sender, System.Data.StateChangeEventArgs e)
        {
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
        #endregion

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

                Debug.Assert(_holdConnection == 0);
                _holdConnection = 0;

                ReleaseConnection();

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
        #endregion





    }
}
