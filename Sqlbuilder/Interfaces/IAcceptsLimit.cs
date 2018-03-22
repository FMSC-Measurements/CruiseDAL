namespace SqlBuilder
{
    public interface IAcceptsLimit
    {
        void Accept(LimitClause limitClause);
    }
}