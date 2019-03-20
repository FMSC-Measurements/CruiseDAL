using FMSC.ORM.Core;
using FMSC.ORM.SQLite;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

#pragma warning disable RECS0122 // Initializing field with default value is redundant
#pragma warning disable RECS0104 // When object creation uses object or collection initializer, empty argument list is redundant

namespace CruiseDAL
{
    public enum CruiseFileType { Unknown, Cruise, Template, Design, Master = 32 | Cruise, Component = 64 | Cruise, Backup = 128 | Cruise }

    public partial class DAL : SQLiteDatastore
    {
        protected class ExternalDatastore
        {
            private string _dbPath;
            public DatastoreRedux DS;

            public string DbPath
            {
                get => _dbPath ?? DS.Path;
                set => _dbPath = value;
            }

            public string Alias;
        }

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

        public CruiseFileType CruiseFileType
        {
            get
            {
                return ExtrapolateCruiseFileType(this.Path);
            }
        }

        /// <summary>
        /// Initializes a new in memory instance of the <see cref="DAL"/> class.
        /// </summary>
        public DAL() : this(IN_MEMORY_DB_PATH, true)
        { }

        /// <summary>
        /// Creates a DAL instance for a database @ path.
        /// </summary>
        /// <exception cref="ArgumentNullException">path can not be null or an empty string</exception>
        /// <exception cref="IOException">problem working with file. wrong extension</exception>
        /// <exception cref="FileNotFoundException">file doesn't exist</exception>
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
        /// <exception cref="IOException">File extension is not valid <see cref="VALID_EXTENSIONS"/></exception>
        /// <exception cref="UnauthorizedAccessException">File open in another application or thread</exception>
        public DAL(string path, bool makeNew, DatabaseBuilder builder) : base(path)
        {
            if (builder == null) { throw new ArgumentNullException("builder"); }

            Path = path;

            this.Initialize(makeNew, builder);
            Logger.Log.V(String.Format("Created DAL instance. Path = {0}\r\n", Path));
        }

        protected void Initialize(bool makeNew, DatabaseBuilder builder)
        {
            if (IsInMemory)
            {
                OpenConnection();
            }
            if (makeNew)
            {
                builder.CreateDatastore(this);
            }
            else if (!makeNew && !Exists)
            {
                throw new FileNotFoundException();
            }

            builder.UpdateDatastore(this);

            try
            {
                this.LogMessage("File Opened", "normal");
            }
            catch (FMSC.ORM.ReadOnlyException)
            {/*ignore, in case we want to allow access to a read-only DB*/}
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

        public void AttachDB(DatastoreRedux dataStore, string alias)
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
    }
}