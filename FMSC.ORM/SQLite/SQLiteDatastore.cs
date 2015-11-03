using FMSC.ORM.Core;
using FMSC.ORM.Core.SQL;
using System;
using System.Collections.Generic;
using System.Data.Common;

namespace FMSC.ORM.SQLite
{
    public class SQLiteDatastore : DatastoreRedux
    {





        /// <summary>
        /// 
        /// </summary>
        public bool Exists
        {
            get
            {
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

        public List<ColumnInfo> GetTableInfo(string tableName)
        {
            return Context.GetTableInfo(tableName);
        }

        public bool HasForeignKeyErrors(string table_name)
        {
            return Context.HasForeignKeyErrors(table_name);
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
            Context.ReleaseAllConnections(true);
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
            Context.ReleaseAllConnections(true);
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

    }
}
