using FMSC.ORM.Core;
using FMSC.ORM.Core.SQL;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.SQLite;

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
                if(Path == IN_MEMORY_DB_PATH)
                {
                    throw new NotImplementedException();
                }

                return System.IO.File.Exists(this.Path);
            }
        }

        /// <summary>
        /// Gets the extension of the file (ex. ".___")
        /// </summary>
        public string Extension
        {
            get { return System.IO.Path.GetExtension(base.Path); }
        }


        /// <summary>
        /// creates instance representing an in memory database
        /// </summary>
        public SQLiteDatastore() : this(IN_MEMORY_DB_PATH)
        { }

        public SQLiteDatastore(string path) : base(new SQLiteProviderFactory())
        {
            Path = path;
        }

        #region abstract methods
        protected override string BuildConnectionString(bool readOnly)
        {
            return string.Format("Data Source={0};Version=3;Read Only={1};", Path, readOnly);
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

        public void AddField(string tableName, string fieldDef)
        {
            string command = string.Format("ALTER TABLE {0} ADD COLUMN {1};", tableName, fieldDef);
            this.Execute(command);
 
        }

        public bool CheckTableExists(string tableName)
        {
            return GetRowCount("sqlite_master", "WHERE type = 'table' AND name = ?", tableName) > 0;
        }

        public bool CheckFieldExists(string tableName, string field)
        {
            try
            {
                this.Execute(String.Format("SELECT {0} FROM {1};", field, tableName));
                return true;
            }
            catch
            {
                return false;
            }
        }

        public override List<ColumnInfo> GetTableInfo(string tableName)
        {
            List<ColumnInfo> colList = new List<ColumnInfo>();
            lock (_readOnlyConnectionSyncLock)
            {

                using (DbCommand command = Provider.CreateCommand("PRAGMA table_info(" + tableName + ");"))
                {
                    using (DbConnection conn = OpenReadOnlyConnection())
                    {
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
                                    ColumnInfo colInfo = new ColumnInfo();
                                    colInfo.Name = reader.GetString(nameOrd);
                                    colInfo.DBType = reader.GetString(dbTypeOrd);
                                    colInfo.IsPK = reader.GetBoolean(pkOrd);
                                    colInfo.IsRequired = reader.GetBoolean(notNullOrd);
                                    if (!reader.IsDBNull(defaultValOrd))
                                    {
                                        colInfo.Default = reader.GetString(defaultValOrd);
                                    }
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
                            ReleaseReadOnlyConnection();
                        }
                    }
                }
                return colList;
            }
        }

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
            lock (_readOnlyConnectionSyncLock)
            {
                using (DbCommand command = Provider.CreateCommand(comStr))
                {
                    using (DbConnection conn = OpenReadOnlyConnection())
                    {
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
                            ReleaseReadOnlyConnection();
                        }
                    }
                }
                return hasErrors;
            }
        }

        public override Int64 GetRowCount(string tableName, string selection, params Object[] selectionArgs)
        {
            string query = string.Format("SELECT Count(1) FROM {0} {1};", tableName, selection);
            return ExecuteScalar<Int64>(query, selectionArgs);
        }

        public string GetTableSQL(String tableName)
        {
            return (String)this.ExecuteScalar("SELECT sql FROM Sqlite_master WHERE name = ? COLLATE NOCASE and type = 'table';", tableName);
        }

        public string[] GetTableUniques(String tableName)
        {
            String tableSQL = this.GetTableSQL(tableName);
            System.Text.RegularExpressions.Match match =
                System.Text.RegularExpressions.Regex.Match(tableSQL, @"(?<=^\s+UNIQUE\s\()[^\)]+(?=\))", System.Text.RegularExpressions.RegexOptions.Multiline);
            if (match != null && match.Success)
            {
                String[] a = match.Value.Split(new char[] { ',', ' ', '\r', '\n' });
                int numNotEmpty = 0;
                foreach (string s in a)
                {
                    if (!string.IsNullOrEmpty(s))
                    {
                        numNotEmpty++;
                    }
                }
                string[] b = new string[numNotEmpty];
                for (int i = 0, j = 0; i < a.Length; i++)
                {
                    if (!string.IsNullOrEmpty(a[i]))
                    {
                        b[j] = a[i];
                        j++;
                    }
                }

                return b;
                //return match.Value.Split(new char[]{',',' ','\r','\n'},  StringSplitOptions.RemoveEmptyEntries);

            }
            return new string[0];

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

        /// <summary>
        /// Creates copy at location, and changes database path to new location
        /// </summary>
        /// <param name="desPath"></param>
        public bool CopyAs(string desPath)
        {
            ReleaseAllConnections(true);
            try
            {
                System.IO.File.Copy(this.Path, desPath);
                //_DBFileInfo.CopyTo(desPath);
                this.Path = desPath;
                //this._DBFileInfo = new FileInfo(desPath);
                //_ConnectionString = BuildConnectionString(false);
            }
            catch (Exception)
            {
                return false;
            }

            return true;
        }

        

        public bool MoveTo(string path)
        {
            ReleaseAllConnections(true);
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
                            if (innerException.Message.IndexOf("UNIQUE constraint failed") >= 0)
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
