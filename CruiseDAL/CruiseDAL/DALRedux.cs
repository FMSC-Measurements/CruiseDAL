using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CruiseDAL.CruiseDAL
{
    public class DALRedux
    {
        /// <summary>
        /// Gets the extension of the file (ex. ".___")
        /// </summary>
        public string Extension
        {
            get { return System.IO.Path.GetExtension(base.Path); }
        }


        /// <summary>
        /// Get the schema version
        /// </summary>
        private string _databaseVersion;
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
        public string User
        {
            get
            {
                if (_userInfo == null)
                {
                    _userInfo = this.GetUserInformation();
                }
                return _userInfo;
            }
        }


        public void LogMessage(string message, string level);

        protected abstract void BuildDBFile();

        protected abstract string GetCreateSQL();

        protected abstract string GetUserInformation();

    }
}
