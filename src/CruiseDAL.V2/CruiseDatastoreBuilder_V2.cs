using FMSC.ORM.Core;
using System.Data.Common;

namespace CruiseDAL
{
    public class CruiseDatastoreBuilder_V2 : IDatastoreBuilder
    {
        public void BuildDatabase(DbConnection connection, DbTransaction transaction, IExceptionProcessor exceptionProcessor = null)
        {
            connection.ExecuteNonQuery(Schema.Schema.CREATE_TABLES, transaction: transaction, exceptionProcessor: exceptionProcessor);
            connection.ExecuteNonQuery(Schema.Schema.CREATE_TRIGGERS, transaction: transaction, exceptionProcessor: exceptionProcessor);
        }
    }
}