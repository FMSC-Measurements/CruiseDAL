using FMSC.ORM.Core.SQL;
namespace FMSC.ORM.Core.EntityModel
{
    public interface IDataObject : IPersistanceTracking
    {
        DatastoreRedux DAL { get; set; }

        void Save();
        void Save(OnConflictOption option);
        void Delete();
    }
}
