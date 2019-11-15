using FluentAssertions;
using FMSC.ORM.EntityModel.Attributes;
using FMSC.ORM.Test;
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
        

        private void VerifyDataObjectInfo(Type dataType, EntityDescription doi)
        {
            doi.Should().NotBeNull();
            doi.EntityType.Should().Be(dataType);
            doi.SourceName.Should().NotBeNullOrWhiteSpace();

            VerifyDataObjectInfoFields(doi);
        }

        private void VerifyDataObjectInfoFields(EntityDescription doi)
        {
            var primaryKeyField = doi.Fields.PrimaryKeyField;
            primaryKeyField.Should().NotBeNull();
            primaryKeyField.Name.Should().NotBeNullOrEmpty();

            doi.Fields.PrimaryKeyField.Property.Getter.Should().NotBeNull();
            doi.Fields.PrimaryKeyField.Property.Setter.Should().NotBeNull();

            VerifyField(doi, nameof(POCOMultiTypeObject.ID));
            VerifyField(doi, nameof(POCOMultiTypeObject.StringField));
            VerifyField(doi, nameof(POCOMultiTypeObject.IntField));
            VerifyField(doi, nameof(POCOMultiTypeObject.NIntField));
            VerifyField(doi, nameof(POCOMultiTypeObject.LongField));
            VerifyField(doi, nameof(POCOMultiTypeObject.NLongField));
            VerifyField(doi, nameof(POCOMultiTypeObject.BoolField));
            VerifyField(doi, nameof(POCOMultiTypeObject.NBoolField));
            VerifyField(doi, nameof(POCOMultiTypeObject.FloatField));
            VerifyField(doi, nameof(POCOMultiTypeObject.NFloatField));
            VerifyField(doi, nameof(POCOMultiTypeObject.DoubleField));
            VerifyField(doi, nameof(POCOMultiTypeObject.NDoubleField));
            VerifyField(doi, nameof(POCOMultiTypeObject.GuidField));
            VerifyField(doi, nameof(POCOMultiTypeObject.DateTimeField));
            VerifyField(doi, nameof(POCOMultiTypeObject.EnumField));

            VerifyField(doi, nameof(POCOMultiTypeObject.PartialyPublicField));
            VerifyField(doi, nameof(POCOMultiTypeObject.PartialyPublicAutomaticField));
            VerifyField(doi, nameof(POCOMultiTypeObject.AutomaticStringField));

            //VerifyField(doi, "PrivateField");

            //verify non visible field
            VerifyNonvisableField(doi, nameof(POCOMultiTypeObject.IgnoredField));
            VerifyNonvisableField(doi, nameof(POCOMultiTypeObject.ListField));
            VerifyNonvisableField(doi, nameof(POCOMultiTypeObject.ArrayField));
            VerifyNonvisableField(doi, nameof(POCOMultiTypeObject.ObjectField));

            VerifyNonvisableField(doi, "PrivateField");
            VerifyNonvisableField(doi, "PrivateIgnoredField");
            VerifyNonvisableField(doi, "PrivateAutomaticField");
            VerifyNonvisableField(doi, "IInterface.InterfaceProperty");
            VerifyNonvisableField(doi, "InterfaceProperty");
        }

        private void VerifyField(EntityDescription doi, string fieldName)
        {
            doi.Fields.Should().Contain(x => x.Name == fieldName, because: fieldName);

            var field = doi.Fields.Where(x => x.Name == fieldName).Single();

            field.RunTimeType.Should().NotBeNull();
            field.Property.Getter.Should().NotBeNull();
            field.Property.Setter.Should().NotBeNull();

            field.PersistanceFlags.Should().HaveFlag(PersistanceFlags.OnUpdate);
            field.PersistanceFlags.Should().HaveFlag(PersistanceFlags.OnInsert);

        }

        private void VerifyNonvisableField(EntityDescription doi, string fieldName)
        {
            doi.Fields.Should().NotContain(x => x.Name == fieldName);
        }
    }
}