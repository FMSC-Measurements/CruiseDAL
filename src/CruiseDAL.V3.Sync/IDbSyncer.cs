using System.Data.Common;
using System.Threading.Tasks;

namespace CruiseDAL.V3.Sync
{
    public interface IDbSyncer
    {
        Task SyncAsync(string cruiseID, DbConnection source, DbConnection destination, CruiseSyncOptions options);

        void Sync(string cruiseID, DbConnection source, DbConnection destination, CruiseSyncOptions options);

        //bool CheckForDesignChanges(DbConnection source, DbConnection destination);
    }
}