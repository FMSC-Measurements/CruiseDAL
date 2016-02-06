using FMSC.ORM.Core;
using FMSC.ORM.Core.SQL;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FMSC.ORM.SQLite
{
    public class SQLiteMultiDBDataStore : SQLiteDatastore
    {
        protected class ExternalDatastore
        {
            public DatastoreRedux DS;
            public string Alias;
        }

        int _multiDBtransactionHold = 0;
        protected int _multiDBholdConnection = 0;
        protected int _multiDBtransactionDepth = 0;
        protected bool _multiDBtransactionCanceled = false;

        protected Object _multiDBpersistentConnectionSyncLock = new object();
        protected DbConnection MultiDBPersistentConnection { get; set; }

        public object MultiDBTransactionSyncLock = new object();
        protected DbTransaction _multiDBCurrentTransaction;

        protected ICollection<ExternalDatastore> _attachedDataStores;

        public IEnumerable<DatastoreRedux> AttachedDataStores { get; set; }

        #region multiDB execute commands
        public int ExecuteMultiDB(String command, params object[] parameters)
        {
            using (DbCommand com = Provider.CreateCommand(command))
            {
                return this.Execute(com, parameters);
            }
        }

        public int ExecuteMultiDB(String command, IEnumerable<KeyValuePair<String, object>> parameters)
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

        protected int ExecuteMultiDB(DbCommand command, params object[] parameters)
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

        protected int ExecuteSQLMultiDB(DbCommand command)
        {
            lock (_persistentConnectionSyncLock)
            {
                DbConnection conn = OpenMultiDBConnection();
                try
                {
                    return ExecuteSQL(command, conn);
                }
                finally
                {
                    ReleaseMultiDBConnection();
                }
            }
        }

        protected int ExecuteSQLMultiDB(DbCommand command, DbConnection conn)
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

        public object ExecuteScalarMultiDB(string query, params object[] parameters)
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
                object value = ExecuteScalarMultiDB(comm);
                return (value is DBNull) ? null : value;
            }
        }

        protected object ExecuteScalarMultiDB(DbCommand command)
        {
            lock (_multiDBpersistentConnectionSyncLock)
            {
                DbConnection conn = OpenMultiDBConnection();
                try
                {
                    return ExecuteScalarMultiDB(command, conn);
                }
                finally
                {
                    ReleaseMultiDBConnection();
                }
            }
        }

        protected object ExecuteScalarMultiDB(DbCommand command, DbConnection conn)
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

        public T ExecuteScalarMultiDB<T>(String query)
        {
            return ExecuteScalar<T>(query, (object[])null);
        }

        public T ExecuteScalarMultiDB<T>(String query, params object[] parameters)
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
                return ExecuteScalarMultiDB<T>(comm);
            }
        }

        public T ExecuteScalarMultiDB<T>(String query, IEnumerable<KeyValuePair<String, object>> parameters)
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
                return ExecuteScalarMultiDB<T>(comm);
            }
        }

        protected T ExecuteScalarMultiDB<T>(DbCommand command)
        {
            DbConnection conn = OpenMultiDBConnection();
            try
            {
                return this.ExecuteScalarMultiDB<T>(command, conn);
            }
            finally
            {
                ReleaseMultiDBConnection();
            }
        }

        protected T ExecuteScalarMultiDB<T>(DbCommand command, DbConnection conn)
        {
            object result = ExecuteScalarMultiDB(command, conn);
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
        #endregion

        public void AttachDB(DatastoreRedux dataStore, string alias)
        {
            if (dataStore == null) { throw new ArgumentNullException("dataStore"); }
            if (String.IsNullOrEmpty(alias)) { throw new ArgumentException("alias can't be null or empty", "alias"); }
            Debug.Assert(_attachedDataStores != null);

            var externalDS = new ExternalDatastore()
            {
                DS = dataStore,
                Alias = alias
            };

            _attachedDataStores.Add(externalDS);
            AttachDBInternal(externalDS);
        }

        protected void AttachDBInternal(ExternalDatastore externalDB)
        {
            if (this.MultiDBPersistentConnection != null)
            {
                this.ExecuteMultiDB("ATTACH DATABASE \"" + externalDB.DS.Path
                    + "\" AS " + externalDB.Alias + ";");
            }
        }

        public void DetachDB(string alias)
        {
            if (String.IsNullOrEmpty(alias)) { throw new ArgumentException("alias can't be null or empty", "alias"); }
            Debug.Assert(_attachedDataStores != null);

            ExternalDatastore exDS = null;
            foreach (var ds in _attachedDataStores)
            {
                if (ds.Alias == alias)
                {
                    exDS = ds;
                    break;
                }
            }

            if (exDS != null)
            {
                _attachedDataStores.Remove(exDS);
                DetachDBInternal(exDS);
            }
        }

        protected void DetachDBInternal(ExternalDatastore externalDB)
        {
            if (this.MultiDBPersistentConnection != null)
            {
                this.ExecuteMultiDB("DETACH DATABASE \""
                    + externalDB.Alias + "\";");
            }
        }

        protected DbConnection OpenMultiDBConnection()
        {
            lock (_multiDBpersistentConnectionSyncLock)
            {
                DbConnection conn;
                if (_multiDBholdConnection == 0)
                {
                    conn = CreateConnection();
                }
                else
                {
                    Debug.Assert(MultiDBPersistentConnection != null);
                    conn = MultiDBPersistentConnection;
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
                    OnMultiDBConnectionOpened();
                }
                catch (Exception e)
                {
                    throw new ConnectionException("failed to open connection", e);
                }

                MultiDBPersistentConnection = conn;
                EnterMultiDBConnectionHold();

                return conn;
            }
        }

        protected void ReleaseMultiDBConnection()
        {
            lock (_multiDBpersistentConnectionSyncLock)
            {
                if (_multiDBholdConnection > 0)
                {
                    ExitConnectionHold();
                    if (_multiDBholdConnection == 0)
                    {
                        Debug.Assert(MultiDBPersistentConnection != null);
                        MultiDBPersistentConnection.Dispose();
                        MultiDBPersistentConnection = null;
                    }
                }
            }
        }

        protected void EnterMultiDBConnectionHold()
        {
            System.Threading.Interlocked.Increment(ref this._multiDBholdConnection);
        }

        protected void ExitMultiDBConnectionHold()
        {
            Debug.Assert(_holdConnection > 0);
            System.Threading.Interlocked.Decrement(ref this._multiDBholdConnection);
        }

        private void OnMultiDBConnectionOpened()
        {
            throw new NotImplementedException();
        }
    }
}
