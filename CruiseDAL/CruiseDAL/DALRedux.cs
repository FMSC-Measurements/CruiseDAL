
using CruiseDAL.Core.SQL;
using CruiseDAL.SQLite;
using System;
using System.Data.Common;
using System.Data.SQLite;
using System.IO;
using System.Threading;

namespace CruiseDAL
{
    public class DALRedux : SQLiteDatastore
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

        /// <summary>
        /// Creates a DAL instance for a database @ path. 
        /// </summary>
        /// <exception cref="ArgumentNullException">path can not be null or an empty string</exception>
        /// <exception cref="System.IO.IOException">File extension is not valid <see cref="VALID_EXTENSIONS"/></exception>
        /// <exception cref="System.UnauthorizedAccessException">File open in another application or thread</exception>
        public DALRedux(string path, bool makeNew)
        {
            System.Diagnostics.Debug.Assert(!String.IsNullOrEmpty(path), "path is null or empty, is this intentional?");

            Path = path;
            if (makeNew)
            {
                BuildDBFile();
            }
            else if (!makeNew && !Exists)
            {
                throw new FileNotFoundException();
            }


            this.Initialize();
            Logger.Log.V(String.Format("Created DAL instance. Path = {0},ConnectionString = {1} User = {2}\r\n", Path, _ConnectionString, User));
        }

        ~DALRedux()
        {
            this.Dispose(false);
        }


        protected void Initialize()
        {
            DbConnection connection = null;
            try
            {
                String dbVersion = this.ReadGlobalValue("Database", "Version");
                if (dbVersion != null)
                {
                    DatabaseVersion = dbVersion;
                }


                this.SchemaVersion = this.ExecuteScalar<long>("PRAGMA user_version;");
                CruiseDAL.Updater.Update(this);

                try
                {
                    this.LogMessage("File Opened" , "normal");
                }
                catch (ReadOnlyException)
                {/*ignore, in case we want to allow access to a read-only DB*/}
            }
            catch (Exception e)
            {
                throw this.ThrowDatastoreExceptionHelper(connection, null, e);
            }


        }

        public CruiseFileType ExtrapolateCruiseFileType(String path)
        {
            String normPath = this.Path.ToLower().TrimEnd();
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


        protected void BuildDBFile(string path)
        {
            if (File.Exists(path))
            {
                File.Delete(path);
            }

            String createSQL = this.GetCreateSQL();
            String createTriggers = this.GetCreateTriggers();

            SQLiteConnection.CreateFile(path);

            try
            {
                Context.BeginTransaction();
                Execute(createSQL);
                Execute(createTriggers);

                Context.CommitTransaction();
            }
            catch (Exception e)
            {
                Context.CancelTransaction();
                throw this.ThrowDatastoreExceptionHelper(conn, sqlCommand, e);
            }
        }

        protected override String GetCreateSQL()
        {
            return CruiseDAL.Properties.Resources.CruiseCreate;
        }

        internal string GetCreateTriggers()
        {
            return CruiseDAL.Properties.Resources.CreateTriggers;
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

        public void StartSavePoint(String name)
        {
            this.Execute("SAVEPOINT " + name + ";");
        }

        public void ReleaseSavePoint(String name)
        {
            this.Execute("RELEASE SAVEPOINT " + name + ";");
        }

        public void RollbackSavePoint(String name)
        {
            this.Execute("ROLLBACK TO SAVEPOINT " + name + ";");
        }

        /// <summary>
        /// Sets the starting value of a AutoIncrement field for a table
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="start"></param>
        /// <exception cref="DatabaseExecutionException"></exception>
        public virtual void SetTableAutoIncrementStart(String tableName, Int64 start)
        {
            string commandText = null;
            //check sqlite_sequence to see if we need to perform update or insert
            if (this.GetRowCount("sqlite_sequence", "WHERE name = ?", tableName) >= 1)
            {
                commandText = "UPDATE sqlite_sequence SET seq = @start WHERE name = @tableName";
            }
            else
            {
                commandText = "INSERT INTO sqlite_sequence  (seq, name) VALUES (@start, @tableName);";
            }

            this.Execute(commandText, tableName, start);

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

        protected override void Dispose(bool isDisposing)
        {
            base.Dispose(isDisposing);
            if (_disposed)
            {
                return;
            }

            releaseAccessControl();

            _disposed = true;
        }

        #endregion
    }
}
