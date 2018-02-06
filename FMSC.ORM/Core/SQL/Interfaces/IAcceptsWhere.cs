namespace FMSC.ORM.Core.SQL.Interfaces
{
    public interface IAcceptsWhere : IAcceptsGroupBy
    {
        void Accept(WhereClause where);

        IAcceptsGroupBy Where(string expression);
    }
}