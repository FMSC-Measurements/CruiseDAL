
using CruiseDAL.SQLite;
using System;

namespace CruiseDAL.CruiseDAL
{
    public class DALRedux : SQLiteDatastore
    {



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
                    _userInfo = GetUserInformation();
                }
                return _userInfo;
            }
        }

        /// <summary>
        /// Creates a DAL instance for a database @ path. 
        /// </summary>
        /// <exception cref="ArgumentNullException">path can not be null or an impty string</exception>
        /// <exception cref="IOException">problem working with file. wrong extension</exception>
        /// <exception cref="FileNotFoundException"
        /// <param name="path"></param>
        public DALRedux(string path) : this(path, false)
        {
        }

        /// <exception cref="System.IO.IOException">File extension is not valid <see cref="VALID_EXTENSIONS"/></exception>
        /// <exception cref="System.UnauthorizedAccessException">File open in another application or thread</exception>
        public DALRedux(string path, bool makeNew)
        {
            Path = path;
            _ConnectionString = BuildConnectionString(false);

            base.InitializeBase();

            if (makeNew)
            {
                Create();
            }
            else if (!makeNew && !Exists)
            {
                throw new FileNotFoundException();
            }

            //allow null or empty path to create in memory DB?
            //if (Exists)
            //{
            //    this.Initialize();
            //}
            this.Initialize();
            Log.V(String.Format("Created DAL instance. Path = {0},ConnectionString = {1} User = {2}\r\n", Path, _ConnectionString, User));
        }

        ~DAL()
        {
            this.Dispose(false);
        }


        public void LogMessage(string message, string level);

        protected abstract void BuildDBFile();

        protected abstract string GetCreateSQL();

        protected static string GetUserInformation()
        {
#if Mobile

            return FMSC.Util.DeviceInfo.GetMachineDescription() + "|" + FMSC.Util.DeviceInfo.GetMachineName();
            //FMSC.Utility.MobileDeviceInfo di = new FMSC.Utility.MobileDeviceInfo();
            //return di.GetModelAndSerialNumber();
            //return "Mobile User";
#elif FullFramework
            return Environment.UserName + " on " + System.Environment.MachineName;
#elif ANDROID
			return "AndroidUser";
#endif
            //return Environment.UserName + " on " + System.Windows.Forms.SystemInformation.ComputerName;
        }

    }
}
