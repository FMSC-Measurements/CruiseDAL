namespace FMSC.ORM.Core.SQL
{
    public class LegacySelectPlaceholder : SelectClause
    {
        public uint Index { get; set; }

        public override string ToSQL()
        {
            return "{" + Index.ToString() + "}";
        }
    }
}