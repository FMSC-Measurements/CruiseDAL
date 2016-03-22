using System.Collections.Generic;
using System.Linq;

namespace CruiseDAL.DataObjects
{
    public partial class CruiseMethodsDO 
    {
        public static List<T> ReadCruiseMethods<T>(DAL db, bool reconMethodsOnly) where T : CruiseMethodsDO, new()
        {
            string condition = string.Format("Code NOT IN ( '{0}' ) {1}", 
                string.Join("', '", CruiseDAL.Schema.CruiseMethods.UNSUPPORTED_METHODS),
                (reconMethodsOnly) ? "AND Code = 'FIX' OR Code = 'PNT'" : string.Empty);

            return db.From<T>().Where(condition).Read().ToList();
        }

        public static List<CruiseMethodsDO> ReadCruiseMethods(DAL db, bool reconMethodsOnly)
        {
            return ReadCruiseMethods<CruiseMethodsDO>(db, reconMethodsOnly);
        }

        public static string[] ReadCruiseMethodStr(DAL db, bool reconMethodsOnly)
        {
            string command = string.Format("Select group_concat(Code,',') FROM CruiseMethods WHERE Code NOT IN ( '{0}' ) {1};",
                string.Join("', '", CruiseDAL.Schema.CruiseMethods.UNSUPPORTED_METHODS),
                (reconMethodsOnly) ? "AND Code = 'FIX' OR Code = 'PNT'" : string.Empty);
            string result = db.ExecuteScalar(command) as string ?? string.Empty;
            return result.Split(',');
        }

    }
}