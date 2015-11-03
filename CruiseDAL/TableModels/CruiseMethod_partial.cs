using System.Collections.Generic;
namespace CruiseDAL.DataObjects
{
    public partial class CruiseMethodsDO 
    {
        public static List<T> ReadCruiseMethods<T>(DALRedux db, bool reconMethodsOnly) where T : CruiseMethodsDO, new()
        {
            string condition = string.Format("WHERE Code NOT IN ( '{0}' ) {1}", 
                string.Join("', '", CruiseDAL.Schema.Constants.CruiseMethods.UNSUPPORTED_METHODS),
                (reconMethodsOnly) ? "AND Code = 'FIX' OR Code = 'PNT'" : string.Empty);

            return db.Read<T>(condition);
        }

        public static List<CruiseMethodsDO> ReadCruiseMethods(DALRedux db, bool reconMethodsOnly)
        {
            return ReadCruiseMethods<CruiseMethodsDO>(db, reconMethodsOnly);
        }

        public static string[] ReadCruiseMethodStr(DALRedux db, bool reconMethodsOnly)
        {
            string command = string.Format("Select group_concat(Code,',') FROM CruiseMethods WHERE Code NOT IN ( '{0}' ) {1};",
                string.Join("', '", CruiseDAL.Schema.Constants.CruiseMethods.UNSUPPORTED_METHODS),
                (reconMethodsOnly) ? "AND Code = 'FIX' OR Code = 'PNT'" : string.Empty);
            string result = db.ExecuteScalar(command) as string ?? string.Empty;
            return result.Split(',');
        }

    }
}