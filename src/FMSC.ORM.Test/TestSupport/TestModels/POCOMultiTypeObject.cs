using FMSC.ORM.EntityModel.Attributes;
using System;

namespace FMSC.ORM.TestSupport.TestModels
{
    [EntitySource(FMSC.ORM.TestSupport.TestSQLConstants.MULTI_PROP_TABLE_NAME)]
    public class POCOMultiTypeObject : IInterface
    {
        [PrimaryKeyField(Name = "ID")]
        public int? ID { get; set; }

        //[Field(Name = "RowID")]
        //public int RowID { get; set; }

        [Field(Name = "StringField")]
        public string StringField { get; set; }

        [Field(Name = "IntField")]
        public int IntField { get; set; }

        [Field(Name = "NIntField")]
        public int? NIntField { get; set; }

        [Field(Name = "LongField")]
        public long LongField { get; set; }

        [Field(Name = "NLongField")]
        public long? NLongField { get; set; }

        [Field(Name = "BoolField")]
        public bool BoolField { get; set; }

        [Field(Name = "NBoolField")]
        public bool? NBoolField { get; set; }

        [Field(Name = "FloatField")]
        public float FloatField { get; set; }

        [Field(Name = "NFloatField")]
        public float? NFloatField { get; set; }

        [Field(Name = "DoubleField")]
        public double DoubleField { get; set; }

        [Field(Name = "NDoubleField")]
        public double? NDoubleField { get; set; }

        [Field(Name = "GuidField")]
        public Guid GuidField { get; set; }

        [Field(Name = "DateTimeField")]
        public DateTime DateTimeField { get; set; }

        [Field(Name = "PartialyPublicField")]
        public string PartialyPublicField { get; protected set; }

        public string AutomaticStringField { get; set; }



        #region non visible fields

        [Field(Name = "PrivateField")]
        private string PrivateField { get; set; }

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

        #endregion non visible fields
    }
}