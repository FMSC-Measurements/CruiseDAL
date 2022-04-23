using System;

namespace CruiseDAL.V3.Sync.Util
{
    public static class DateMath
    {
        public static DateTime Max(this DateTime x, DateTime y)
        {
            return DateTime.Compare(x, y) > 0 ? x : y;
        }

        public static DateTime? Max(DateTime? x, DateTime? y)
        {
            if (x == null) { return y; }
            if (y == null) { return x; }

            return DateTime.Compare(x.Value, y.Value) > 0 ? x : y;
        }

        public static DateTime Max(this DateTime x, DateTime? y)
        {
            if (y == null) { return x; }

            var yValue = y.Value;
            return DateTime.Compare(x, yValue) > 0 ? x : yValue;
        }

        public static DateTime Min(this DateTime @this, DateTime x)
        {
            return DateTime.Compare(@this, x) < 0 ? @this : x;
        }
    }
}