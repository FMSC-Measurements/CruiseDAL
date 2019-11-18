using FMSC.ORM.Core;

namespace FMSC.ORM.EntityModel
{
    public interface ISupportInitializeFromDatastore
    {
        void Initialize(Datastore datastore);
    }
}