using System;
using System.Data.Common;

namespace FMSC.ORM.Core
{
    public interface IExceptionProcessor
    {
        Exception ProcessException(Exception innerException, DbConnection connection, string commandText, DbTransaction transaction);
    }
}