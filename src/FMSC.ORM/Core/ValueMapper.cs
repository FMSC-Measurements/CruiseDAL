using System;

namespace FMSC.ORM.Core
{
    public class ValueMapper
    {
        public static T ProcessValue<T>(object value)
        {
            if (value == null || value is DBNull)
            {
                return default(T);
            }
            else if (value is T)
            {
                return (T)value;
            }
            else
            {
                Type targetType = typeof(T);
                return (T)ProcessValue(targetType, value);
            }
        }

        public static object ProcessValue(Type targetType, object value)
        {
            if (value == null || value == DBNull.Value)
            {
                if (targetType.IsValueType) { return Activator.CreateInstance(targetType); }
                else { return null; }
            }

            targetType = Nullable.GetUnderlyingType(targetType) ?? targetType;

            if (targetType == typeof(Guid))
            {
                if (value is string str) { return new Guid((str)); }
                if (value is byte[] aByte) { return new Guid(aByte); }
            }
            if (targetType.IsEnum)
            {
                var valStr = (value is string) ? (string)value : value.ToString();

                try
                {
                    return Enum.Parse(targetType, valStr, true);
                }
                catch
                {
                    // change target type to enum underlying type so that the converter can have a chance to convert it
                    //targetType = Enum.GetUnderlyingType(targetType);
                }
            }

            return Convert.ChangeType(value, targetType
                , System.Globalization.CultureInfo.CurrentCulture);
        }
    }
}