using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CruiseDAL.V3.Sync
{
    public class CruiseChecker
    {
        public bool HasDesignKeyChanges(CruiseDatastore_V3 source, CruiseDatastore_V3 destination, string cruiseID)
        {
            var sourceAlias = "src";
            destination.AttachDB(source, sourceAlias);
            try
            {
                var hasUnitChanges = destination.ExecuteScalar<int>(
$@"SELECT count(*) FROM main.CuttingUnit AS cu1
    JOIN {sourceAlias}.CuttingUnit AS cu2 USING (CuttingUnitID) 
    WHERE cu1.CruiseID = @p1 AND cu1.CuttingUnitCode != cu2.CuttingUnitCode;", cruiseID) > 0;

                var hasStratumChanges = destination.ExecuteScalar<int>(
$@"SELECT count(*) FROM main.Stratum as st1
    JOIN {sourceAlias}.Stratum AS st2 USING (StratumID)
    WHERE st1.CruiseID = @p1 AND st1.StratumCode != st2.StratumCode;", cruiseID) > 0;

                var hasSampleGroupChanges = destination.ExecuteScalar<int>(
$@"SELECT count(*) FROM main.SampleGroup AS sg1
    JOIN {sourceAlias}.SampleGroup AS sg2 USING (SampleGroupID)
    WHERE sg1.CruiseID = @p1 AND sg1.StratumCode != sg2.StratumCode OR sg1.SampleGroupCode != sg2.SampleGroupCode;", cruiseID) > 0;

                var hasSubPopChanges = destination.ExecuteScalar<int>(
$@"SELECT count(*) FROM main.SubPopulation AS sp1
    JOIN {sourceAlias}.SubPopulation AS sp2 USING (SubPopulationID)
    WHERE sp1.CruiseID = @p1 AND sp1.SpeciesCode != sp2.SpeciesCode OR sp1.LiveDead != sp2.LiveDead;", cruiseID) > 0;

                return hasUnitChanges
                    || hasStratumChanges
                    || hasStratumChanges
                    || hasSubPopChanges;
            }
            finally
            {
                destination.DetachDB(sourceAlias);
            }
        }

        public IEnumerable<CuttingUnitDiffResult> DiffCuttingUnitKeys(CruiseDatastore_V3 source, CruiseDatastore_V3 destination, string cruiseID)
        {
            var sourceAlias = "src";
            destination.AttachDB(source, sourceAlias);
            try
            {
                return destination.Query<CuttingUnitDiffResult>(
$@"SELECT 
    cu1.CuttingUnitID, 
    cu1.CuttingUnitCode AS DestCuttingUnitCode, 
    cu2.CuttingUnitCode AS SrcCuttingUnitCode 
FROM main.CuttingUnit AS cu1
JOIN {sourceAlias}.CuttingUnit AS cu2 USING (CuttingUnitID) 
WHERE cu1.CruiseID = @p1 AND cu1.CuttingUnitCode != cu2.CuttingUnitCode;", cruiseID).ToArray();
            }
            finally
            {
                destination.DetachDB(sourceAlias);
            }
        }

        public IEnumerable<StratumDiffResult> DiffStratumKeys(CruiseDatastore_V3 source, CruiseDatastore_V3 destination, string cruiseID)
        {
            var sourceAlias = "src";
            destination.AttachDB(source, sourceAlias);
            try
            {
                return destination.Query<StratumDiffResult>(
$@"SELECT 
    st1.StratumID, 
    st1.StratumCode AS DestStratumCode, 
    st2.StratumCode AS SrcStratumCode 
FROM main.Stratum AS st1
JOIN {sourceAlias}.Stratum AS st2 USING (StratumID) 
WHERE st1.CruiseID = @p1 AND st1.StratumCode != st2.StratumCode;", cruiseID).ToArray();
            }
            finally
            {
                destination.DetachDB(sourceAlias);
            }
        }

        public IEnumerable<SampleGroupDiffResult> DiffSampleGroupKeys(CruiseDatastore_V3 source, CruiseDatastore_V3 destination, string cruiseID)
        {
            var sourceAlias = "src";
            destination.AttachDB(source, sourceAlias);
            try
            {
                return destination.Query<SampleGroupDiffResult>(
$@"SELECT   
    sg1.SampleGroupID, 
    sg1.SampleGroupCode AS DestSampleGroupCode, 
    sg2.SampleGroupCode AS SrcSampleGroupCode, 
    sg1.StratumCode AS DestStratumCode,
    sg2.StratumCode AS SrcStratumCode
FROM main.SampleGroup AS sg1
JOIN {sourceAlias}.SampleGroup AS sg2 USING (SampleGroupID)
WHERE sg1.CruiseID = @p1 AND sg1.StratumCode != sg2.StratumCode OR sg1.SampleGroupCode != sg2.SampleGroupCode;", cruiseID).ToArray();
            }
            finally
            {
                destination.DetachDB(sourceAlias);
            }
        }

        public IEnumerable<SubPopulationDiffResult> DiffSubPopulationKeys(CruiseDatastore_V3 source, CruiseDatastore_V3 destination, string cruiseID)
        {
            var sourceAlias = "src";
            destination.AttachDB(source, sourceAlias);
            try
            {
                return destination.Query<SubPopulationDiffResult>(
$@"SELECT   
    sp1.SubPopulationID, 
    sp1.SpeciesCode AS DestSpeciesCode,
    sp2.SpeciesCode AS SrcSpeciesCode, 
    sp1.LiveDead AS DestLiveDead,
    sp2.LiveDead AS SrcLiveDead
FROM main.SubPopulation AS sp1
JOIN {sourceAlias}.SubPopulation AS sp2 USING (SubPopulationID)
WHERE sp1.CruiseID = @p1 AND sp1.SpeciesCode != sp2.SpeciesCode OR sp1.LiveDead != sp2.LiveDead;", cruiseID).ToArray();
            }
            finally
            {
                destination.DetachDB(sourceAlias);
            }
        }


        public IEnumerable<SaleConflict> GetSaleConflicts(CruiseDatastore_V3 source, CruiseDatastore_V3 destination, string cruiseID)
        {
            var sourceAlias = "src";
            destination.AttachDB(source, sourceAlias);
            try
            {
                return destination.Query<SaleConflict>(
$@"SELECT 
    srcS.SaleNumber,
    srcS.Name,
    destS.SaleID AS DestSaleID,
    srcS.SaleID AS SrcSaleID
FROM {sourceAlias}.Sale AS srcS
JOIN {sourceAlias}.Cruise AS srcCr USING (SaleNumber)
JOIN main.Sale AS destS USING (SaleNumber)
WHERE srcCr.CruiseID = @p1 AND srcS.SaleID != destS.SaleID;
", cruiseID).ToArray();
            }
            finally
            {
                destination.DetachDB(sourceAlias);
            }
        }

        public IEnumerable<CruiseConflict> GetCruiseConflicts(CruiseDatastore_V3 source, CruiseDatastore_V3 destination, string cruiseID)
        {
            var sourceAlias = "src";
            destination.AttachDB(source, sourceAlias);
            try
            {
                return destination.Query<CruiseConflict>(
$@"SELECT 
    srcCr.CruiseNumber,
    destCr.CruiseID AS DestCruiseID,
    srcCr.CruiseID AS SrcCruiseID
FROM {sourceAlias}.Cruise AS srcCr 
JOIN main.Cruise AS destCr USING (CruiseNumber)
WHERE srcCr.CruiseID =@p1 AND destCr.CruiseID != srcCr.CruiseID;
", cruiseID).ToArray();
            }
            finally
            {
                destination.DetachDB(sourceAlias);
            }
        }

        public IEnumerable<PlotConflict> GetPlotConflicts(CruiseDatastore_V3 source, CruiseDatastore_V3 destination, string cruiseID)
        {
            var sourceAlias = "src";
            destination.AttachDB(source, sourceAlias);
            try
            {
                return destination.Query<PlotConflict>(
$@"SELECT 
    destP.CuttingUnitCode, 
    destP.PlotNumber, 
    srcP.PlotID AS SrcPlotID, 
    destP.PlotID AS DestPlotID
FROM main.Plot AS destP
    JOIN {sourceAlias}.Plot AS srcP USING (CuttingUnitCode, PlotNumber, CruiseID) 
    WHERE destP.CruiseID = @p1 AND destP.PlotID != srcP.PlotID;", cruiseID).ToArray();
            }
            finally
            {
                destination.DetachDB(sourceAlias);
            }
        }

        public IEnumerable<TreeConflict> GetTreeConflicts(CruiseDatastore_V3 source, CruiseDatastore_V3 destination, string cruiseID)
        {
            var sourceAlias = "src";
            destination.AttachDB(source, sourceAlias);
            try
            {
                return destination.Query<TreeConflict>(
$@"SELECT 
    destT.CuttingUnitCode, 
    destT.PlotNumber, 
    destT.TreeNumber,
    srcT.TreeID AS SrcTreeID, 
    destT.TreeID AS DestTreeID
FROM main.Tree AS destT
    JOIN {sourceAlias}.Tree AS srcT ON 
        destT.CruiseID = srcT.CruiseID 
        AND destT.CuttingUnitCode = srcT.CuttingUnitCode 
        AND destT.PlotNumber = srcT.PlotNumber
        AND destT.TreeNumber = srcT.TreeNumber
WHERE destT.CruiseID = @p1 AND destT.TreeID != srcT.TreeID;", cruiseID).ToArray();
            }
            finally
            {
                destination.DetachDB(sourceAlias);
            }
        }

        public IEnumerable<LogConflict> GetLogConflicts(CruiseDatastore_V3 source, CruiseDatastore_V3 destination, string cruiseID)
        {
            var sourceAlias = "src";
            destination.AttachDB(source, sourceAlias);
            try
            {
                return destination.Query<LogConflict>(
$@"SELECT 
    destT.CuttingUnitCode,
    destT.PlotNumber,
    destT.TreeNumber,
    destL.LogNumber,
    destL.LogID AS DestLogID,
    srcL.LogID AS SrcLogID
FROM main.Log AS destL 
    JOIN main.Tree AS destT USING (TreeID)
    JOIN {sourceAlias}.Tree AS srcT ON 
        destT.CruiseID = srcT.CruiseID
        AND destT.CuttingUnitCode = srcT.CuttingUnitCode 
        AND ifnull(destT.PlotNumber, 0) = ifnull(srcT.PlotNumber, 0)
        AND destT.TreeNumber = srcT.TreeNumber
    JOIN {sourceAlias}.Log AS srcL ON srcT.TreeID = srcL.TreeID
        AND destL.LogNumber = srcL.LogNumber
WHERE destT.CruiseID = @p1 AND destL.LogID != srcL.LogID;", cruiseID).ToArray();
            }
            finally
            {
                destination.DetachDB(sourceAlias);
            }
        }
    }

    public class SaleConflict
    {
        public string SaleNumber { get; set; }
        public string Name { get; set; }
        public string SrcSaleID { get; set; }
        public string DestSaleID { get; set; }
    }

    public class CruiseConflict
    {
        public string SaleNumber { get; set; }
        public string Name { get; set; }
        public string SrcCruiseID { get; set; }
        public string DestCruiseID { get; set; }
    }

    public class PlotConflict
    {
        public string CuttingUnitCode { get; set; }
        public string PlotNumber { get; set; }
        public string SrcPlotID { get; set; }
        public string DestPlotID { get; set; }
    }

    public class TreeConflict
    {
        public string CuttingUnitCode { get; set; }
        public string PlotNumber { get; set; }
        public string TreeNumber { get; set; }
        public string SrcTreeID { get; set; }
        public string DestTreeID { get; set; }
    }

    public class LogConflict
    {
        public string CuttingUnitCode { get; set; }
        public string PlotNumber { get; set; }
        public string TreeNumber { get; set; }
        public string LogNumber { get; set; }
        public string SrcLogID { get; set; }
        public string DestTreeID { get; set; }
    }

    public class CuttingUnitDiffResult
    {
        public string CuttingUnitID { get; set; }
        public string SrcCuttingUnitCode { get; set; }
        public string DestCuttingUnitCode { get; set; }
    }

    public class StratumDiffResult
    {
        public string StratumID { get; set; }
        public string SrcStratumCode { get; set; }
        public string DestStratumCode { get; set; }
    }

    public class SampleGroupDiffResult
    {
        public string SampleGroupID { get; set; }
        public string SrcSampleGroupCode { get; set; }
        public string DestSampleGroupCode { get; set; }
        public string SrcStratumCode { get; set; }
        public string DestStratumCode { get; set; }
    }

    public class SubPopulationDiffResult
    {
        public string SubPopulationID { get; set; }
        public string SrcSpeciesCode { get; set; }
        public string DestSpeciesCode { get; set; }
        public string SrcLiveDead { get; set; }
        public string DestLiveDead { get; set; }
    }
}
