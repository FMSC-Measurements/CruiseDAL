using System;
using System.Collections.Generic;

namespace System.Text
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
