#if WindowsCE

namespace System.Data.Common
{
    public abstract class DbProviderFactory
    {
        protected DbProviderFactory()
        {
        }

        virtual public bool CanCreateDataSourceEnumerator
        {
            get
            {
                return false;
            }
        }

        public virtual DbCommand CreateCommand()
        {
            return null;
        }

        public virtual DbCommandBuilder CreateCommandBuilder()
        {
            return null;
        }

        public virtual DbConnection CreateConnection()
        {
            return null;
        }

        //public virtual DbConnectionStringBuilder CreateConnectionStringBuilder()
        //{
        //    return null;
        //}

        public virtual DbDataAdapter CreateDataAdapter()
        {
            return null;
        }

        public virtual DbParameter CreateParameter()
        {
            return null;
        }

        //public virtual CodeAccessPermission CreatePermission(PermissionState state)
        //{
        //    return null;
        //}

        //public virtual DbDataSourceEnumerator CreateDataSourceEnumerator()
        //{
        //    return null;
        //}
    }
}
#endif