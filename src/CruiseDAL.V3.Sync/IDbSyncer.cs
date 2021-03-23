using System;
using System.Data.Common;
using System.Threading.Tasks;

namespace CruiseDAL.V3.Sync
{
    public interface IDbSyncer
    {
        Task SyncAsync(string cruiseID, DbConnection source, DbConnection destination, CruiseSyncOptions options, IProgress<float> progress = null);

        void Sync(string cruiseID, DbConnection source, DbConnection destination, CruiseSyncOptions options, IProgress<float> progress = null);

        //bool CheckForDesignChanges(DbConnection source, DbConnection destination);
    }
}