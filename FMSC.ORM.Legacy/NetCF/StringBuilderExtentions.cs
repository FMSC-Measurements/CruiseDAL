namespace System.Text
{
    public static class StringBuilderExtentions
    {
        public static StringBuilder AppendLine(this StringBuilder @this)
        {
            @this.Append("\r\n");
            return @this;
        }

        public static StringBuilder AppendLine<TValue>(this StringBuilder @this, TValue value)
        {
            @this.Append(value);
            @this.Append("\r\n");
            return @this;
        }
    }
}
