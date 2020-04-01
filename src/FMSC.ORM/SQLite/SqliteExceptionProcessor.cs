using FMSC.ORM.Core;
using FMSC.ORM.Logging;
using System;
using System.Data;
using System.Data.Common;


#if SYSTEM_DATA_SQLITE
using SqliteException = System.Data.SQLite.SQLiteException;
#elif MICROSOFT_DATA_SQLITE
using Microsoft.Data.Sqlite;
#else
#warning "either SYSTEM_DATA_SQLITE OR MICROSOFT_DATA_SQLITE should be defined" 
#endif

namespace FMSC.ORM.SQLite
{
    public class SqliteExceptionProcessor : IExceptionProcessor
    {
        protected ILogger Logger { get; set; } = LoggerProvider.Get();

        public Exception ProcessException(Exception innerException, DbConnection connection, string commandText, DbTransaction transaction)
        {
            Logger.LogException(innerException);

            if (innerException is SqliteException ex)
            {
                SQLException sqlEx;

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
#if MICROSOFT_DATA_SQLITE
            else if((innerException is InvalidOperationException ioex)
                && ioex.Source == "Microsoft.Data.Sqlite")
            {
                return new SQLException(null, innerException) { CommandText = commandText };
            }
#endif
            else
            {
                return new ORMException(null, innerException);
            }
        }
    }
}