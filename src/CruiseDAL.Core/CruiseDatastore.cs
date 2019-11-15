using FMSC.ORM.Core;
using FMSC.ORM.SQLite;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace CruiseDAL
{
    public class CruiseDatastore : SQLiteDatastore
    {
        private ICollection<ExternalDatastore> _attachedDataStores = new List<ExternalDatastore>();

        /// <summary>
        /// Get the database version
        /// </summary>
        public string DatabaseVersion
        {
            get
            {
                try
                {
                    return ReadGlobalValue("Database", "Version");
                }
                catch
                {
                    return "";
                }
            }
        }

        public CruiseDatastore()
            : this(SQLiteDatastore.IN_MEMORY_DB_PATH, false, (IDatastoreBuilder)null, (IUpdater)null)
        {
        }

        public CruiseDatastore(string path)
            : this(path, false, (IDatastoreBuilder)null, (IUpdater)null)
        { }

        /// <summary>
        /// Creates a CruiseDatastore instance for a database @ path.
        /// </summary>
        /// <exception cref="ArgumentNullException">path can not be null or an empty string</exception>
        /// <exception cref="IOException">File extension is not valid</exception>
        /// <exception cref="UnauthorizedAccessException">File open in another application or thread</exception>
        public CruiseDatastore(string path, bool makeNew, IDatastoreBuilder builder, IUpdater updater) : base(path)
        {
            if (path == null) { throw new ArgumentNullException("path"); }
            if (path != IN_MEMORY_DB_PATH && IsExtentionValid(path) == false)
            { throw new IOException("File extension is not recognized"); }

            Path = path;

            Initialize(makeNew, builder, updater);
            Logger.Log.V(String.Format("Created DAL instance. Path = {0}\r\n", Path));
        }

        protected void Initialize(bool makeNew, IDatastoreBuilder builder, IUpdater updater)
        {
            if (IsInMemory)
            {
                // HACK we need to open a connection when we start using a in memory db
                // and keep it open because our database will die if we close the connection
                OpenConnection();
                if (builder != null)
                {
                    builder.CreateDatastore(this);
                }
            }
            else if (makeNew)
            {
                if (builder != null)
                {
                    builder.CreateDatastore(this);
                }
            }
            else if (!makeNew && !Exists)
            {
                throw new FileNotFoundException();
            }
            else
            {
                // only run updater if db is not in memory and not new
                if (updater != null)
                {
                    updater.Update(this);
                }
            }

            try
            {
                LogMessage("File Opened", "normal");
            }
            
            catch (FMSC.ORM.ReadOnlyException)
            {/*ignore, in case we want to allow access to a read-only DB*/}
            catch (FMSC.ORM.SQLException)
            { }
        }

        protected virtual bool IsExtentionValid(string path)
        {
            return true;
        }

        public void LogMessage(string message, string level)
        {
            string program = GetCallingProgram();

            Logger.Log.L(message);

            if (Exists)
            {

                Execute("INSERT INTO MessageLog (Program, Message, Level, Date, Time) " +
                    "VALUES " +
                    "(@p1, @p2, @p3, @p4, @p5)",
                    new object[] {
                        program,
                        message,
                        level,
                        DateTime.Now.ToString("yyyy/MM/dd"),
                        DateTime.Now.ToString("HH:mm") }
                    );

            }
        }

        private static string GetCallingProgram()
        {
#if !NetCF
            try
            {
                return System.Reflection.Assembly.GetEntryAssembly().FullName;
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

        public static string GetUserInformation()
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

        #region Overridden Methods

        protected override void OnConnectionOpened()
        {
            base.OnConnectionOpened();

            foreach (var ds in _attachedDataStores)
            {
                AttachDBInternal(ds);
            }
        }

        #endregion Overridden Methods

        #region Attach/Detach

        public void AttachDB(Datastore dataStore, string alias)
        {
            if (dataStore == null) { throw new ArgumentNullException("dataStore"); }
            if (String.IsNullOrEmpty(alias)) { throw new ArgumentException("alias can't be null or empty", "alias"); }

            var externalDS = new ExternalDatastore()
            {
                DS = dataStore,
                Alias = alias
            };

            _attachedDataStores.Add(externalDS);
            AttachDBInternal(externalDS);
        }

        public void AttachDB(string dbPath, string alias)
        {
            if (dbPath == null) { throw new ArgumentNullException("dbPath"); }
            if (String.IsNullOrEmpty(alias)) { throw new ArgumentException("alias can't be null or empty", "alias"); }

            var externalDS = new ExternalDatastore()
            {
                DbPath = dbPath,
                Alias = alias
            };

            _attachedDataStores.Add(externalDS);
            AttachDBInternal(externalDS);
        }

        //TODO test
        protected void AttachDBInternal(ExternalDatastore externalDB)
        {
            lock (_persistentConnectionSyncLock)
            {
                var connection = PersistentConnection;
                if (connection != null)
                {
                    var commandText = "ATTACH DATABASE \"" + externalDB.DbPath + "\" AS " + externalDB.Alias + ";";
                    try
                    {
                        connection.ExecuteNonQuery(commandText, (object[])null, CurrentTransaction);
                    }
                    catch (Exception e)
                    {
                        throw ExceptionProcessor.ProcessException(e, connection, commandText, CurrentTransaction);
                    }
                }
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

        //TODO test
        protected void DetachDBInternal(ExternalDatastore externalDB)
        {
            lock (_persistentConnectionSyncLock)
            {
                var connection = PersistentConnection;
                if (connection != null)
                {
                    var commandText = "DETACH DATABASE \"" + externalDB.Alias + "\";";
                    try
                    {
                        connection.ExecuteNonQuery(commandText, (object[])null, CurrentTransaction);
                    }
                    catch (Exception e)
                    {
                        throw ExceptionProcessor.ProcessException(e, connection, commandText, CurrentTransaction);
                    }
                }
            }
        }

        #endregion Attach/Detach

        #region cruise/cut specific stuff

        public string ReadGlobalValue(String block, String key)
        {
            return this.ExecuteScalar("SELECT Value FROM GLOBALS WHERE " +
            "ifnull(Block, '') = ifnull(@p1, '') " +
            "AND ifnull(Key, '') = ifnull(@p2, '');", block, key) as string;
        }

        public void WriteGlobalValue(String block, String key, String value)
        {
            this.Execute("INSERT OR REPLACE INTO Globals (Block, Key, Value) " +
                "Values (@p1, @p2, @p3);", block, key, value);
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

#if SYSTEM_DATA_SQLITE
            System.Data.SQLite.SQLiteConnection.ClearAllPools();
#endif
            GC.Collect();
            GC.WaitForPendingFinalizers();

            System.IO.File.Copy(this.Path, destPath, overwrite);
        }

        public void CopyAs(string desPath, bool overwrite)
        {
            CopyTo(desPath, overwrite);
            this.Path = desPath;
        }

        #endregion file utility

        protected class ExternalDatastore
        {
            private string _dbPath;
            public Datastore DS;

            public string DbPath
            {
                get => _dbPath ?? DS.Path;
                set => _dbPath = value;
            }

            public string Alias;
        }
    }
}