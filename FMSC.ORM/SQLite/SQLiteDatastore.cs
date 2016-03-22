using FMSC.ORM.Core;
using FMSC.ORM.Core.SQL;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.SQLite;
using System.Linq;
using System.Text;

namespace FMSC.ORM.SQLite
{
    public class SQLiteDatastore : DatastoreRedux
    {
        const string IN_MEMORY_DB_PATH = ":memory:";

        /// <summary>
        /// Gets value indicating if database file exists
        /// </summary>
        public bool Exists
        {
            get
            {
                if(IsInMemory)
                {
                    return true;
                }

                return System.IO.File.Exists(this.Path);
            }
        }

        /// <summary>
        /// Gets the extension of the file (ex. ".___")
        /// </summary>
        public string Extension
        {
            get
            {
                if (IsInMemory)
                {
                    return string.Empty;
                }
                return System.IO.Path.GetExtension(base.Path);
            }
        }

        /// <summary>
        /// Gets a value indicating whether the datastore is in memory.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is in memory; otherwise, <c>false</c>.
        /// </value>
        public bool IsInMemory
        {
            get { return Path == IN_MEMORY_DB_PATH; }
        }


        /// <summary>
        /// creates instance representing an in memory database
        /// </summary>
        public SQLiteDatastore() : this(IN_MEMORY_DB_PATH)
        {
            OpenConnection(); // we will need to open a persistent connection 
            //TODO find a better way to hold the connection for the life of the DAL

        }

        public SQLiteDatastore(string path) : base(new SQLiteProviderFactory())
        {
            if(path == null) { throw new ArgumentNullException("path"); }
            Path = path;
        }

        #region abstract methods
        protected override string BuildConnectionString()
        {
            System.Diagnostics.Debug.Assert(!string.IsNullOrEmpty(Path));
            return string.Format("Data Source={0};Version=3;", Path);
        }
        #endregion

        #region SavePoints
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
        #endregion

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


        [Obsolete("use AddField(string tableName, ColumnInfo fieldDef)")]
        public void AddField(string tableName, string fieldDef)
        {
            string command = string.Format("ALTER TABLE {0} ADD COLUMN {1};", tableName, fieldDef);
            this.Execute(command);
 
        }

        /// <summary>
        /// Adds field to table.
        /// </summary>
        /// <param name="tableName">Name of the table.</param>
        /// <param name="fieldDef">The field definition.</param>
        public void AddField(string tableName, ColumnInfo fieldDef)
        {
            var command = string.Format("ALTER TABLE {0} ADD COLUMN {1};", tableName, fieldDef.GetColumnDef(true));
            Execute(command);
        }

        /// <summary>
        /// Checks if table exists.
        /// </summary>
        /// <param name="tableName">Name of the table.</param>
        /// <returns></returns>
        public bool CheckTableExists(string tableName)
        {
            return GetRowCount("sqlite_master", "WHERE type = 'table' AND name = ?", tableName) > 0;
        }

        /// <summary>
        /// Checks if table has a given field
        /// </summary>
        /// <param name="tableName">Name of the table.</param>
        /// <param name="field">field name.</param>
        /// <returns></returns>
        public bool CheckFieldExists(string tableName, string field)
        {
            foreach(var col in GetTableInfo(tableName))
            {
                if(field.Equals(col.Name, StringComparison.InvariantCultureIgnoreCase)) { return true; }
            }
            return false;
        }

        protected string BuildCreateTable(string tableName, IEnumerable<ColumnInfo> cols, bool temp)
        {
            if (string.IsNullOrEmpty(tableName)) { throw new ArgumentNullException("tableName"); }
            if (cols == null) { throw new ArgumentNullException("cols"); }

            var sb = new StringBuilder();
            sb.Append("CREATE ");
            if (temp) { sb.Append("TEMP "); }
            sb.AppendLine("TABLE " + tableName + PlatformHelper.NewLine);
            using (var enu = cols.GetEnumerator())
            {
                if (!enu.MoveNext()) { throw new ArgumentException("cols can't be empty", "cols"); }
                sb.Append("( " + enu.Current.GetColumnDef(true));

                while (enu.MoveNext())
                {
                    sb.Append("," + PlatformHelper.NewLine + enu.Current.GetColumnDef(true));
                }
                sb.Append(")");
            }
            sb.Append(";");
            return sb.ToString();
        }

        public void CreateTable(string tableName, IEnumerable<ColumnInfo> cols, bool temp)
        {            
            Execute(BuildCreateTable(tableName, cols, temp));            
        }

        /// <summary>
        /// Gets the table column information.
        /// </summary>
        /// <param name="tableName">Name of the table.</param>
        /// <returns></returns>
        public override List<ColumnInfo> GetTableInfo(string tableName)
        {
            var colList = new List<ColumnInfo>();
            lock (_persistentConnectionSyncLock)
            {
                using (DbCommand command = Provider.CreateCommand("PRAGMA table_info(" + tableName + ");"))
                {
                    DbConnection conn = OpenConnection();
                    command.Connection = conn;

                    try
                    {
                        using (var reader = command.ExecuteReader())
                        {
                            int nameOrd = reader.GetOrdinal("name");
                            int dbTypeOrd = reader.GetOrdinal("type");
                            int pkOrd = reader.GetOrdinal("pk");
                            int notNullOrd = reader.GetOrdinal("notnull");
                            int defaultValOrd = reader.GetOrdinal("dflt_value");
                            while (reader.Read())
                            {
                                var colInfo = new ColumnInfo()
                                {
                                    Name = reader.GetString(nameOrd),
                                    DBType = reader.GetString(dbTypeOrd),
                                    IsPK = reader.GetBoolean(pkOrd),
                                    IsRequired = reader.GetBoolean(notNullOrd),
                                    Default = (!reader.IsDBNull(defaultValOrd)) ? reader.GetString(defaultValOrd) : null
                                };
                                colList.Add(colInfo);
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
                }
                return colList;
            }
        }

        /// <summary>
        /// Gets the raw SQL that defines a given table.
        /// </summary>
        /// <param name="tableName">Name of the table.</param>
        /// <returns></returns>
        public string GetTableSQL(String tableName)
        {
            return (String)this.ExecuteScalar("SELECT sql FROM Sqlite_master WHERE name = ? COLLATE NOCASE and type = 'table';", tableName);
        }

        /// <summary>
        /// Determines whether the specified table has foreign key errors.
        /// </summary>
        /// <param name="table_name">The table_name.</param>
        /// <returns></returns>
        public override bool HasForeignKeyErrors(string table_name)
        {
            bool hasErrors = false;
            string comStr;
            if (string.IsNullOrEmpty(table_name))
            {
                comStr = "PRAGMA foreign_key_check;";
            }
            else
            {
                comStr = "PRAGMA foreign_key_check(" + table_name + ");";
            }
            lock (_persistentConnectionSyncLock)
            {
                using (DbCommand command = Provider.CreateCommand(comStr))
                {
                    DbConnection conn = OpenConnection();
                    command.Connection = conn;
                    try
                    {
                        using (var reader = command.ExecuteReader())
                        {
                            hasErrors = reader.Read();
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
                    
                }
                return hasErrors;
            }
        }

        /// <summary>
        /// Gets the number of row that would be returned by a given selection.
        /// </summary>
        /// <param name="tableName">Name of the table.</param>
        /// <param name="selection">The selection.</param>
        /// <param name="selectionArgs">The selection arguments.</param>
        /// <returns></returns>
        public override Int64 GetRowCount(string tableName, string selection, params Object[] selectionArgs)
        {
            string query = string.Format("SELECT Count(1) FROM {0} {1};", tableName, selection);
            return ExecuteScalar<Int64>(query, selectionArgs);
        }


        public IEnumerable<string> GetTableUniques(String tableName)
        {
            String tableSQL = this.GetTableSQL(tableName);

            System.Text.RegularExpressions.Match match =
                System.Text.RegularExpressions.Regex.Match(tableSQL, @"(?<=^\s+UNIQUE\s\()[^\)]+(?=\))", System.Text.RegularExpressions.RegexOptions.Multiline);
            if (match != null && match.Success)
            {
                String[] a = match.Value.Split(new char[] { ',', ' ', '\r', '\n' });
                foreach (string s in a)
                {
                    if (!string.IsNullOrEmpty(s))
                    {
                        yield return s;
                    }
                }

                //return match.Value.Split(new char[]{',',' ','\r','\n'},  StringSplitOptions.RemoveEmptyEntries);
            }
        }

        #region File utility methods
        ///// <summary>
        ///// Copies entire file to <paramref name="path"/> Overwriting any existing file
        ///// </summary>
        ///// <param name="path"></param>
        //public void CopyTo(string path)
        //{
        //    this.CopyTo(path, false);
        //}

        //public void CopyTo(string destPath, bool overwrite)
        //{
        //    Context.ReleaseAllConnections(true);
        //    System.IO.File.Copy(this.Path, destPath, overwrite);
        //}

        

        

        public bool MoveTo(string path)
        {
            System.Diagnostics.Debug.Assert(_holdConnection == 0);
            try
            {
                System.IO.File.Move(this.Path, path);
                this.Path = path;
                //_DBFileInfo.MoveTo(path);
                //this._DBFileInfo = new FileInfo(path);
                //_ConnectionString = BuildConnectionString(false);
            }
            catch
            {
                return false;
            }
            return true;
        }

        #endregion


        protected override Exception ThrowExceptionHelper(DbConnection conn, DbCommand comm, Exception innerException)
        {
            if (innerException is SQLiteException)
            {
                SQLException sqlEx;
                SQLiteException ex = innerException as SQLiteException;
                switch (ex.ResultCode)
                {
                    case SQLiteErrorCode.Corrupt:
                    case SQLiteErrorCode.NotADb:
                    case SQLiteErrorCode.Perm:
                    case SQLiteErrorCode.IoErr:
                    case SQLiteErrorCode.CantOpen:
                    case SQLiteErrorCode.Full:
                    case SQLiteErrorCode.Auth:
                        {
                            return new FileAccessException(ex.ResultCode.ToString(), innerException);
                        }
                    case SQLiteErrorCode.ReadOnly:
                        {
                            sqlEx = new ReadOnlyException(null, innerException);
                            break;
                        }
                    case SQLiteErrorCode.Constraint:
                        {
                            if (innerException.Message.IndexOf("UNIQUE constraint failed", StringComparison.OrdinalIgnoreCase) >= 0)
                            {
                                sqlEx = new UniqueConstraintException(null, innerException);
                            }
                            else
                            {
                                sqlEx = new ConstraintException(null, innerException);
                            }
                            break;
                        }
                    default:
                        {
                            sqlEx = new SQLException(null, innerException);
                            break;
                        }
                }
                if (conn != null)
                {
                    try
                    {
                        sqlEx.ConnectionString = conn.ConnectionString;
                        sqlEx.ConnectionState = conn.State.ToString();
                    }
                    catch (ObjectDisposedException)
                    {
                        sqlEx.ConnectionState = "Disposed";
                    }
                }
                if (comm != null)
                {
                    sqlEx.CommandText = comm.CommandText;
                    //newEx.Data.Add("CommandText", comm.CommandText);
                }

                return sqlEx;
            }
            else
            {
                return new ORMException(null, innerException);
            }
        }

    }
}
