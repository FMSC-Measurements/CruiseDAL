using System;

namespace System.Text
{
    internal static class StringBuilderExtentions
    {
        public static void AppendLine(this StringBuilder @this)
        {
            @this.Append("\r\n");
        }

        public static void AppendLine(this StringBuilder @this, string value)
        {
            @this.Append(value + "\r\n");
        }
    }
}
