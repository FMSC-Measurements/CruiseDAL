using System;

using FMSC.ORM.Core;
using System.Data.Common;

#if Mono
using Mono.Data.Sqlite;
using SQLiteCommand = Mono.Data.Sqlite.SqliteCommand;
using SQLiteConnection = Mono.Data.Sqlite.SqliteConnection;
using SQLiteParameter = Mono.Data.Sqlite.SqliteParameter;
#else
using System.Data.SQLite;
#endif

namespace FMSC.ORM.SQLite
{
    public class SQLiteProviderFactory : DbProviderFactoryAdapter
    {
        public override DbCommand CreateCommand()
        {
            return new SQLiteCommand();
        }

        public override DbConnection CreateConnection()
        {
            return new SQLiteConnection();
        }

        public override DbParameter CreateParameter(string name, object value)
        {
            return new SQLiteParameter(name, value);
        }
    }
}
