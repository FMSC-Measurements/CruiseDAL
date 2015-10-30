namespace CruiseDAL.Core.EntityModel
{
    public interface IPersistanceTracking
    {
        bool IsPersisted { get; set; }
        bool HasChanges { get; set; }
        bool IsDeleted { get; set; }

        void OnInserted();

        void OnUpdating();
        void OnUpdated();

        void OnDeleting();
        void OnDeleted();
    }
}
