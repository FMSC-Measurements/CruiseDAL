namespace FMSC.ORM.EntityModel
{
    public interface IPersistanceTracking
    {
        bool IsPersisted { get;}
        bool IsDeleted { get;}

        void OnRead();

        void OnInserting();
        void OnInserted();

        void OnUpdating();
        void OnUpdated();

        void OnDeleting();
        void OnDeleted();
    }
}
