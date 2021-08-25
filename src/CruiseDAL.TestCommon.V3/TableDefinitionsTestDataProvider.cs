using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace CruiseDAL.TestCommon
{
    public class TableDefinitionsTestDataProvider : IEnumerable<object[]>
    {
        public IEnumerator<object[]> GetEnumerator()
        {
            return CruiseDatastoreBuilder_V3.TABLE_DEFINITIONS
                .Select(x => new object[] { x, })
                .GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
