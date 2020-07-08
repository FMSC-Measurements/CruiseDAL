using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CruiseDAL.Util
{
    public interface IEnvironmentInfoProvider
    {
        string UserInfo { get; }
    }


    public abstract class EnvironmentInfoProvider
    {
        private static EnvironmentInfoProvider _instance;

        public static EnvironmentInfoProvider Instance
        {
            get => _instance ?? (_instance = new DefaultEnvironmentInfoProvider());
            set => _instance = value;
        }

        public abstract string UserInfo { get; }
    }

    public class DefaultEnvironmentInfoProvider : EnvironmentInfoProvider
    {
        public override string UserInfo => GetUserName();

        /// <summary>
        /// Get the username
        /// </summary>
        /// <returns>Whatever has been entered into (Settings > Owner Information > Identification > Name)</returns>
        private static string GetUserName()
        {
#if WindowsCE
            return GetMachineDescription() + "|" + GetMachineName();
            //FMSC.Utility.MobileDeviceInfo di = new FMSC.Utility.MobileDeviceInfo();
#elif ANDROID
			return "AndroidUser";
#else //full framework
            return Environment.UserName + " on " + System.Environment.MachineName;
#endif
        }


#if WindowsCE
                /// <summary>
        /// This should be where the user enters the device's serial number, e.g. "AllegroMX_77404"
        /// </summary>
        /// <returns>Whatever has been entered into (Settings > System > About > Device ID > Device Name)</returns>
        public static string GetMachineName()
        {
            string str = "<Unknown>";

            try
            {
                var key = Microsoft.Win32.Registry.LocalMachine.OpenSubKey("Ident");
                str = key.GetValue("Name").ToString();                
                key.Close();
            }
            catch
            {
            }

            return str;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns>Whatever has been entered into (Settings > System > About > Device ID > Description)</returns>
        public static string GetMachineDescription()
        {
            string str = "<Unknown>";

            try
            {
                var key = Microsoft.Win32.Registry.LocalMachine.OpenSubKey("Ident");
                str = key.GetValue("Desc").ToString();
                key.Close();
            }

            catch
            {
            }

            return str;
        }
#endif
    }
}
