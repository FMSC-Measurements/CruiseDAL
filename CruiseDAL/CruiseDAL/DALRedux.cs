
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

    public partial class DALRedux : SQLiteDatastore
    {

        /// <summary>
        /// represents value returned by PRAGMA user_version;  
        /// </summary>
        internal long SchemaVersion { get; set; }

        /// <summary>
        /// Get the database version
        /// </summary>
        private string _databaseVersion = "Unknown";
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

        private string _userInfo;
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

        private CruiseFileType _cruiseFileType;
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
        public DALRedux(string path) : this(path, false)
        {
        }

        public DALRedux(string path, bool makeNew)
            : this(path, makeNew, new CruiseDALDatastoreBuilder())
        {

        }

        /// <summary>
        /// Creates a DAL instance for a database @ path. 
        /// </summary>
        /// <exception cref="ArgumentNullException">path can not be null or an empty string</exception>
        /// <exception cref="System.IO.IOException">File extension is not valid <see cref="VALID_EXTENSIONS"/></exception>
        /// <exception cref="System.UnauthorizedAccessException">File open in another application or thread</exception>
        public DALRedux(string path, bool makeNew, DatastoreBuilder builder)
        {
            Debug.Assert(builder != null);
            System.Diagnostics.Debug.Assert(!String.IsNullOrEmpty(path), "path is null or empty");

            builder.DataStore = this;

            Path = path;
            
            this.Initialize(makeNew, builder);
            Logger.Log.V(String.Format("Created DAL instance. Path = {0}\r\n", Path));
        }

        ~DALRedux()
        {
            this.Dispose(false);
        }

        
        protected void Initialize(bool makeNew, DatastoreBuilder builder)
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



        protected static string GetUserInformation()
        {
#if Mobile

            return FMSC.Util.DeviceInfo.GetMachineDescription() + "|" + FMSC.Util.DeviceInfo.GetMachineName();
            //FMSC.Utility.MobileDeviceInfo di = new FMSC.Utility.MobileDeviceInfo();
            //return di.GetModelAndSerialNumber();
            //return "Mobile User";
#elif FullFramework
            return Environment.UserName + " on " + System.Environment.MachineName;
#elif ANDROID
			return "AndroidUser";
#endif
            //return Environment.UserName + " on " + System.Windows.Forms.SystemInformation.ComputerName;
        }

        public DALRedux CopyTo(string path)
        {
            return this.CopyTo(path, false);
        }

        public DALRedux CopyTo(string destPath, bool overwrite)
        {

            Context.ReleaseAllConnections(true);

            System.IO.File.Copy(this.Path, destPath, overwrite);
            //_DBFileInfo.CopyTo(destPath, overwrite);
            return new DALRedux(destPath);
        }

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

        /// <summary>
        /// Copies selection directly from external Database
        /// </summary>
        /// <param name="fileName">Path to external file</param>
        /// <param name="table"></param>
        /// <param name="selection"></param>
        /// <param name="selectionArgs"></param>
        public void DirectCopy(String fileName, String table, String selection, params Object[] selectionArgs)
        {
            DirectCopy(new DALRedux(fileName), table, selection, OnConflictOption.Abort, selectionArgs);
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
            DirectCopy(new DALRedux(fileName), table, selection, option, selectionArgs);
        }

        /// <summary>
        /// Copies selection directly FROM external database
        /// </summary>
        /// <param name="dataBase">external database</param>
        /// <param name="table"></param>
        /// <param name="selection"></param>
        /// <param name="selectionArgs"></param>
        public void DirectCopy(DALRedux dataBase, string table, String selection, OnConflictOption option, params Object[] selectionArgs)
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

        public void AttachDB(DALRedux externalDB, string externalDBAlias)
        {
            lock (this._connectionSyncLock)
            {
                try
                {
                    OpenConnection();
                    this.Execute("ATTACH DATABASE ? AS ?;", externalDB.Path, externalDBAlias);
                }
                catch
                {
                    ReleaseConnection();
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
                this.ReleaseConnection();
            }
        }

        


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
        }
        #endregion

        #region IDisposable Members
        private bool _disposed = false;

        protected void Dispose(bool isDisposing)
        {
            //base.Dispose(isDisposing);
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

        #endregion
    }
}
