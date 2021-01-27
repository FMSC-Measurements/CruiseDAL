using FMSC.ORM.EntityModel.Attributes;
using System;
using System.Collections.Generic;

namespace FMSC.ORM.TestSupport.TestModels
{
    [Table(FMSC.ORM.TestSupport.TestSQLConstants.MULTI_PROP_TABLE_NAME)]
    public class POCOMultiTypeObject : IInterface
    {
        private static Bogus.Randomizer _randomizer = new Bogus.Randomizer();
        public static POCOMultiTypeObject CreateWithID(bool nullableSetNull = false)
        {
            var poco = new POCOMultiTypeObject()
            {
                BoolField = _randomizer.Bool(),
                DateTimeField = DateTime.Now,
                NDateTimeField = (nullableSetNull) ? (DateTime?)null : DateTime.Now,
                StrDateTime = (nullableSetNull) ? (string)null : DateTime.Now.ToShortTimeString(),
                DoubleField = _randomizer.Double(),
                FloatField = _randomizer.Float(),
                GuidField = _randomizer.Guid(),
                NGuidField = (nullableSetNull) ? (Guid?)null : _randomizer.Guid(),
                ID = _randomizer.Int(),
                IntField = _randomizer.Int(),
                LongField = _randomizer.Long(),
                NBoolField = (nullableSetNull) ? (bool?)null : _randomizer.Bool(),
                NDoubleField = (nullableSetNull) ? (double?)null : _randomizer.Double(),
                NFloatField = (nullableSetNull) ? (float?)null : _randomizer.Float(),
                NIntField = (nullableSetNull) ? (int?)null : _randomizer.Int(),
                NLongField = (nullableSetNull) ? (long?)null : _randomizer.Long(),
                //RowID = randomizer.Int(),
                StringField = _randomizer.String2(16),
            };
            return poco;
        }

        public static POCOMultiTypeObject CreateWithNullID(bool nullableSetNull = false)
        {
            var poco = new POCOMultiTypeObject()
            {
                BoolField = _randomizer.Bool(),
                DateTimeField = DateTime.Now,
                NDateTimeField = (nullableSetNull) ? (DateTime?)null : DateTime.Now,
                StrDateTime = (nullableSetNull) ? (string)null : DateTime.Now.ToShortTimeString(),
                DoubleField = _randomizer.Double(),
                FloatField = _randomizer.Float(),
                GuidField = _randomizer.Guid(),
                NGuidField = (nullableSetNull) ? (Guid?)null : _randomizer.Guid(),
                IntField = _randomizer.Int(),
                LongField = _randomizer.Long(),
                NBoolField = (nullableSetNull) ? (bool?)null : _randomizer.Bool(),
                NDoubleField = (nullableSetNull) ? (double?)null : _randomizer.Double(),
                NFloatField = (nullableSetNull) ? (float?)null : _randomizer.Float(),
                NIntField = (nullableSetNull) ? (int?)null : _randomizer.Int(),
                NLongField = (nullableSetNull) ? (long?)null : _randomizer.Long(),
                //RowID = randomizer.Int(),
                StringField = _randomizer.String2(16),
            };
            return poco;
        }

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

        [Field(Name = "NGuidField")]
        public Guid? NGuidField { get; set; }

        [Field(Name = "DateTimeField")]
        public DateTime DateTimeField { get; set; }

        [Field(Name = "NDateTimeField")]
        public DateTime? NDateTimeField { get; set; }

        [Field(Name = "StrDateTimeField")]
        public string StrDateTime { get; set; }

        [Field(Name = "PartialyPublicField")]
        public string PartialyPublicField { get; protected set; }

        [Field("EnumField")]
        public TypeCode EnumField { get; set; }

        [Field]
        // test field mapped without needing to explicitly provide field name
        public string ImplicitNamedField { get; set; }

        // test field mapped without a [Field] attribute
        public string AutomaticStringField { get; set; }


        #region non visible fields

        public object ObjectField { get; set; }

        public List<object> ListField { get; set; }

        public object[] ArrayField { get; set; }

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

        public string this[int index]
        {
            get { return ""; }
        }

        #endregion non visible fields
    }
}