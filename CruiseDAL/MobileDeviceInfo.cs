using System;
using Microsoft.Win32;
using System.Text;
using System.Runtime.InteropServices;

//using System.Windows.Forms; /// temp

// So what do we really need?
// Windows CE version
// Manufacturer
// Model
// User Name
// Device Name
// Device Serial Number!

// Spreadsheet with information from available devices:
// https://docs.google.com/spreadsheet/ccc?key=0Ag56Xk8n1aJGdGMxaXBPdlA2dG1EQkQzcnRHM2VPLWc

public class MobileDeviceInfo
{
    public string platform;
    public int OSMajorVersion;
    public int OSMinorVersion;
    public int OSBuild;
    public int OSRevision;
    public string OSVersion;
    public string userName;
    public string machineName;
    public string machineDescription;
    private string platformType;
    public string betterPlatformType;
    public string OEMInfo;
    public string platformName;
    public string platformManufacturer;
    public string allInfoAsReport;
    public string serialNumber;
    public string deviceModel;
    public uint physicalMemoryLoadPercent; ////
    public uint mainBatteryLifePercent; ////
    public uint backupBatteryLifePercent; ////

    
    public MobileDeviceInfo()
	{
        GatherRawDataFromDevice();
        AssembleUsefulInformationFromRawData();
        physicalMemoryLoadPercent = GetPhysicalMemoryLoadPercent();
        mainBatteryLifePercent = GetMainBatteryChargePercent();
        backupBatteryLifePercent = GetBackupBatteryChargePercent();
	}

    
    private void GatherRawDataFromDevice()
    {
        platform = Environment.OSVersion.Platform.ToString(); // I think this will always be "WinCE"
        OSMajorVersion = Environment.OSVersion.Version.Major; // e.g. the first 5 in 5.2.19959
        OSMinorVersion = Environment.OSVersion.Version.Minor; // e.g. the 2 in 5.2.19959
        OSBuild = Environment.OSVersion.Version.Build;        // e.g. the 19959
        OSRevision = Environment.OSVersion.Version.Revision;  // this usually returns -1
        OSVersion = Environment.OSVersion.Version.ToString(); // e.g. the whole 5.2.19959
        userName = GetUserName(); // Whatever has been entered into (Settings > Owner Information > Identification > Name)
        machineName = GetMachineName(); // Whatever has been entered into (Settings > System > About > Device ID > Device Name)
        machineDescription = GetMachineDescription(); // Whatever has been entered into (Settings > System > About > Device ID > Description)
        platformType = GetPlatformType();
        OEMInfo = GetOEMInfo();
        platformName = GetPlatformName();
        platformManufacturer = GetPlatformManufacturer();
    }


    private void AssembleUsefulInformationFromRawData()
    {
        // Get Pocket PC or Windows Mobile version
        betterPlatformType = GetBetterPlatformType();

        // Get better platform manufacturer
        GetBetterPlatformManufacturer();

        // Try and glean the serial number.
        GetSerialNumber();

        // Try and glean the device model.
        GetDeviceModel(); 
    }


    private void GetBetterPlatformManufacturer()
    {
        // Get better platform manufacturer
        if (platformManufacturer == "<Unknown>")
        {
            if (Contains(OEMInfo,"Juniper") || Contains(OEMInfo,"ARCHER"))
                platformManufacturer = "Juniper Systems";

            if (Contains(OEMInfo, "Juno") || Contains(OEMInfo,"Trimble") || Contains(OEMInfo,"Ranger"))
                platformManufacturer = "Trimble";

            // etc., as necessary ...
        }
    }

    private static bool Contains(String s, String value)
    {
        return s.IndexOf(value, StringComparison.Ordinal) >= 0;
    }

    private void GetSerialNumber()
    {
        // Try and glean the serial number (it might be in the machine name).
        if (machineName != "<Unknown>")
        {
            // Parse string backwards for first contiguous string of numbers.
            bool started = false;
            for (int i = machineName.Length -1; i > -1; i--)
            {
                if (machineName[i] > 47 && machineName[i] < 58) // then it's a digit 0 through 9
                {
                    serialNumber += machineName[i];
                    started = true;
                }
                else if (machineName[1] > 64 && machineName[i] < 71) // then it's A through F (Trimble and TDS use hexidecimal seral numbers)
                {
                    serialNumber += machineName[i];
                    started = true;
                }
                else
                {
                    if (started)
                    {
                        break;
                    }
                }
            }
            
            if (serialNumber != null)
            {
                // reverse the string
                char[] charArray = serialNumber.ToCharArray();
                Array.Reverse(charArray);
                serialNumber = new string(charArray);

                // Serial numbers should be between 5 and 8 digits (Juniper's seem to be all 5 digits).
                if (serialNumber.Length < 5 || serialNumber.Length > 10)
                    serialNumber = "<Unknown>";
            }
            else
            {
                serialNumber = "<Unknown>";
            }
        }
    }



    private void GetDeviceModel()
    {
        // Try and glean the device model (e.g. Allegro MX) (it might be in the machine name, platform type or platform name).
        if (platformType != "PocketPC")
        {
            // If it doesn't have the generic "PocketPC", then it is probably the specific model name.
            // For example, platform type is "Allegro CX" on the Allegro CX. 
            deviceModel = platformType;
        }
        else
        {
            // Now the fun begins ... where is the model name?
            // (Check PlatformName first. It will either be the model or <Unknown>.)
            if (platformName != "<Unknown>" && platformName.Length > 0)
            {
                deviceModel = platformName;
            }
            else if (OEMInfo.Length > 0)
            {
                deviceModel = OEMInfo;
            }
            else
            {
                deviceModel = "<Unknown>";
            }
        }
    }


/// <summary>
    /// All information as a multi-line report string.
    /// (This is useful for finding out what information is available on a particular device
    //  and where it is ... for future parsing.)
/// </summary>
/// <returns>A multi-line report string of raw and calculated values.</returns>
    public string GetAllInfoAsMultiLineReportString()
    {
        string returnString = string.Empty;

        returnString += "\nVersion: " + OSVersion;
        returnString += "\nUserName: " + userName;
        returnString += "\nMachine Name: " + machineName;
        returnString += "\nMachine Description: " + machineDescription;
        returnString += "\nPlatform Type: " + platformType;
        returnString += "\nBetter Platform Type: " + betterPlatformType;
        returnString += "\nOEM Info: " + OEMInfo;
        returnString += "\nPlatform Name: " + platformName;
        returnString += "\nPlatform Manufacturer: " + platformManufacturer;
        returnString += "\nDevice Model: " + deviceModel;
        returnString += "\nDevice Serial Number: " + serialNumber;

        return returnString;
    }


    public string GetModelAndSerialNumber()
    {
        if (serialNumber == "<Unknown>")
            return deviceModel + " " + machineName; // (can't find serial number, hopefully machineName is unique).
        else
            return deviceModel + " " + serialNumber;
    }


/// <summary>
/// Get the username
/// </summary>
    /// <returns>Whatever has been entered into (Settings > Owner Information > Identification > Name)</returns>
    public static string GetUserName()
    {
        string str = "<Unknown>";

        try
        {
            RegistryKey key = Registry.CurrentUser.OpenSubKey(@"ControlPanel\Owner");
            if (key == null) { return str; }

            str = (string)key.GetValue("Name", string.Empty);
            if (str != string.Empty)
            {
                return str;
            }

            byte[] bytes = (byte[])key.GetValue("Owner", null);

            if (bytes != null)
            {
                str = Encoding.Unicode.GetString(bytes, 0, 0x48);
                str = str.Substring(0, str.IndexOf("\0"));
            }
        }
        catch
        {
        }

        return str; 
    }

/// <summary>
/// This should be where the user enters the device's serial number, e.g. "AllegroMX_77404"
/// </summary>
    /// <returns>Whatever has been entered into (Settings > System > About > Device ID > Device Name)</returns>
    public static string GetMachineName()
    {
        string str = "<Unknown>";

        try
        {
            RegistryKey key = Registry.LocalMachine.OpenSubKey("Ident");
            str = key.GetValue("Name").ToString();
            key.Close();
        }

        catch
        {
            throw new PlatformNotSupportedException();
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
            RegistryKey key = Registry.LocalMachine.OpenSubKey("Ident");
            str = key.GetValue("Desc").ToString();
            key.Close();
        }

        catch
        {
            throw new PlatformNotSupportedException();
        }

        return str;
    }

    private const int SPI_GETPLATFORMTYPE = 257;
    private const int SPI_GETOEMINFO = 258;
    private const int SPI_GETPLATFORMNAME = 260;
    private const int SPI_GETPLATFORMMANUFACTURER = 262;

    [DllImport("coredll.dll", EntryPoint = "SystemParametersInfo", SetLastError = true)]

    private static extern int SystemParametersInfoString(int uiAction, int uiParam, StringBuilder pvParam, int fWinIni);

    
    public static string GetPlatformType()
    {
        try
        {
            StringBuilder sb = new StringBuilder(256);

            if (SystemParametersInfoString(SPI_GETPLATFORMTYPE, sb.Capacity, sb, 0) != 0)
            {
                return sb.ToString();
            }
        }
        catch
        {
        }

        return "<Unknown>";
    }


    public static string GetOEMInfo()
    {
        try
        {
            StringBuilder sb = new StringBuilder(256);

            if (SystemParametersInfoString(SPI_GETOEMINFO, sb.Capacity, sb, 0) != 0)
            {
                return sb.ToString();
            }
        }
       catch
        {
        }

        return "<Unknown>";
    }


    public static string GetPlatformName()
    {
        try
        {
            StringBuilder sb = new StringBuilder(256);

            if (SystemParametersInfoString(SPI_GETPLATFORMNAME, sb.Capacity, sb, 0) != 0)
            {
                return sb.ToString();
            }
        }
        catch
        {
        }

        return "<Unknown>";
    }

    /// <summary>
    /// Returns Platform Manager, e.g. "Juniper Systems" or "Unknown"
    /// </summary>
    /// <returns></returns>
    public static string GetPlatformManufacturer()
    {
        try
        {
            StringBuilder sb = new StringBuilder(256);

            if (SystemParametersInfoString(SPI_GETPLATFORMMANUFACTURER, sb.Capacity, sb, 0) != 0)
            {
                return sb.ToString();
            }
        }
        catch
        {
        }

        return "<Unknown>";
    }

    // ToDo: Find the OSVersion for newer devices such as the Mesa and Windows Phone 7's
    public string GetBetterPlatformType()
    {
        if (OSMajorVersion == 3)
        {
            if (platformType == "PocketPC")
                return "Pocket PC 2002";

            return "Pocket PC 2000";
        }
        else if (OSMajorVersion == 4)
        {
            if (platformType == "PocketPC")
                return "Pocket PC 2003";

          //  return "<cannot determine>"; // Allegro CX falls here (CE .NET 4.2).
            return "CE .NET 4." + OSMinorVersion;
        }
        else if (OSMajorVersion == 5)
        {
            if (OSMinorVersion == 1)
                return "Windows Mobile 5";
            else if (OSMinorVersion == 2)
                return "Windows Mobile 6";

            return "<cannot determine>";
        }
        else
        {
            return "<cannot determine>";
        }
    }

////////////////////////////////////////////////////////////////////////////
    // from http://msdn.microsoft.com/en-us/library/ms172518(v=vs.80).aspx
    // I'm just going to report "Memory Load" which is the % physical memory in use.
    // (not going to worry about virtual memory, page file, etc)
    // http://www.windowsfordevices.com/c/a/Windows-For-Devices-Articles/What-is-virtual-memory/

    public struct MEMORYSTATUS
    {
        public UInt32 dwLength;
        public UInt32 dwMemoryLoad;
        public UInt32 dwTotalPhys;
        public UInt32 dwAvailPhys;
        public UInt32 dwTotalPageFile;
        public UInt32 dwAvailPageFile;
        public UInt32 dwTotalVirtual;
        public UInt32 dwAvailVirtual;
    }

    [DllImport("CoreDll.dll")]
    public static extern void GlobalMemoryStatus
    (
        ref MEMORYSTATUS lpBuffer
    );

    [DllImport("CoreDll.dll")]
    public static extern int GetSystemMemoryDivision
    (
        ref UInt32 lpdwStorePages,
        ref UInt32 lpdwRamPages,
        ref UInt32 lpdwPageSize
    );

    public uint GetPhysicalMemoryLoadPercent()
    {
        //UInt32 storePages = 0;
        //UInt32 ramPages = 0;
        //UInt32 pageSize = 0;
        //int res = GetSystemMemoryDivision(ref storePages, 
        // ref ramPages, ref pageSize);

        // Call the native GlobalMemoryStatus method
        // with the defined structure.
        MEMORYSTATUS memStatus = new MEMORYSTATUS();
        GlobalMemoryStatus(ref memStatus);

        return memStatus.dwMemoryLoad;
    }


    public uint GetAppVirtualMemoryLoadPercent()
    {
        MEMORYSTATUS memStatus = new MEMORYSTATUS();
        GlobalMemoryStatus(ref memStatus);
        return memStatus.dwAvailVirtual / memStatus.dwTotalVirtual;
    }

///////////////////////////////////////////////////////////////////
    // http://msdn.microsoft.com/en-us/library/aa457088.aspx

    public class SYSTEM_POWER_STATUS_EX
    {
        public byte ACLineStatus;
        public byte BatteryFlag;
        public byte BatteryLifePercent;
        public byte Reserved1;
        public uint BatteryLifeTime;
        public uint BatteryFullLifeTime;
        public byte Reserved2;
        public byte BackupBatteryFlag;
        public byte BackupBatteryLifePercent;
        public byte Reserved3;
        public uint BackupBatteryLifeTime;
        public uint BackupBatteryFullLifeTime;
    }

    [DllImport("coredll")]
    public static extern uint GetSystemPowerStatusEx
    (
        SYSTEM_POWER_STATUS_EX lpSystemPowerStatus, 
        bool fUpdate
    );

    public uint GetMainBatteryChargePercent()
    {
        SYSTEM_POWER_STATUS_EX status = new SYSTEM_POWER_STATUS_EX();

        if(GetSystemPowerStatusEx(status, false) == 1)
        {
            return status.BatteryLifePercent;
        }

        return 0;
    }

    public uint GetBackupBatteryChargePercent()
    {
        SYSTEM_POWER_STATUS_EX status = new SYSTEM_POWER_STATUS_EX();

        if (GetSystemPowerStatusEx(status, false) == 1)
        {
            uint backupBatteryPercent = status.BackupBatteryLifePercent;

            return status.BackupBatteryLifePercent; // careful. returns 255 if no backup battery
        }

        return 0;
    }



        //public string GetDeviceStorageSpace()
        //{

        //}
}
