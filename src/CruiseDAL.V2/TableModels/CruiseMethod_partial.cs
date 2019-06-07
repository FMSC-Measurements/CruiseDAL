using System.Collections.Generic;
using System.Linq;

namespace CruiseDAL.DataObjects
{
    public partial class CruiseMethodsDO
    {
        public static List<T> ReadCruiseMethods<T>(DAL db, bool reconMethodsOnly) where T : CruiseMethodsDO, new()
        {
            if (reconMethodsOnly)
            {
                return db.From<T>()
                    .Where("Code = 'FIX' OR Code = 'PNT'")
                    .Read().ToList();
            }
            else
            {
                return db.From<T>().Read().ToList();
            }
        }

        public static List<CruiseMethodsDO> ReadCruiseMethods(DAL db, bool reconMethodsOnly)
        {
            return ReadCruiseMethods<CruiseMethodsDO>(db, reconMethodsOnly);
        }

        public static string[] ReadCruiseMethodStr(DAL db, bool reconMethodsOnly)
        {
            var format = "Select group_concat(Code,',') FROM CruiseMethods {0};";

            string command = string.Format(format,
                (reconMethodsOnly) ? "WHERE Code = 'FIX' OR Code = 'PNT'" : string.Empty);

            string result = db.ExecuteScalar(command) as string ?? string.Empty;

            if (string.IsNullOrEmpty(result))
            {
                return new string[0];
            }
            else
            {
                return result.Split(',');
            }
        }
    }
}