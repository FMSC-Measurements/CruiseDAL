using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace CruiseDAL.V3.Sync.Test.Support
{
    public static class TableRowExtentions
    {
        public static T GetValueOrDefault<T>(this TableRow @this, string key)
        {
            if (@this.TryGetValue(key, out var strValue))
            {
                if (strValue is T tValue) return tValue;

                var type = typeof(T);
                if (String.IsNullOrEmpty(strValue)) return default;

                
                var underlyingType = Nullable.GetUnderlyingType(type);
                //var isNullable = underlyingType!= null;
                type ??= underlyingType;

                return (T)Convert.ChangeType(strValue, type);
            }
            return default;

        }
    }
}
