using System.Collections.Generic;

namespace FMSC.ORM.Util
{
    internal static class EnumerableExtentions
    {
        public static IEnumerable<T> BookEnd<T>(this IEnumerable<T> @this, T front, T back)
        {
            yield return front;
            foreach (var i in @this)
            {
                yield return i;
            }
            yield return back;
        }

        public static IEnumerable<T> BookEnd<T>(this IEnumerable<T> @this, T front, T back, T seperator)
        {
            yield return front;
            bool first = true;
            foreach (var i in @this)
            {
                if (first) { first = false; }
                else
                {
                    yield return seperator;
                }

                yield return i;
            }
            yield return back;
        }
    }
}