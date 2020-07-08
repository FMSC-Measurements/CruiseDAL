using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FMSC.ORM;
using FMSC.ORM.EntityModel.Attributes;
using FMSC.ORM.EntityModel;

namespace FMSC.ORM.TestSupport.TestModels
{
    [Table(FMSC.ORM.TestSupport.TestSQLConstants.MULTI_PROP_TABLE_NAME)]
    public class DOMultiPropType : INPC_Base, IInterface
    {
        int _id;

        [PrimaryKeyField(Name = "ID")]
        public int ID
        {
            get { return _id; }
            set
            {
                _id = value;
                NotifyPropertyChanged(nameof(ID));
            }
        }

        string _stringField;

        [Field(Name = "StringField")]
        public string StringField
        {
            get { return _stringField; }
            set
            {
                _stringField = value;
                NotifyPropertyChanged(nameof(StringField));
            }
        }

        int _intField;

        [Field(Name = "IntField")]
        public int IntField
        {
            get { return _intField; }
            set
            {
                _intField = value;
                NotifyPropertyChanged(nameof(IntField));
            }
        }

        int? _nIntField;

        [Field(Name = "NIntField")]
        public int? NIntField
        {
            get { return _nIntField; }
            set
            {
                _nIntField = value;
                NotifyPropertyChanged(nameof(NIntField));
            }
        }

        long _longField;

        [Field(Name = "LongField")]
        public long LongField
        {
            get { return _longField; }
            set
            {
                _longField = value;
                NotifyPropertyChanged(nameof(LongField));
            }
        }

        long? _nLongField;

        [Field(Name = "NLongField")]
        public long? NLongField
        {
            get { return _nLongField; }
            set
            {
                _nLongField = value;
                NotifyPropertyChanged(nameof(NLongField));
            }
        }

        bool _boolField;

        [Field(Name = "BoolField")]
        public bool BoolField
        {
            get { return _boolField; }
            set
            {
                _boolField = value;
                NotifyPropertyChanged(nameof(BoolField));
            }
        }

        bool? _nBoolField;

        [Field(Name = "NBoolField")]
        public bool? NBoolField
        {
            get { return _nBoolField; }
            set
            {
                _nBoolField = value;
                NotifyPropertyChanged(nameof(NBoolField));
            }
        }

        float _floatField;

        [Field(Name = "FloatField")]
        public float FloatField
        {
            get { return _floatField; }
            set
            {
                _floatField = value;
                NotifyPropertyChanged(nameof(FloatField));
            }
        }

        float? _nFloatField;

        [Field(Name = "NFloatField")]
        public float? NFloatField
        {
            get { return _nFloatField; }
            set
            {
                _nFloatField = value;
                NotifyPropertyChanged(nameof(NFloatField));
            }
        }

        double _doubleField;

        [Field(Name = "DoubleField")]
        public double DoubleField
        {
            get { return _doubleField; }
            set
            {
                _doubleField = value;
                NotifyPropertyChanged(nameof(DoubleField));
            }
        }

        double? _nDoubleField;

        [Field(Name = "NDoubleField")]
        public double? NDoubleField
        {
            get { return _nDoubleField; }
            set
            {
                _nDoubleField = value;
                NotifyPropertyChanged(nameof(NDoubleField));
            }
        }

        Guid _guidField;

        [Field(Name = "GuidField")]
        public Guid GuidField
        {
            get { return _guidField; }
            set
            {
                _guidField = value;
                NotifyPropertyChanged(nameof(GuidField));
            }
        }

        DateTime _dateTimeField;

        [Field(Name = "DateTimeField")]
        public DateTime DateTimeField
        {
            get { return _dateTimeField; }
            set
            {
                _dateTimeField = value;
                NotifyPropertyChanged(nameof(DateTimeField));
            }
        }

        [Field(Name = "PartialyPublicField")]
        public string PartialyPublicField { get; protected set; }

        [Field(Name = "PrivateField")]
        private string PrivateField { get; set; }

        [Field]
        // test field mapped without needing to explicitly provide field name
        public string ImplicitNamedField { get; set; }

        #region automatic fields

        public string AutomaticStringField { get; set; }

        public string PartialyPublicAutomaticField { get; protected set; }

        #endregion automatic fields

        #region non visible fields

        [IgnoreField]
        public string IgnoredField { get; set; }

        [IgnoreField]
        private string PrivateIgnoredField { get; set; }


        private string PrivateAutomaticField { get; set; }

        string IInterface.InterfaceProperty
        {
            get; set;
        }

        #endregion non visible fields
    }
}