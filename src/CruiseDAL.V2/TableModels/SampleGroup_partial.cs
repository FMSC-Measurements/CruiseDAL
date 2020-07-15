using System;
using System.Collections.Generic;
using System.Text;
using CruiseDAL.MappingCollections;
using FMSC.ORM.Core;
using CruiseDAL.Enums;
using System.Linq;
using CruiseDAL.Schema;

namespace CruiseDAL.DataObjects
{
    public partial class SampleGroupDO
    {
        

       protected TreeDefaultValueCollection _TreeDefaultValues = null;
       public TreeDefaultValueCollection TreeDefaultValues
       {
           get
           {
               if (_TreeDefaultValues == null)
               {
                   _TreeDefaultValues = new TreeDefaultValueCollection(this);
               }
               return _TreeDefaultValues;
           }
       }

       //public TallyMode TallyMode
       //{
       //    get
       //    {


       //        try
       //        {
       //            return (TallyMode)Enum.Parse(typeof(TallyMode), base.TallyMethod, true);
       //        }
       //        catch
       //        {
       //            return TallyMode.Unknown;
       //        }
       //    }
       //    set
       //    {
       //        base.TallyMethod = value.ToString(); 
       //    } 
       //}

       protected override void OnDALChanged(DAL newDAL)
       {
           base.OnDALChanged(newDAL);
           if (_TreeDefaultValues != null)
           {
               _TreeDefaultValues.DAL = newDAL;
           }
       }

       #region custom deletion 
       public override void Delete()
       {
           this.TreeDefaultValues.Populate();
           this.TreeDefaultValues.Clear();
           this.TreeDefaultValues.Save();
           base.Delete();
       }

        
   

        public static void RecutsiveDeleteSampleGroup(SampleGroupDO sg)
        {
            var db = sg.DAL;
            if (db == null) { return; }

            db.BeginTransaction();
            try
            {
                
                string command = String.Format(@"
            DELETE FROM Log WHERE Tree_CN IN (SELECT Tree_CN FROM Tree WHERE Tree.SampleGroup_CN = {0});
            DELETE FROM LogStock WHERE Tree_CN IN (SELECT Tree_CN FROM Tree WHERE Tree.SampleGroup_CN = {0});
            DELETE FROM TreeCalculatedValues WHERE Tree_CN IN (SELECT Tree_CN FROM Tree WHERE Tree.SampleGroup_CN = {0});
            DELETE FROM Tree WHERE SampleGroup_CN = {0};
            DELETE FROM TreeEstimate WHERE CountTree_CN IN (SELECT CountTree_CN FROM CountTree WHERE SampleGroup_CN = {0});
            DELETE FROM CountTree WHERE SampleGroup_CN = {0};
            DELETE FROM SampleGroupTreeDefaultValue WHERE SampleGroup_CN = {0};
            DELETE FROM FixCNTTallyPopulation WHERE SampleGroup_CN = {0};
            DELETE FROM SamplerState WHERE SampleGroup_CN = {0};", sg.SampleGroup_CN);
                db.Execute(command);
                sg.Delete();
                db.CommitTransaction();
            }
            catch (Exception e)
            {
                db.RollbackTransaction();
                throw e; 
            }
        }

        public int DeleteSampleGroup(Datastore dal, long? SampleGroup_CN)
        {
            // check tree table for data
            if (dal.GetRowCount("Tree", "WHERE SampleGroup_CN = @p1", SampleGroup_CN) > 0) return (-1);
            // Check Count table for each sample group
            if (dal.GetRowCount("CountTree", "WHERE SampleGroup_CN = @p1 AND TreeCount > 0", SampleGroup_CN) > 0) return (-1);

            //Delete Count Records for stratum
            var allCountInSg = dal.From<CountTreeDO>()
                    .Where("SampleGroup_CN = @p1")
                    .Read(SampleGroup_CN).ToList();
                                
            foreach (CountTreeDO Cnt in allCountInSg)
            { Cnt.Delete(); }
             
            Delete();

            return (0);
        }
        #endregion

       protected override bool ValidateProperty(string name, object value)
       {
           bool isValid = true;
           if (PropertyChangedEventsDisabled) { return isValid; }
           isValid = base.ValidateProperty(name, value);
           if (name == "CutLeave")
           {
               var cl = value as string;
               if (string.IsNullOrEmpty(cl))
               {
                   AddError(name, "Cut/Leave can't be empty");
                   isValid = false;
               }
               else
               {
                   RemoveError(name, "Cut/Leave can't be empty");
               }
           }
           else if (name == "UOM")
           {
               var uom = value as string;
               if (string.IsNullOrEmpty(uom))
               {
                   AddError(name, "UOM can't be empty");
                   isValid = false;
               }
               else
               {
                   RemoveError(name, "UOM can't be empty");
                   
               }
           }
           else if (name == "PrimaryProduct")
           {
               var pp = value as string;
               if (string.IsNullOrEmpty(pp))
               {
                   AddError(name, "PrimaryProduct can't be empty");
                   isValid = false;
               }
               else
               {
                   RemoveError(name, "PrimaryProduct can't be empty");
               }
           }
           else if (name == "SamplingFrequency")
           {
               if (Stratum == null || Stratum.Method == "FCM" || Stratum.Method == "PCM")
               {
                   //allow sampling frequency to be 0 for FCM and PCM
                   RemoveError(name, "Frequency can't be 0");
               }
               else if (CanEnableFrequency(Stratum))
               {
                   if (SamplingFrequency == 0)
                   {
                        AddError(name, "Frequency can't be 0");
                       isValid = false;
                   }
                   else
                   {
                        RemoveError(name, "Frequency can't be 0");
                   }
               }
           }
           else if (name == "KZ")
           {
               if (CanEnableKZ(Stratum) && KZ <= 0)
               {
                    AddError(name, "KZ can't be 0");
                   isValid = false;
               }
               else
               {
                    RemoveError(name, "KZ can't be 0");
               }
           }
           else if (name == "BigBAF")
           {
               if (CanEnableBigBAF(Stratum) && (SamplingFrequency > 0 && BigBAF > 0))
               {
                    AddError(name, "Sampling Frequency and BigBAF, only one can be greater than 0");
                   isValid = false;
               }
               else
               {
                    RemoveError(name, "Sampling Frequency and BigBAF, only one can be greater than 0");
               }
           }

           return isValid;
       }

       public TallyMode GetSampleGroupTallyMode()
       {
           TallyMode mode = TallyMode.Unknown;
           if (base.DAL.GetRowCount("CountTree", "WHERE SampleGroup_CN = @p1", SampleGroup_CN) == 0)
           {
               return TallyMode.None;
           }

           if (base.DAL.GetRowCount("CountTree",
               "WHERE SampleGroup_CN = @p1 AND ifnull(TreeDefaultValue_CN, 0) == 0",
               SampleGroup_CN) > 0)
           {
               mode = mode | TallyMode.BySampleGroup;
           }
           if (base.DAL.GetRowCount("CountTree",
               "WHERE SampleGroup_CN = @p1 AND TreeDefaultValue_CN NOT NULL AND TreeDefaultValue_CN > 0",
               this.SampleGroup_CN) > 0)
           {
               mode = mode | TallyMode.BySpecies;
           }
           if (base.DAL.GetRowCount("CountTree",
               "WHERE SampleGroup_CN = @p1 AND TreeCount > 0", this.SampleGroup_CN) > 0)
           {
               mode = mode | TallyMode.Locked;
           }

           return mode;
       }

       #region validation helper methods

        /// <summary>
        /// Validates that all tree defaults have the same primary product as sample group
        /// </summary>
        /// <param name="error">error message</param>
        /// <returns>false if validation fails</returns>
       public bool ValidatePProdOnTDVs(ref string error)
       {
           foreach (TreeDefaultValueDO tdv in this.TreeDefaultValues)
           {
               if (tdv.PrimaryProduct != this.PrimaryProduct)
               {
                   error += String.Format("Tree Defaults have been selected with product codes different than the product code for the SG {0} in Stratum {1}. Please correct this.\r\n", 
                       this.Code, 
                       this.Stratum.Code);
                   return false;
               }

           }
           return true;
       }

       public static bool ValidateSetup(SampleGroupDO sg, StratumDO st, out string error)
       {
           bool isValid = true; 
           error = null;
           sg.Validate();
           string pre = "Stratum: " + st.Code + " IN SG: " + sg.Code + " -"; 
           if (sg.HasErrors())
           {
               isValid = false;
               error += pre + sg.Error;
           }
           if (String.IsNullOrEmpty(sg.Code))
           {
               isValid = false;
               error += pre + " Code can't be empty";
           }
           if (sg.TreeDefaultValues.Count == 0)
           {
               isValid = false;
               error += pre + " No Tree Defaults Selected";
           }
           
           isValid  = sg.ValidatePProdOnTDVs(ref error) && isValid;
           return isValid;
       }

       public bool CanEditSampleGroup()
       {
           if (base.DAL == null) { return true; }
           return base.DAL.GetRowCount("Tree", "WHERE SampleGroup_CN = @p1", this.SampleGroup_CN) == 0
               && base.DAL.GetRowCount("CountTree", "WHERE SampleGroup_CN = @p1 AND TreeCount > 0", this.SampleGroup_CN) == 0;
       }

       public static bool CanEnableFrequency(StratumDO stratum)
       {
           if (stratum == null) { return false; }
           if (stratum.Method == null) { return true; }
           if (stratum.Method.IndexOf("3P") != -1)
           {
               return false;
           }
           if (stratum.Method == CruiseMethods.PNT 
                || stratum.Method == CruiseMethods.FIX 
                || stratum.Method == CruiseMethods.H_PCT 
                || stratum.Method == CruiseMethods.FIXCNT)
           {
               return false;
           }
           return true;
       }

       public static bool CanEnableBigBAF(StratumDO stratum)
       {
           if (stratum == null) { return false; }
           if (stratum.Method == null) { return true; }
           if (stratum.Method == "PCM")
           {
               return true;
           }
           return false;
       }

       public static bool CanEnableKZ(StratumDO stratum)
       {
           if (stratum == null) { return false; }
           if (stratum.Method == null) { return true; }
           if (Array.IndexOf(Schema.CruiseMethods.THREE_P_METHODS, stratum.Method) >= 0)
           {
               return true;
           }
           return false;
       }

       public static bool CanEnableIFreq(StratumDO stratum)
       {
           if (stratum == null) { return false; }
           if (stratum.Method == null) { return true; }
           string method = stratum.Method;
           if (method == "3P" || method == "S3P" || method == "STR")
           {
               return true;
           }
           return false;
       }

       public static bool CanChangeSamplerType(SampleGroupDO sg)
       {
           return String.IsNullOrEmpty(sg.SampleSelectorState);
       }
       #endregion

    }
}
