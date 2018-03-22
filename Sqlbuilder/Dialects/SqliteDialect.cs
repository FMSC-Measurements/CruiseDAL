using System;
using System.Data;
using System.Text;

namespace SqlBuilder.Dialects
{
    public static class SqliteDataType
    {
        public const string TEXT = "TEXT";
        public const string REAL = "REAL";
        public const string INTEGER = "INTEGER";
        public const string BLOB = "BLOB";
        public const string NUMERIC = "NUMERIC";

        public const string BOOLEAN = "BOOLEAN";
        public const string DATATIME = "DATETIME";

        public const string DOUBLE = "DOUBLE";
        public const string FLOAT = "FLOAT";
    }

    public class SqliteDialect : SqlDialect
    {
        public override string GetColumnDefinition(ColumnInfo col)
        {
            var sqlType = col.Type ?? MapDbTypeToSQLType(col.DBType);
            if (col.AutoIncrement && sqlType != SqliteDataType.INTEGER) { throw new InvalidOperationException("column type must be integer type if AutoIncrement is true"); }

            StringBuilder columnDef = new StringBuilder(col.Name).Append(" ").Append(sqlType);
            if (col.IsPK)
            {
                columnDef.Append(" PRIMARY KEY");
                if (col.AutoIncrement)
                { columnDef.Append(" AUTOINCREMENT"); }
            }

            if (col.NotNull)
            {
                columnDef.Append(" NOT NULL");
            }
            else if (!string.IsNullOrEmpty(col.Default))
            {
                columnDef.Append(" DEFAULT ").Append(col.Default);
            }

            if (col.Unique)
            {
                columnDef.Append(" UNIQUE");
            }

            if (!string.IsNullOrEmpty(col.Check))
            {
                columnDef.Append(" CHECK ").Append(col.Check);
            }

            return columnDef.ToString();
        }

        public override string MapDbTypeToSQLType(DbType type)
        {
            switch (type)
            {
                case DbType.AnsiString:
                case DbType.AnsiStringFixedLength:
                case DbType.Guid:
                case DbType.String:
                case DbType.StringFixedLength:
                case DbType.Xml:
                    { return SqliteDataType.TEXT; }
                case DbType.Int16:
                case DbType.Int32:
                case DbType.Int64:
                case DbType.UInt16:
                case DbType.UInt32:
                case DbType.UInt64:
                    { return SqliteDataType.INTEGER; }
                case DbType.Binary:
                    { return SqliteDataType.BLOB; }
                case DbType.Double:
                case DbType.Single:
                    { return SqliteDataType.REAL; }
                case DbType.Date:
                case DbType.DateTime:
#if!NetCF
                case DbType.DateTime2:
                case DbType.DateTimeOffset:
#endif
                case DbType.Time:
                case DbType.Byte:
                case DbType.Boolean:
                case DbType.Decimal:
                case DbType.VarNumeric:
                    { return SqliteDataType.NUMERIC; }
                default:
                    { return SqliteDataType.TEXT; }
            }
        }

        public override DbType MapSQLtypeToDbType(string sqlType)
        {
            if(sqlType == null) { throw new ArgumentNullException("sqlType"); }

            switch (sqlType)
            {
                case SqliteDataType.BLOB:
                    { return DbType.Binary; }
                case SqliteDataType.INTEGER:
                    { return DbType.Int64; }
                case SqliteDataType.REAL:
                    { return DbType.Double; }
                case SqliteDataType.TEXT:
                    { return DbType.String; }
                case "BOOL":
                case SqliteDataType.BOOLEAN:
                    { return DbType.Boolean; }
                case SqliteDataType.DATATIME:
                    { return DbType.DateTime; }
                case SqliteDataType.DOUBLE:
                    { return DbType.Double; }
                case SqliteDataType.FLOAT:
                    { return DbType.Single; }                
                default:
                    { throw new ArgumentException("value:" + sqlType + " is invalid", "sqlType"); }
            }
        }
    }
}