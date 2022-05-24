using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CruiseDAL.V3.Models;
using FMSC.ORM.Core;

namespace CruiseDAL.V3.Sync
{
    public class ConflictResolver
    {
        protected const string MODIFY_CUTTINGUNIT_COMMAND = "UPDATE CuttingUnit SET CuttingUnitCode = @CuttingUnitCode WHERE CuttingUnitID = @CuttingUnitID;";
        protected const string MODIFY_STRATUM_COMMAND = "UPDATE Stratum SET StratumCode = @StratumCode WHERE StratumID = @StratumID;";
        protected const string MODIFY_SAMPLEGROUP_COMMAND = "UPDATE SampleGroup SET SampleGroupCode = @SampleGroupCode WHERE SampleGroupID = @SampleGroupID;";
        protected const string MODIFY_PLOT_COMMAND = "UPDATE Plot SET PlotNumber = @PlotNumber WHERE PlotID = @PlotID;";
        protected const string MODIFY_TREE_COMMAND = "UPDATE Tree SET TreeNumber = @TreeNumber WHERE TreeID = @TreeID;";
        protected const string MODIFY_LOG_COMMAND = "UPDATE Log SET LogNumber = @LogNumber WHERE LogID = @LogID;";

        public void ResolveUnitConflicts(DbConnection source, DbConnection destination, IEnumerable<Conflict> conflicts)
        {
            foreach (var conflict in conflicts)
            {
                ResolveUnitConflict(source, destination, conflict);
            }
        }

        protected void ResolveUnitConflict(DbConnection source, DbConnection destination, Conflict conflict)
        {
            var resolution = conflict.ConflictResolution;

            if (resolution == ConflictResolutionType.ChoseLatest)
            {
                resolution = conflict.SourceMod.CompareTo(conflict.SourceMod) > 0 ?
                    ConflictResolutionType.ChoseSource :
                    ConflictResolutionType.ChoseDest;
            }

            switch (resolution)
            {
                case ConflictResolutionType.ChoseSource:
                    {
                        // leave fKeys on so that all child data gets deleted
                        destination.ExecuteNonQuery(
                            "DELETE FROM CuttingUnit WHERE CuttingUnitID = @p1;"
                            , new[] { conflict.DestRecID });
                        break;
                    }
                case ConflictResolutionType.ChoseSourceMergeData:
                    {
                        destination.ExecuteNonQuery(
                            "PRAGMA foreign_keys=off; " +
                            "BEGIN; " + // disable FKeys so that cascading deletes don't trigger 
                            "DELETE FROM CuttingUnit WHERE CuttingUnitID = @p1;" +
                            "COMMIT; " +
                            "PRAGMA foreign_keys=on;", new[] { conflict.DestRecID });
                        break;
                    }
                case ConflictResolutionType.ChoseDest:
                    {
                        // leave fKeys on so that all child data gets deleted
                        source.ExecuteNonQuery(
                            "DELETE FROM CuttingUnit WHERE CuttingUnitID = @p1;"
                            , new[] { conflict.SourctRecID });
                        break; // skip syncing source record if resolution is to Chose Destination record
                    }
                case ConflictResolutionType.ChoseDestMergeData:
                    {
                        source.ExecuteNonQuery(
                            "PRAGMA foreign_keys=off; " +
                            "BEGIN; " + // disable FKeys so that cascading deletes don't trigger 
                            "DELETE FROM CuttingUnit WHERE CuttingUnitID = @p1;" +
                            "COMMIT; " +
                            "PRAGMA foreign_keys=on;", new[] { conflict.SourctRecID });
                        break;
                    }
                case ConflictResolutionType.ModifySource:
                    {
                        source.ExecuteNonQuery2(MODIFY_CUTTINGUNIT_COMMAND, conflict.SourceRec);
                        break;
                    }
                case ConflictResolutionType.ModifyDest:
                    {
                        destination.ExecuteNonQuery2(MODIFY_CUTTINGUNIT_COMMAND, conflict.DestRec);
                        break;
                    }
            }

            if (conflict.DownstreamConflicts != null && conflict.DownstreamConflicts.Any())
            {
                foreach (var c in conflict.DownstreamConflicts)
                {
                    ResolveConflict(source, destination, c);
                }
            }
        }



        public void ResolveStratumConflicts(DbConnection source, DbConnection destination, IEnumerable<Conflict> conflicts)
        {
            foreach (var conflict in conflicts)
            {
                ResolveStratumConflict(source, destination, conflict);
            }
        }

        protected void ResolveStratumConflict(DbConnection source, DbConnection destination, Conflict conflict)
        {
            var resolution = conflict.ConflictResolution;

            if (resolution == ConflictResolutionType.ChoseLatest)
            {
                resolution = conflict.SourceMod.CompareTo(conflict.SourceMod) > 0 ?
                    ConflictResolutionType.ChoseSource :
                    ConflictResolutionType.ChoseDest;
            }

            switch (resolution)
            {
                case ConflictResolutionType.ChoseSource:
                    {
                        throw new NotSupportedException();

                        //destination.ExecuteNonQuery(
                        //    "DELETE FROM Stratum WHERE StratumID = @p1; ", new[] { conflict.DestRecID });
                        //break;
                    }
                case ConflictResolutionType.ChoseSourceMergeData:
                    {
                        destination.ExecuteNonQuery(
                            "PRAGMA foreign_keys=off; " +
                            "BEGIN; " + // disable FKeys so that cascading deletes don't trigger 
                            "DELETE FROM Stratum WHERE StratumID = @p1;" +
                            "COMMIT; " +
                            "PRAGMA foreign_keys=on;", new[] { conflict.DestRecID });
                        break;
                    }
                case ConflictResolutionType.ChoseDest:
                    {
                        throw new NotSupportedException();

                        //source.ExecuteNonQuery(
                        //    "DELETE FROM Stratum WHERE StratumID = @p1; ", new[] { conflict.SourctRecID });
                        //break;
                    }
                case ConflictResolutionType.ChoseDestMergeData:
                    {
                        source.ExecuteNonQuery(
                            "PRAGMA foreign_keys=off; " +
                            "BEGIN; " + // disable FKeys so that cascading deletes don't trigger 
                            "DELETE FROM Stratum WHERE StratumID = @p1;" +
                            "COMMIT; " +
                            "PRAGMA foreign_keys=on;", new[] { conflict.SourctRecID });
                        break;
                    }
                case ConflictResolutionType.ModifySource:
                    {
                        source.ExecuteNonQuery2(MODIFY_STRATUM_COMMAND, conflict.SourceRec);
                        break;
                    }
                case ConflictResolutionType.ModifyDest:
                    {
                        destination.ExecuteNonQuery2(MODIFY_STRATUM_COMMAND, conflict.DestRec);
                        break;
                    }
            }
        }

        public void ResolveSampleGroupConflicts(DbConnection source, DbConnection destination, IEnumerable<Conflict> conflicts)
        {
            foreach (var conflict in conflicts)
            {
                ResolveSampleGroupConflict(source, destination, conflict);
            }
        }

        protected void ResolveSampleGroupConflict(DbConnection source, DbConnection destination, Conflict conflict)
        {
            // TODO we don't want sample group conflict resolutions to clear and child field data, because we are treating field data
            // as belonging to its containing unit/plot. do we consider this type of confile resolution to be chose or chose and merge?

            var resolution = conflict.ConflictResolution;

            if (resolution == ConflictResolutionType.ChoseLatest)
            {
                resolution = conflict.SourceMod.CompareTo(conflict.SourceMod) > 0 ?
                    ConflictResolutionType.ChoseSource :
                    ConflictResolutionType.ChoseDest;
            }

            switch (resolution)
            {
                case ConflictResolutionType.ChoseSource:
                    {
                        // for sample groups we don't want to cascade deletes down to trees, tally ledgers, subpops, or sampler states
                        destination.ExecuteNonQuery(
                            "PRAGMA foreign_keys=off; " +
                            "BEGIN; " + // disable FKeys so that cascading deletes don't trigger 
                            "DELETE FROM SampleGroup WHERE SampleGroupID = @p1;" +
                            "COMMIT; " +
                            "PRAGMA foreign_keys=on;", new[] { conflict.DestRecID });
                        break;

                        //destination.ExecuteNonQuery(
                        //    "DELETE FROM SampleGroup WHERE SampleGroupID = @p1;", new[] { conflict.DestRecID });
                        //break;
                    }
                case ConflictResolutionType.ChoseSourceMergeData:
                    {
                        destination.ExecuteNonQuery(
                            "PRAGMA foreign_keys=off; " +
                            "BEGIN; " + // disable FKeys so that cascading deletes don't trigger 
                            "DELETE FROM SampleGroup WHERE SampleGroupID = @p1;" +
                            "COMMIT; " +
                            "PRAGMA foreign_keys=on;", new[] { conflict.DestRecID });
                        break;

                        throw new NotSupportedException();
                    }
                case ConflictResolutionType.ChoseDest:
                    {
                        // for sample groups we don't want to cascade deletes down to trees, tally ledgers, subpops, or sampler states
                        source.ExecuteNonQuery(
                            "PRAGMA foreign_keys=off; " +
                            "BEGIN; " + // disable FKeys so that cascading deletes don't trigger 
                            "DELETE FROM SampleGroup WHERE SampleGroupID = @p1;" +
                            "COMMIT; " +
                            "PRAGMA foreign_keys=on;", new[] { conflict.SourctRecID });
                        break;

                        //source.ExecuteNonQuery2(
                        //    "DELETE FROM SampleGroup WHERE SampleGroupID = @p1;", new[] { conflict.SourceRec });
                        //break;
                    }
                case ConflictResolutionType.ChoseDestMergeData:
                    {
                        source.ExecuteNonQuery(
                            "PRAGMA foreign_keys=off; " +
                            "BEGIN; " + // disable FKeys so that cascading deletes don't trigger 
                            "DELETE FROM SampleGroup WHERE SampleGroupID = @p1;" +
                            "COMMIT; " +
                            "PRAGMA foreign_keys=on;", new[] { conflict.SourctRecID });
                        break;
                    }
                case ConflictResolutionType.ModifySource:
                    {
                        source.ExecuteNonQuery2(MODIFY_SAMPLEGROUP_COMMAND, conflict.SourceRec);
                        break;
                    }
                case ConflictResolutionType.ModifyDest:
                    {
                        destination.ExecuteNonQuery2(MODIFY_SAMPLEGROUP_COMMAND, conflict.DestRec);
                        break;
                    }
            }
        }

        public void ResolvePlotConflicts(DbConnection source, DbConnection destination, IEnumerable<Conflict> conflicts)
        {
            foreach (var conflict in conflicts)
            {
                ResolvePlotConflict(source, destination, conflict);
            }
        }

        protected void ResolvePlotConflict(DbConnection source, DbConnection destination, Conflict conflict)
        {
            var resolution = conflict.ConflictResolution;

            if (resolution == ConflictResolutionType.ChoseLatest)
            {
                resolution = conflict.SourceMod.CompareTo(conflict.SourceMod) > 0 ?
                    ConflictResolutionType.ChoseSource :
                    ConflictResolutionType.ChoseDest;
            }

            switch (resolution)
            {
                case ConflictResolutionType.ChoseSource:
                    {
                        destination.ExecuteNonQuery(
                            "DELETE FROM Plot WHERE PlotID = @p1; ", new[] { conflict.DestRecID });
                        break;
                    }
                case ConflictResolutionType.ChoseSourceMergeData:
                    {
                        destination.ExecuteNonQuery(
                            "PRAGMA foreign_keys=off; " +
                            "BEGIN; " + // disable FKeys so that cascading deletes don't trigger 
                            "DELETE FROM Plot WHERE PlotID = @p1; " +
                            "COMMIT; " +
                            "PRAGMA foreign_keys=on;", new[] { conflict.DestRecID });
                        break;
                    }
                case ConflictResolutionType.ChoseDest:
                    {
                        source.ExecuteNonQuery(
                            "DELETE FROM Plot WHERE PlotID = @p1; ", new[] { conflict.SourctRecID });
                        break;// do nothing, add plotIDs to ExcludePlotIDs
                    }
                case ConflictResolutionType.ChoseDestMergeData:
                    {
                        source.ExecuteNonQuery(
                            "PRAGMA foreign_keys=off; " +
                            "BEGIN; " + // disable FKeys so that cascading deletes don't trigger 
                            "DELETE FROM Plot WHERE PlotID = @p1; " +
                            "COMMIT; " +
                            "PRAGMA foreign_keys=on;", new[] { conflict.SourctRecID });
                        break;
                    }
                case ConflictResolutionType.ModifySource:
                    {
                        source.ExecuteNonQuery2(MODIFY_PLOT_COMMAND, conflict.SourceRec);
                        break;
                    }
                case ConflictResolutionType.ModifyDest:
                    {
                        destination.ExecuteNonQuery2(MODIFY_PLOT_COMMAND, conflict.DestRec);
                        break;
                    }
            }
        }

        public void ResolveTreeConflicts(DbConnection source, DbConnection destination, IEnumerable<Conflict> conflicts)
        {
            foreach (var conflict in conflicts)
            {
                ResolveTreeConflict(source, destination, conflict);
            }
        }

        protected void ResolveTreeConflict(DbConnection source, DbConnection destination, Conflict conflict)
        {
            var resolution = conflict.ConflictResolution;

            if (resolution == ConflictResolutionType.ChoseLatest)
            {
                resolution = conflict.SourceMod.CompareTo(conflict.SourceMod) > 0 ?
                    ConflictResolutionType.ChoseSource :
                    ConflictResolutionType.ChoseDest;
            }

            switch (resolution)
            {
                case ConflictResolutionType.ChoseSource:
                    {
                        destination.ExecuteNonQuery(
                            "DELETE FROM Tree WHERE TreeID = @p1; ", new[] { conflict.DestRecID });
                        break;
                    }

                case ConflictResolutionType.ChoseDest:
                    {
                        source.ExecuteNonQuery(
                            "DELETE FROM Tree WHERE TreeID = @p1; ", new[] { conflict.SourctRecID });
                        break;
                    }
                // merge resolutions not supported because the only child record type of tree: log
                // references tree using a unique ID, 
                case ConflictResolutionType.ChoseSourceMergeData:
                    {
                        throw new NotImplementedException();
                    }
                case ConflictResolutionType.ChoseDestMergeData:
                    {
                        throw new NotImplementedException();
                    }
                case ConflictResolutionType.ModifySource:
                    {
                        source.ExecuteNonQuery2(MODIFY_TREE_COMMAND, conflict.SourceRec);
                        break;
                    }
                case ConflictResolutionType.ModifyDest:
                    {
                        destination.ExecuteNonQuery2(MODIFY_TREE_COMMAND, conflict.DestRec);
                        break;
                    }
            }
        }

        public void ResolveLogConflicts(DbConnection source, DbConnection destination, IEnumerable<Conflict> conflicts)
        {
            foreach (var conflict in conflicts)
            {
                ResolveLogConflict(source, destination, conflict);
            }
        }

        protected void ResolveLogConflict(DbConnection source, DbConnection destination, Conflict conflict)
        {
            var resolution = conflict.ConflictResolution;

            if (resolution == ConflictResolutionType.ChoseLatest)
            {
                resolution = conflict.SourceMod.CompareTo(conflict.SourceMod) > 0 ?
                    ConflictResolutionType.ChoseSource :
                    ConflictResolutionType.ChoseDest;
            }

            switch (resolution)
            {
                case ConflictResolutionType.ChoseSource:
                    {
                        destination.ExecuteNonQuery("DELETE Log WHERE LogID = @p1;", new[] { conflict.DestRecID });
                        break;
                    }

                case ConflictResolutionType.ChoseDest:
                    {
                        source.ExecuteNonQuery("DELETE Log WHERE LogID = @p1;", new[] { conflict.SourctRecID });
                        break;
                    }
                // mere resolutions not supported because there are no child record types for logs
                case ConflictResolutionType.ChoseSourceMergeData:
                    {
                        throw new NotSupportedException();
                    }
                case ConflictResolutionType.ChoseDestMergeData:
                    {
                        throw new NotSupportedException();
                    }
                case ConflictResolutionType.ModifySource:
                    {
                        source.ExecuteNonQuery2(MODIFY_LOG_COMMAND, conflict.SourceRec);
                        break;
                    }
                case ConflictResolutionType.ModifyDest:
                    {
                        destination.ExecuteNonQuery2(MODIFY_LOG_COMMAND, conflict.DestRec);
                        break;
                    }
            }
        }

        protected void ResolveConflict(DbConnection source, DbConnection destination, Conflict conflict)
        {
            switch (conflict.Table)
            {
                case nameof(Plot):
                    {
                        ResolvePlotConflict(source, destination, conflict);
                        break;
                    }
                case nameof(Tree):
                    {
                        ResolveTreeConflict(source, destination, conflict);
                        break;
                    }
                case nameof(Log):
                    {
                        ResolveLogConflict(source, destination, conflict);
                        break;
                    }

            }
        }
    }
}
