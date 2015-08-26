using System;
using System.Collections.Generic;
using Logger;
using System.Reflection;
using System.IO;
using System.Collections;
using CruiseDAL.DataObjects;
using System.Threading;
using System.Data.Common;
using CruiseDAL;
using System.Text;

#if ANDROID
using Mono.Data.Sqlite;
using SQLiteCommand = Mono.Data.Sqlite.SqliteCommand;
using SQLiteParameter = Mono.Data.Sqlite.SqliteParameter;
using SQLiteConnection = Mono.Data.Sqlite.SqliteConnection;
#else
using System.Data.SQLite;
#endif

#if !Mobile
using System.Security.AccessControl;
using System.Security.Principal;
using System.Security.Permissions;

#endif

namespace CruiseDAL
{
    public enum OnConflictOption { Rollback, Abort, Fail, Ignore, Replace };


    public partial class DAL : DatastoreBase, IDisposable
    {
        //delegate definition for the method that will be called 
        //to build the database schema on a seperate thread
        //protected delegate void AsyncBuildSchemaCaller(); 

        public static string[] VALID_EXTENSIONS = new string[] { ".cruise", ".cut", ".design" };
        protected FileInfo _DBFileInfo;
        Mutex _accessControl;
        

        #region properties


        protected void Initialize()
        {
            

            String dbVersion = this.ExecuteScalar("SELECT Value FROM Globals WHERE Block = 'Database' AND Key = 'Version'") as string;
            if (dbVersion != null)
            {
                base.DatabaseVersion = dbVersion;
            }
            else
            {
                base.DatabaseVersion = "Unknown";
            }


            CruiseDAL.Updater.Update(this);

            

            string callingProgram = GetCallingProgram();
            string command = String.Format("INSERT OR REPLACE INTO Globals (Block, Key, Value) Values ( 'General', 'MRUP', '{0}' );", callingProgram);
            try
            {
                this.Execute(command);
            }
            catch (ReadOnlyException)
            {/*ignore, incase we want to allow access to a readonly DB*/}
            

        }


        /// <summary>
        /// Gets and Sets the file path to the database
        /// </summary>
        /// <exception cref="System.IO.IOException">File extension is not valid <see cref="VALID_EXTENSIONS"/></exception>
        /// <exception cref="CruiseDAL.DatabaseShareException">File open in another application or thread</exception>
        public string Path
        {
            get
            {
                return _DBFileInfo.FullName;
            }
            protected set
            {
                if (string.IsNullOrEmpty(value))
                {
                    throw new ArgumentNullException("Path", "Path can not be null or empty");
                }

                releaseAccessControl(_DBFileInfo);

                _DBFileInfo = new FileInfo(value);

                if (Array.BinarySearch<String>(VALID_EXTENSIONS, _DBFileInfo.Extension) == -1)
                {
                    throw new System.IO.IOException("File extension is not valid");
                }
#if! Mobile     //Read only property not supported in compact framework 
                if (_DBFileInfo.Exists && _DBFileInfo.IsReadOnly)//trying to access a readonly file causes problems 
                {
                    throw new ReadOnlyException(string.Empty);
                }
#else
                if (_DBFileInfo.Exists && (_DBFileInfo.Attributes & FileAttributes.ReadOnly) == FileAttributes.ReadOnly)//trying to access a readonly file causes problems 
                {
                    throw new ReadOnlyException(string.Empty);
                }
#endif


#if !Mobile
                //bool createdNew = false;
                try
                {
                    string semaName = "CruiseDAL" + _DBFileInfo.FullName.GetHashCode().ToString();
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
                        this.releaseAccessControl(this._DBFileInfo);
                        throw new DatabaseShareException("File Open Somewhere Else");
                    }
                }
                //new FileIOPermission(FileIOPermissionAccess.AllAccess, _DBFileInfo.FullName).Demand();
                //FileSecurity fSecurity = _DBFileInfo.GetAccessControl();
                //fSecurity.AddAccessRule(new FileSystemAccessRule(,
                //    FileSystemRights., AccessControlType.Deny));
                
                //_DBFileInfo.SetAccessControl(fSecurity);
#endif
            }
        }

        private void releaseAccessControl(FileInfo file)
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

        /// <summary>
        /// Gets a value that indicates if a file exists at the path
        /// </summary>
        public bool Exists
        {
            get { return _DBFileInfo.Exists; }
        }

        /// <summary>
        /// Gets the extension of the file (ex. ".___")
        /// </summary>
        public string Extension
        {
            get { return _DBFileInfo.Extension; }
        }

        ///// <summary>
        ///// Get the schema version
        ///// </summary>
        //private string _databaseVersion; 
        //public string DatabaseVersion
        //{
        //    get
        //    {
        //        return _databaseVersion; 
        //    }
        //    internal set
        //    {
        //        this._databaseVersion = value;
        //    }

        //}

        //stores the active database connection, should be null when not in use
        

        //id table is used as a data object look up, to insure that multiple objects
        //representing the same data don't exist. 
        

        //a dataObjectFactory for this spacific DAL
        //all dataObjects created using this will 
        //have their dal member set to this dal
        //public DataObjectFactory DOFactory { get; set; }

        //private string _userInfo;
        ///// <summary>
        ///// Gets the string used to identify the user, for the purpose of CreatedBy and ModifiedBy values
        ///// </summary>
        //public string User 
        //{
        //    get
        //    {
        //        if (_userInfo == null)
        //        {
        //            _userInfo = GetUserInformation();
        //        }
        //        return _userInfo;
        //    }
        //}

        protected override String GetUserInformation()
        {
#if Mobile
            FMSC.Utility.MobileDeviceInfo di = new FMSC.Utility.MobileDeviceInfo();
            return di.GetModelAndSerialNumber();
            //return "Mobile User";
#elif FullFramework
            return Environment.UserName + " on " + System.Windows.Forms.SystemInformation.ComputerName;
			#elif ANDROID
			return "AndroidUser";
#endif
           //return Environment.UserName + " on " + System.Windows.Forms.SystemInformation.ComputerName;
        }


        //private Dictionary<string, RowValidator> _validators = new Dictionary<string, RowValidator>();


        #endregion


        #region ctor

        /// <summary>
        /// Creates a DAL instance for a database @ path. 
        /// </summary>
        /// <exception cref="ArgumentNullException">path can not be null or an impty string</exception>
        /// <exception cref="IOException">problem working with file. wrong extension</exception>
        /// <exception cref="FileNotFoundException"
        /// <param name="path"></param>
        public DAL(string path) : this(path, false)
        {
        }

        /// <exception cref="System.IO.IOException">File extension is not valid <see cref="VALID_EXTENSIONS"/></exception>
        /// <exception cref="System.UnauthorizedAccessException">File open in another application or thread</exception>
        public DAL(string path, bool makeNew)
        {
            Path = path;
            _ConnectionString = BuildConnectionString(false);

            base.InitializeBase();

            if (makeNew)
            {
                Create();
            }
            else if(!makeNew && !Exists)
            {
                throw this.ThrowDatastoreExceptionHelper(null, null, new FileNotFoundException());
            }

            //allow null or empty path to create in memory DB?
            if (Exists)
            {
                this.Initialize();
            }

            Log.V(String.Format("Created DAL instance. Path = {0},ConnectionString = {1} User = {2}\r\n", Path, _ConnectionString, User));
        }

        ~DAL()
        {
            this.Dispose(false);
        }


        #endregion

        #region methods

        //The following methods deal specificly with dealing with the file where the database is stored
        #region File utility methods
        /// <summary>
        /// Copies entire file to <paramref name="path"/> Overwriting any existing file
        /// </summary>
        /// <param name="path"></param>
        public DAL CopyTo(string path)
        {
            return this.CopyTo(path, false);
        }

        public DAL CopyTo(string path, bool overwrite)
        {
            this.CloseConnection();
            _DBFileInfo.CopyTo(path, overwrite);
            return new DAL(path);
        }

        /// <summary>
        /// Creates copy at location, and changes database path to new location
        /// </summary>
        /// <param name="path"></param>
        public bool CopyAs(string path)
        {
            this.CloseConnection();
            try
            {
                _DBFileInfo.CopyTo(path);
                this._DBFileInfo = new FileInfo(path);
                _ConnectionString = BuildConnectionString(false);
            }
            catch (Exception)
            {
                return false;
            }

            return true;
        }

        public bool MoveTo(string path)
        {
            this.CloseConnection();
            try
            {
                _DBFileInfo.MoveTo(path);
                this._DBFileInfo = new FileInfo(path);
                _ConnectionString = BuildConnectionString(false);
            }
            catch
            {
                return false;
            }
            return true;
        }


        internal void MigrateTo(string path)
        {
            //close connections to existing 
            this.CloseConnection();
            //invalidate all outstanding dataobjects
            this.FlushCache();
            try
            {
                
                this._DBFileInfo = new FileInfo(path);
                _ConnectionString = BuildConnectionString(false);//rebuild connection string
            }
            catch (Exception e)
            {
                throw this.ThrowDatastoreExceptionHelper("Failed to migrate database", e, false);
            }
        }

        #endregion

        #region Database utiltity methods


        

        

        


        
        ///// <summary>
        ///// Asyncronisly Starts Creating a file at <see cref="Path"/> 
        ///// User should call <see cref="EndCreate"/> at some point after calling 
        ///// </summary>
        ///// <example><code>
        ///// void DoSomething()
        ///// {
        /////     ...
        /////     var result = BeginCreate(new AsyncCallback(OnCreateDone));
        /////     ...
        ///// }
        ///// 
        ///// void OnCreateDone(IAsyncCallback result)
        ///// {
        /////     ...
        /////     EndCreate(result);
        /////     ...
        ///// }
        ///// 
        ///// or 
        ///// 
        ///// void DoSomething()
        ///// {
        /////     ...
        /////     var result = BeginCreate(null);
        /////     ...
        /////     EndCreate(result);
        /////     ...
        ///// }
        ///// </code></example>
        ///// <param name="callbackFunct">optional</param>
        ///// <returns></returns>
        //public IAsyncResult BeginCreate(AsyncCallback callbackFunct)
        //{
        //    _buildSchemaCallerHandle = new AsyncBuildSchemaCaller(this.BuildDBFile);
        //    return _buildSchemaCallerHandle.BeginInvoke(callbackFunct, null);
        //}



        
        protected override string BuildConnectionString(bool isNew)
        {
            return BuildConnectionString(isNew, _DBFileInfo.FullName);
        }


        protected override string BuildConnectionString(bool isNew, string path)
        {

            return string.Format("Data Source={0};New= {1};Version=3;", path, isNew);
        }



        protected override void BuildDBFile()
        {
            //if overwriting existing file
            if (Exists)
            {
                _DBFileInfo.Delete();
            }

            String createSQLText = this.GetCreateSQL();


            //open database connection, using a connection string with the parameter New = True
            SQLiteConnection.CreateFile(this.Path);
            //using (SQLiteConnection cn = (SQLiteConnection)this.OpenConnection())
            //{
                
            //    cn.ChangePassword("something");
            //}
            this.Execute(createSQLText);

            //update status of file
            _DBFileInfo.Refresh();

            Initialize();
        }

        protected void BuildDBFile(String path)
        {
            String createSQL = this.GetCreateSQL();
            String createTriggers = this.GetCreateTriggers();
            this.BuildDBFile(path, createSQL, createTriggers);            
        }

        protected void BuildDBFile(string path, string createSQL, string createTriggers)
        {
            if (File.Exists(path))
            {
                File.Delete(path);
            }

            SQLiteConnection.CreateFile(path);
            DbCommand sqlCommand = this.CreateCommand(createSQL);
            DbCommand tCommand = this.CreateCommand(createTriggers);
            String connectionString = this.BuildConnectionString(true, path);
            DbConnection conn = this.CreateConnection(connectionString);
            try
            {
                conn.Open();
                base.ExecuteSQL(sqlCommand, conn);
                base.ExecuteSQL(tCommand, conn);
            }
            catch (Exception e)
            {
                throw this.ThrowDatastoreExceptionHelper(conn, sqlCommand, e);
            }
            finally
            {
                if (conn != null)
                {
                    conn.Close();
                }
                sqlCommand.Dispose();
                tCommand.Dispose();
            }
        }



        internal string GetCreateTriggers()
        {
            String createTriggers = null;
            StreamReader reader = null;
            //read in the contents of CruiseCreate
            try
            {
                Assembly assembley = Assembly.GetExecutingAssembly();
                System.IO.Stream stream = assembley.GetManifestResourceStream("CruiseDAL.AutoGen.CruiseTriggers.sql");
                reader = new System.IO.StreamReader(stream);
                createTriggers = reader.ReadToEnd();
                return createTriggers;
            }
            catch (Exception e)
            {
                Log.E("Unable to read CreateTriggers file", e);
                throw e;
            }
            finally
            {

                if (reader != null) { reader.Close(); }
            }

        }


        protected override String GetCreateSQL()
        {
            String createSQLText = null;
            StreamReader reader = null;
            //read in the contents of CruiseCreate
            try
            {
                Assembly assembley = Assembly.GetExecutingAssembly();
                System.Diagnostics.Debug.Assert(assembley.GetManifestResourceNames().Length != 0);
                System.IO.Stream stream = assembley.GetManifestResourceStream("CruiseDAL.CruiseDAL.CruiseCreate.sql");
                reader = new System.IO.StreamReader(stream);
                createSQLText = reader.ReadToEnd();
                return createSQLText;
            }
            catch (Exception e)
            {
                Log.E("Unable to read CruiseCreate file", e);
                throw e;
            }
            finally
            {
                
                if (reader != null) { reader.Close(); }
            }
        }


        protected override void WriteTableDumpRowValues(System.IO.TextWriter writer, DbDataReader reader)
        {
            String[] values = new String[reader.VisibleFieldCount];
            for (int i = 0; i < reader.VisibleFieldCount; i++)
            {
                object obj = reader.GetValue(i);
                if (obj is DBNull)
                {
                    values[i] = "NULL";
                }
                else if (obj is bool)
                {
                    values[i] = ((bool)obj) ? "1" : "0"; 
                }
                else if (obj is string)
                {
                    values[i] = "'" + (string)obj + "'";
                }
                else if (obj is DateTime)
                {
                    values[i] = "'" + reader.GetString(i) + "'";//HACK unless we can figure out how to format DT the way the adapter does we will just try to get the text 
                }
                else
                {
                    values[i] = SQLiteConvert.ToStringWithProvider(obj, System.Globalization.CultureInfo.CurrentCulture);
                }
            }

            writer.WriteLine("(" + String.Join(",", values) + ")");
        }

        private static string GetCallingProgram()
        {
#if !Mobile
            try
            {
                return Assembly.GetEntryAssembly().FullName;
            }
            catch
            {
                //TODO add error report message so we know when we encounter this exception and what platforms 
                return AppDomain.CurrentDomain.FriendlyName;
            }
#else
            return AppDomain.CurrentDomain.FriendlyName;

#endif

        }

        protected override Exception ThrowDatastoreExceptionHelper(DbConnection conn, DbCommand comm, Exception innerException, bool throwException)
        {
            string message = String.Format("Read/Write Error Command:{0} ConnStr:{1} ConnState:{2} HoldConn:{3} OpenConnCount:{4}",
                (comm != null) ? comm.CommandText : "n/a",
                (conn != null) ? conn.ConnectionString : "n/a",
                (conn != null) ? conn.State.ToString() : "n/a",
                this._holdConnection,
                this._openConnectionCount);
            return this.ThrowDatastoreExceptionHelper(message, innerException, throwException);


        }
        
        protected override Exception ThrowDatastoreExceptionHelper(string message, Exception innerException, bool throwException)
        {
            Exception newEx;
            if (innerException is SQLiteException)
            {
                SQLiteException ex = innerException as SQLiteException;
                switch (ex.ResultCode)
                {
                    case SQLiteErrorCode.Corrupt:
                        {
                            newEx = new DatabaseMalformedException(this, innerException);
                            break;
                        }
                    case SQLiteErrorCode.NotADb:
                    case SQLiteErrorCode.Perm:                    
                    case SQLiteErrorCode.IoErr:
                    case SQLiteErrorCode.CantOpen:
                    case SQLiteErrorCode.Full:
                    case SQLiteErrorCode.Auth:
                        {
                            newEx = new FileAccessException(message, innerException);
                            break;
                        }
                    case SQLiteErrorCode.ReadOnly:
                        {
                            newEx = new ReadOnlyException(message, innerException);
                            break;
                        }
                    default:
                        {
                            newEx = new DatabaseExecutionException(message, innerException);
                            break;
                        }
                }
            }
            else
            {
                newEx = new DatabaseExecutionException(message, innerException);
            }
            Log.E(newEx);
            if (throwException)
            {
                throw newEx;
            }
            return newEx;
        }



        private void LogMessage(string program, string message, string level)
        {
            Logger.Log.L(message);

            if (this.Exists)
            {
                MessageLogDO msg = new MessageLogDO(this);
                msg.Program = program;
                msg.Message = message;
                msg.Level = level;
                msg.Date = DateTime.Now.ToString("yyyy/MM/dd");
                msg.Time = DateTime.Now.ToString("HH:mm"); 
                msg.Save();
            }

        }

        public override void LogMessage(string message, string level)
        {
            string appStr = GetCallingProgram();
            LogMessage(appStr, message, level);
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
        public void DirectCopy(DAL dataBase, string table, String selection,OnConflictOption option, params Object[] selectionArgs)
        {
            if (dataBase.Exists == false) { return; }

            

            DbConnection conn = null;
            DbCommand copyCommand = null;
            try
            {
                string cOpt = option.ToString().ToUpper();
                string copy = String.Format("INSERT OR {2} INTO {0} SELECT * FROM destDB.{0} {1};", table, selection, cOpt);

                conn = InternalAttachDB(dataBase, "destDB");

                copyCommand = conn.CreateCommand();
                copyCommand.CommandText = copy;
                copyCommand.ExecuteNonQuery();
            }
            catch (ThreadAbortException)
            {

            }
            catch (Exception e)
            {
                throw this.ThrowDatastoreExceptionHelper(conn, copyCommand, e);
            }
            finally
            {
                InternalDetachDB("destDB");
                CloseConnection();
                if (copyCommand != null) { copyCommand.Dispose(); }
            }
        }

        internal DbConnection InternalAttachDB(DAL externalDB, string externalDBAlias)
        {
            //once connection is closed all attached DB's are detached so we need to leave it open
            //calling detachDB will cause the connection hold to be released 
            this.EnterConnectionHold();
            
            lock (this._connectionSyncLock)
            {

                DbConnection conn = OpenConnection();
                this.Execute("ATTACH DATABASE ? AS ?;", externalDB.Path, externalDBAlias);
                return conn;
            }
        }

        public void AttachDB(DAL externalDB, string externalDBAlias)
        {
            this.InternalAttachDB(externalDB, externalDBAlias);
        }

        public void DetachDB(string externalDBAlias)
        {
            try
            {
                this.InternalDetachDB(externalDBAlias);
            }
            finally
            {
                this.CloseConnection();
            }
        }


        internal void InternalDetachDB(string externalDBAlias)
        {

            try
            {
                string detach = string.Format("DETACH DATABASE {0};", externalDBAlias);
                this.Execute(detach);
            }
            finally
            {
                this.ExitConnectionHold();
            }

        }

        public bool HasCruiseErrors(out string[] errors)
        {
            bool hasErrors = false;
            List<string> errorList = new List<string>();

            if (HasForeignKeyErrors(null))
            {
                errorList.Add("File contains Foreign Key errors");
                hasErrors = true;
            }

            //if (HasMismatchSpecies())
            //{
            //    errorList.Add("Tree table has mismatch species codes");
            //    hasErrors = true;
            //}

            if (HasSampleGroupUOMErrors())
            {
                errorList.Add("Sample Group table has invalid mix of UOM");
                hasErrors = true;
            }

            if (HasBlankCountOrMeasure())
            {
                errorList.Add("Tree table has record(s) with blank Count or Measure value");
                hasErrors = true;
            }
            if (HasBlankDefaultLiveDead())
            {
                errorList.Add("Sample Group table has record(s) with blank default live dead vaule");
                hasErrors = true;
            }
            if (HasBlankLiveDead())
            {
                errorList.Add("Tree table has record(s) with blank Live Dead value");
                hasErrors = true;
            }
            if (HasBlankSpeciesCodes())
            {
                this.Execute(
                @"Update Tree 
                SET Species = 
                    (Select Species FROM TreeDefaultValue 
                        WHERE TreeDefaultValue.TreeDefaultValue_CN = Tree.TreeDefaultValue_CN) 
                WHERE ifnull(Tree.Species, '') = '' 
                AND ifnull(Tree.TreeDefaultValue_CN, 0) = 0;");
                if (HasBlankSpeciesCodes())
                {
                    errorList.Add("Tree table has record(s) with blank species or no tree default");
                    hasErrors = true;
                }
            }

            if (HasOrphanedStrata())
            {
                errorList.Add("Stratum table has record(s) that have not been assigned to a cutting unit");
                hasErrors = true;
            }
            if (HasStrataWithNoSampleGroups())
            {
                errorList.Add("Stratum table has record(s) that have not been assigned any sample groups");
                hasErrors = true;
            }

            errors = errorList.ToArray();
            return hasErrors;
        }

        public bool HasCruiseErrors()
        {
            string[] errors;
            return this.HasCruiseErrors(out errors);
        }

        private bool HasBlankSpeciesCodes()
        {
            return this.GetRowCount(Schema.TREE._NAME, "WHERE ifnull(Species, '') = ''") > 0;
        }

        private bool HasBlankLiveDead()
        {
            return this.GetRowCount(Schema.TREE._NAME, "WHERE ifnull(LiveDead, '') = ''") > 0;
        }

        private bool HasBlankCountOrMeasure()
        {
            return this.GetRowCount(Schema.TREE._NAME, "WHERE ifnull(CountOrMeasure, '') = ''") > 0;
        }

        private bool HasBlankDefaultLiveDead()
        {
            return this.GetRowCount(Schema.SAMPLEGROUP._NAME, "WHERE ifnull(DefaultLiveDead, '') = ''") > 0;
        }

        //private bool HasMismatchSpecies()
        //{
        //    return this.GetRowCount("Tree", "JOIN TreeDefaultValue USING (TreeDefaultValue_CN) WHERE Tree.Species != TreeDefaultValue.Species") > 0;
        //}

        private bool HasSampleGroupUOMErrors()
        { 
            return ((long)this.ExecuteScalar("Select Count(DISTINCT UOM) FROM SampleGroup WHERE UOM != '04';")) > 1L;
            //return this.GetRowCount("SampleGroup", "WHERE UOM != '04' GROUP BY UOM") > 1;
        }

        private bool HasOrphanedStrata()
        {
            return this.GetRowCount(Schema.STRATUM._NAME, "LEFT JOIN CuttingUnitStratum USING (Stratum_CN) WHERE CuttingUnitStratum.Stratum_CN IS NULL") > 0;
        }

        private bool HasStrataWithNoSampleGroups()
        {
            return this.GetRowCount(Schema.STRATUM._NAME, "LEFT JOIN SampleGroup USING (Stratum_CN) WHERE SampleGroup.Stratum_CN IS NULL") > 0;
        }



        public bool HasForeignKeyErrors(string table_name)
        {
            bool hasErrors = false;
            string comStr;
            if (string.IsNullOrEmpty(table_name))
            {
                comStr = "PRAGMA foreign_key_check;";
            }
            else
            {
                comStr = string.Format("PRAGMA foreign_key_check({0});", table_name);
            }
            lock (this._connectionSyncLock)
            {
                DbConnection conn = OpenConnection();
                DbDataReader reader = null;
                DbCommand command = this.CreateCommand(conn, comStr);
                try
                {
                    reader = command.ExecuteReader();
                    hasErrors = reader.Read();
                }
                catch (Exception e)
                {
                    throw this.ThrowDatastoreExceptionHelper(conn, command, e);
                }
                finally
                {
                    if (reader != null) { reader.Dispose(); }
                    if (command != null) { command.Dispose(); }
                    CloseConnection();
                }

                return hasErrors;
            }

        }


        #region read methods

        /// <summary>
        /// Sets the starting value of a AutoIncrement field for a table
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="start"></param>
        /// <exception cref="DatabaseExecutionException"></exception>

        public virtual void SetTableAutoIncrementStart(String tableName, Int64 start)
        {
                        
            DbCommand command = null;
            lock (this._connectionSyncLock)
            {
                DbConnection conn = OpenConnection();
                try
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

                    command = this.CreateCommand(conn, commandText);
                    command.Parameters.Add(this.CreateParameter("@tableName", tableName));
                    command.Parameters.Add(this.CreateParameter("@start", start));

                    command.ExecuteNonQuery();

                }
                catch (ThreadAbortException)
                {

                }
                catch (Exception e)
                {
                    throw this.ThrowDatastoreExceptionHelper(conn, command, e);
                }
                finally
                {
                    if (command != null)
                    {
                        command.Dispose();
                    }
                    CloseConnection();
                }
            }
        }

        #endregion


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

            releaseAccessControl(this._DBFileInfo);
            _disposed = true;
        }

        #endregion
    }//end DAL class



    public class DatabaseMalformedException : CruiseDAL.FileAccessException
    {
        public DatabaseMalformedException(DAL database, Exception innerException)
            : base("Database Malformed (" + database.Path + ")", innerException)
        { }        
    }
}
