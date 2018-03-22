using System.Data;

namespace SqlBuilder.Dialects
{
    public interface ISqlDialect
    {
        string GetColumnDefinition(ColumnInfo columnInfo);

        string MapDbTypeToSQLType(DbType type);

        DbType MapSQLtypeToDbType(string sqlType);
    }
}