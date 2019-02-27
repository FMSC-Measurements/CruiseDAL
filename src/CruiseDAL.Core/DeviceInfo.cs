using System.Text;
using Microsoft.Win32;
using System;

namespace FMSC.Util
{
    public static class DeviceInfo
    {  
        public static string UserName
        {
            get
            {
                return GetUserName();
            }
        }

        public static string SerialNumber
        {
            get
            {
                return GetSerialNumber();
            }
        }

        public static string MachineName
        {
            get
            {
                return GetMachineName();
            }
        }


        #region Device Info methods

        private static string GetSerialNumber()
        {
            string machineName = GetMachineName();
            string serialNumber = "<Unknown>";
            // Try and glean the serial number (it might be in the machine name).
            if (machineName != "<Unknown>")
            {
                System.Text.RegularExpressions.Match match=
                    System.Text.RegularExpressions.Regex.Match(machineName, @"\s+[a-fA-F]*[0-9]+$");//find number of hex value
                if(match != null && match.Success && 
                    (match.Length < 5 || match.Length > 10))// Serial numbers should be between 5 and 8 digits (Juniper's seem to be all 5 digits).
                {
                    serialNumber = match.Value;
                }
                else
                {
                    serialNumber = "<Unknown>";
                }
            }
            return serialNumber;
        }

        /// <summary>
        /// Get the username
        /// </summary>
        /// <returns>Whatever has been entered into (Settings > Owner Information > Identification > Name)</returns>
        private static string GetUserName()
        {
#if NET40
            try
            {
                RegistryKey key = Registry.CurrentUser.OpenSubKey(@"ControlPanel\Owner");
                if (key == null) { return "<Unknown>"; }

                var userName = (string)key.GetValue("Name", string.Empty);
                if (userName != string.Empty)
                {
                    return userName;
                }

                byte[] bytes = (byte[])key.GetValue("Owner", null);

                if (bytes != null)
                {
                    userName = Encoding.Unicode.GetString(bytes, 0, 0x48);
                    userName = userName.Substring(0, userName.IndexOf("\0"));
                    return userName;
                }
                else
                { return "<Unknown>"; }
            }
            catch
            {
                return "<Unknown>";
            }
#else
            return Environment.UserName;
#endif
        }

        /// <summary>
        /// This should be where the user enters the device's serial number, e.g. "AllegroMX_77404"
        /// </summary>
        /// <returns>Whatever has been entered into (Settings > System > About > Device ID > Device Name)</returns>
        public static string GetMachineName()
        {
#if NET40
            try
            {
                RegistryKey key = Registry.LocalMachine.OpenSubKey("Ident");
                var machineName = key.GetValue("Name").ToString();                
                key.Close();
                return machineName;
            }
            catch
            {
                return "<Unknown>";
            }
#else
            return Environment.MachineName;
#endif
        }

#if NetCF
        /// <summary>
        /// 
        /// </summary>
        /// <returns>Whatever has been entered into (Settings > System > About > Device ID > Description)</returns>
        public static string GetMachineDescription()
        {
            string str = "<Unknown>";

            try
            {
                RegistryKey key = Registry.LocalMachine.OpenSubKey("Ident");
                str = key.GetValue("Desc").ToString();
                key.Close();
            }

            catch
            {
            }

            return str;
        }
#endif

#endregion
    }
}
