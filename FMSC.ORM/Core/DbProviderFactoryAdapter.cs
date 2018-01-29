using System.Data.Common;

namespace FMSC.ORM.Core
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


        public abstract DbConnection CreateConnection();
    }
}
