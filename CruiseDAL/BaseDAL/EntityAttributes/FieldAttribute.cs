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
        string _FieldName;
        string _alias;
        string _mapExpression;
        bool _persist;
        //bool _isPrimaryKey;
        //bool _allowNull;
        DbType _dataType;

        public DbType DataType
        {
            get { return _dataType; }
            set
            {
                _dataType = value;

            }
        }

        public SepcialFieldType SpecialFieldType { get; set; }
        //TODO the fallowing properties do not affect behavior, but could 
        //could be use useful for new puropses later on
        public object DefaultValue { get; set; }
        public bool IsPrimaryKey { get; set; }
        public bool IsUnique { get; set; }
        public string References { get; set; }
        public bool IsDepreciated { get; set; }
        public bool AllowNull { get; set; }
        public int MinValue { get; set; }
        public int MaxValue { get; set; }




        public string FieldName
        {
            get { return _FieldName; }
            set { _FieldName = value; }
        }

        public string Alias
        {
            get { return _alias; }
            set { _alias = value; }
        }

        public string MapExpression
        {
            get { return _mapExpression; }
            set { _mapExpression = value; }
        }

        public bool IsPersisted
        {
            get { return _persist && !String.IsNullOrEmpty(_FieldName); }
            set { _persist = value; }
        }

    }
}
