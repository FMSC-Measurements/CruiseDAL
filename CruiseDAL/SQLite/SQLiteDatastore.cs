using CruiseDAL.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CruiseDAL.SQLite
{
    public class SQLiteDatastore : DatastoreRedux
    {
        /// <summary>
        /// Gets the extension of the file (ex. ".___")
        /// </summary>
        public string Extension
        {
            get { return System.IO.Path.GetExtension(base.Path); }
        }

        public bool Exists
        {
            get
            {
                return System.IO.File.Exists(this.Path);
            }
        }

        #region File utility methods
        /// <summary>
        /// Copies entire file to <paramref name="path"/> Overwriting any existing file
        /// </summary>
        /// <param name="path"></param>
        public DAL CopyTo(string path)
        {
            return this.CopyTo(path, false);
        }

        public DAL CopyTo(string destPath, bool overwrite)
        {
            ReleaseConnection();

            System.IO.File.Copy(this.Path, destPath, overwrite);
            //_DBFileInfo.CopyTo(destPath, overwrite);
            return new DAL(destPath);
        }

        /// <summary>
        /// Creates copy at location, and changes database path to new location
        /// </summary>
        /// <param name="desPath"></param>
        public bool CopyAs(string desPath)
        {
            ReleaseConnection();
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
            this.ReleaseConnection();
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
