using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CruiseDAL.Core
{
    public abstract class DbProviderFactoryAdapter
    {
        public static DbProviderFactoryAdapter Instance
        {
            get;
            protected set;
        }

        public abstract DbParameter CreateParameter(string name, object value);
        

        public abstract DbCommand CreateCommand();
        public virtual DbCommand CreateCommand(string commandText)
        {
            DbCommand cmd = CreateCommand();
            cmd.CommandText = commandText;
            return cmd;
        }

        //public virtual DbCommand CreateCommand(DbConnection connection, String commandText)
        //{
        //    DbCommand cmd = connection.CreateCommand();
        //    cmd.CommandText = commandText;
        //    return cmd;
        //}

        public abstract DbConnection CreateConnection();
    }
}
