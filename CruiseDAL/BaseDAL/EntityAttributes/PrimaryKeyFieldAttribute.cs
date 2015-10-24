using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CruiseDAL.BaseDAL.EntityAttributes
{
    public enum KeyType { None = 0, RowID, GUID}

    public class PrimaryKeyFieldAttribute : FieldAttribute
    {
        public KeyType KeyType { get; set; }
    }
}
