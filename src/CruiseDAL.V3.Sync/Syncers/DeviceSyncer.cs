using CruiseDAL.V3.Models;
using FMSC.ORM.Core;
using System.Data.Common;

namespace CruiseDAL.V3.Sync.Syncers
{
    public class DeviceSyncer : TableSyncerBase
    {
        public DeviceSyncer() : base(nameof(Device))
        {
        }

        public override TableSyncResult SyncRecords(string cruiseID, DbConnection source, DbConnection destination, TableSyncOptions options, IExceptionProcessor exceptionProcessor)
        {
            var syncResult = new TableSyncResult(nameof(Device));

            var flags = options.Device;
            if (flags == SyncOption.Lock) { return syncResult; }

            var sourceItems = source.From<Device>().Where("CruiseID = @p1").Query(cruiseID);

            foreach (var i in sourceItems)
            {
                var hasMatch = destination.ExecuteScalar<long>("SELECT count(*) FROM Device WHERE DeviceID = @p1 AND CruiseID = @p2;", new object[] { i.DeviceID, cruiseID }) > 0;

                if (hasMatch == false && flags.HasFlag(SyncOption.Insert))
                {
                    destination.Insert(i, persistKeyvalue: false, exceptionProcessor: exceptionProcessor);
                }
            }

            return syncResult;
        }
    }
}