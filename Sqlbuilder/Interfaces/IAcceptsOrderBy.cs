namespace SqlBuilder
{
    public interface IAcceptsOrderBy : IAcceptsLimit
    {
        void Accept(OrderByClause orderByClause);
    }
}