using FMSC.ORM.Core;
using System.Data.Common;

namespace CruiseDAL
{
    public interface IUpdater
    {
        //void Update(DbConnection connection, IExceptionProcessor exceptionProcessor);

        void Update(CruiseDatastore datastore);

        //bool HasUpdate(DbConnection connection);
    }
}