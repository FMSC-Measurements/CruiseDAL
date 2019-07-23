using FMSC.ORM.EntityModel.Attributes;
using System;


namespace CruiseDAL.EntityAttributes
{
    public class ModifiedByFieldAttribute : InfrastructureFieldAttribute
    {
        public ModifiedByFieldAttribute() : base("ModifiedBy")
        {
            PersistanceFlags = PersistanceFlags.OnUpdate;
        }

        public static string GetUserID()
        {
            return UserInfoProvider.Instance.UserInfo;
        }

        public override Func<object> DefaultValueProvider => new Func<object>(GetUserID);
    }
}
