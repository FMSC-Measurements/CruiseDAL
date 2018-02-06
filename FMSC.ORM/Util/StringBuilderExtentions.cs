using System.Collections.Generic;
using System.Text;

namespace FMSC.ORM.Util
{
    public static class StringBuilderExtentions
    {
        public static void AppendMany(this StringBuilder @this, IEnumerable<string> values)
        {
            foreach (var value in values)
            {
                @this.Append(value);
            }
        }

        public static void AppendManyLines(this StringBuilder @this, IEnumerable<string> values)
        {
            foreach (var value in values)
            {
                @this.AppendLine(value);
            }
        }
    }
}