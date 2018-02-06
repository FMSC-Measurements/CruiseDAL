namespace FMSC.ORM.Core.SQL
{
    public class WhereClause : SelectClause
    {
        public WhereClause(string expression)
        {
            Expression = expression;
        }

        public string Expression { get; set; }

        public override string ToSQL()
        {
            return " WHERE " + Expression + "\r\n";
        }
    }
}