namespace FMSC.ORM.Core.SQL.Interfaces
{
    public interface ISelectElement
    {
        ISelectElement ParentElement { get; set; }

        void Accept(ISelectElement parentElement);

        string ToSQL();
    }
}