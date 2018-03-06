using FMSC.ORM.Core;
using FMSC.ORM.Core.SQL;
using FMSC.ORM.Util;
using System;
using System.Collections.Generic;
using System.Data;

using System.Linq;
using System.Text;

#if SYSTEM_DATA_SQLITE
using SqliteConnection = System.Data.SQLite.SQLiteConnection;
using SqliteParameter = System.Data.SQLite.SQLiteParameter;
using SqliteCommand = System.Data.SQLite.SQLiteCommand;
#elif MICROSOFT_DATA_SQLITE
using Microsoft.Data.Sqlite;
#else
#warning " " 
#endif

namespace FMSC.ORM.SQLite
{
    public static class SqliteDataType
    {
        public const string TEXT = "TEXT";
        public const string REAL = "REAL";
        public const string INTEGER = "INTEGER";
        public const string BLOB = "BLOB";
        public const string NUMERIC = "NUMERIC";
    }

    public class SqliteDialect : ISqlDialect
    {
        private static Logger _logger = new Logger();

        public string BuildConnectionString(DatastoreRedux datastore)
        {
            System.Diagnostics.Debug.Assert(!string.IsNullOrEmpty(datastore.Path));
#if SYSTEM_DATA_SQLITE
            return string.Format("Data Source={0};Version=3;", datastore.Path);
#else
            return string.Format("Data Source={0};", datastore.Path);
#endif
        }

        public IDbConnection CreateConnection()
        {
            var connection = new SqliteConnection();
            connection.StateChange += _Connection_StateChange;
            return connection;
        }

        public IDbDataParameter CreateParameter(string name, object value)
        {
            return new SqliteParameter(name, value);
        }

        public IDbCommand CreateCommand()
        {
            return new SqliteCommand();
        }

        public string GetColumnDef(ColumnInfo col, bool includeConstraint)
        {
            StringBuilder columnDef = new StringBuilder(col.Name).Append(" ").Append(col.DBType).Append(" ");
            if (includeConstraint)
            {
                if (col.IsPK)
                {
                    columnDef.Append("PRIMARY KEY ");
                    if (col.DBType == "INTEGER" && col.AutoIncrement)
                    { columnDef.Append("AUTOINCREMENT "); }
                }

                if (col.IsRequired)
                {
                    columnDef.Append("NOT NULL ");
                }
                else if (!string.IsNullOrEmpty(col.Default))
                {
                    columnDef.Append("DEFAULT ").Append(col.Default).Append(" ");
                }

                if (col.Unique)
                {
                    columnDef.Append("UNIQUE ");
                }

                if (!string.IsNullOrEmpty(col.Check))
                {
                    columnDef.Append("CHECK ").Append(col.Check).Append(" ");
                }
            }

            return columnDef.ToString();
        }

        public string BuildCreateTable(string tableName, IEnumerable<ColumnInfo> cols, bool temp)
        {
            if (string.IsNullOrEmpty(tableName)) { throw new ArgumentNullException("tableName"); }
            if (cols == null) { throw new ArgumentNullException("cols"); }
            if (cols.Count() == 0) { throw new ArgumentException("cols can't be empty", "cols"); }

            var sb = new StringBuilder();
            sb.Append("CREATE ");
            if (temp) { sb.Append("TEMP "); }
            sb.AppendLine("TABLE ").AppendLine(tableName);

            var colsList = cols.Select(c => GetColumnDef(c, true)).BookEnd("( ", ")", ",\r\n");

            sb.AppendMany(colsList);
            sb.Append(";");
            return sb.ToString();
        }

        //for logging connection state changes
        private void _Connection_StateChange(object sender, System.Data.StateChangeEventArgs e)
        {
            _logger.LogConnectionEvent(e);
        }

        //public string MapDbTypeToSQLType(DbType type)
        //{
        //    switch (type)
        //    {
        //        case DbType.AnsiString:
        //        case DbType.AnsiStringFixedLength:
        //        case DbType.Guid:
        //        case DbType.String:
        //        case DbType.StringFixedLength:
        //        case DbType.Xml:
        //            { return SqliteDataType.TEXT; }
        //        case DbType.Int16:
        //        case DbType.Int32:
        //        case DbType.Int64:
        //        case DbType.UInt16:
        //        case DbType.UInt32:
        //        case DbType.UInt64:
        //            { return SqliteDataType.INTEGER; }
        //        case DbType.Binary:
        //            { return SqliteDataType.BLOB; }
        //        case DbType.Double:
        //        case DbType.Single:
        //            { return SqliteDataType.REAL; }
        //        case DbType.Date:
        //        case DbType.DateTime:
        //        case DbType.DateTime2:
        //        case DbType.DateTimeOffset:
        //        case DbType.Time:
        //        case DbType.Byte:
        //        case DbType.Boolean:
        //        case DbType.Decimal:
        //        case DbType.VarNumeric:
        //            { return SqliteDataType.NUMERIC; }
        //        default:
        //            { return SqliteDataType.TEXT; }
        //    }
        //}

        //public DbType MapSQLtypeToDbType(string sqlType)
        //{
        //    switch (sqlType)
        //    {
        //        case SqliteDataType.BLOB:
        //            { return DbType.Binary; }
        //        case SqliteDataType.INTEGER:
        //            { return DbType.Int64; }
        //        case SqliteDataType.REAL:
        //            { return DbType.Double; }
        //        case SqliteDataType.TEXT:
        //            { return DbType.String; }
        //        default:
        //            { throw new InvalidOperationException(); }
        //    }
        //}
    }
}