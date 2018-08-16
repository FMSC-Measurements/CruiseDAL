using FMSC.ORM.Core;
using FMSC.ORM.Core.SQL;
using Backpack.SqlBuilder;
using Backpack.SqlBuilder.Dialects;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Text;

namespace FMSC.ORM.SQLite
{
    public class SQLiteDatastore : DatastoreRedux
    {
        protected const string IN_MEMORY_DB_PATH = ":memory:";

        /// <summary>
        /// Gets value indicating if database file exists
        /// </summary>
        public bool Exists
        {
            get
            {
                if (IsInMemory)
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
                return System.IO.Path.GetExtension(Path);
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

#if MICROSOFT_DATA_SQLITE
        public SQLiteDatastore(string path) : base(new SqliteDialect(), new SqliteExceptionProcessor(), Microsoft.Data.Sqlite.SqliteFactory.Instance)
#elif SYSTEM_DATA_SQLITE
        public SQLiteDatastore(string path) : base(new SqliteDialect(), new SqliteExceptionProcessor(), System.Data.SQLite.SQLiteFactory.Instance)
#endif
        {
            if (path == null) { throw new ArgumentNullException("path"); }
            Path = path;
        }

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

#endregion SavePoints

        protected override string BuildConnectionString()
        {
            if(Path == null) { throw new InvalidOperationException("Path can not be null"); }

#if SYSTEM_DATA_SQLITE
            return string.Format("Data Source={0};Version=3;", Path);
#else
            return string.Format("Data Source={0};", Path);
#endif
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
            if (this.ExecuteScalar<int>("SELECT count(*) FROM sqlite_sequence WHERE name = @p1;", tableName) >= 1)
            {
                commandText = "UPDATE sqlite_sequence SET seq = @p1 WHERE name = @p2";
            }
            else
            {
                commandText = "INSERT INTO sqlite_sequence  (seq, name) VALUES (@p1, @p2);";
            }

            this.Execute(commandText, start, tableName);
        }

        /// <summary>
        /// Checks if table exists.
        /// </summary>
        /// <param name="tableName">Name of the table.</param>
        /// <returns></returns>
        public bool CheckTableExists(string tableName)
        {
            return GetRowCount("sqlite_master", "WHERE type = 'table' AND name = @p1", tableName) > 0;
        }

        /// <summary>
        /// Checks if table has a given field
        /// </summary>
        /// <param name="tableName">Name of the table.</param>
        /// <param name="field">field name.</param>
        /// <returns></returns>
        public bool CheckFieldExists(string tableName, string field)
        {
            field = field.Trim();
            foreach (var col in GetTableInfo(tableName))
            {
                if (field.Equals(col.Name, StringComparison.InvariantCultureIgnoreCase)) { return true; }
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
            sb.AppendLine("TABLE " + tableName + "\r\n");
            using (var enu = cols.GetEnumerator())
            {
                if (!enu.MoveNext()) { throw new ArgumentException("cols can't be empty", "cols"); }
                sb.Append("( " + enu.Current.ToString());

                while (enu.MoveNext())
                {
                    sb.Append("," + "\r\n" + enu.Current.ToString());
                }
                sb.Append(")");
            }
            sb.Append(";");
            return sb.ToString();
        }

        /// <summary>
        /// Gets the table column information.
        /// </summary>
        /// <param name="tableName">Name of the table.</param>
        /// <returns></returns>
        public override IEnumerable<ColumnInfo> GetTableInfo(string tableName)
        {
            var colList = new List<ColumnInfo>();
            lock (_persistentConnectionSyncLock)
            {
                DbConnection conn = OpenConnection();

                try
                {
                    var commandText = "PRAGMA table_info(" + tableName + ");";

                    using (var reader = conn.ExecuteReader(commandText, (object[])null, CurrentTransaction))
                    {
                        int nameOrd = reader.GetOrdinal("name");
                        int dbTypeOrd = reader.GetOrdinal("type");
                        int pkOrd = reader.GetOrdinal("pk");
                        int notNullOrd = reader.GetOrdinal("notnull");
                        int defaultValOrd = reader.GetOrdinal("dflt_value");

                        try
                        {
                            while (reader.Read())
                            {
                                var colInfo = new ColumnInfo()
                                {
                                    Name = reader.GetString(nameOrd),
                                    DBType = SqlDialect.MapSQLtypeToDbType(reader.GetString(dbTypeOrd)),
                                    IsPK = reader.GetBoolean(pkOrd),
                                    NotNull = reader.GetBoolean(notNullOrd),
                                    Default = (!reader.IsDBNull(defaultValOrd)) ? reader.GetString(defaultValOrd) : null
                                };
                                colList.Add(colInfo);
                            }
                        }
                        catch (Exception e)
                        {
                            throw ExceptionProcessor.ProcessException(e, conn, commandText, (IDbTransaction)null);
                        }
                    }
                }
                finally
                {
                    ReleaseConnection();
                }

                return colList;
            }
        }

        public override long GetLastInsertRowID(DbConnection connection, DbTransaction transaction)
        {
            return connection.ExecuteScalar<long>("SELECT last_insert_rowid()", (object[])null, transaction);
        }

        public override object GetLastInsertKeyValue(DbConnection connection, String tableName, String fieldName, DbTransaction transaction)
        {
            var ident = GetLastInsertRowID(connection, transaction);

            var query = "SELECT " + fieldName + " FROM " + tableName + " WHERE rowid = @p1;";

            return connection.ExecuteScalar(query, new object[] { ident }, transaction);

            //String query = "Select " + fieldName + " FROM " + tableName + " WHERE rowid = last_insert_rowid();";
        }

        /// <summary>
        /// Gets the raw SQL that defines a given table.
        /// </summary>
        /// <param name="tableName">Name of the table.</param>
        /// <returns></returns>
        public string GetTableSQL(String tableName)
        {
            return (String)this.ExecuteScalar("SELECT sql FROM Sqlite_master WHERE name = @p1 COLLATE NOCASE and type = 'table';", tableName);
        }

        /// <summary>
        /// Determines whether the specified table has foreign key errors.
        /// </summary>
        /// <param name="table_name">The table_name.</param>
        /// <returns></returns>
        public override bool HasForeignKeyErrors(string table_name)
        {
            bool hasErrors = false;
            string commandText;
            if (string.IsNullOrEmpty(table_name))
            {
                commandText = "PRAGMA foreign_key_check;";
            }
            else
            {
                commandText = "PRAGMA foreign_key_check(" + table_name + ");";
            }
            lock (_persistentConnectionSyncLock)
            {
                var connection = OpenConnection();
                try
                {
                    using (var reader = connection.ExecuteReader(commandText, (object[])null, CurrentTransaction))
                    {
                        hasErrors = reader.Read();
                    }
                }
                finally
                {
                    ReleaseConnection();
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

        [Obsolete]
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

#endregion File utility methods
    }
}