using CruiseDAL.V3.Models;
using System;
using System.Linq;

namespace CruiseDAL.V3.Sync
{
    public static class CruiseFileUtilities
    {
        public static void ClearTombstoneRecords(CruiseDatastore_V3 db, string cruiseID)
        {
            if (string.IsNullOrEmpty(cruiseID))
            { throw new ArgumentException($"'{nameof(cruiseID)}' cannot be null or empty.", nameof(cruiseID)); }

            db.Execute("DELETE FROM PlotLocation_Tombstone WHERE PlotID IN (SELECT PlotID FROM Plot_Tombstone WHERE CruiseID = @p1);", cruiseID);
            db.Execute("DELETE FROM TreeMeasurment_Tombstone WHERE TreeID IN (SELECT TreeID FROM Tree_Tombstone WHERE CruiseID = @p1);", cruiseID);
            db.Execute("DELETE FROM TreeLocation_Tombstone WHERE TreeID IN (SELECT TreeID FROM Tree_Tombstone WHERE CruiseID = @p1);", cruiseID);
            db.Execute("DELETE FROM TreeFieldValue_Tombstone WHERE TreeID IN (SELECT TreeID FROM Tree_Tombstone WHERE CruiseID = @p1);", cruiseID);


            var tombstoneTables = db.GetTableNames().Where(x => x.EndsWith("_Tombstone", StringComparison.OrdinalIgnoreCase));

            foreach (var table in tombstoneTables)
            {
                if (table == nameof(PlotLocation_Tombstone)) { continue; }
                if (table == nameof(TreeMeasurment_Tombstone)) { continue;}
                if (table == nameof(TreeLocation_Tombstone)) { continue; }
                if (table == nameof(TreeFieldValue_Tombstone)) { continue; }

                db.Execute($"DELETE FROM {table} WHERE CruiseID = @p1", cruiseID);
            }
        }
    }
}