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
            //
            // TODO: Add constructor logic here
            //
        }


        public void Test_VanillaMultiTypeObject()
        {
            var doi = new DataObjectInfo(typeof(VanillaMultiTypeObject));
            Assert.NotNull(doi);
        }

        public void Test_DOMultiPropType()
        {
            var doi = new DataObjectInfo(typeof(DOMultiPropType));
            Assert.NotNull(doi);
        }

        public void LoadDataObjects()
        {
            var types = (from t in System.Reflection.Assembly.GetAssembly(typeof(CruiseDAL.DataObject)).GetTypes()
                         where t.IsClass && t.Namespace == "CruiseDAL.DataObjects"
                         select t).ToList();

            foreach (Type t in types)
            {
                _output.WriteLine(t.FullName);
                var doi = new DataObjectInfo(t);
                
                //Assert.True(doi.ReadSource != null, doi._dataObjectType.Name);
            }
        }
    }
}
