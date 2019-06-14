using FluentAssertions;
using FMSC.ORM.EntityModel.Support;
using System;
using Xunit;
using Xunit.Abstractions;

namespace FMSC.ORM.Test.Core
{
    public class EntityDescriptionLookup_Test : TestBase
    {
        public EntityDescriptionLookup_Test(ITestOutputHelper output) : base(output)
        {
        }

        [Fact]
        public void LookUpEntityByType()
        {
            var lookup = GlobalEntityDescriptionLookup.Instance;

            var type = typeof(ORM.TestSupport.TestModels.POCOMultiTypeObject);
            var description = lookup.LookUpEntityByType(type);

            description.Should().NotBeNull();
            description.EntityType.Should().Be(type);

            var descriptionAgain = lookup.LookUpEntityByType(type);
            description.Should().BeSameAs(descriptionAgain);
        }

        [Fact]
        public void LookUpEntityByType_same_named_type()
        {
            var lookup = GlobalEntityDescriptionLookup.Instance;

            var type = typeof(ORM.TestSupport.TestModels.POCOMultiTypeObject);
            var description = lookup.LookUpEntityByType(type);

            description.Should().NotBeNull();
            description.EntityType.Should().Be(type);

            Type typeOther = typeof(EntityDescriptionLookup_Test.POCOMultiTypeObject);
            var descriptionOther = lookup.LookUpEntityByType(typeOther);
            description.Should().NotBeSameAs(descriptionOther);
        }

        private class POCOMultiTypeObject
        {
        }
    }
}