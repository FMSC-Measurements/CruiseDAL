﻿using System;


namespace FMSC.ORM.Core.EntityAttributes
{
    public class ModifiedByFieldAttribute : InfrastructureFieldAttribute
    {
        public ModifiedByFieldAttribute() : base("ModifiedBy")
        {
            PersistMode = PersistMode.OnUpdate;
        }

        private static string _userInfo;

        public override object DefaultValue
        {
            get
            {
                if(_userInfo == null)
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
#if NetCF
            throw new NotImplementedException();
            //return FMSC.Util.DeviceInfo.GetMachineDescription() + "|" + FMSC.Util.DeviceInfo.GetMachineName();
            //FMSC.Utility.MobileDeviceInfo di = new FMSC.Utility.MobileDeviceInfo();
            //return di.GetModelAndSerialNumber();
            //return "Mobile User";
#elif ANDROID
			return "AndroidUser";
#else //full framework
            return Environment.UserName + " on " + System.Environment.MachineName;
#endif
            //return Environment.UserName + " on " + System.Windows.Forms.SystemInformation.ComputerName;

        }

    }
}