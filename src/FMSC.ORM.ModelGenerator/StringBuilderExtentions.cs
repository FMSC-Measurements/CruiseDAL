using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FMSC.ORM.ModelGenerator
{
    public static class StringBuilderExtentions
    {
        public static string Tab { get; set; } = "    ";

        public static StringBuilder TabAppendLine(this StringBuilder @this, string value, int tabIndex)
        {
            var tab = Tab;
            foreach(var i in Enumerable.Range(0, tabIndex))
            {
                @this.Append(tab);
            }

            return @this.AppendLine(value);
        }

        public static StringBuilder TabAppend(this StringBuilder @this, string value, int tabIndex)
        {
            var tab = Tab;
            foreach (var i in Enumerable.Range(0, tabIndex))
            {
                @this.Append(tab);
            }

            return @this.Append(value);
        }
    }
}
