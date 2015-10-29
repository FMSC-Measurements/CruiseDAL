using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CruiseDAL.Core.EntityModel
{
    public static class DataObjectExtentions
    {
        public static void Save(this IDataObject data)
        {
            Debug.Assert(data.DAL != null);
            data.DAL.Save(data, OnConflictOption.Fail);
        }

        public static void Save(this IDataObject data, OnConflictOption conflictOption)
        {
            Debug.Assert(data.DAL != null);
            data.DAL.Save(data, conflictOption);
        }

        public static void Delete(this IDataObject data)
        {
            Debug.Assert(data.DAL != null);
            data.DAL.Delete(data);
        }

    }
}
