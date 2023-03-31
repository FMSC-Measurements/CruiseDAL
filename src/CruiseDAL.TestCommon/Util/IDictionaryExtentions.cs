using System;
using System.Collections.Generic;

namespace CruiseDAL.TestCommon.Util
{
    public static class IDictionaryExtentions
    {
        public static TValue GetValueOrDefault<TKey, TValue>(this IDictionary<TKey, TValue> @this, TKey key)
        {
            if (@this == null) throw new ArgumentNullException(nameof(@this));
            if (@this.ContainsKey(key))
            {
                return @this[key];
            }
            return default(TValue);
        }
    }
}