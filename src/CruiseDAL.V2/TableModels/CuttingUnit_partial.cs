﻿using System;
using System.Collections.Generic;
using System.Text;
using CruiseDAL.MappingCollections;
using FMSC.ORM.Core;
using System.Linq;

namespace CruiseDAL.DataObjects
{
    public partial class CuttingUnitDO
    {
        protected StratumCollection _Strata = null;
        public StratumCollection Strata
        {
            get
            {
                if (_Strata == null)
                {
                    _Strata = new StratumCollection(this);
                }
                return _Strata;
                

            }
            
        }

        protected override void OnDALChanged(DAL newDAL)
        {
            base.OnDALChanged(newDAL);
            if (_Strata != null)
            {
                _Strata.DAL = newDAL;
            }
        }

        public override void Delete()
        {
            base.Delete();
            this.Strata.Populate();
            this.Strata.Clear();
            this.Strata.Save();
        }

        public static List<CuttingUnitDO> ReadByStratumCode(Datastore dal, String code)
        {
            if (dal == null) { return null; }
            return dal.From<CuttingUnitDO>()
                .Join("CuttingUnitStratum", "USING (CuttingUnit_CN)")
                .Join("Stratum", "Using (Stratum_CN)")
                .Where("Stratum.Code = @p1")
                .Read(code).ToList();

            //.Read<CuttingUnitDO>("JOIN CuttingUnitStratum JOIN Stratum WHERE CuttingUnit.CuttingUnit_CN = CuttingUnitStratum.CuttingUnit_CN AND CuttingUnitStratum.Stratum_CN = Stratum.Stratum_CN AND Stratum.Code = ?;", (object)code);
        }

        public List<StratumDO> ReadStrata()
        {
            return this.ReadStrata<StratumDO>();
        }

        public List<T> ReadStrata<T>() where T : StratumDO, new()
        {
            return this.DAL.From<T>()
                .Join("CuttingUnitStratum", "USING (Stratum_CN)")
                .Where("CuttingUnit_CN = @p1")
                .Read(this.CuttingUnit_CN).ToList(); 

                //.Read<T>("JOIN CuttingUnitStratum USING (Stratum_CN) WHERE CuttingUnit_CN = ?", this.CuttingUnit_CN);
        }

        

        public static void RecursiveDelete(CuttingUnitDO unit)
        {
            var dal = unit.DAL;
            string commandFormat =
@"Delete From CuttingUnitStratum WHERE CuttingUnit_CN = {0};
DELETE FROM Log WHERE EXISTS (SELECT 1 FROM Tree WHERE Tree.Tree_CN = Log.Tree_CN AND Tree.CuttingUnit_CN = {0});
DELETE FROM LogStock WHERE EXISTS (SELECT 1 FROM Tree WHERE Tree.Tree_CN = LogStock.Tree_CN AND Tree.CuttingUnit_CN = {0});
DELETE FROM TreeCalculatedValues WHERE EXISTS (SELECT 1 FROM Tree WHERE Tree.Tree_CN = TreeCalculatedValues.Tree_CN AND Tree.CuttingUnit_CN = {0});
DELETE FROM Tree WHERE CuttingUnit_CN = {0};
DELETE FROM Plot WHERE CuttingUnit_CN = {0};
DELETE FROM CountTree WHERE CuttingUnit_CN = {0};";

            dal.BeginTransaction();
            try
            {
                
                dal.Execute(String.Format(commandFormat, unit.CuttingUnit_CN));
                unit.Delete();

                dal.CommitTransaction();
            }
            catch (Exception e)
            {
                dal.RollbackTransaction();
                throw e;
            }
        }
        
    }
}