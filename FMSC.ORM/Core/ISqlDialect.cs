using FMSC.ORM.Core.SQL;
using System.Collections.Generic;
using System.Data;

namespace FMSC.ORM.Core
{
    public interface ISqlDialect
    {
        string BuildConnectionString(DatastoreRedux dataStore);

        IDbConnection CreateConnection();

        string GetColumnDef(ColumnInfo col, bool includeConstraint);

        string BuildCreateTable(string tableName, IEnumerable<ColumnInfo> cols, bool temp);

        //string MapDbTypeToSQLType(DbType type);
    }
}