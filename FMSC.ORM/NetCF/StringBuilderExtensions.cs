using System;

using System.Collections.Generic;
using System.Text;

namespace FMSC.ORM.NetCF
{
    public static class StringBuilderExtensions
    {
        public static void AppendLine(this StringBuilder sb)
        {
            sb.Append("\r\n");
        }

        public static void AppendLine(this StringBuilder sb, string text)
        {
            sb.Append(text + "\r\n");
        }
    }
}
