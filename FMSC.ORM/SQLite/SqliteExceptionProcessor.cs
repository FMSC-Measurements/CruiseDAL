using FMSC.ORM.Core;
using System;
using System.Data;


#if SYSTEM_DATA_SQLITE
using SqliteException = System.Data.SQLite.SQLiteException;
#elif MICROSOFT_DATA_SQLITE
using Microsoft.Data.Sqlite;
#else
#warning " " 
#endif

namespace FMSC.ORM.SQLite
{
    public class SqliteExceptionProcessor : IExceptionProcessor
    {
        public Exception ProcessException(Exception innerException, IDbConnection connection, string commandText, IDbTransaction transaction)
        {
            if (innerException is SqliteException)
            {
                SQLException sqlEx;

                var ex = innerException as SqliteException;

#if MICROSOFT_DATA_SQLITE
                var errorCode = (SqliteResultCode)ex.SqliteErrorCode;
#elif SYSTEM_DATA_SQLITE
                var errorCode = (SqliteResultCode)ex.ErrorCode;
#endif

                switch (errorCode)
                {
                    case SqliteResultCode.Corrupt:
                    case SqliteResultCode.NotADb:
                    case SqliteResultCode.Perm:
                    case SqliteResultCode.IoErr:
                    case SqliteResultCode.CantOpen:
                    case SqliteResultCode.Full:
                    case SqliteResultCode.Auth:
                        {
                            return new FileAccessException(errorCode.ToString(), innerException);
                        }
                    case SqliteResultCode.ReadOnly:
                        {
                            sqlEx = new ReadOnlyException(null, innerException);
                            break;
                        }
                    case SqliteResultCode.Locked:
                        {
                            sqlEx = new ConnectionException("file is locked", ex);
                            break;
                        }
                    case SqliteResultCode.Constraint:
                        {
                            if (innerException.Message.IndexOf("UNIQUE constraint failed", StringComparison.OrdinalIgnoreCase) >= 0)
                            {
                                sqlEx = new UniqueConstraintException(null, innerException);
                            }
                            else
                            {
                                sqlEx = new ConstraintException(null, innerException);
                            }
                            break;
                        }
                    default:
                        {
                            sqlEx = new SQLException(null, innerException);
                            break;
                        }
                }
                if (connection != null)
                {
                    try
                    {
                        sqlEx.ConnectionString = connection.ConnectionString;
                        sqlEx.ConnectionState = connection.State.ToString();
                    }
                    catch (ObjectDisposedException)
                    {
                        sqlEx.ConnectionState = "Disposed";
                    }
                }
                if (connection != null)
                {
                    sqlEx.CommandText = commandText;
                    //newEx.Data.Add("CommandText", comm.CommandText);
                }

                return sqlEx;
            }
            else
            {
                return new ORMException(null, innerException);
            }
        }
    }
}