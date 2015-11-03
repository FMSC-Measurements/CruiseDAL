using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FMSCORM;
using FMSCORM.BaseDAL.EntityAttributes;

namespace CruiseDALTest.TestTypes
{
    public class POCOMultiTypeObject
    {
        [PrimaryKeyField(FieldName = "ID")]
        public int ID { get; set; }

        [Field(FieldName = "StringField")]
        public string StringField { get; set; }

        [Field(FieldName = "IntField")]
        public int IntField { get; set; }

        [Field(FieldName = "NIntField")]
        public int? NIntField { get; set; }

        [Field(FieldName = "LongField")]
        public long LongField { get; set; }

        [Field(FieldName = "NLongField")]
        public long? NLongField { get; set; }

        [Field(FieldName = "BoolField")]
        public bool BoolField { get; set; }

        [Field(FieldName = "NBoolField")]
        public bool? NBoolField { get; set; }

        [Field(FieldName = "FloatField")]
        public float FloatField { get; set; }

        [Field(FieldName = "NFloatField")]
        public float? NFloatField { get; set; }

        [Field(FieldName = "DoubleField")]
        public double DoubleField { get; set; }

        [Field(FieldName = "NDoubleField")]
        public double? NDoubleField { get; set; }

        [Field(FieldName = "GuidField")]
        public Guid GuidField { get; set; }

        [Field(FieldName = "DateTimeField")]
        public DateTime DateTimeField { get; set; }

    }
}
