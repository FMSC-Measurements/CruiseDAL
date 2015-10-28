using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CruiseDAL.Core.EntityModel
{
    interface IDataObject
    {
        DatastoreBase DAL { get; set; }

    }
}
