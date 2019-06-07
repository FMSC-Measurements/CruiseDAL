using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CruiseDAL.EntityAttributes
{
    public abstract class UserInfoProvider
    {
        private static UserInfoProvider _instance;

        public static UserInfoProvider Instance
        {
            get => _instance ?? (_instance = new DefaultrUserInfoProvider());
            set => _instance = value;
        }

        public abstract string UserInfo { get; }
    }

    public class DefaultrUserInfoProvider : UserInfoProvider
    {
        public override string UserInfo => GetUserName();

        /// <summary>
        /// Get the username
        /// </summary>
        /// <returns>Whatever has been entered into (Settings > Owner Information > Identification > Name)</returns>
        private static string GetUserName()
        {
#if NetCF
            return FMSC.Util.DeviceInfo.GetMachineDescription() + "|" + FMSC.Util.DeviceInfo.GetMachineName();
            //FMSC.Utility.MobileDeviceInfo di = new FMSC.Utility.MobileDeviceInfo();
#elif ANDROID
			return "AndroidUser";
#else //full framework
            return Environment.UserName + " on " + System.Environment.MachineName;
#endif
        }
    }
}
