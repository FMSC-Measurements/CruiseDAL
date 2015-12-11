
using FMSC.ORM;
using FMSC.ORM.Core;
using FMSC.ORM.Core.EntityModel;
using FMSC.ORM.Core.SQL;
using FMSC.ORM.SQLite;
using System;
using System.Collections;
using System.Data.Common;
using System.Data.SQLite;
using System.Diagnostics;
using System.IO;
using System.Threading;

namespace CruiseDAL
{
    public enum CruiseFileType { Unknown, Cruise, Template, Design, Master, Component }

    public partial class DAL : SQLiteDatastore
    {
        private string _userInfo;
        private string _databaseVersion = "Unknown";
        private CruiseFileType _cruiseFileType;

        protected object _multiDatabaseConnectionSyncLock = new Object();
        protected DbConnection _multiDatabaseConnection;
        protected int _holdMultiDatabaseConnection;

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
                this.LogMessage("File Opened" , "normal");
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

        #region cruise/cut specific stuff
        public static CruiseFileType ExtrapolateCruiseFileType(String path)
        {
            String normPath = path.ToLower().TrimEnd();
            if (String.IsNullOrEmpty(normPath))
            {
                return CruiseFileType.Unknown;
            }

            else if (System.Text.RegularExpressions.Regex.IsMatch(normPath, @".+\.m\.cruise\s*$"))
            {
                return CruiseFileType.Master;
            }
            else if (System.Text.RegularExpressions.Regex.IsMatch(normPath, @".+\.\d+\.cruise\s*$"))
            {
                return CruiseFileType.Component;
            }
            else if (normPath.EndsWith(".cruise"))
            {
                return CruiseFileType.Cruise;
            }
            else if (normPath.EndsWith(".cut"))
            {
                return CruiseFileType.Template;
            }
            else if (normPath.EndsWith(".design"))
            {
                return CruiseFileType.Design;
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
        #endregion

        #region file util
        public DAL CopyTo(string path)
        {
            return this.CopyTo(path, false);
        }

        public DAL CopyTo(string destPath, bool overwrite)
        {

            ReleaseAllConnections(true);

            System.IO.File.Copy(this.Path, destPath, overwrite);
            //_DBFileInfo.CopyTo(destPath, overwrite);
            return new DAL(destPath);
        }
        #endregion


        

        /// <summary>
        /// Copies selection directly from external Database
        /// </summary>
        /// <param name="fileName">Path to external file</param>
        /// <param name="table"></param>
        /// <param name="selection"></param>
        /// <param name="selectionArgs"></param>
        public void DirectCopy(String fileName, String table, String selection, params Object[] selectionArgs)
        {
            DirectCopy(new DAL(fileName), table, selection, OnConflictOption.Abort, selectionArgs);
        }

        /// <summary>
        /// Copies selection directly from external Database
        /// </summary>
        /// <param name="fileName">Path to external file</param>
        /// <param name="table"></param>
        /// <param name="selection"></param>
        /// <param name="selectionArgs"></param>
        public void DirectCopy(String fileName, String table, String selection, OnConflictOption option, params Object[] selectionArgs)
        {
            DirectCopy(new DAL(fileName), table, selection, option, selectionArgs);
        }

        /// <summary>
        /// Copies selection directly FROM external database
        /// </summary>
        /// <param name="dataBase">external database</param>
        /// <param name="table"></param>
        /// <param name="selection"></param>
        /// <param name="selectionArgs"></param>
        public void DirectCopy(DAL dataBase, string table, String selection, OnConflictOption option, params Object[] selectionArgs)
        {
            if (dataBase.Exists == false) { return; }



            string cOpt = option.ToString().ToUpper();
            string copy = String.Format("INSERT OR {2} INTO {0} SELECT * FROM destDB.{0} {1};", table, selection, cOpt);

            this.AttachDB(dataBase, "destDB");
            try
            {
                this.Execute(copy);
            }
            finally
            {
                this.DetachDB("destDB");
            }

        }

        public void AttachDB(DAL externalDB, string externalDBAlias)
        {
            lock (_multiDatabaseConnectionSyncLock)
            {
                try
                {
                    OpenMultiDatabaseConnection(true);
                    this.Execute("ATTACH DATABASE ? AS ?;", externalDB.Path, externalDBAlias);
                }
                catch
                {
                    ReleaseMultiDatabaseConnection(true);
                    throw;
                }
            }
        }

        public void DetachDB(string externalDBAlias)
        {
            try
            {
                string detach = string.Format("DETACH DATABASE {0};", externalDBAlias);
                this.Execute(detach);
            }
            finally
            {
                this.ReleaseMultiDatabaseConnection(false);
            }
        }

        protected void EnterMultiDatabaseConnectionHold()
        {
            System.Threading.Interlocked.Increment(ref this._holdMultiDatabaseConnection);
        }

        protected void ExitMultiDatabaseConnectionHold()
        {
            if (this._holdMultiDatabaseConnection > 0)
            {
                System.Threading.Interlocked.Decrement(ref this._holdMultiDatabaseConnection);
            }
        }

        protected DbConnection OpenMultiDatabaseConnection(bool retry)
        {
            lock(_multiDatabaseConnectionSyncLock)
            {
                DbConnection conn;
                if (_multiDatabaseConnection == null)
                {
                    _multiDatabaseConnection = CreateReadWriteConnection();
                }
                conn = _multiDatabaseConnection;

                try
                {
                    if (conn.State != System.Data.ConnectionState.Open)
                    {
                        conn.Open();
                    }
                    EnterMultiDatabaseConnectionHold();
                    return conn;
                }
                catch (Exception e)
                {
                    if (!retry)
                    {
                        var newEx = new ConnectionException(null, e);
                        newEx.AddConnectionInfo(conn);
                        throw newEx;
                    }
                    else
                    {
                        conn.Dispose();
                        _multiDatabaseConnection = null;
                        Thread.Sleep(100);
                        return OpenMultiDatabaseConnection(false);
                    }
                }
            }
        }

        protected void ReleaseMultiDatabaseConnection(bool force)
        {
            lock(_multiDatabaseConnectionSyncLock)
            {
                Debug.Assert(_multiDatabaseConnection != null);
                ExitMultiDatabaseConnectionHold();
                if(_multiDatabaseConnection == null) { return; }
                if (_holdMultiDatabaseConnection == 0 || force)
                {
                    ReleaseConnection(_multiDatabaseConnection);
                    _multiDatabaseConnection = null;
                    Debug.WriteLine("Multi Database Connection Released", FMSC.ORM.Core.Constants.Logging.DB_CONTROL_VERBOSE);
                }
                else
                {
                    Debug.WriteLine("Multi Database Connection Survived", FMSC.ORM.Core.Constants.Logging.DB_CONTROL_VERBOSE);
                }
            }
        }

        public override void ReleaseAllConnections(bool force)
        {
            base.ReleaseAllConnections(force);
            ReleaseMultiDatabaseConnection(force);
        }

        #region not implemented 
        public void ChangeRowID(DataObject data, long newRowID, OnConflictOption option)
        {
            throw new NotImplementedException();
        }

        public void Save(IEnumerable list)
        {
            this.Save(list, FMSC.ORM.Core.SQL.OnConflictOption.Fail);
        }

        public void Save(IEnumerable list, FMSC.ORM.Core.SQL.OnConflictOption opt)
        {
            throw new NotImplementedException();
        }
        #endregion

        #region accessControl
        Mutex _accessControl;
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
        #endregion

        #region IDisposable Members
        private bool _disposed = false;

        protected override void Dispose(bool isDisposing)
        {
            base.Dispose(isDisposing);
            if (_disposed)
            {
                return;
            }

            if(isDisposing)
            {

            }

            releaseAccessControl();

            _disposed = true;
        }

        ~DAL()
        {
            this.Dispose(false);
        }

        #endregion
    }
}
