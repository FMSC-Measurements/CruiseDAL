using CruiseDAL.TestCommon;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace CruiseDAL.V3.Sync
{
    public class CruiseCopier_Test
    {
        [Fact]
        public void Copy()
        {
            var initializer = new DatabaseInitializer();
            using (var srcDb = initializer.CreateDatabase())
            using (var destDb = new CruiseDatastore_V3())
            {
                var copier = new CruiseCopier();
                copier.Copy(srcDb, destDb, initializer.CruiseID);
            }
        }
    }
}
