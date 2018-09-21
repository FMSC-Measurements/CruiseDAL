using System;
using System.Data;

namespace FMSC.ORM.Core
{
    public interface IExceptionProcessor
    {
        Exception ProcessException(Exception innerException, IDbConnection connection, string commandText, IDbTransaction transaction);
    }
}