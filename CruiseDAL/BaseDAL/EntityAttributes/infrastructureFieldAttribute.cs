using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CruiseDAL.BaseDAL.EntityAttributes
{
    public enum InfrastructureFieldType { RowVersion, CreatedBy, CreatedDate, ModifiedBy, ModifiedDate }


    public class InfrastructureFieldAttribute : FieldAttribute
    {
        public InfrastructureFieldType FieldType { get; set; }
        public bool PopulatedByDB { get; set; }
        public bool ReadOnly { get; set; }

        //public object DefaultValue { get; set; }

    }
}
