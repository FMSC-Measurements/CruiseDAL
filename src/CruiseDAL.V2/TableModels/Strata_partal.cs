using CruiseDAL.MappingCollections;
using CruiseDAL.Schema;
using FMSC.ORM.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CruiseDAL.DataObjects
{
    public partial class StratumDO
    {
        protected CuttingUnitCollection _CuttingUnits = null;

        public CuttingUnitCollection CuttingUnits
        {
            get
            {
                if (_CuttingUnits == null)
                {
                    _CuttingUnits = new CuttingUnitCollection(this);
                }
                return _CuttingUnits;
            }
        }

        protected override void OnDALChanged(Datastore newDAL)
        {
            base.OnDALChanged(newDAL);
            if (_CuttingUnits != null)
            {
                _CuttingUnits.DAL = newDAL;
            }
        }

        public override void Delete()
        {
            base.Delete();
            this.CuttingUnits.Populate();
            this.CuttingUnits.Clear();
            this.CuttingUnits.Save();
        }

        protected override bool ValidateProperty(string name, object value)
        {
            bool isValid = true;
            if (name == STRATUM.KZ3PPNT)
            {
                if (this.Method == "3PPNT" && this.KZ3PPNT <= 0)
                {
                    isValid = false;
                    this.AddError(name, "KZ invalid for 3PPNT");
                }
                else
                {
                    this.RemoveError(name, "KZ invalid for 3PPNT");
                }
            }
            else if (name == STRATUM.METHOD)
            {
                if (Array.IndexOf(CruiseDAL.Schema.CruiseMethods.UNSUPPORTED_METHODS, this.Method) >= 0)
                {
                    isValid = false;
                    this.AddError(name, "Cruise Method Not Supported");
                }
                else
                {
                    this.RemoveError(name, "Cruise Method Not Supported");
                }
            }

            isValid = base.ValidateProperty(name, value) && isValid;
            return isValid;
        }

        public static List<StratumDO> ReadByUnitCode(Datastore dal, String code)
        {
            if (dal == null) { return null; }
            return dal.From<StratumDO>()
                .Join("CuttingUnitStratum", "USING (Stratum_CN)")
                .Join("CuttingUnit", "USING (CuttingUnit_CN)")
                .Where("CuttingUnit.Code = @p1")
                .Read(code).ToList();
        }

        public static bool CanDefineTallys(StratumDO stratum)
        {
            if (stratum.Method == "FIX" || stratum.Method == "PNT" || stratum.Method == "100")
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public static int DeleteStratum(Datastore dal, StratumDO stratum)
        {
            // check tree table for data
            if (dal.GetRowCount("Tree", "WHERE Stratum_CN = @p1", stratum.Stratum_CN) > 0) { return (-1); }
            // check plot table for data
            if (dal.GetRowCount("Plot", "WHERE Stratum_CN = @p1", stratum.Stratum_CN) > 0) { return (-1); }
            // Check Count table for each sample group
            if (dal.GetRowCount("CountTree", "JOIN SampleGroup ON CountTree.SampleGroup_CN = SampleGroup.SampleGroup_CN WHERE SampleGroup.Stratum_CN = @p1 AND CountTree.TreeCount > 0", stratum.Stratum_CN) > 0) return (-1);

            //Delete sample groups for stratum
            List<SampleGroupDO> allSGInStratum = dal.From<SampleGroupDO>()
                 .Where("Stratum_CN = @p1")
                 .Read(stratum.Stratum_CN).ToList();

            //.Read<SampleGroupDO>("WHERE Stratum_CN = ?", stratum.Stratum_CN);
            foreach (SampleGroupDO Sg in allSGInStratum)
            {
                //Delete Count Records for stratum
                List<CountTreeDO> allCountInSG = dal.From<CountTreeDO>()
                      .Where("SampleGroup_CN = @p1")
                      .Read(Sg.SampleGroup_CN).ToList();

                //.Read<CountTreeDO>("WHERE SampleGroup_CN = ?", Sg.SampleGroup_CN);
                foreach (CountTreeDO Cnt in allCountInSG)
                {
                    Cnt.Delete();
                }
                Sg.Delete();
            }

            //Delete stratum stats for stratum
            List<StratumStatsDO> allStratumStatsInStratum = dal.From<StratumStatsDO>()
                 .Where("Stratum_CN = @p1")
                 .Read(stratum.Stratum_CN)
                 .ToList();
            //.Read<StratumStatsDO>(
            //"WHERE Stratum_CN = ?", stratum.Stratum_CN);
            foreach (StratumStatsDO StratumStats in allStratumStatsInStratum)
            {
                StratumStats.DeleteStratumStats(dal, StratumStats.StratumStats_CN);
            }

            stratum.Delete();

            return (0);
        }

        public static void RecursiveDeleteStratum(StratumDO stratum)
        {
            Datastore db = stratum.DAL;

            db.BeginTransaction();
            try
            {
                string command =
            String.Format(@"DELETE From CuttingUnitStratum WHERE Stratum_CN = {0};
DELETE FROM Log WHERE Tree_CN IN (SELECT Tree_CN FROM Tree WHERE Tree.Stratum_CN = {0});
DELETE FROM LogStock WHERE Tree_CN IN (SELECT Tree_CN FROM Tree WHERE Tree.Stratum_CN = {0});
DELETE FROM TreeCalculatedValues WHERE Tree_CN IN (SELECT Tree_CN FROM Tree WHERE Tree.Stratum_CN = {0});
DELETE FROM Tree WHERE Stratum_CN = {0};
DELETE FROM Plot WHERE Stratum_CN = {0};
DELETE FROM TreeEstimate WHERE CountTree_CN IN (SELECT CountTree_CN FROM CountTree JOIN SampleGroup USING (SampleGroup_CN) WHERE Stratum_CN = {0});
DELETE FROM CountTree WHERE SampleGroup_CN IN (SELECT SampleGroup_CN FROM SampleGroup WHERE SampleGroup.Stratum_CN = {0});
DELETE FROM SampleGroupTreeDefaultValue WHERE SampleGroup_CN IN (SELECT SampleGroup_CN FROM SampleGroup WHERE SampleGroup.Stratum_CN = {0});
DELETE FROM SampleGroup WHERE Stratum_CN = {0};
DELETE FROM TreeFieldSetup WHERE Stratum_CN = {0};
DELETE FROM LogFieldSetup WHERE Stratum_CN = {0};
DELETE FROM FixCNTTallyPopulation WHERE FixCNTTallyClass_CN IN (SELECT FixCNTtallyClass_CN FROM FixCNTTallyClass WHERE Stratum_CN = {0});
DELETE FROM FixCNTTallyClass WHERE Stratum_CN = {0};",
                stratum.Stratum_CN);
                db.Execute(command);
                stratum.Delete();
                db.CommitTransaction();
            }
            catch (Exception e)
            {
                db.RollbackTransaction();
                throw e;
            }
        }

        public List<T> ReadSampleGroups<T>() where T : SampleGroupDO, new()
        {
            return this.DAL.From<T>()
                .Where("Stratum_CN = @p")
                .Read(this.Stratum_CN).ToList();

            //.Read<T>("WHERE Stratum_CN = ?", this.Stratum_CN);
        }

        public List<SampleGroupDO> ReadSampleGroups()
        {
            return this.ReadSampleGroups<SampleGroupDO>();
        }
    }
}