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
            if (destination.ExecuteScalar<int>("SELECT count(*) FROM Sale JOIN Cruise USING (SaleNumber) WHERE CruiseID = @p1;", new[] { cruiseID }) == 0)
            {
                var sale = source.From<Sale>()
                    .Join("Cruise AS cr", "USING (SaleNumber)")
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

            var unit_tmbs = source.From<CuttingUnit_Tombstone>().Where("CruiseID = @p1;").Query(cruiseID);
            foreach (var unitt in unit_tmbs)
            {
                destination.Insert(unitt);
            }

            var strata = source.From<Stratum>().Where("CruiseID =  @p1;").Query(cruiseID);
            foreach (var st in strata)
            {
                destination.Insert(st);
            }

            var stratatmb = source.From<Stratum_Tombstone>().Where("CruiseID = @p1;").Query(cruiseID);
            foreach(var sttmb in stratatmb)
            {
                destination.Insert(sttmb);
            }

            var unitStrata = source.From<CuttingUnit_Stratum>().Where("CruiseID = @p1;").Query(cruiseID);
            foreach (var cust in unitStrata)
            {
                destination.Insert(cust);
            }

            var unitStratatmb = source.From<CuttingUnit_Stratum_Tombstone>().Where("CruiseID = @p1;").Query(cruiseID);
            foreach( var custtmb in unitStratatmb)
            {
                destination.Insert(custtmb);
            }

            var samplegroups = source.From<SampleGroup>().Where("CruiseID = @p1;").Query(cruiseID);
            foreach (var sg in samplegroups)
            {
                destination.Insert(sg);
            }

            var samplegroupstmb = source.From<SampleGroup_Tombstone>().Where("CruiseID = @p1;").Query(cruiseID);
            foreach(var sgtmb in samplegroupstmb)
            {
                destination.Insert(sgtmb);
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

            var subpopulationstmb = source.From<SubPopulation_Tombstone>().Where("CruiseID = @p1;").Query(cruiseID);
            foreach(var subpoptmb in subpopulationstmb)
            {
                destination.Insert(subpoptmb);
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

            var plotstmb = source.From<Plot_Tombstone>().Where("CruiseID = @p1;").Query(cruiseID);
            foreach(var ptmb in plotstmb)
            {
                destination.Insert(ptmb);
            }

            var plotLocations = source.From<PlotLocation>().Join("Plot AS p", "USING(PlotID)").Where("p.CruiseID = @p1;").Query(cruiseID);
            foreach (var pl in plotLocations)
            {
                destination.Insert(pl);
            }

            //var plotLocationstmb = source.From<PlotLocation_Tombstone>()

            var plotStrata = source.From<Plot_Stratum>().Where("CruiseID =  @p1;").Query(cruiseID);
            foreach (var pst in plotStrata)
            {
                destination.Insert(pst);
            }

            var plotStratatmb = source.From<Plot_Stratum_Tombstone>().Where("CruiseID = @p1;").Query(cruiseID);
            foreach(var pstmb in plotStratatmb)
            {
                destination.Insert(pstmb);
            }

            var trees = source.From<Tree>().Where("CruiseID = @p1;").Query(cruiseID);
            foreach (var t in trees)
            {
                destination.Insert(t);
            }

            var treetmb = source.From<Tree_Tombstone>().Where("CruiseID = @p1;").Query(cruiseID);
            foreach(var ttmb in treetmb)
            {
                destination.Insert(ttmb);

                var tmtmb = source.From<TreeMeasurment_Tombstone>().Where("TreeID = @p1;").Query(ttmb.TreeID).FirstOrDefault();
                if (tmtmb != null) { destination.Insert(tmtmb); }

                var treeFieldValuestmb = source.From<TreeFieldValue_Tombstone>().Where("TreeID = @p1;").Query(ttmb.TreeID);
                foreach(var tfvtmb in treeFieldValuestmb)
                {
                    destination.Insert(tfvtmb);
                }

                var tltmb = source.From<TreeLocation_Tombstone>().Where("TreeID = @p1;").Query(ttmb.TreeID).FirstOrDefault();
                if(tltmb != null) { destination.Insert(tltmb); }
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

            var logstmb = source.From<Log_Tombstone>().Where("CruiseID = @p1;").Query(cruiseID);
            foreach(var ltmb in logstmb)
            {
                destination.Insert(ltmb);
            }

            var stems = source.From<Stem>().Join("Tree AS t", "USING(TreeID)").Where("t.CruiseID = @p1;").Query(cruiseID);
            foreach (var s in stems)
            {
                destination.Insert(s);
            }

            var stemstmb = source.From<Stem_Tombstone>().Where("CruiseID = @p1;").Query(cruiseID);
            foreach(var stmb in stemstmb)
            {
                destination.Insert(stmb);
            }

            var tallyLedgers = source.From<TallyLedger>().Where("CruiseID = @p1;").Query(cruiseID);
            foreach (var tl in tallyLedgers)
            {
                destination.Insert(tl);
            }

            var tallyLedgerstmb = source.From<TallyLedger_Tombstone>().Where("CruiseID = @p1;").Query(cruiseID);
            foreach(var tltmb in tallyLedgerstmb)
            {
                destination.Insert(tltmb);
            }

            var logFieldSetups = source.From<LogFieldSetup>().Where("CruiseID = @p1;").Query(cruiseID);
            foreach (var lfs in logFieldSetups)
            {
                destination.Insert(lfs);
            }

            var logFieldSetupstmb = source.From<LogFieldSetup_Tombstone>().Where("CruiseID = @p1;").Query(cruiseID);
            foreach(var lfstmb in logFieldSetupstmb)
            {
                destination.Insert(lfstmb);
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

            var treeFieldSetuptmb = source.From<TreeFieldSetup_Tombstone>().Where("CruiseID = @p1;").Query(cruiseID);
            foreach(var tfstmb in treeFieldSetuptmb)
            {
                destination.Insert(tfstmb);
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

            var treeAuditRulestmb = source.From<TreeAuditRule_Tombstone>().Where("CruiseID = @p1;").Query(cruiseID);
            foreach(var tartmb in treeAuditRulestmb)
            {
                destination.Insert(tartmb);
            }

            var treeAuditRuleSelector = source.From<TreeAuditRuleSelector>().Where("CruiseID = @p1;").Query(cruiseID);
            foreach (var tars in treeAuditRuleSelector)
            {
                destination.Insert(tars);
            }

            var treeAuditRuleSelectorstmb = source.From<TreeAuditRuleSelector_Tombstone>().Where("CruiseID = @p1;").Query(cruiseID);
            foreach(var tarstmb in treeAuditRuleSelectorstmb)
            {
                destination.Insert(tarstmb);
            }

            var treeAuditResolutions = source.From<TreeAuditResolution>().Where("CruiseID = @p1").Query(cruiseID);
            foreach (var taRes in treeAuditResolutions)
            {
                destination.Insert(taRes);
            }

            var treeAuditResolutionstmb = source.From<TreeAuditResolution_Tombstone>().Where("CruiseID = @p1;").Query(cruiseID);
            foreach(var taRestmb in treeAuditResolutionstmb)
            {
                destination.Insert(taRestmb);
            }

            var logGradeAuditRules = source.From<LogGradeAuditRule>().Where("CruiseID = @p1").Query(cruiseID);
            foreach(var lgar in logGradeAuditRules)
            {
                destination.Insert(lgar);
            }

            var logGradeAuditRulestmb = source.From<LogGradeAuditRule_Tombstone>().Where("CruiseID = @p1;").Query(cruiseID);
            foreach(var lgartmb in logGradeAuditRulestmb)
            {
                destination.Insert(lgartmb);
            }

            var treeDefaultValues = source.From<TreeDefaultValue>().Where("CruiseID = @p1").Query(cruiseID);
            foreach(var tdv in treeDefaultValues)
            {
                destination.Insert(tdv);
            }

            var treeDefaultValuestmb = source.From<TreeDefaultValue_Tombstone>().Where("CruiseID = @p1;").Query(cruiseID);
            foreach(var tdvtmb in treeDefaultValuestmb)
            {
                destination.Insert(tdvtmb);
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

            // template
            var stratumTemplates = source.From<StratumTemplate>().Where("CruiseID = @p1").Query(cruiseID);
            foreach(var st in stratumTemplates)
            {
                destination.Insert(st);
            }

            var stratumTemplatestmb = source.From<StratumTemplate_Tombstone>().Where("CruiseID = @p1;").Query(cruiseID);
            foreach(var sttmb in stratumTemplatestmb)
            {
                destination.Insert(sttmb);
            }

            var stTemplateTfs = source.From<StratumTemplateTreeFieldSetup>().Where("CruiseID = @p1").Query(cruiseID);
            foreach(var sttfs in stTemplateTfs)
            {
                destination.Insert(sttfs);
            }

            var stTemplateTfstmb = source.From<StratumTemplateTreeFieldSetup_Tombstone>().Where("CruiseID = @p1;").Query(cruiseID);
            foreach(var sttfstmb in stTemplateTfstmb)
            {
                destination.Insert(sttfstmb);
            }

            var stTemplateLfs = source.From<StratumTemplateLogFieldSetup>().Where("CruiseID = @p1").Query(cruiseID);
            foreach(var stlfs in stTemplateLfs)
            {
                destination.Insert(stlfs);
            }

            var stTemplateLfstmb = source.From<StratumTemplateLogFieldSetup_Tombstone>().Where("CruiseID = @p1;").Query(cruiseID);
            foreach(var stlfstmb in stTemplateLfstmb)
            {
                destination.Insert(stlfstmb);
            }
        }
    }
}
