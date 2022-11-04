using CruiseDAL.V3.Models;
using FMSC.ORM.Core;
using System.Data.Common;
using System.Linq;

namespace CruiseDAL.V3.Sync.Syncers
{
    public class TreeMeasurmentSyncer : TableSyncerBase
    {
        public TreeMeasurmentSyncer() : base(nameof(TreeMeasurment))
        {
        }

        public override TableSyncResult SyncRecords(string cruiseID, DbConnection source, DbConnection destination, TableSyncOptions options, IExceptionProcessor exceptionProcessor)
        {
            // not checking the tombstone tables for TreeMeasurment because at this
            // point we should already know that the tree has not be deleted.
            // I assuming that TreeMeasurment records wont be deleted unless the tree has been deleted

            var syncResult = new TableSyncResult(nameof(TreeMeasurment));

            var flags = options.TreeMeasurment;
            if (flags == SyncOption.Lock) { return syncResult; }

            var trees = destination.From<Tree>().Where("CruiseID = @p1")
                .Query(cruiseID);
            foreach (var tree in trees)
            {
                var treeID = tree.TreeID;

                var sourceMeasurmentsRecord = source.From<TreeMeasurment>().Where("TreeID = @p1").Query(treeID).FirstOrDefault();
                if (sourceMeasurmentsRecord != null)
                {
                    var hasMeasurmentsRecord = destination.ExecuteScalar<long>("SELECT count(*) FROM TreeMeasurment WHERE TreeID =  @p1;", parameters: new[] { treeID }) > 0;

                    if (hasMeasurmentsRecord)
                    {
                        if (flags.HasFlag(SyncOption.Update))
                        {
                            destination.Update(sourceMeasurmentsRecord, whereExpression: "TreeID =  @TreeID", exceptionProcessor: exceptionProcessor);
                            syncResult.IncrementUpdates();
                        }
                    }
                    else
                    {
                        if (flags.HasFlag(SyncOption.Insert))
                        {
                            destination.Insert(sourceMeasurmentsRecord, persistKeyvalue: false, exceptionProcessor: exceptionProcessor);
                            syncResult.IncrementInserts();
                        }
                    }
                }
            }
            return syncResult;
        }
    }
}