using CruiseDAL.Util;
using FMSC.ORM.EntityModel.Attributes;
using System;

namespace CruiseDAL.EntityAttributes
{
    public class CreatedByFieldAttribute : InfrastructureFieldAttribute
    {
        public CreatedByFieldAttribute()
            : base("CreatedBy")
        {
            PersistanceFlags = PersistanceFlags.OnInsert;
        }

        public static string GetUserID()
        {
            return EnvironmentInfoProvider.Instance.UserInfo;
        }

        public override Func<object> DefaultValueProvider => new Func<object>(GetUserID);
    }
}