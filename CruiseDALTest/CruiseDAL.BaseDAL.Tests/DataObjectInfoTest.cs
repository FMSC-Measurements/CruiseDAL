using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using CruiseDAL;
using CruiseDALTest.TestTypes;
using Xunit;
using Xunit.Abstractions;

namespace CruiseDAL.BaseDAL.Tests
{
    public class DataObjectInfoTest 
    {
        private readonly ITestOutputHelper _output;

        public DataObjectInfoTest(ITestOutputHelper output)
        {
            _output = output;
            
        }




        [Fact]
        public void Test_VanillaMultiTypeObject()
        {
            Type t = typeof(VanillaMultiTypeObject);
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
            var types = (from t in System.Reflection.Assembly.GetAssembly(typeof(CruiseDAL.DataObject)).GetTypes()
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
            Assert.False(String.IsNullOrWhiteSpace(doi.ReadSource));
            Assert.NotEmpty(doi.Properties);
            Assert.True(doi.Properties.Values.All(x => x.Getter != null || x.Setter != null));


        }
    }
}
