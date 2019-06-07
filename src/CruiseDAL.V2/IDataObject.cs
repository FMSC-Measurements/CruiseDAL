using FMSC.ORM.Core;
using FMSC.ORM.Core.SQL;
using Backpack.SqlBuilder;
using System.ComponentModel;

namespace FMSC.ORM.EntityModel
{
    public interface IDataObject : ISupportInitializeFromDatastore, IPersistanceTracking, IChangeTracking, ISupportInitialize
    {
        Datastore DAL { get; set; }

        void Save();

        void Save(OnConflictOption option);

        void Delete();
    }
}