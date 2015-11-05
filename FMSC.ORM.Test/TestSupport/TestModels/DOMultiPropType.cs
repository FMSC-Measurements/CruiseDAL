using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FMSC.ORM;
using FMSC.ORM.Core.EntityAttributes;
using FMSC.ORM.Core.EntityModel;

namespace FMSC.ORM.TestSupport.TestModels
{
    [SQLEntity(SourceName = FMSC.ORM.TestSupport.TestSQLConstants.MULTI_PROP_TABLE_NAME)]
    public class DOMultiPropType : DataObject
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


        [Field(FieldName = "PartialyPublicField")]
        public string PartialyPublicField { get; protected set; }

        [Field(FieldName = "PrivateField")]
        private string PrivateField { get; set; }

        [CreatedByField]
        public string CreatedBy { get; set; }

        [ModifiedByField]
        public string ModifiedBy { get; set; }

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
        #endregion

        public override void SetValues(DataObject obj)
        {
            throw new NotImplementedException();
        }
    }
}
