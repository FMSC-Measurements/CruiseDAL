using System.Collections.Generic;
using System;
using FMSC.ORM.Core.EntityModel;
using FMSC.ORM.Core;
using CruiseDAL.Schema;

namespace CruiseDAL.DataObjects
{
    public partial class TreeDO
    {
        private static Dictionary<long, RowValidator> _validatorLookup = new Dictionary<long, RowValidator>();
        //private RowValidator _treeValidator;
        public RowValidator TreeValidator
        {
            get
            {
                RowValidator _treeValidator = null;
                if(TreeDefaultValue_CN == null || TreeDefaultValue_CN.Value == 0 || this.DAL == null) { return null; }
                if (_treeValidator == null)
                {
                    lock (_validatorLookup)
                    {
                        if (_validatorLookup.ContainsKey(this.TreeDefaultValue_CN.Value) == false)
                        {
                            //this.TreeDefaultValue.TreeAuditValues.Populate();
                            List<TreeAuditValueDO> tavList = this.DAL.Read<TreeAuditValueDO>(
                                @"JOIN TreeDefaultValueTreeAuditValue Using (TreeAuditValue_CN) 
                            WHERE TreeDefaultValue_CN = ?", this.TreeDefaultValue_CN);


                            _treeValidator = new RowValidator();
                            foreach (TreeAuditValueDO tav in tavList)
                            {
                                _treeValidator.Add(tav);
                            }
                            _validatorLookup.Add(TreeDefaultValue_CN.Value, _treeValidator);
                        }
                        else
                        {
                            _treeValidator = _validatorLookup[TreeDefaultValue_CN.Value];
                        }
                    }
                }

                return _treeValidator;
            }
        }

        public override void Delete()
        {
            lock (this)
            {
                var db = this.DAL;
                List<LogDO> logs = db.Read<LogDO>("WHERE Tree_CN = ?", this.Tree_CN);
                List<TreeCalculatedValuesDO> tcvList = db.Read<TreeCalculatedValuesDO>("WHERE Tree_CN = ?", this.Tree_CN);
                List<LogStockDO> lsList = db.Read<LogStockDO>("WHERE Tree_CN = ?", this.Tree_CN);

                db.BeginTransaction();
                try
                {
                    foreach (LogDO l in logs)
                    {
                        l.Delete();
                    }
                    foreach (TreeCalculatedValuesDO tcv in tcvList)
                    {
                        tcv.Delete();
                    }
                    foreach (LogStockDO ls in lsList)
                    {
                        ls.Delete();
                    }
                    base.Delete();
                    db.CommitTransaction();
                }
                catch (Exception)
                {
                    db.RollbackTransaction();
                    throw;
                }
            }
        }


        //public static void RecursiveDeleteTree(TreeDO tree)
        //{
        //    DAL db = tree.DAL;
        //    try
        //    {

        //        db.BeginTransaction();
        //        string command = string.Format("DELETE FROM Log WHERE Tree_cn = {0};", tree.Tree_CN);
        //        db.Execute(command);
        //        tree.Delete();
        //        db.EndTransaction();
        //    }
        //    catch (Exception e)
        //    {
        //        db.CancelTransaction();
        //        throw e;
        //    }
        //}
        

        protected override void NotifyPropertyChanged(string name)
        {
            base.NotifyPropertyChanged(name);
            if (PropertyChangedEventsDisabled) { return; }
            if (name == "TreeDefaultValue" || name == TREE.TREEDEFAULTVALUE_CN)
            {
                //_treeValidator = null;
                this.ErrorCollection.ClearErrors();
                this.Validate();
            }
        }



        protected override bool ValidateProperty(string name, object value)
        {
            bool isValid = true;
            if (PropertyChangedEventsDisabled) { return isValid; }


            isValid = base.ValidateProperty(name, value) && isValid; //perform base validation 
            
            if (name == TREE.SPECIES)
            {
                isValid = this.ValidateSpecies() && isValid;
            }
            else if (name == TREE.SAMPLEGROUP_CN)
            {
                isValid = this.ValidateSampleGroup() && isValid;
            }
            else if (name == TREE.TREEDEFAULTVALUE_CN)
            {
                isValid = this.ValidateTreeDefaultValue() && isValid;
            }
            else if (name == TREE.LIVEDEAD)
            {
                isValid = this.ValidateLiveDead() && isValid;
            }
            else if (name == TREE.COUNTORMEASURE)
            {
                isValid = this.ValidateCountOrMeasure() && isValid;
            }

            bool isMeasure = this.CountOrMeasure == "M";

            if (name == TREE.TOTALHEIGHT || name == TREE.MERCHHEIGHTPRIMARY || name == TREE.HEIGHTTOFIRSTLIVELIMB || name == TREE.UPPERSTEMHEIGHT)
            {
                isValid = this.ValidateHeights(isMeasure) && isValid;
            }
            if (name == TREE.MERCHHEIGHTPRIMARY || name == TREE.MERCHHEIGHTSECONDARY)
            {
                isValid = this.ValidateMerchHeightSecondary(isMeasure) && isValid;
            }

            if (name == TREE.UPPERSTEMHEIGHT || name == TREE.MERCHHEIGHTPRIMARY)
            {
                isValid = this.ValidateUpperStemHeight(isMeasure) && isValid;
            }
            else if (name == TREE.DBH || name == TREE.DRC)
            {
                isValid = this.ValidateDiameters(isMeasure) && isValid;
            }
            else if (name == TREE.TOPDIBPRIMARY || name == TREE.TOPDIBSECONDARY)
            {
                isValid = this.ValidateTopDIBSecondary(isMeasure) && isValid;
            }
            else if (name == TREE.RECOVERABLEPRIMARY || name == TREE.SEENDEFECTPRIMARY)
            {
                isValid = this.ValidateSeenDefectPrimary(isMeasure) && isValid;
            }

            if (this.CountOrMeasure != null && (this.CountOrMeasure.ToUpper() == "C" || this.CountOrMeasure.ToUpper() == "I" ))
            {
                this.ErrorCollection.ClearWarnings();
                return isValid; 
            }


            if (TreeValidator != null)
            {
                isValid = TreeValidator.Validate(this, name, value) && isValid;
            }

            

            return isValid;
        }

        protected bool ValidateSpecies()
        {
            if (string.IsNullOrEmpty(this.Species))
            {
                this.AddError(TREE.SPECIES, "Species is required");
                return false;
            }
            this.RemoveError(TREE.SPECIES, "Species is required");
            return true;
        }

        protected bool ValidateSampleGroup()
        {
            if (this.SampleGroup_CN == null || this.SampleGroup_CN.Value == 0)
            {
                this.AddError("SampleGroup", "Sample Group is required");
                return false;
            }
            this.RemoveError("SampleGroup", "Sample Group is required");
            return true;
        }

        protected bool ValidateTreeDefaultValue()
        {
            if (this.TreeDefaultValue_CN == null || this.TreeDefaultValue_CN.Value == 0)
            {
                this.AddError("TreeDefaultValue", "Tree Default Value is required");
                return false;
            }
            this.RemoveError("TreeDefaultValue", "Tree Default Value is required");
            return true; 
        }

        protected bool ValidateDiameters(bool isMeasure)
        {
            if (!isMeasure || (this.DBH > 0 || this.DRC > 0))
            {
                this.RemoveError("Diameters", "DBH or DRC must be greater than 0");
                return true;
            }
            else
            {
                this.AddError("Diameters", "DBH or DRC must be greater than 0");
                return false;
            }

        }

        protected bool ValidateCountOrMeasure()
        {
            if(String.IsNullOrEmpty(this.CountOrMeasure))
            {
                this.AddError(TREE.COUNTORMEASURE, "Count or Measure value invalid");
                return false; 
            }
            else 
            {
                this.RemoveError(TREE.COUNTORMEASURE, "Count or Measure value invalid");
                return true;
            }
        }


        protected bool ValidateHeights(bool isMeasure)
        {
            if (!isMeasure || 
                (TotalHeight > 0 || MerchHeightPrimary > 0 || MerchHeightSecondary > 0 || UpperStemHeight > 0))
            {
                this.RemoveError("Heights", "HT, MerchHtP, MerchHtS, or UStemHT must be greater than 0");
                return true;
            }
            else
            {
                this.AddError("Heights", "HT, MerchHtP, MerchHtS, or UStemHT must be greater than 0");
                return false;
            }

        }

        protected bool ValidateLiveDead()
        {
            if (String.IsNullOrEmpty(this.LiveDead))
            {
                this.AddError(TREE.LIVEDEAD, "Live Dead can not be blank");
                return false;
            }
            else
            {
                this.RemoveError(TREE.LIVEDEAD, "Live Dead can not be blank");
                return true;
            }
        }

        protected bool ValidateMerchHeightSecondary(bool isMeasure)
        {
            if (isMeasure && (MerchHeightSecondary > 0))
            {
                if (MerchHeightSecondary <= MerchHeightPrimary)
                {
                    this.AddError(TREE.MERCHHEIGHTSECONDARY, "Merch Height Secondary must be greater than Merch Height Primary");
                    return false;
                }
            }
            this.RemoveError(TREE.MERCHHEIGHTSECONDARY, "Merch Height Secondary must be greater than Merch Height Primary");
            return true;
        }

        protected bool ValidateUpperStemHeight(bool isMeasure)
        {
            if (isMeasure && (UpperStemHeight > 0))
            {
                if (UpperStemHeight < MerchHeightPrimary)
                {
                    this.AddError(TREE.UPPERSTEMHEIGHT, "Upper Stem Height must be greater or equal to Merch Height Primary");
                    return false;
                }
                else
                {
                    this.RemoveError(TREE.UPPERSTEMHEIGHT, "Upper Stem Height must be greater or equal to Merch Height Primary");
                    return true;
                }
            }
            this.RemoveError(TREE.UPPERSTEMHEIGHT, "Upper Stem Height must be greater or equal to Merch Height Primary");
            return true;
        }

        protected bool ValidateUpperStemDiameter(bool isMeasure)
        {
            if (isMeasure && (UpperStemDiameter > 0))
            {
                if (UpperStemDiameter >= DBH)
                {
                    this.AddError(TREE.UPPERSTEMDIAMETER, "Upper Stem Diameter must be smaller than DBH");
                    return false;
                }
            }
            this.RemoveError(TREE.UPPERSTEMDIAMETER, "Upper Stem Diameter must be smaller than DBH");
            return true;
        }

        protected bool ValidateTopDIBSecondary(bool isMeasure)
        {
            if (isMeasure && (TopDIBSecondary > 0))
            {
                if (TopDIBSecondary > TopDIBPrimary)
                {
                    this.AddError(TREE.TOPDIBSECONDARY, "Top DIB Secondary must be less Top DIB Primary");
                    return false;
                }
            }
            this.RemoveError(TREE.TOPDIBSECONDARY, "Top DIB Secondary must be less Top DIB Primary");
            return true;
        }

        protected bool ValidateSeenDefectPrimary(bool isMeasure)
        {
            if (isMeasure && (SeenDefectPrimary > 0))
            {
                if (SeenDefectPrimary < RecoverablePrimary)
                {
                    this.AddError(TREE.SEENDEFECTPRIMARY, "Seen Defect Primary must be greater than Recoverable Primary");
                    return false;
                }
            }
            this.RemoveError(TREE.SEENDEFECTPRIMARY, "Seen Defect Primary must be greater than Recoverable Primary");
            return true;
        }


    }
}
