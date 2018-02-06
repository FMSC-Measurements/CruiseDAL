namespace FMSC.ORM.Core.SQL.Interfaces
{
    public interface IAcceptsLimit : ISelectElement
    {
        void Accept(LimitClause limitClause);

        ISelectElement Limit(int limit, int offset);
    }
}