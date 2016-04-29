using FMSC.ORM;
using FMSC.ORM.Core;
using FMSC.ORM.SQLite;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Diagnostics;
using System.IO;
using System.Threading;

#pragma warning disable RECS0122 // Initializing field with default value is redundant
#pragma warning disable RECS0104 // When object creation uses object or collection initializer, empty argument list is redundant

namespace CruiseDAL
{
    public enum CruiseFileType { Unknown, Cruise, Template, Design, Master, Component, Backup }

    public partial class DAL : SQLiteDatastore
    {
        protected class ExternalDatastore
        {
            public DatastoreRedux DS;
            public string Alias;
        }

        #region multiDB Fields

        int _multiDBtransactionHold = 0;

        protected int _multiDBholdConnection = 0;
        protected int _multiDBtransactionDepth = 0;
        protected bool _multiDBtransactionCanceled = false;
        private object _multiDBTransactionSyncLock = new object();

        protected Object _multiDBpersistentConnectionSyncLock = new object();

        protected DbTransaction _multiDBCurrentTransaction;

        protected ICollection<ExternalDatastore> _attachedDataStores = new List<ExternalDatastore>();
        protected DbConnection MultiDBPersistentConnection { get; set; }

        public object MultiDBTransactionSyncLock { get { return _multiDBTransactionSyncLock; } }
        public IEnumerable<DatastoreRedux> AttachedDataStores { get; set; }

        #endregion multiDB Fields

        private string _userInfo;
        private string _databaseVersion = "Unknown";
        private CruiseFileType _cruiseFileType;

        /// <summary>
        /// represents value returned by PRAGMA user_version;
        /// </summary>
        internal long SchemaVersion { get; set; }

        /// <summary>
        /// Get the database version
        /// </summary>
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

        /// <summary>
        /// Gets the string used to identify the user, for the purpose of CreatedBy and ModifiedBy values
        /// </summary>
        [Obsolete]
        public string User
        {
            get
            {
                if (_userInfo == null)
                {
                    _userInfo = GetUserInformation();
                }
                return _userInfo;
            }
        }

        public CruiseFileType CruiseFileType
        {
            get
            {
                _cruiseFileType = this.ReadCruiseFileType();
                if (_cruiseFileType == CruiseFileType.Unknown)
                {
                    _cruiseFileType = ExtrapolateCruiseFileType(this.Path);
                    if (_cruiseFileType == CruiseFileType.Unknown)
                    {
                        WriteCruiseFileType(_cruiseFileType);
                    }
                }
                return _cruiseFileType;
            }
            protected set
            {
                _cruiseFileType = CruiseFileType.Unknown;
                WriteCruiseFileType(value);
            }
        }

        /// <summary>
        /// Creates a DAL instance for a database @ path.
        /// </summary>
        /// <exception cref="ArgumentNullException">path can not be null or an empty string</exception>
        /// <exception cref="IOException">problem working with file. wrong extension</exception>
        /// <exception cref="FileNotFoundException"
        /// <param name="path"></param>
        public DAL(string path) : this(path, false)
        {
        }

        public DAL(string path, bool makeNew)
            : this(path, makeNew, new CruiseDALDatastoreBuilder())
        {
        }

        /// <summary>
        /// Creates a DAL instance for a database @ path.
        /// </summary>
        /// <exception cref="ArgumentNullException">path can not be null or an empty string</exception>
        /// <exception cref="System.IO.IOException">File extension is not valid <see cref="VALID_EXTENSIONS"/></exception>
        /// <exception cref="System.UnauthorizedAccessException">File open in another application or thread</exception>
        public DAL(string path, bool makeNew, DatabaseBuilder builder) : base(path)
        {
            Debug.Assert(builder != null);
            System.Diagnostics.Debug.Assert(!String.IsNullOrEmpty(path), "path is null or empty");

            builder.Datastore = this;

            Path = path;

            this.Initialize(makeNew, builder);
            Logger.Log.V(String.Format("Created DAL instance. Path = {0}\r\n", Path));
        }

        protected void Initialize(bool makeNew, DatabaseBuilder builder)
        {
            if (makeNew)
            {
                builder.CreateDatastore();
            }
            else if (!makeNew && !Exists)
            {
                throw new FileNotFoundException();
            }

            String dbVersion = this.ReadGlobalValue("Database", "Version");
            if (dbVersion != null)
            {
                DatabaseVersion = dbVersion;
            }

            this.SchemaVersion = this.ExecuteScalar<long>("PRAGMA user_version;");
            builder.UpdateDatastore();

            try
            {
                this.LogMessage("File Opened", "normal");
            }
            catch (FMSC.ORM.ReadOnlyException)
            {/*ignore, in case we want to allow access to a read-only DB*/}
        }

        protected static string GetUserInformation()
        {
#if NetCF

            return FMSC.Util.DeviceInfo.GetMachineDescription() + "|" + FMSC.Util.DeviceInfo.GetMachineName();
            //FMSC.Utility.MobileDeviceInfo di = new FMSC.Utility.MobileDeviceInfo();
            //return di.GetModelAndSerialNumber();
            //return "Mobile User";

#elif ANDROID
			return "AndroidUser";
#else
            return Environment.UserName + " on " + System.Environment.MachineName;
#endif
            //return Environment.UserName + " on " + System.Windows.Forms.SystemInformation.ComputerName;
        }

        #region multiDB methods

        #region multiDB execute commands

        public int ExecuteMultiDB(String command, params object[] parameters)
        {
            using (DbCommand com = Provider.CreateCommand(command))
            {
                return this.ExecuteMultiDB(com, parameters);
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
                return ExecuteSQLMultiDB(comm);
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
            return ExecuteSQLMultiDB(command);
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

        //protected int ExecuteSQLMultiDB(DbCommand command, DbConnection conn)
        //{
        //    try
        //    {
        //        command.Connection = conn;
        //        return command.ExecuteNonQuery();
        //    }
        //    catch (Exception e)
        //    {
        //        throw this.ThrowExceptionHelper(conn, command, e);
        //    }
        //}

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

        #endregion multiDB execute commands

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
            AttachDBInternal(externalDS, this.MultiDBPersistentConnection);
        }

        protected void AttachDBInternal(ExternalDatastore externalDB, DbConnection conn)
        {
            if (conn != null)
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
                        OnMultiDBConnectionOpened();
                        MultiDBPersistentConnection = conn;
                        InitializeMultiDBConnection(conn);
                    }

                    EnterMultiDBConnectionHold();

                    return conn;
                }
                catch (Exception e)
                {
                    throw new ConnectionException("failed to open connection", e);
                }
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

        protected void InitializeMultiDBConnection(DbConnection conn)
        {
            foreach (var ds in _attachedDataStores)
            {
                AttachDBInternal(ds, conn);
            }
        }

        private void OnMultiDBConnectionOpened()
        {
            Debug.WriteLine("MultiDB Connection Opened", FMSC.ORM.Core.Constants.Logging.DB_CONTROL);
        }

        public void BeginMultiDBTransaction()
        {
            lock (MultiDBTransactionSyncLock)
            {
                _multiDBtransactionDepth++;
                if (_multiDBtransactionDepth == 1)
                {
                    Debug.Assert(_multiDBCurrentTransaction == null);

                    DbConnection connection = OpenMultiDBConnection();
                    _multiDBCurrentTransaction = connection.BeginTransaction();

                    _multiDBtransactionCanceled = false;

                    this.EnterMultiDBConnectionHold();
                    OnMultiDBTransactionStarted();
                }
            }
        }

        public void CommitMultiDBTransaction()
        {
            lock (MultiDBTransactionSyncLock)
            {
                OnMultiDBTransactionEnding();

                _multiDBtransactionDepth--;
                if (_multiDBtransactionDepth == 0)
                {
                    ReleaseMultiDBTransaction();
                }
            }
        }

        public void RollbackMultiDBTransaction()
        {
            lock (MultiDBTransactionSyncLock)
            {
                OnMultiDBTransactionCanceling();
                _multiDBtransactionCanceled = true;
                _multiDBtransactionDepth--;
                if (_multiDBtransactionDepth == 0)
                {
                    ReleaseMultiDBTransaction();
                }
            }
        }

        private void ReleaseMultiDBTransaction()
        {
            OnMultiDBTransactionReleasing();

            if (_multiDBtransactionCanceled)
            {
                _multiDBCurrentTransaction.Rollback();
            }
            else
            {
                _multiDBCurrentTransaction.Commit();
            }

            _multiDBCurrentTransaction.Dispose();
            _multiDBCurrentTransaction = null;
            ExitMultiDBConnectionHold();
            ReleaseMultiDBConnection();
        }

        protected virtual void OnMultiDBTransactionStarted()
        {
            Debug.WriteLine("MultiDB Transaction Started", FMSC.ORM.Core.Constants.Logging.DB_CONTROL);
        }

        protected virtual void OnMultiDBTransactionEnding()
        {
            Debug.WriteLine("MultiDB Transaction Ending", FMSC.ORM.Core.Constants.Logging.DB_CONTROL);
        }

        protected virtual void OnMultiDBTransactionCanceling()
        {
            Debug.WriteLine("MultiDB Transaction Canceling", FMSC.ORM.Core.Constants.Logging.DB_CONTROL);
        }

        protected virtual void OnMultiDBTransactionReleasing()
        {
            Debug.WriteLine("MultiDB Transaction Releasing", FMSC.ORM.Core.Constants.Logging.DB_CONTROL);
        }

        #endregion multiDB methods

        #region cruise/cut specific stuff

        public static CruiseFileType ExtrapolateCruiseFileType(String path)
        {
            String normPath = path.ToLower().TrimEnd();
            if (String.IsNullOrEmpty(normPath))
            {
                return CruiseFileType.Unknown;
            }
            else if (System.Text.RegularExpressions.Regex.IsMatch(normPath, @".+\.m\.cruise\s*$", System.Text.RegularExpressions.RegexOptions.IgnoreCase))
            {
                return CruiseFileType.Master;
            }
            else if (System.Text.RegularExpressions.Regex.IsMatch(normPath, @".+\.\d+\.cruise\s*$", System.Text.RegularExpressions.RegexOptions.IgnoreCase))
            {
                return CruiseFileType.Component;
            }
            else if (normPath.EndsWith(".cruise", StringComparison.OrdinalIgnoreCase))
            {
                return CruiseFileType.Cruise;
            }
            else if (normPath.EndsWith(".cut", StringComparison.OrdinalIgnoreCase))
            {
                return CruiseFileType.Template;
            }
            else if (normPath.EndsWith(".design", StringComparison.OrdinalIgnoreCase))
            {
                return CruiseFileType.Design;
            }
            else if (normPath.EndsWith(".back-cruise", StringComparison.OrdinalIgnoreCase))
            {
                return CruiseFileType.Backup;
            }

            return CruiseFileType.Unknown;
        }

        protected CruiseFileType ReadCruiseFileType()
        {
            String s = this.ReadGlobalValue("Database", "CruiseFileType");
            try
            {
                return (CruiseFileType)Enum.Parse(typeof(CruiseFileType), s, true);
            }
            catch
            {
                return CruiseFileType.Unknown;
            }
        }

        protected void WriteCruiseFileType(CruiseFileType cType)
        {
            this.WriteGlobalValue("Database", "CruiseFileType", cType.ToString());
        }

        public string ReadGlobalValue(String block, String key)
        {
            return this.ExecuteScalar("SELECT Value FROM GLOBALS WHERE " +
            "ifnull(Block, '') = ifnull(?, '') " +
            "AND ifnull(Key, '') = ifnull(?, '');", block, key) as string;
        }

        public void WriteGlobalValue(String block, String key, String value)
        {
            this.Execute("INSERT OR REPLACE INTO Globals (Block, Key, Value) " +
                "Values (?, ?, ?);", block, key, value);
        }

        #endregion cruise/cut specific stuff

        #region file utility

        public void CopyTo(string path)
        {
            this.CopyTo(path, false);
        }

        public void CopyTo(string destPath, bool overwrite)
        {
            ReleaseConnection(true);

            System.IO.File.Copy(this.Path, destPath, overwrite);
        }

        public void CopyAs(string desPath, bool overwrite)
        {
            CopyTo(desPath, overwrite);
            this.Path = desPath;
        }

        #endregion file util

        ///// <summary>
        ///// Copies selection directly from external Database
        ///// </summary>
        ///// <param name="fileName">Path to external file</param>
        ///// <param name="table"></param>
        ///// <param name="selection"></param>
        ///// <param name="selectionArgs"></param>
        //public void DirectCopy(String fileName, String table, String selection, params Object[] selectionArgs)
        //{
        //    DirectCopy(new DAL(fileName), table, selection, OnConflictOption.Abort, selectionArgs);
        //}

        ///// <summary>
        ///// Copies selection directly from external Database
        ///// </summary>
        ///// <param name="fileName">Path to external file</param>
        ///// <param name="table"></param>
        ///// <param name="selection"></param>
        ///// <param name="selectionArgs"></param>
        //public void DirectCopy(String fileName, String table, String selection, OnConflictOption option, params Object[] selectionArgs)
        //{
        //    DirectCopy(new DAL(fileName), table, selection, option, selectionArgs);
        //}

        ///// <summary>
        ///// Copies selection directly FROM external database
        ///// </summary>
        ///// <param name="dataBase">external database</param>
        ///// <param name="table"></param>
        ///// <param name="selection"></param>
        ///// <param name="selectionArgs"></param>
        //public void DirectCopy(DAL dataBase, string table, String selection, OnConflictOption option, params Object[] selectionArgs)
        //{
        //    if (dataBase.Exists == false) { return; }

        //    string cOpt = option.ToString().ToUpper();
        //    string copy = String.Format("INSERT OR {2} INTO {0} SELECT * FROM destDB.{0} {1};", table, selection, cOpt);

        //    this.AttachDB(dataBase, "destDB");
        //    try
        //    {
        //        this.Execute(copy);
        //    }
        //    finally
        //    {
        //        this.DetachDB("destDB");
        //    }

        //}

        #region not implemented

        //[Obsolete]
        //public void ChangeRowID(DataObject data, long newRowID, OnConflictOption option)
        //{
        //    throw new NotImplementedException();
        //}

        //[Obsolete]
        //public void Save(IEnumerable list)
        //{
        //    this.Save(list, FMSC.ORM.Core.SQL.OnConflictOption.Default);
        //}

        //[Obsolete]
        //public void Save(IEnumerable list, FMSC.ORM.Core.SQL.OnConflictOption opt)
        //{
        //    throw new NotImplementedException();
        //}

        #endregion not implemented

        #region accessControl

        private Mutex _accessControl;

        private void releaseAccessControl()
        {
#if !Mobile
            if (_accessControl != null)
            {
                try
                {
                    _accessControl.ReleaseMutex();
                }
                catch
                {
                }
                _accessControl.Close();
                _accessControl = null;
            }
            //FileSecurity fSecurity = file.GetAccessControl();
            //fSecurity.PurgeAccessRules(new NTAccount(@"FMSC\CruiseDAL"));
            //file.SetAccessControl(fSecurity);
#endif
        }

        private void establishAccessControl()
        {
            //releaseAccessControl();
#if !NetCF
            try
            {
                string semaName = "CruiseDAL" + Path.GetHashCode().ToString();
                _accessControl = new Mutex(false, semaName);
            }
            catch
            {
            }
            if (_accessControl == null)
            {
                throw new DatabaseShareException("File Open Somewhere Else");
            }
            else
            {
                if (!_accessControl.WaitOne(0, true))
                {
                    this.releaseAccessControl();
                    throw new DatabaseShareException("File Open Somewhere Else");
                }
            }
#endif
        }

        #endregion accessControl

        #region IDisposable Members

        private bool _disposed = false;

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            if (_disposed)
            {
                return;
            }

            if (disposing)
            {
            }

            //ReleaseMultiDatabaseConnection(true);
            releaseAccessControl();

            _disposed = true;
        }

        ~DAL()
        {
            this.Dispose(false);
        }

        #endregion IDisposable Members
    }
}