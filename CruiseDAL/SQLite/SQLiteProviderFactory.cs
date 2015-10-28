using CruiseDAL.BaseDAL;
using System;

using System.Data.Common;
using System.Data.SQLite;

namespace CruiseDAL.SQLite
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
