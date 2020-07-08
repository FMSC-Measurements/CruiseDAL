using CruiseDAL.V2.Test;
using FluentAssertions;
using FMSC.ORM.EntityModel;
using FMSC.ORM.EntityModel.Attributes;
using FMSC.ORM.EntityModel.Support;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace CruiseDAL.V2.DataObjects
{
    public class TableModels_Test : TestBase
    {
        public TableModels_Test(ITestOutputHelper output) : base(output)
        {
        }

        [Fact]
        public void LoadDataObjects()
        {
            var types = (from t in System.Reflection.Assembly.GetAssembly(typeof(DataObject_Base)).GetTypes()
                         where t.IsClass && t.Namespace == "CruiseDAL.DataObjects"
                         select t).ToList();

            foreach (Type t in types)
            {
                Output.WriteLine(t.FullName);
                var doi = new EntityDescription(t);

                VerifyDataObjectInfo(t, doi);
            }
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

        }

        private void VerifyField(EntityDescription doi, string fieldName)
        {
            Output.WriteLine("Verifying " + fieldName);
            doi.Fields.Should().Contain(x => x.Name == fieldName, $"expected {doi.SourceName} to contain {fieldName}");

            var field = doi.Fields.Where(x => x.Name == fieldName).Single();
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

            //if (isPrivate)
            //{
            //    Assert.DoesNotContain(doi.Properties, x => x.Key == fieldName);
            //}
            //else
            //{
            //    Assert.Contains(doi.Properties, x => x.Key == fieldName);
            //}
        }
    }
}
