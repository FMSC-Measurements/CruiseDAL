using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        public void ResolveUnitConflicts(CruiseDatastore_V3 source, CruiseDatastore_V3 destination, IEnumerable<Conflict> conflicts)
        {
            foreach (var conflict in conflicts)
            {
                switch (conflict.ConflictResolution)
                {
                    case Conflict.ConflictResolutionType.ChoseSource:
                        {
                            destination.Execute("DELETE FROM CuttingUnit WHERE CuttingUnitID = @p1", conflict.DestRecID);
                            break;
                        }
                    case Conflict.ConflictResolutionType.ChoseDest:
                        {
                            break;// do nothing, make sure to add unit id to exclude units in SyncOptions
                        }
                    case Conflict.ConflictResolutionType.ModifySource:
                        {
                            source.Execute2(MODIFY_CUTTINGUNIT_COMMAND, conflict.SourceRec);
                            break;
                        }
                    case Conflict.ConflictResolutionType.ModifyDest:
                        {
                            destination.Execute2(MODIFY_CUTTINGUNIT_COMMAND, conflict.DestRec);
                            break;
                        }
                }
            }
        }

        public void ResolveStratumConflicts(CruiseDatastore_V3 source, CruiseDatastore_V3 destination, IEnumerable<Conflict> conflicts)
        {
            foreach (var conflict in conflicts)
            {
                switch (conflict.ConflictResolution)
                {
                    case Conflict.ConflictResolutionType.ChoseSource:
                        {
                            destination.Execute("DELETE FROM Stratum WHERE StratumID = @p1", conflict.DestRecID);
                            break;
                        }
                    case Conflict.ConflictResolutionType.ChoseDest:
                        {
                            break;// do nothing, 
                        }
                    case Conflict.ConflictResolutionType.ModifySource:
                        {
                            source.Execute2(MODIFY_STRATUM_COMMAND, conflict.SourceRec);
                            break;
                        }
                    case Conflict.ConflictResolutionType.ModifyDest:
                        {
                            destination.Execute2(MODIFY_STRATUM_COMMAND, conflict.DestRec);
                            break;
                        }
                }
            }
        }

        public void ResolveSampleGroupConflicts(CruiseDatastore_V3 source, CruiseDatastore_V3 destination, IEnumerable<Conflict> conflicts)
        {
            foreach (var conflict in conflicts)
            {
                switch (conflict.ConflictResolution)
                {
                    case Conflict.ConflictResolutionType.ChoseSource:
                        {
                            destination.Execute("DELETE FROM SampleGroup WHERE SampleGroupID = @p1", conflict.DestRecID);
                            break;
                        }
                    case Conflict.ConflictResolutionType.ChoseDest:
                        {
                            break;// do nothing, 
                        }
                    case Conflict.ConflictResolutionType.ModifySource:
                        {
                            source.Execute2(MODIFY_SAMPLEGROUP_COMMAND, conflict.SourceRec);
                            break;
                        }
                    case Conflict.ConflictResolutionType.ModifyDest:
                        {
                            destination.Execute2(MODIFY_SAMPLEGROUP_COMMAND, conflict.DestRec);
                            break;
                        }
                }
            }
        }

        public void ResolvePlotConflicts(CruiseDatastore_V3 source, CruiseDatastore_V3 destination, IEnumerable<Conflict> conflicts)
        {
            foreach (var conflict in conflicts)
            {
                switch (conflict.ConflictResolution)
                {
                    case Conflict.ConflictResolutionType.ChoseSource:
                        {
                            destination.Execute("DELETE FROM Plot WHERE PlotID = @p1", conflict.DestRecID);
                            break;
                        }
                    case Conflict.ConflictResolutionType.ChoseDest:
                        {
                            break;// do nothing, add plotIDs to ExcludePlotIDs
                        }
                    case Conflict.ConflictResolutionType.ModifySource:
                        {
                            source.Execute2(MODIFY_PLOT_COMMAND, conflict.SourceRec);
                            break;
                        }
                    case Conflict.ConflictResolutionType.ModifyDest:
                        {
                            destination.Execute2(MODIFY_PLOT_COMMAND, conflict.DestRec);
                            break;
                        }
                }
            }
        }

        public void ResolveTreeConflicts(CruiseDatastore_V3 source, CruiseDatastore_V3 destination, IEnumerable<Conflict> conflicts)
        {
            foreach (var conflict in conflicts)
            {
                switch (conflict.ConflictResolution)
                {
                    case Conflict.ConflictResolutionType.ChoseSource:
                        {
                            destination.Execute("DELETE Tree WHERE TreeID = @p1;", conflict.DestRecID);
                            break;
                        }
                    case Conflict.ConflictResolutionType.ChoseDest:
                        {
                            break;// do nothing, add plotIDs to ExcludePlotIDs
                        }
                    case Conflict.ConflictResolutionType.ModifySource:
                        {
                            source.Execute2(MODIFY_TREE_COMMAND, conflict.SourceRec);
                            break;
                        }
                    case Conflict.ConflictResolutionType.ModifyDest:
                        {
                            destination.Execute2(MODIFY_TREE_COMMAND, conflict.DestRec);
                            break;
                        }
                }
            }
        }

        public void ResolveLogConflicts(CruiseDatastore_V3 source, CruiseDatastore_V3 destination, IEnumerable<Conflict> conflicts)
        {
            foreach (var conflict in conflicts)
            {
                switch (conflict.ConflictResolution)
                {
                    case Conflict.ConflictResolutionType.ChoseSource:
                        {
                            destination.Execute("DELETE Log WHERE LogID = @p1;", conflict.DestRecID);
                            break;
                        }
                    case Conflict.ConflictResolutionType.ChoseDest:
                        {
                            break;// do nothing, add plotIDs to ExcludePlotIDs
                        }
                    case Conflict.ConflictResolutionType.ModifySource:
                        {
                            source.Execute2(MODIFY_LOG_COMMAND, conflict.SourceRec);

                            break;
                        }
                    case Conflict.ConflictResolutionType.ModifyDest:
                        {
                            destination.Execute2(MODIFY_LOG_COMMAND, conflict.DestRec);
                            break;
                        }
                }
            }
        }
    }
}
