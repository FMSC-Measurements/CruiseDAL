using System;
using System.Collections.Generic;
using System.Text;
using CruiseDAL.MappingCollections;
using CruiseDAL.Enums;

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

       protected override void OnDALChanged(DatastoreBase newDAL)
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
            DAL db = sg.DAL;
            if (db == null) { return; }
            try
            {
                db.BeginTransaction();
                string command = String.Format(@"
            DELETE FROM Log WHERE Tree_CN IN (SELECT Tree_CN FROM Tree WHERE Tree.SampleGroup_CN = {0});
            DELETE FROM LogStock WHERE Tree_CN IN (SELECT Tree_CN FROM Tree WHERE Tree.SampleGroup_CN = {0});
            DELETE FROM TreeCalculatedValues WHERE Tree_CN IN (SELECT Tree_CN FROM Tree WHERE Tree.SampleGroup_CN = {0});
            DELETE FROM Tree WHERE SampleGroup_CN = {0};
            DELETE FROM TreeEstimate WHERE CountTree_CN IN (SELECT CountTree_CN FROM CountTree WHERE SampleGroup_CN = {0});
            DELETE FROM CountTree WHERE SampleGroup_CN = {0};
            DELETE FROM SampleGroupTreeDefaultValue WHERE SampleGroup_CN = {0};", sg.SampleGroup_CN);
                db.Execute(command);
                sg.Delete();
                db.EndTransaction();
            }
            catch (Exception e)
            {
                db.CancelTransaction();
                throw e; 
            }
        }

       public int DeleteSampleGroup(DAL dal, long? SampleGroup_CN)
       {
          // check tree table for data
          if (dal.GetRowCount("Tree", "WHERE SampleGroup_CN = ?", SampleGroup_CN) > 0) return (-1);
          // Check Count table for each sample group
          if (dal.GetRowCount("CountTree", "WHERE SampleGroup_CN = ? AND TreeCount > 0", SampleGroup_CN) > 0) return (-1);

          //Delete Count Records for stratum
          List<CountTreeDO> allCountInSg = dal.Read<CountTreeDO>("CountTree", "WHERE SampleGroup_CN = ?", SampleGroup_CN);
          foreach (CountTreeDO Cnt in allCountInSg)
             Cnt.Delete();

          Delete();

          return (0);
       }
       #endregion

       protected override bool ValidateProperty(string name, object value)
       {
           bool isValid = true;
           if (base.inWriteMode) { return isValid; }
           isValid = base.ValidateProperty(name, value);
           if (name == "CutLeave")
           {
               string cl = value as string;
               if (string.IsNullOrEmpty(cl))
               {
                   base.AddError(name, "Cut/Leave can't be empty");
                   isValid = false;
               }
               else
               {
                   base.RemoveError(name, "Cut/Leave can't be empty");
                   
               }
           }
           else if (name == "UOM")
           {
               string uom = value as string;
               if (string.IsNullOrEmpty(uom))
               {
                   base.AddError(name, "UOM can't be empty");
                   isValid = false;
               }
               else
               {
                   base.RemoveError(name, "UOM can't be empty");
                   
               }
           }
           else if (name == "PrimaryProduct")
           {
               string pp = value as string;
               if (string.IsNullOrEmpty(pp))
               {
                   base.AddError(name, "PrimaryProduct can't be empty");
                   isValid = false;
               }
               else
               {
                   base.RemoveError(name, "PrimaryProduct can't be empty");
               }
           }
           else if (name == "SamplingFrequency")
           {
               if (this.Stratum == null || this.Stratum.Method == "FCM" || this.Stratum.Method == "PCM")
               {
                   //allow sampling frequency to be 0 for fcm and pcm
                   base.RemoveError(name, "Frequency can't be 0");
               }
               else if (CanEnableFrequency(this.Stratum))
               {
                   if (this.SamplingFrequency == 0)
                   {
                       this.AddError(name, "Frequency can't be 0");
                       isValid = false;
                   }
                   else
                   {
                       this.RemoveError(name, "Frequency can't be 0");
                   }
               }
           }
           else if (name == "KZ")
           {
               if (CanEnableKZ(this.Stratum) && this.KZ <= 0)
               {
                   this.AddError(name, "KZ can't be 0");
                   isValid = false;
               }
               else
               {
                   this.RemoveError(name, "KZ can't be 0");
               }
           }
           else if (name == "BigBAF")
           {
               if (CanEnableBigBAF(this.Stratum) && (this.SamplingFrequency > 0 && this.BigBAF > 0))
               {
                   this.AddError(name, "Sampling Frequency and BigBAF, only one can be greater than 0");
                   isValid = false;
               }
               else
               {
                   this.RemoveError(name, "Sampling Frequency and BigBAF, only one can be greater than 0");
               }
           }

           return isValid;
       }

       public CruiseDAL.Enums.TallyMode GetSampleGroupTallyMode()
       {
           TallyMode mode = TallyMode.Unknown;
           if (base.DAL.GetRowCount("CountTree", "WHERE SampleGroup_CN = ?", this.SampleGroup_CN) == 0)
           {
               return TallyMode.None;
           }

           if (base.DAL.GetRowCount("CountTree",
               "WHERE SampleGroup_CN = ? AND ifnull(TreeDefaultValue_CN, 0) == 0",
               this.SampleGroup_CN) > 0)
           {
               mode = mode | TallyMode.BySampleGroup;
           }
           if (base.DAL.GetRowCount("CountTree",
               "WHERE SampleGroup_CN = ? AND TreeDefaultValue_CN NOT NULL AND TreeDefaultValue_CN > 0",
               this.SampleGroup_CN) > 0)
           {
               mode = mode | TallyMode.BySpecies;
           }
           if (base.DAL.GetRowCount("CountTree",
               "WHERE SampleGroup_CN = ? AND TreeCount > 0", this.SampleGroup_CN) > 0)
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
               error += pre + " No Tree Tree Defaults Selected";
           }
           
           isValid  = sg.ValidatePProdOnTDVs(ref error) && isValid;
           return isValid;
       }

       public bool CanEditSampleGroup()
       {
           if (base.DAL == null) { return true; }
           return base.DAL.GetRowCount("Tree", "WHERE SampleGroup_CN = ?", this.SampleGroup_CN) == 0
               && base.DAL.GetRowCount("CountTree", "WHERE SampleGroup_CN = ? AND TreeCount > 0", this.SampleGroup_CN) == 0;
       }

       public static bool CanEnableFrequency(StratumDO stratum)
       {
           if (stratum == null) { return false; }
           if (stratum.Method == null) { return true; }
           if (stratum.Method.IndexOf("3P") != -1)
           {
               return false;
           }
           if (stratum.Method == "PNT" || stratum.Method == "FIX" ||
               stratum.Method == "100")
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
           if (Array.IndexOf(Schema.Constants.CruiseMethods.THREE_P_METHODS, stratum.Method) >= 0)
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
