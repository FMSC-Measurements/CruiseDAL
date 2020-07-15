using FMSC.ORM.Core;
using FMSC.ORM.Core.SQL;
using Backpack.SqlBuilder;
using System.ComponentModel;
using CruiseDAL;

namespace FMSC.ORM.EntityModel
{
    public interface IDataObject : ISupportInitializeFromDatastore, IPersistanceTracking, IChangeTracking, ISupportInitialize
    {
        DAL DAL { get; set; }

        void Save();

        void Save(OnConflictOption option);

        void Delete();
    }
}