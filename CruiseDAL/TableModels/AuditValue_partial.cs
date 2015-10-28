using System.Collections.Generic;
namespace CruiseDAL.DataObjects 
{
    //public partial class AuditValueDO : IFieldValidator
    //{

    //    //public string ErrorMessage { get { return this.ErrorMessage; } }    

    //    public bool Validate(object sender, object value)
    //    {
    //        bool isValid = true;
            
    //        if (this.Required != null)
    //        {
    //            if (this.Required == true && value == null) { isValid = false; }
    //        }

    //        if (isValid && this.Min != null)
    //        {
    //            var num = value as float?;
    //            if (num != null)
    //            {
    //                if (num < this.Min) { isValid = false; }
    //            }
    //        }
    //        if (isValid && this.Max != null)
    //        {
    //            var num = value as float?;
    //            if (num != null)
    //            {
    //                if (num > this.Max) { isValid = false; }
    //            }
    //        }

    //        if (isValid && value != null && !string.IsNullOrEmpty(this.ValueSet))
    //        {
    //            var valueList = new List<string>(this.ValueSet.Split(' '));
    //            if (!valueList.Contains(value.ToString())) { isValid = false; }
    //        }
    //        return isValid;
    //    }

    //    public override void Save()
    //    {
    //        if (DAL == null) { return; }
    //        DAL.GetTableValidator(this.TableName).AddConstraint(this);
    //        base.Save();
    //    }

    //}
}