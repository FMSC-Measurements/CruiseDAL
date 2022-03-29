using CruiseDAL.V3.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CruiseDAL.V3.Sync
{
    public class ConflictChecker
    {
        

        public IEnumerable<Conflict> CheckCuttingUnits(CruiseDatastore_V3 source, CruiseDatastore_V3 destination, string cruiseID)
        {
            var sourceItems = source.From<CuttingUnit>().Where("CruiseID = @p1").Query(cruiseID);

            foreach(var item in sourceItems)
            {
                var conflictItem = destination.From<CuttingUnit>()
                    .Where("CruiseID = @CruiseID AND CuttingUnitCode = @CuttingUnitCode AND CuttingUnitID != @CuttingUnitID")
                    .Query2(item).FirstOrDefault();

                if(conflictItem != null)
                {
                    yield return new Conflict
                    {
                        Table = nameof(CuttingUnit),
                        SourctRecID = item.CuttingUnitID,
                        DestRecID = conflictItem.CuttingUnitID,
                        SourceRec = item,
                        DestRec = conflictItem,
                    };
                }
            }
        }

        public IEnumerable<Conflict> CheckStrata(CruiseDatastore_V3 source, CruiseDatastore_V3 destination, string cruiseID)
        {
            var sourceItems = source.From<Stratum>().Where("CruiseID = @p1").Query(cruiseID);

            foreach (var item in sourceItems)
            {
                var conflictItem = destination.From<Stratum>()
                    .Where("CruiseID = @CruiseID AND StratumCode = @StratumCode AND StratumID != @StratumID")
                    .Query2(item).FirstOrDefault();

                if (conflictItem != null)
                {
                    yield return new Conflict
                    {
                        Table = nameof(Stratum),
                        SourctRecID = item.StratumID,
                        DestRecID = conflictItem.StratumID,
                        SourceRec = item,
                        DestRec = conflictItem,
                    };
                }
            }
        }

        public IEnumerable<Conflict> CheckSampleGroups(CruiseDatastore_V3 source, CruiseDatastore_V3 destination, string cruiseID)
        {
            var sourceItems = source.From<SampleGroup>().Where("CruiseID = @p1").Query(cruiseID);

            foreach (var item in sourceItems)
            {
                var conflictItem = destination.From<SampleGroup>()
                    .Where("CruiseID = @CruiseID AND SampleGroupCode = @SampleGroupCode AND StratumCode = @StratumCode AND SampleGroupID != @SampleGroupID")
                    .Query2(item).FirstOrDefault();

                if (conflictItem != null)
                {
                    yield return new Conflict
                    {
                        Table = nameof(SampleGroup),
                        SourctRecID = item.SampleGroupID,
                        DestRecID = conflictItem.SampleGroupID,
                        SourceRec = item,
                        DestRec = conflictItem,
                    };
                }
            }
        }

        public IEnumerable<Conflict> CheckPlots(CruiseDatastore_V3 source, CruiseDatastore_V3 destination, string cruiseID)
        {
            var sourceItems = source.From<Plot>().Where("CruiseID = @p1").Query(cruiseID);

            foreach (var item in sourceItems)
            {
                var conflictItem = destination.From<Plot>()
                    .Where("CruiseID = @CruiseID AND PlotNumber = @PlotNumber AND PlotID != @PlotID")
                    .Query2(item).FirstOrDefault();

                if (conflictItem != null)
                {
                    yield return new Conflict
                    {
                        Table = nameof(Plot),
                        SourctRecID = item.PlotID,
                        DestRecID = conflictItem.PlotID,
                        SourceRec = item,
                        DestRec = conflictItem,
                    };
                }
            }
        }

        public IEnumerable<Conflict> CheckTrees(CruiseDatastore_V3 source, CruiseDatastore_V3 destination, string cruiseID)
        {
            var sourceItems = source.From<Tree>().Where("CruiseID = @p1").Query(cruiseID);

            foreach (var item in sourceItems)
            {
                var conflictItem = destination.From<Tree>()
                    .Where("CruiseID = @CruiseID AND TreeNumber = @TreeNumber AND CuttingUnitCode = @CuttingUnitCode AND ifnull(PlotNumber, 0) = ifnull(@PlotNumber, 0) AND TreeID != @TreeID")
                    .Query2(item).FirstOrDefault();

                if (conflictItem != null)
                {
                    yield return new Conflict
                    {
                        Table = nameof(Tree),
                        SourctRecID = item.TreeID,
                        DestRecID = conflictItem.TreeID,
                        SourceRec = item,
                        DestRec = conflictItem,
                    };
                }
            }
        }

        public IEnumerable<Conflict> CheckLogs(CruiseDatastore_V3 source, CruiseDatastore_V3 destination, string cruiseID)
        {
            var sourceItems = source.From<Log>().Where("CruiseID = @p1").Query(cruiseID);

            foreach (var item in sourceItems)
            {
                var conflictItem = destination.From<Log>()
                    .Where("CruiseID = @CruiseID AND LogNumber = @LogNumber AND TreeID = @TreeID AND LogID != @LogID")
                    .Query2(item).FirstOrDefault();

                if (conflictItem != null)
                {
                    yield return new Conflict
                    {
                        Table = nameof(Log),
                        SourctRecID = item.LogID,
                        DestRecID = conflictItem.LogID,
                    };
                }
            }
        }


    }


}
