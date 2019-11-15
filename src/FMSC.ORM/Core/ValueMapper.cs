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
            var underlyingType = Nullable.GetUnderlyingType(targetType);
            var isNullable = underlyingType != null;
            targetType = underlyingType ?? targetType;
            

            if (value == null || value == DBNull.Value)
            {
                if (targetType.IsValueType && !isNullable)
                { return Activator.CreateInstance(targetType); }
                else { return null; }
            }

            if (targetType == typeof(Guid))
            {
                try
                {

                    if (value is string str) { return new Guid((str)); }
                    if (value is byte[] aByte) { return new Guid(aByte); }
                }
                catch
                {
                    if(isNullable)
                    {
                        return null;
                    }
                    throw;
                }
            }
            else if (targetType.IsEnum)
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
            else if (targetType == typeof(string)
                && value is byte[] aByte)
            {

                return System.Text.Encoding.Default.GetString(aByte);
            }

            try
            {
                return Convert.ChangeType(value, targetType
                    , System.Globalization.CultureInfo.CurrentCulture);
            }
            catch (Exception e)
            {
                if (value is String str && str == "")
                {
                    if (targetType.IsValueType && !isNullable)
                    {
                        return Activator.CreateInstance(targetType);
                    }
                    else
                    {
                        return null;
                    }
                }
                else
                {
                    if(isNullable)
                    {
                        return null;
                    }
                }


                throw new ORMException(
                    string.Format("unable to process value: {0} to {1}", value?.ToString() ?? "null", targetType.Name),
                    e);
            }
        }
    }
}