using FMSC.ORM.Core;
using System;
using System.Data.Common;

namespace CruiseDAL.V3.Sync.Syncers
{
    public abstract class TableSyncerBase
    {
        protected TableSyncerBase(string tableName)
        {
            TableName = tableName;
        }

        public string TableName { get; }

        public abstract TableSyncResult SyncRecords(string cruiseID, DbConnection source, DbConnection destination, TableSyncOptions options, IExceptionProcessor exceptionProcessor);

        protected static bool ShouldUpdate(DateTime? srcMod, DateTime? desMod, SyncOption syncFlags)
        {
            if (srcMod.HasValue == false) { return false; }
            else if ((desMod.HasValue == false)
                || syncFlags.HasFlag(SyncOption.ForceUpdate)
                || (DateTime.Compare(srcMod.Value, desMod.Value) > 0) && syncFlags.HasFlag(SyncOption.Update))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}