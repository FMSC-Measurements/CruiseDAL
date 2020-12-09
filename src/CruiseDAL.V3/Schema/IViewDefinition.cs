namespace CruiseDAL.Schema
{
    public interface IViewDefinition
    {
        string ViewName { get; }

        string CreateView { get; }
    }
}