using CruiseDAL.Core.SQL;
using System.Diagnostics;

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
