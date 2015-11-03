namespace FMSC.ORM.Core.SQL
{
    public class WhereClause
    {
        public WhereClause(string expression)
        {
            Expression = expression;
        }

        public string Expression { get; set; }

        public override string ToString()
        {
            return "WHERE " + Expression;
        }
    }
}
