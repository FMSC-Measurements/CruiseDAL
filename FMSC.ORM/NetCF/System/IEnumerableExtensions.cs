using System;

using System.Collections.Generic;
using System.Text;

namespace System.Linq
{
    public static class IEnumerableExtensions
    {
        public static List<T> ToList<T>(this IEnumerable<T> source)
        {
            if (source == null) { throw new ArgumentNullException("source"); }
            return new List<T>(source);
        }

        public static T[] ToArray<T>(this IEnumerable<T> source)
        {
            if (source == null) { throw new ArgumentNullException("source"); }
            var list = new List<T>(source);
            return list.ToArray();
        }

        public static TSource FirstOrDefault<TSource>(this IEnumerable<TSource> source)
        {
            if (source == null) throw new ArgumentNullException("source");
            IList<TSource> list = source as IList<TSource>;
            if (list != null)
            {
                if (list.Count > 0) return list[0];
            }
            else
            {
                using (IEnumerator<TSource> e = source.GetEnumerator())
                {
                    if (e.MoveNext()) return e.Current;
                }
            }
            return default(TSource);
        }
    }
}
