using FMSC.ORM.Core;
using System;
using System.Data;
using System.Data.SQLite;

namespace FMSC.ORM.SQLite
{
    public class SqliteExceptionProcessor : IExceptionProcessor
    {
        public Exception ProcessException(Exception innerException, IDbConnection connection, string commandText, IDbTransaction transaction)
        {
            if (innerException is SQLiteException)
            {
                SQLException sqlEx;

                var ex = innerException as SQLiteException;
#if Mono
                var errorCode = ex.ErrorCode;
#else
                var errorCode = ex.ResultCode;
#endif
                switch (errorCode)
                {
                    case SQLiteErrorCode.Corrupt:
#if Mono
                    case SQLiteErrorCode.NotADatabase:
#else
                    case SQLiteErrorCode.NotADb:
#endif
                    case SQLiteErrorCode.Perm:
#if Mono
                    case SQLiteErrorCode.IOErr:
#else
                    case SQLiteErrorCode.IoErr:
#endif

                    case SQLiteErrorCode.CantOpen:
                    case SQLiteErrorCode.Full:
                    case SQLiteErrorCode.Auth:
                        {
                            return new FileAccessException(errorCode.ToString(), innerException);
                        }
                    case SQLiteErrorCode.ReadOnly:
                        {
                            sqlEx = new ReadOnlyException(null, innerException);
                            break;
                        }
                    case SQLiteErrorCode.Locked:
                        {
                            sqlEx = new ConnectionException("file is locked", ex);
                            break;
                        }
                    case SQLiteErrorCode.Constraint:
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