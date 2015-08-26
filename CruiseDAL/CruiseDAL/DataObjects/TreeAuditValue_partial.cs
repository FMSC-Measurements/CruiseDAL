using CruiseDAL.MappingCollections;
namespace CruiseDAL.DataObjects
{
    public partial class TreeAuditValueDO : IFieldValidator
    {

        private TreeAuditValueTreeDefaultValueCollection _treeDefaultValueCollection;
        public TreeAuditValueTreeDefaultValueCollection TreeDefaultValues
        {
            get
            {
                if (_treeDefaultValueCollection == null)
                {
                    _treeDefaultValueCollection = new TreeAuditValueTreeDefaultValueCollection(this);
                }
                return _treeDefaultValueCollection;
            }
        }

        protected override void OnDALChanged(DatastoreBase newDAL)
        {
            base.OnDALChanged(newDAL);
            if (_treeDefaultValueCollection != null)
            {
                _treeDefaultValueCollection.DAL = newDAL;
            }
        }

        public override void Delete()
        {
            base.Delete();
            TreeDefaultValues.Populate();
            TreeDefaultValues.Clear();
            TreeDefaultValues.Save();
        }

        protected override void NotifyPropertyChanged(string name)
        {
            base.NotifyPropertyChanged(name);
            if (!inWriteMode && 
                (name == "Field" || name == "Min" || name == "Max" || name == "ValueSet" || name == "Required"))
            {
                _errorMessage = null;
            }
        }

        #region IFieldValidator Members

        private string _errorMessage;
        public string ErrorMessage
        {
            get
            {
                if (_errorMessage == null)
                {
                    _errorMessage = BuildErrorMessage();
                }
                return _errorMessage;
            }
        }

        private string BuildErrorMessage()
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            if (this.Min > 0)
            {
                sb.AppendFormat(null, "{0} should be greater than {1}", this.Field, this.Min);
            }
            if (this.Max > 0)
            {
                sb.AppendFormat(null, "{0} should be less than {1}", this.Field, this.Max);
            }
            if (this.Required)
            {
                sb.AppendFormat(null, "{0} is Required", this.Field);
            }
            if (!string.IsNullOrEmpty(this.ValueSet))
            {
                sb.AppendFormat(null, "Value for {0} is not valid", this.Field);
            }
            return sb.ToString();
        }

                
        public ErrorLevel Level { get { return ErrorLevel.Warning; } }

        public string TableName
        {
            get { return "Tree"; }
        }

        public bool Validate(object sender, object value)
        {
            bool isValid = true;//valid until proven unvalid

            if (this.Required == true && value == null) { isValid = false; }

            if (isValid && this.Min != double.NaN)
            {
                var num = value as float?;
                if (num != null)
                {
                    if (num < this.Min) { isValid = false; }
                }
            }
            if (isValid && this.Max != double.NaN)
            {
                var num = value as float?;
                if (num != null)
                {
                    if (num > this.Max) { isValid = false; }
                }
            }

            if (isValid && value != null && !string.IsNullOrEmpty(ValueSet))
            {
                if (!(ValueSet.IndexOf(value.ToString(), System.StringComparison.Ordinal) >= 0)) { isValid = false; }
            }
            return isValid;
        }

        #endregion

        
    }
}
