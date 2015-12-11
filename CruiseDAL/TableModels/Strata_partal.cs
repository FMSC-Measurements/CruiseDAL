using System;
using System.Collections.Generic;
using System.Text;
using CruiseDAL.MappingCollections;
using FMSC.ORM.Core;
using CruiseDAL.Schema;

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

        protected override void OnDALChanged(DatastoreRedux newDAL)
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
                if (Array.IndexOf(CruiseDAL.Schema.Constants.CruiseMethods.UNSUPPORTED_METHODS, this.Method) >= 0)
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

        public static List<StratumDO> ReadByUnitCode(DatastoreRedux dal, String code)
        {
            if (dal == null) { return null; }
            return dal.Read<StratumDO>("JOIN CuttingUnitStratum JOIN CuttingUnit WHERE Stratum.Stratum_CN = CuttingUnitStratum.Stratum_CN AND CuttingUnitStratum.CuttingUnit_CN = CuttingUnit.CuttingUnit_CN AND CuttingUnit.Code = ?;", (object)code);
        }

        public static void MirgeStratum(DatastoreRedux dal, CuttingUnitDO unit, StratumDO fromStratum, StratumDO toStratum)
        {
            if (unit.DAL != fromStratum.DAL || unit.DAL != toStratum.DAL)
            {
                throw new InvalidOperationException("can only mirge statum within the same file");
            }

            List<TreeDO> allTreesInUnit = dal.Read<TreeDO>(
                "WHERE CuttingUnit_CN = ? AND Stratum_CN = ?",
                unit.CuttingUnit_CN,
                fromStratum.Stratum_CN);

            foreach (TreeDO tree in allTreesInUnit)
            {
                tree.Stratum = toStratum;
                tree.Save();
            }
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

        public static int DeleteStratum(DatastoreRedux dal, StratumDO stratum)
        {
           // check tree table for data
            if (dal.GetRowCount("Tree", "WHERE Stratum_CN = ?", stratum.Stratum_CN) > 0) { return (-1); }
           // check plot table for data
            if (dal.GetRowCount("Plot", "WHERE Stratum_CN = ?", stratum.Stratum_CN) > 0) { return (-1); }
           // Check Count table for each sample group
           if (dal.GetRowCount("CountTree", "JOIN SampleGroup ON CountTree.SampleGroup_CN = SampleGroup.SampleGroup_CN WHERE SampleGroup.Stratum_CN = ? AND CountTree.TreeCount > 0", stratum.Stratum_CN) > 0) return (-1);

           //Delete sample groups for stratum
           List<SampleGroupDO> allSGInStratum = dal.Read<SampleGroupDO>("WHERE Stratum_CN = ?", stratum.Stratum_CN);
           foreach (SampleGroupDO Sg in allSGInStratum)
           {
              //Delete Count Records for stratum
              List<CountTreeDO> allCountInSG = dal.Read<CountTreeDO>("WHERE SampleGroup_CN = ?", Sg.SampleGroup_CN);
              foreach (CountTreeDO Cnt in allCountInSG)
              {
                  Cnt.Delete();
              }
              Sg.Delete();
           }

           //Delete stratum stats for stratum
           List<StratumStatsDO> allStratumStatsInStratum = dal.Read<StratumStatsDO>(
               "WHERE Stratum_CN = ?", stratum.Stratum_CN);
           foreach (StratumStatsDO StratumStats in allStratumStatsInStratum)
           {
              StratumStats.DeleteStratumStats(dal, StratumStats.StratumStats_CN);
           }

           stratum.Delete();
           
           return (0);
        }

        public static void RecursiveDeleteStratum(StratumDO stratum)
        {
            DatastoreRedux db = stratum.DAL;
            try
            {
                db.BeginTransaction();
                string command =
            String.Format(@"DELETE From CuttingUnitStratum WHERE Stratum_CN = {0};
DELETE FROM Log WHERE Tree_CN IN (SELECT Tree_CN FROM Tree WHERE Tree.Stratum_CN = {0});
DELETE FROM LogStock WHERE Tree_CN IN (SELECT Tree_CN FROM Tree WHERE Tree.Stratum_CN = {0});
DELETE FROM TreeCalculatedValues WHERE Tree_CN IN (SELECT 1 FROM Tree WHERE Tree.Stratum_CN = {0});
DELETE FROM Tree WHERE Stratum_CN = {0};
DELETE FROM Plot WHERE Stratum_CN = {0};
DELETE FROM TreeEstimate WHERE CountTree_CN IN (SELECT CountTree_CN FROM CountTree JOIN SampleGroup USING (SampleGroup_CN) WHERE Stratum_CN = {0});
DELETE FROM CountTree WHERE SampleGroup_CN IN (SELECT SampleGroup_CN FROM SampleGroup WHERE SampleGroup.Stratum_CN = {0});
DELETE FROM SampleGroupTreeDefaultValue WHERE SampleGroup_CN IN (SELECT SampleGroup_CN FROM SampleGroup WHERE SampleGroup.Stratum_CN = {0});
DELETE FROM SampleGroup WHERE Stratum_CN = {0};
DELETE FROM TreeFieldSetup WHERE Stratum_CN = {0};
DELETE FROM LogFieldSetup WHERE Stratum_CN = {0};",
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
            return this.DAL.Read<T>("WHERE Stratum_CN = ?", this.Stratum_CN);
        }

        public List<SampleGroupDO> ReadSampleGroups()
        {
            return this.ReadSampleGroups<SampleGroupDO>();
        }
    }
}
