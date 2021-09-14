using FMSC.ORM.Core;
using FMSC.ORM.Logging;
using FMSC.ORM.SQLite;
using System;
using System.IO;
using System.Reflection;

namespace CruiseDAL
{
    public class CruiseDatastore : SQLiteDatastore
    {
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

        public string CreatedVersion
        {
            get
            {
                try
                {
                    return ReadGlobalValue("Database", "CreateVersion");
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
            if (path != IN_MEMORY_DB_PATH)
            {
                // I wanted to refactor IsExtentionValid to just take in the extention 
                // instead of the full path but V2 uses ExtrapolateCruiseFileType 
                // to determin if extention is valid and that method wants the full file name
                if (IsExtentionValid(path) is false)
                {
                    var extension = System.IO.Path.GetExtension(path);
                    throw new IOException($"File extension {extension} is not recognized");
                }
            }


            Path = path;

            Initialize(makeNew, builder, updater);
            Logger.Log($"Created DAL instance. Path = {Path}", LogCategory.Datastore, LogLevel.Info);
        }

        protected void Initialize(bool makeNew, IDatastoreBuilder builder, IUpdater updater)
        {
            var isInMemory = IsInMemory;
            var exists = Exists;

            if (isInMemory)
            {
                // HACK we need to open a connection when we start using a in memory db
                // and keep it open because our database will die if we close the connection
                OpenConnection();
                if (builder != null)
                {
                    CreateDatastore(builder);
                }
            }
            else if (makeNew)
            {
                if (builder != null)
                {
                    CreateDatastore(builder);
                }
            }
            else if (!makeNew && !exists)
            {
                throw new FileNotFoundException();
            }

            try
            {
                LogMessage("File Opened", "normal");
            }
            catch (FMSC.ORM.ReadOnlyException)
            {/*ignore, in case we want to allow access to a read-only DB*/}
            catch (FMSC.ORM.SQLException)
            { }

            if (isInMemory == false
                && makeNew == false)
            {
                // only run updater if db is not in memory and not new
                if (updater != null)
                {
                    updater.Update(this);
                }
            }
        }

        protected virtual bool IsExtentionValid(string path)
        {
            return true;
        }

        public void LogMessage(string message, string level = "I")
        {
            string program = GetCallingProgram();

            Logger.Log(message, "MessageLog", LogLevel.Info);

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

        internal static string GetCallingProgram()
        {
#if !WindowsCE
            try
            {
                var assm = Assembly.GetEntryAssembly();
                if (assm != null)
                {
                    return assm.FullName;
                }
                else
                {
                    return AppDomain.CurrentDomain.FriendlyName;
                }
            }
            catch(Exception e)
            {
                //TODO add error report message so we know when we encounter this exception and what platforms
                try
                {
                    return AppDomain.CurrentDomain.FriendlyName;
                }
                catch
                {
                    return "Unknown";
                }
            }
#else
            try
            {
                return AppDomain.CurrentDomain.FriendlyName;
            }
            catch
            {
                return "Unknown";
            }
#endif
        }

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
    }
}