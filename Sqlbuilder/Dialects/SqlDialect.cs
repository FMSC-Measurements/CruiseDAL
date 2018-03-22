using System.Data;

namespace SqlBuilder.Dialects
{
    public abstract class SqlDialect : ISqlDialect
    {
        public static ISqlDialect DefaultDialect { get; set; }

        public abstract string GetColumnDefinition(ColumnInfo columnInfo);

        public abstract string MapDbTypeToSQLType(DbType type);

        public abstract DbType MapSQLtypeToDbType(string sqlType);
    }
}