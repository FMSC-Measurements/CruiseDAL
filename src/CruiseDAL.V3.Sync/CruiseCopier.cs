using CruiseDAL.V3.Models;
using FMSC.ORM.Core;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;

namespace CruiseDAL.V3.Sync
{
    public class CruiseCopier
    {
        public void Copy(CruiseDatastore_V3 source, CruiseDatastore_V3 destination, string cruiseID)
        {
            var srcConn = source.OpenConnection();
            try
            {
                var destConn = destination.OpenConnection();
                try
                {
                    destination.BeginTransaction();
                    try
                    {
                        Copy(srcConn, destConn, cruiseID);
                        destination.CommitTransaction();
                    }
                    catch
                    {
                        destination.RollbackTransaction();
                        throw;
                    }
                }
                finally
                {
                    destination.ReleaseConnection();
                }
            }
            finally
            {
                source.ReleaseConnection();
            }
        }

        public void Copy(DbConnection source, DbConnection destination, string cruiseID)
        {
            if (destination.ExecuteScalar<int>("SELECT count(*) FROM Sale JOIN Cruise USING (SaleID) WHERE CruiseID = @p1;", new[] { cruiseID }) == 0)
            {
                var sale = source.From<Sale>()
                    .Join("Cruise AS cr", "USING (SaleID)")
                    .Where("CruiseID = @p1")
                    .Query(cruiseID).FirstOrDefault();
                destination.Insert(sale);
            }

            var cruise = source.From<Cruise>().Where("CruiseID = @p1;").Query(cruiseID).FirstOrDefault();
            destination.Insert(cruise);

            var devices = source.From<Device>().Where("CruiseID = @p1;").Query(cruiseID);
            foreach (var d in devices)
            {
                destination.Insert(d);
            }

            var units = source.From<CuttingUnit>().Where("CruiseID = @p1;").Query(cruiseID);
            foreach (var unit in units)
            {
                destination.Insert(unit);
            }

            var strata = source.From<Stratum>().Where("CruiseID =  @p1;").Query(cruiseID);
            foreach (var st in strata)
            {
                destination.Insert(st);
            }

            var unitStrata = source.From<CuttingUnit_Stratum>().Where("CruiseID = @p1;").Query(cruiseID);
            foreach (var cust in unitStrata)
            {
                destination.Insert(cust);
            }

            var samplegroups = source.From<SampleGroup>().Where("CruiseID = @p1;").Query(cruiseID);
            foreach (var sg in samplegroups)
            {
                destination.Insert(sg);
            }

            var samplerStates = source.From<SamplerState>().Where("CruiseID =  @p1;").Query(cruiseID);
            foreach (var ss in samplerStates)
            {
                destination.Insert(ss);
            }

            var species = source.From<Species>().Where("CruiseID = @p1;").Query(cruiseID);
            foreach (var sp in species)
            {
                destination.Insert(sp);
            }

            var subpopulations = source.From<SubPopulation>().Where("CruiseID = @p1;").Query(cruiseID);
            foreach (var subpop in subpopulations)
            {
                destination.Insert(subpop);
            }

            var tallyDescriptions = source.From<TallyDescription>().Where("CruiseID = @p1;").Query(cruiseID);
            foreach (var tDesc in tallyDescriptions)
            {
                destination.Insert(tDesc);
            }

            var tallyHotkeys = source.From<TallyHotKey>().Where("CruiseID = @p1;").Query(cruiseID);
            foreach (var thk in tallyHotkeys)
            {
                destination.Insert(thk);
            }

            var fixCNTTallyPop = source.From<FixCNTTallyPopulation>().Where("CruiseID = @p1;").Query(cruiseID);
            foreach (var fctp in fixCNTTallyPop)
            {
                destination.Insert(fctp);
            }

            var plots = source.From<Plot>().Where("CruiseID = @p1;").Query(cruiseID);
            foreach (var p in plots)
            {
                destination.Insert(p);
            }

            var plotLocations = source.From<PlotLocation>().Join("Plot AS p", "USING(PlotID)").Where("p.CruiseID = @p1;").Query(cruiseID);
            foreach (var pl in plotLocations)
            {
                destination.Insert(pl);
            }

            var plotStrata = source.From<Plot_Stratum>().Where("CruiseID =  @p1;").Query(cruiseID);
            foreach (var pst in plotStrata)
            {
                destination.Insert(pst);
            }

            var trees = source.From<Tree>().Where("CruiseID = @p1;").Query(cruiseID);
            foreach (var t in trees)
            {
                destination.Insert(t);
            }

            var treeMeasurments = source.From<TreeMeasurment>().Join("Tree AS t", "USING(TreeID)").Where("t.CruiseID = @p1;").Query(cruiseID);
            foreach (var tm in treeMeasurments)
            {
                destination.Insert(tm);
            }

            var treeFieldValues = source.From<TreeFieldValue>().Join("Tree AS t", "USING(TreeID)").Where("t.CruiseID = @p1;").Query(cruiseID);
            foreach (var tfv in treeFieldValues)
            {
                destination.Insert(tfv);
            }

            var treeLocations = source.From<TreeLocation>().Join("Tree AS t", "USING(TreeID)").Where("t.CruiseID = @p1;").Query(cruiseID);
            foreach (var tl in treeLocations)
            {
                destination.Insert(tl);
            }

            var logs = source.From<Log>().Join("Tree AS t", "USING(TreeID)").Where("t.CruiseID = @p1;").Query(cruiseID);
            foreach (var l in logs)
            {
                destination.Insert(l);
            }

            var stems = source.From<Stem>().Join("Tree AS t", "USING(TreeID)").Where("t.CruiseID = @p1;").Query(cruiseID);
            foreach (var s in stems)
            {
                destination.Insert(s);
            }

            var tallyLedgers = source.From<TallyLedger>().Where("CruiseID = @p1;").Query(cruiseID);
            foreach (var tl in tallyLedgers)
            {
                destination.Insert(tl);
            }

            var logFieldSetups = source.From<LogFieldSetup>().Where("CruiseID = @p1;").Query(cruiseID);
            foreach (var lfs in logFieldSetups)
            {
                destination.Insert(lfs);
            }

            var logFieldHeadings = source.From<LogFieldHeading>().Where("CruiseID = @p1").Query(cruiseID);
            foreach(var lfh in logFieldHeadings)
            {
                destination.Insert(lfh);
            }

            var treeFieldSetup = source.From<TreeFieldSetup>().Where("CruiseID = @p1;").Query(cruiseID);
            foreach (var tfs in treeFieldSetup)
            {
                destination.Insert(tfs);
            }

            var treeFieldHeadings = source.From<TreeFieldHeading>().Where("CruiseID = @p1").Query(cruiseID);
            foreach(var tfh in treeFieldHeadings)
            {
                destination.Insert(tfh);
            }

            var treeAuditRules = source.From<TreeAuditRule>().Where("CruiseID = @p1;").Query(cruiseID);
            foreach (var tar in treeAuditRules)
            {
                destination.Insert(tar);
            }

            var treeAuditRuleSelector = source.From<TreeAuditRuleSelector>().Where("CruiseID = @p1;").Query(cruiseID);
            foreach (var tars in treeAuditRuleSelector)
            {
                destination.Insert(tars);
            }

            var treeAuditResolutions = source.From<TreeAuditResolution>().Where("CruiseID = @p1").Query(cruiseID);
            foreach (var taRes in treeAuditResolutions)
            {
                destination.Insert(taRes);
            }

            var logGradeAuditRules = source.From<LogGradeAuditRule>().Where("CruiseID = @p1").Query(cruiseID);
            foreach(var lgar in logGradeAuditRules)
            {
                destination.Insert(lgar);
            }

            var treeDefaultValues = source.From<TreeDefaultValue>().Where("CruiseID = @p1").Query(cruiseID);
            foreach(var tdv in treeDefaultValues)
            {
                destination.Insert(tdv);
            }



            // Processing

            var biomassEqs = source.From<BiomassEquation>().Where("CruiseID = @p1").Query(cruiseID);
            foreach(var bioEq in biomassEqs)
            {
                destination.Insert(bioEq);
            }

            var reports = source.From<Reports>().Where("CruiseID = @p1").Query(cruiseID);
            foreach(var report in reports)
            {
                destination.Insert(report);
            }

            var valueEqs = source.From<ValueEquation>().Where("CruiseID = @p1").Query(cruiseID);
            foreach(var valueEq in valueEqs)
            {
                destination.Insert(valueEq);
            }

            var volumeEqs = source.From<VolumeEquation>().Where("CruiseID = @p1").Query(cruiseID);
            foreach(var volumeEq in volumeEqs)
            {
                destination.Insert(volumeEq);
            }
        }
    }
}
