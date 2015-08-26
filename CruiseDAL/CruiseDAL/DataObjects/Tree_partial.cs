using System.Collections.Generic;
using System;

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
                            List<TreeAuditValueDO> tavList = this.DAL.Read<TreeAuditValueDO>("TreeAuditValue",
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
                DAL db = this.DAL;
                List<LogDO> logs = db.Read<LogDO>("Log", "WHERE Tree_CN = ?", this.Tree_CN);
                List<TreeCalculatedValuesDO> tcvList = db.Read<TreeCalculatedValuesDO>("TreeCalculatedValues", "WHERE Tree_CN = ?", this.Tree_CN);
                List<LogStockDO> lsList = db.Read<LogStockDO>("LogStock", "WHERE Tree_CN = ?", this.Tree_CN);

                try
                {
                    db.BeginTransaction();
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
                    db.EndTransaction();
                }
                catch (Exception e)
                {
                    db.CancelTransaction();
                    throw e;
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
            if (inWriteMode) { return; }
            if (name == "TreeDefaultValue" || name == Schema.TREE.TREEDEFAULTVALUE_CN)
            {
                //_treeValidator = null;
                this.ClearErrors();
                this.Validate();
            }
        }



        protected override bool ValidateProperty(string name, object value)
        {
            bool isValid = true;
            if (inWriteMode) { return isValid; }


            isValid = base.ValidateProperty(name, value) && isValid; //perform base validation 
            
            if (name == Schema.TREE.SPECIES)
            {
                isValid = this.ValidateSpecies() && isValid;
            }
            else if (name == Schema.TREE.SAMPLEGROUP_CN)
            {
                isValid = this.ValidateSampleGroup() && isValid;
            }
            else if (name == Schema.TREE.TREEDEFAULTVALUE_CN)
            {
                isValid = this.ValidateTreeDefaultValue() && isValid;
            }
            else if (name == Schema.TREE.LIVEDEAD)
            {
                isValid = this.ValidateLiveDead() && isValid;
            }
            else if (name == Schema.TREE.COUNTORMEASURE)
            {
                isValid = this.ValidateCountOrMeasure() && isValid;
            }

            bool isMeasure = this.CountOrMeasure == "M";

            if (name == Schema.TREE.TOTALHEIGHT || name == Schema.TREE.MERCHHEIGHTPRIMARY || name == Schema.TREE.HEIGHTTOFIRSTLIVELIMB || name == Schema.TREE.UPPERSTEMHEIGHT)
            {
                isValid = this.ValidateHeights(isMeasure) && isValid;
            }
            if (name == Schema.TREE.MERCHHEIGHTPRIMARY || name == Schema.TREE.MERCHHEIGHTSECONDARY)
            {
                isValid = this.ValidateMerchHeightSecondary(isMeasure) && isValid;
            }

            if (name == Schema.TREE.UPPERSTEMHEIGHT || name == Schema.TREE.MERCHHEIGHTPRIMARY)
            {
                isValid = this.ValidateUpperStemHeight(isMeasure) && isValid;
            }
            else if (name == Schema.TREE.DBH || name == Schema.TREE.DRC)
            {
                isValid = this.ValidateDiameters(isMeasure) && isValid;
            }
            else if (name == Schema.TREE.TOPDIBPRIMARY || name == Schema.TREE.TOPDIBSECONDARY)
            {
                isValid = this.ValidateTopDIBSecondary(isMeasure) && isValid;
            }
            else if (name == Schema.TREE.RECOVERABLEPRIMARY || name == Schema.TREE.SEENDEFECTPRIMARY)
            {
                isValid = this.ValidateSeenDefectPrimary(isMeasure) && isValid;
            }

            if (this.CountOrMeasure != null && (this.CountOrMeasure.ToUpper() == "C" || this.CountOrMeasure.ToUpper() == "I" ))
            {
                this.ClearWarnings();
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
                this.AddError(Schema.TREE.SPECIES, "Species is required");
                return false;
            }
            this.RemoveError(Schema.TREE.SPECIES, "Species is required");
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
                this.AddError(Schema.TREE.COUNTORMEASURE, "Count or Measure value invalid");
                return false; 
            }
            else 
            {
                this.RemoveError(Schema.TREE.COUNTORMEASURE, "Count or Measure value invalid");
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
                this.AddError(Schema.TREE.LIVEDEAD, "Live Dead can not be blank");
                return false;
            }
            else
            {
                this.RemoveError(Schema.TREE.LIVEDEAD, "Live Dead can not be blank");
                return true;
            }
        }

        protected bool ValidateMerchHeightSecondary(bool isMeasure)
        {
            if (isMeasure && (MerchHeightSecondary > 0))
            {
                if (MerchHeightSecondary <= MerchHeightPrimary)
                {
                    this.AddError(Schema.TREE.MERCHHEIGHTSECONDARY, "Merch Height Secondary must be greater than Merch Height Primary");
                    return false;
                }
            }
            this.RemoveError(Schema.TREE.MERCHHEIGHTSECONDARY, "Merch Height Secondary must be greater than Merch Height Primary");
            return true;
        }

        protected bool ValidateUpperStemHeight(bool isMeasure)
        {
            if (isMeasure && (UpperStemHeight > 0))
            {
                if (UpperStemHeight < MerchHeightPrimary)
                {
                    this.AddError(Schema.TREE.UPPERSTEMHEIGHT, "Upper Stem Height must be greater or equal to Merch Height Primary");
                    return false;
                }
                else
                {
                    this.RemoveError(Schema.TREE.UPPERSTEMHEIGHT, "Upper Stem Height must be greater or equal to Merch Height Primary");
                    return true;
                }
            }
            this.RemoveError(Schema.TREE.UPPERSTEMHEIGHT, "Upper Stem Height must be greater or equal to Merch Height Primary");
            return true;
        }

        protected bool ValidateUpperStemDiameter(bool isMeasure)
        {
            if (isMeasure && (UpperStemDOB > 0))
            {
                if (UpperStemDOB >= DBH)
                {
                    this.AddError(Schema.TREE.UPPERSTEMDOB, "Upper Stem DOB must be smaller than DBH");
                    return false;
                }
            }
            this.RemoveError(Schema.TREE.UPPERSTEMDOB, "Upper Stem DOB must be smaller than DBH");
            return true;
        }

        protected bool ValidateTopDIBSecondary(bool isMeasure)
        {
            if (isMeasure && (TopDIBSecondary > 0))
            {
                if (TopDIBSecondary > TopDIBPrimary)
                {
                    this.AddError(Schema.TREE.TOPDIBSECONDARY, "Top DIB Secondary must be less Top DIB Primary");
                    return false;
                }
            }
            this.RemoveError(Schema.TREE.TOPDIBSECONDARY, "Top DIB Secondary must be less Top DIB Primary");
            return true;
        }

        protected bool ValidateSeenDefectPrimary(bool isMeasure)
        {
            if (isMeasure && (SeenDefectPrimary > 0))
            {
                if (SeenDefectPrimary < RecoverablePrimary)
                {
                    this.AddError(Schema.TREE.SEENDEFECTPRIMARY, "Seen Defect Primary must be greater than Recoverable Primary");
                    return false;
                }
            }
            this.RemoveError(Schema.TREE.SEENDEFECTPRIMARY, "Seen Defect Primary must be greater than Recoverable Primary");
            return true;
        }


    }
}
