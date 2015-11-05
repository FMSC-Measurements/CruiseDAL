using FMSC.ORM.TestSupport.TestModels;
using FMSC.ORM.XUnit;
using System;
using System.Linq;
using Xunit;
using Xunit.Abstractions;

namespace FMSC.ORM.Core.EntityModel
{
    public class EntityDescriptionTest : TestClassBase 
    {

        public EntityDescriptionTest(ITestOutputHelper output)
            : base(output)
        { }

        [Fact]
        public void Test_VanillaMultiTypeObject()
        {
            Type t = typeof(POCOMultiTypeObject);
            var doi = new EntityDescription(t);
            VerifyDataObjectInfo(t, doi);
        }


        [Fact]
        public void Test_DOMultiPropType()
        {
            var t = typeof(DOMultiPropType);
            var doi = new EntityDescription(t);
            VerifyDataObjectInfo(t, doi);
        }

        public void LoadDataObjects()
        {
            var types = (from t in System.Reflection.Assembly.GetAssembly(typeof(DataObject)).GetTypes()
                         where t.IsClass && t.Namespace == "CruiseDAL.DataObjects"
                         select t).ToList();

            foreach (Type t in types)
            {
                _output.WriteLine(t.FullName);
                var doi = new EntityDescription(t);

                VerifyDataObjectInfo(t, doi);
            }
        }

        void VerifyDataObjectInfo(Type dataType, EntityDescription doi)
        {
            Assert.NotNull(doi);
            Assert.Equal(dataType, doi.EntityType);
            Assert.False(String.IsNullOrWhiteSpace(doi.SourceName));

            VerifyDataObjectInfoFields(doi);
        }

        void VerifyDataObjectInfoFields(EntityDescription doi)
        {
            Assert.NotNull(doi.Fields.PrimaryKeyField);
            Assert.NotNull(doi.Fields.PrimaryKeyField.Getter);
            Assert.NotNull(doi.Fields.PrimaryKeyField.Setter);

            VerifyField(doi, "ID");
            VerifyField(doi, "StringField");
            VerifyField(doi, "IntField");
            VerifyField(doi, "NIntField");
            VerifyField(doi, "LongField");
            VerifyField(doi, "NLongField");
            VerifyField(doi, "BoolField");
            VerifyField(doi, "NBoolField");
            VerifyField(doi, "FloatField");
            VerifyField(doi, "NFloatField");
            VerifyField(doi, "DoubleField");
            VerifyField(doi, "NDoubleField");
            VerifyField(doi, "GuidField");
            VerifyField(doi, "DateTimeField");

            VerifyField(doi, "PartialyPublicField");
            VerifyField(doi, "PrivateField");
            VerifyField(doi, "CreatedBy");
            VerifyField(doi, "ModifiedBy");

            //verify 
            Assert.DoesNotContain(doi.Fields, x => x.FieldName == "IgnoredField");
            Assert.DoesNotContain(doi.Fields, x => x.FieldName == "PrivateIgnoredField");
            Assert.DoesNotContain(doi.Fields, x => x.FieldName == "PartialyPublicAutomaticField");
            Assert.DoesNotContain(doi.Fields, x => x.FieldName == "PrivateAutomaticField");
            Assert.DoesNotContain(doi.Fields, x => x.FieldName == "IInterface.InterfaceProperty");
            Assert.DoesNotContain(doi.Fields, x => x.FieldName.Contains("InterfaceProperty"));
        }

        void VerifyField(EntityDescription doi, string fieldName)
        {
            _output.WriteLine("Verifying " + fieldName);
            Assert.Contains(doi.Fields, x => x.FieldName == fieldName);

            var field = doi.Fields[fieldName];
            Assert.NotNull(field);
            Assert.NotNull(field.Getter);
            Assert.NotNull(field.Setter);
            Assert.NotNull(field.RunTimeType);
            Assert.True(field.IsPersisted);

            _output.WriteLine("done");

        }
    }
}
