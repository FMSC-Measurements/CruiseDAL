namespace System.Text
{
    public static class StringBuilderExtentions
    {
        public static void AppendLine(this StringBuilder @this)
        {
            @this.Append("\r\n");
        }

        public static void AppendLine<TValue>(this StringBuilder @this, TValue value)
        {
            @this.Append(value);
            @this.Append("\r\n");
        }
    }
}
