using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CruiseDAL.Core.EntityAttributes
{
    public class CreatedByFieldAttribute : InfrastructureFieldAttribute
    {

        public CreatedByFieldAttribute()
            : base("CreatedBy")
        {
            PersistMode = PersistMode.OnInsert;
        }

        private static string _userInfo;

        public override object DefaultValue
        {
            get
            {
                if (_userInfo == null)
                {
                    _userInfo = GetUserInformation();
                }
                return _userInfo;
            }

            set
            {
                throw new NotImplementedException();
            }
        }

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
