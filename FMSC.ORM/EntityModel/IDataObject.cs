using FMSC.ORM.Core;
using FMSC.ORM.Core.SQL;
using System.ComponentModel;

namespace FMSC.ORM.EntityModel
{
    public interface IDataObject : IPersistanceTracking, IChangeTracking
    {
        DatastoreRedux DAL { get; set; }

        void Save();
        void Save(OnConflictOption option);
        void Delete();
    }
}
