namespace FMSC.ORM.Core.EntityModel
{
    public interface IDataObject : IPersistanceTracking
    {
        DatastoreRedux DAL { get; set; }
    }
}
