using FluentAssertions;
using FMSC.ORM.EntityModel.Attributes;
using FMSC.ORM.TestSupport.TestModels;
using System;
using System.Linq;
using Xunit;
using Xunit.Abstractions;

namespace FMSC.ORM.EntityModel.Support
{
    public class EntityDescriptionTest : TestBase
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

        

        private void VerifyDataObjectInfo(Type dataType, EntityDescription doi)
        {
            doi.Should().NotBeNull();
            doi.EntityType.Should().Be(dataType);
            doi.SourceName.Should().NotBeNullOrWhiteSpace();

            VerifyDataObjectInfoFields(doi);
        }

        private void VerifyDataObjectInfoFields(EntityDescription doi)
        {
            Assert.NotNull(doi.Fields.PrimaryKeyField);
            //Assert.NotNull(doi.Fields.PrimaryKeyField.Property.Getter);
            //Assert.NotNull(doi.Fields.PrimaryKeyField.Property.Setter);

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
            //VerifyField(doi, "PrivateField");

            //verify non visible field
            VerifyNonvisableField(doi, "IgnoredField");
            VerifyNonvisableField(doi, "PartialyPublicAutomaticField");

            VerifyNonvisableField(doi, "PrivateIgnoredField", true);
            VerifyNonvisableField(doi, "PrivateAutomaticField", true);
            VerifyNonvisableField(doi, "IInterface.InterfaceProperty", true);
            VerifyNonvisableField(doi, "InterfaceProperty", true);
        }

        private void VerifyField(EntityDescription doi, string fieldName)
        {
            Output.WriteLine("Verifying " + fieldName);
            Assert.Contains(doi.Fields, x => x.Name == fieldName);

            var field = doi.Fields[fieldName];
            Assert.NotNull(field);
            //Assert.NotNull(field.Property.Getter);
            //Assert.NotNull(field.Property.Setter);
            //Assert.NotNull(field.RunTimeType);
            Assert.True(field.PersistanceFlags.HasFlag(PersistanceFlags.OnUpdate));
            Assert.True(field.PersistanceFlags.HasFlag(PersistanceFlags.OnInsert));

            Output.WriteLine("done");
        }

        private void VerifyNonvisableField(EntityDescription doi, string fieldName)
        {
            VerifyNonvisableField(doi, fieldName, false);
        }

        private void VerifyNonvisableField(EntityDescription doi, string fieldName, bool isPrivate)
        {
            Assert.DoesNotContain(doi.Fields, x => x.Name == fieldName);

            if (isPrivate)
            {
                Assert.DoesNotContain(doi.Properties, x => x.Key == fieldName);
            }
            else
            {
                Assert.Contains(doi.Properties, x => x.Key == fieldName);
            }
        }
    }
}