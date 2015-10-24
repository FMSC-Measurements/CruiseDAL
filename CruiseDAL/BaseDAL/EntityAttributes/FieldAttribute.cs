using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CruiseDAL
{
    public enum SepcialFieldType { None = 0, CreatedBy, CreatedDate, ModifiedBy, ModifiedDate, RowVersion };

    [AttributeUsage(AttributeTargets.Property, Inherited = true)]
    public class FieldAttribute : Attribute
    {
        int _oridnal = -1;

        public string FieldName { get; set; }
        public string Source { get; set; }
        public string SQLExpression { get; set; }
        public bool IsPersisted { get; set; }

        public int Ordinal
        {
            get { return _oridnal; }
            set { _oridnal = value; }
        }


        public object DefaultValue { get; set; }
        public string References { get; set; }
        public bool IsDepreciated { get; set; }   
        
        public FieldAttribute(string fieldName)
        {
            this.FieldName = fieldName;
        } 
    }
}
