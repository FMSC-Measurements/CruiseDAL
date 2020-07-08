using System.Collections.Generic;

namespace FMSC.ORM
{
    public static class EnumerableExtentions
    {
        public static IEnumerable<int> Diff<T>(this IEnumerable<T> @this, IEnumerable<T> other)
        {
            var e1 = @this.GetEnumerator();
            var e2 = other.GetEnumerator();
            var i = 0;

            while (true)
            {
                var n1 = e1.MoveNext();
                var n2 = e2.MoveNext();

                if (!n1 && !n2) { break; }
                else if (!n1 || !n2) { yield return i++; }
                else
                {
                    var v1 = e1.Current;
                    var v2 = e2.Current;

                    if (v1.Equals(v2)) { i++; continue; }
                    yield return i++;
                }
            }
        }
    }
}