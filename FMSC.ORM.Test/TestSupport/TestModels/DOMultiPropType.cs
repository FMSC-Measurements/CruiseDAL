using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FMSC.ORM;
using FMSC.ORM.EntityModel.Attributes;
using FMSC.ORM.EntityModel;

namespace FMSC.ORM.TestSupport.TestModels
{
    [EntitySource(SourceName = FMSC.ORM.TestSupport.TestSQLConstants.MULTI_PROP_TABLE_NAME)]
    public class DOMultiPropType : DataObject_Base, IInterface
    {
        int _id;
        [PrimaryKeyField(FieldName = "ID")]
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
        [Field(FieldName = "StringField")]
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
        [Field(FieldName = "IntField")]
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
        [Field(FieldName = "NIntField")]
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
        [Field(FieldName = "LongField")]
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
        [Field(FieldName = "NLongField")]
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
        [Field(FieldName = "BoolField")]
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
        [Field(FieldName = "NBoolField")]
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
        [Field(FieldName = "FloatField")]
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
        [Field(FieldName = "NFloatField")]
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
        [Field(FieldName = "DoubleField")]
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
        [Field(FieldName = "NDoubleField")]
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
        [Field(FieldName = "GuidField")]
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
        [Field(FieldName = "DateTimeField")]
        public DateTime DateTimeField
        {
            get { return _dateTimeField; }
            set
            {
                _dateTimeField = value;
                NotifyPropertyChanged(nameof(DateTimeField));
            }
        }

        [Field(FieldName = "PartialyPublicField")]
        public string PartialyPublicField { get; protected set; }

        [Field(FieldName = "PrivateField")]
        private string PrivateField { get; set; }

        string _createdBy;
        [CreatedByField]
        public string CreatedBy
        {
            get { return _createdBy; }
            set
            {
                _createdBy = value;
                NotifyPropertyChanged(nameof(CreatedBy));
            }
        }

        string _modifiedBy;
        [ModifiedByField]
        public string ModifiedBy
        {
            get { return _modifiedBy; }
            set
            {
                _modifiedBy = value;
                NotifyPropertyChanged(nameof(ModifiedBy));
            }
        }

        #region automatic fields
        //TODO test automatic fields
        #endregion


        #region non visible fields
        [IgnoreField]
        public string IgnoredField { get; set; }

        [IgnoreField]
        private string PrivateIgnoredField { get; set; }

        public string PartialyPublicAutomaticField { get; protected set; }

        private string PrivateAutomaticField { get; set; }

        string IInterface.InterfaceProperty
        {
            get; set;
        }
        #endregion

    }
}
