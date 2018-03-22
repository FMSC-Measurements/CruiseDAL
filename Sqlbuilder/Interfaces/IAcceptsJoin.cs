namespace SqlBuilder
{
    public interface IAcceptsJoin : IAcceptsWhere
    {
        void Accept(JoinClause joinClause);
    }
}