using FMSC.ORM.EntityModel.Attributes;
using System;

namespace CruiseDAL.EntityAttributes
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