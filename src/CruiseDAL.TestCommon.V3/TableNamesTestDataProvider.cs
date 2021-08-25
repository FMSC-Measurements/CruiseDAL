using System.Collections.Generic;
using System.Linq;

namespace CruiseDAL.TestCommon
{
    public class TableNamesTestDataProvider : IEnumerable<object[]>
    {
        public IEnumerator<object[]> GetEnumerator()
        {
            return CruiseDatastoreBuilder_V3.TABLE_DEFINITIONS
                .Select(x => new object[] { x.TableName, })
                .GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() => GetEnumerator();
    }
}