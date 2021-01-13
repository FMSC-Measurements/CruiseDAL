using System.Data.Common;

namespace FMSC.ORM.Core
{
    public interface IDatastoreBuilder
    {
        void BuildDatabase(DbConnection connection, DbTransaction transaction, IExceptionProcessor exceptionProcessor = null);
    }
}