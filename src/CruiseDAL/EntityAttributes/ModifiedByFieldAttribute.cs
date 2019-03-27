using FMSC.ORM.EntityModel.Attributes;
using System;


namespace CruiseDAL.EntityAttributes
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
                    _userInfo = UserInfoProvider.Instance.UserInfo;
                }
                return _userInfo;
            }

            set
            {
                throw new NotSupportedException();
            }
        }
    }
}
